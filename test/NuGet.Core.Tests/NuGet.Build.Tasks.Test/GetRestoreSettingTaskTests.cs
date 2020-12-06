// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using FluentAssertions;
using Microsoft.Build.Framework;
using Moq;
using NuGet.Configuration;
using NuGet.Configuration.Test;
using NuGet.Test.Utility;
using Xunit;

namespace NuGet.Build.Tasks.Test
{
    public class GetRestoreSettingsTaskTests
    {
        class TestMachineWideSettings : IMachineWideSettings
        {
            public IEnumerable<Settings> Settings { get; }

            public TestMachineWideSettings(Settings settings)
            {
                Settings = new List<Settings>() { settings };
            }
        }

        [Fact]
        public void GetRestoreSettingsTask_GetValueGetFirstValue()
        {
            RestoreSettingsUtils.GetValue(
                () => "a",
                () => "b",
                () => null).Should().Be("a");
        }

        [Fact]
        public void GetRestoreSettingsTask_GetValueGetLastValue()
        {
            RestoreSettingsUtils.GetValue(
                () => null,
                () => null,
                () => new string[0]).ShouldBeEquivalentTo(new string[0]);
        }

        [Fact]
        public void GetRestoreSettingsTask_GetValueAllNull()
        {
            RestoreSettingsUtils.GetValue<string[]>(
                () => null,
                () => null).Should().BeNull();
        }

        [Fact]
        public void TestSolutionSettings()
        {
            // Arrange
            var subFolderConfig = @"<?xml version=""1.0"" encoding=""utf-8""?>
            <configuration>
                <fallbackPackageFolders>
                    <add key=""a"" value=""C:\Temp\a"" />
                </fallbackPackageFolders>
            </configuration>";

            var baseConfig = @"<?xml version=""1.0"" encoding=""utf-8""?>
            <configuration>
                <fallbackPackageFolders>
                    <add key=""b"" value=""C:\Temp\b"" />
                </fallbackPackageFolders>
                <packageSources>
                    <add key=""c"" value=""C:\Temp\c"" />
                </packageSources>
            </configuration>";

        

            var baseConfigPath = "NuGet.Config";

            using (var machineWide = TestDirectory.Create())
            using (var mockBaseDirectory = TestDirectory.Create())
            {
                var subFolder = Path.Combine(mockBaseDirectory, "sub");
                var solutionDirectoryConfig = Path.Combine(mockBaseDirectory, NuGetConstants.NuGetSolutionSettingsFolder);

                ConfigurationFileTestUtility.CreateConfigurationFile(baseConfigPath, solutionDirectoryConfig, baseConfig);
                ConfigurationFileTestUtility.CreateConfigurationFile(baseConfigPath, subFolder, subFolderConfig);
                ConfigurationFileTestUtility.CreateConfigurationFile(baseConfigPath, machineWide, machineWideSettingsConfig);
                var machineWideSettings = new Lazy<IMachineWideSettings>(() => new TestMachineWideSettings(new Settings(machineWide, baseConfigPath, true)));

                // Test

                var settings = RestoreSettingsUtils.ReadSettings(mockBaseDirectory, mockBaseDirectory,null, machineWideSettings);
                var filePaths = SettingsUtility.GetConfigFilePaths(settings);

                Assert.Equal(3, filePaths.Count()); // Solution, app data + machine wide
                Assert.True(filePaths.Contains(Path.Combine(solutionDirectoryConfig, baseConfigPath)));
                Assert.True(filePaths.Contains(Path.Combine(machineWide, baseConfigPath)));

                // Test 
                 settings = RestoreSettingsUtils.ReadSettings(mockBaseDirectory, mockBaseDirectory, Path.Combine(subFolder, baseConfigPath), machineWideSettings);
                 filePaths = SettingsUtility.GetConfigFilePaths(settings);

                Assert.Equal(1, filePaths.Count());
                Assert.True(filePaths.Contains(Path.Combine(subFolder, baseConfigPath)));
            }
        }

        [Fact]
        public void GetRestoreSettingsTask_FindConfigInProjectFolder()
        {
            // Verifies that we include any config file found in the project folder
            using (var machineWide = TestDirectory.Create())
            using (var workingDir = TestDirectory.Create())
            {
                // Arrange
                ConfigurationFileTestUtility.CreateConfigurationFile(Settings.DefaultSettingsFileName, machineWide, machineWideSettingsConfig);
                var machineWideSettings = new Lazy<IMachineWideSettings>(() => new TestMachineWideSettings(new Settings(machineWide, Settings.DefaultSettingsFileName, true)));

                var innerConfigFile = Path.Combine(workingDir, "sub", Settings.DefaultSettingsFileName);
                var outerConfigFile = Path.Combine(workingDir, Settings.DefaultSettingsFileName);

                var projectDirectory = Path.GetDirectoryName(innerConfigFile);
                Directory.CreateDirectory(projectDirectory);

                File.WriteAllText(innerConfigFile, InnerConfig);
                File.WriteAllText(outerConfigFile, OuterConfig);

                var settings = RestoreSettingsUtils.ReadSettings(null, projectDirectory, null, machineWideSettings);

                var innerValue = settings.GetValue("SectionName", "inner-key");
                var outerValue = settings.GetValue("SectionName", "outer-key");

                // Assert
                Assert.Equal("inner-value", innerValue);
                Assert.Equal("outer-value", outerValue);
                Assert.True(SettingsUtility.GetConfigFilePaths(settings).Contains(innerConfigFile));
                Assert.True(SettingsUtility.GetConfigFilePaths(settings).Contains(outerConfigFile));
            }
        }

        [Fact]
        public void GetRestoreSettingsTask_VerifyRestoreAdditionalProjectSourcesAreAppended()
        {
            using (var testDir = TestDirectory.Create())
            {
                // Arrange
                var buildEngine = new TestBuildEngine();
                var testLogger = buildEngine.TestLogger;

                var settingsPerFramework = new List<ITaskItem>();
                var settings1 = new Mock<ITaskItem>();
                settings1.SetupGet(e => e.ItemSpec).Returns("a");
                settings1.Setup(e => e.GetMetadata("RestoreAdditionalProjectSources")).Returns(Path.Combine(testDir, "sourceC"));
                settingsPerFramework.Add(settings1.Object);

                var task = new GetRestoreSettingsTask()
                {
                    BuildEngine = buildEngine,
                    ProjectUniqueName = Path.Combine(testDir, "a.csproj"),
                    RestoreSources = new[] { Path.Combine(testDir, "sourceA"), Path.Combine(testDir, "sourceB") },
                    RestoreSettingsPerFramework = settingsPerFramework.ToArray()
                };

                // Act
                var result = task.Execute();

                // Assert
                result.Should().BeTrue();
                task.OutputSources.ShouldBeEquivalentTo(new[] { Path.Combine(testDir, "sourceA"), Path.Combine(testDir, "sourceB"), Path.Combine(testDir, "sourceC") });
            }
        }

        [Fact]
        public void GetRestoreSettingsTask_VerifyRestoreAdditionalProjectFallbackFoldersAreAppended()
        {
            using (var testDir = TestDirectory.Create())
            {
                // Arrange
                var buildEngine = new TestBuildEngine();
                var testLogger = buildEngine.TestLogger;

                var settingsPerFramework = new List<ITaskItem>();
                var settings1 = new Mock<ITaskItem>();
                settings1.SetupGet(e => e.ItemSpec).Returns("a");
                settings1.Setup(e => e.GetMetadata("RestoreAdditionalProjectFallbackFolders")).Returns(Path.Combine(testDir, "sourceC"));
                settingsPerFramework.Add(settings1.Object);

                var task = new GetRestoreSettingsTask()
                {
                    BuildEngine = buildEngine,
                    ProjectUniqueName = Path.Combine(testDir, "a.csproj"),
                    RestoreFallbackFolders = new[] { Path.Combine(testDir, "sourceA"), Path.Combine(testDir, "sourceB") },
                    RestoreSettingsPerFramework = settingsPerFramework.ToArray()
                };

                // Act
                var result = task.Execute();

                // Assert
                result.Should().BeTrue();
                task.OutputFallbackFolders.ShouldBeEquivalentTo(new[] { Path.Combine(testDir, "sourceA"), Path.Combine(testDir, "sourceB"), Path.Combine(testDir, "sourceC") });
            }
        }

        [Fact]
        public void GetRestoreSettingsTask_VerifyRestoreAdditionalProjectFallbackFoldersWithExcludeAreNotAdded()
        {
            using (var testDir = TestDirectory.Create())
            {
                // Arrange
                var buildEngine = new TestBuildEngine();
                var testLogger = buildEngine.TestLogger;

                var settingsPerFramework = new List<ITaskItem>();
                var settings1 = new Mock<ITaskItem>();
                settings1.SetupGet(e => e.ItemSpec).Returns("a");
                settings1.Setup(e => e.GetMetadata("RestoreAdditionalProjectFallbackFolders")).Returns(Path.Combine(testDir, "sourceC"));
                settingsPerFramework.Add(settings1.Object);

                var settings2 = new Mock<ITaskItem>();
                settings2.SetupGet(e => e.ItemSpec).Returns("b");
                settings2.Setup(e => e.GetMetadata("RestoreAdditionalProjectFallbackFoldersExcludes")).Returns(Path.Combine(testDir, "sourceC"));
                settingsPerFramework.Add(settings2.Object);

                var task = new GetRestoreSettingsTask()
                {
                    BuildEngine = buildEngine,
                    ProjectUniqueName = Path.Combine(testDir, "a.csproj"),
                    RestoreFallbackFolders = new[] { Path.Combine(testDir, "sourceA"), Path.Combine(testDir, "sourceB") },
                    RestoreSettingsPerFramework = settingsPerFramework.ToArray()
                };

                // Act
                var result = task.Execute();

                // Assert
                result.Should().BeTrue();
                task.OutputFallbackFolders.ShouldBeEquivalentTo(new[] { Path.Combine(testDir, "sourceA"), Path.Combine(testDir, "sourceB") });
            }
        }

        [Fact]
        public void GetRestoreSettingsTask_VerifyAggregationAcrossFrameworks()
        {
            using (var testDir = TestDirectory.Create())
            {
                // Arrange
                var buildEngine = new TestBuildEngine();
                var testLogger = buildEngine.TestLogger;

                var settingsPerFramework = new List<ITaskItem>();
                var settings1 = new Mock<ITaskItem>();
                settings1.SetupGet(e => e.ItemSpec).Returns("a");
                settings1.Setup(e => e.GetMetadata("RestoreAdditionalProjectSources")).Returns($"{Path.Combine(testDir, "a")};{Path.Combine(testDir, "b")}");
                settings1.Setup(e => e.GetMetadata("RestoreAdditionalProjectFallbackFolders")).Returns($"{Path.Combine(testDir, "m")};{Path.Combine(testDir, "n")}");
                settingsPerFramework.Add(settings1.Object);

                var settings2 = new Mock<ITaskItem>();
                settings2.SetupGet(e => e.ItemSpec).Returns("b");
                settings2.Setup(e => e.GetMetadata("RestoreAdditionalProjectSources")).Returns(Path.Combine(testDir, "c"));
                settings2.Setup(e => e.GetMetadata("RestoreAdditionalProjectFallbackFolders")).Returns(Path.Combine(testDir, "s"));
                settingsPerFramework.Add(settings2.Object);

                var settings3 = new Mock<ITaskItem>();
                settings3.SetupGet(e => e.ItemSpec).Returns("c");
                settings3.Setup(e => e.GetMetadata("RestoreAdditionalProjectSources")).Returns(Path.Combine(testDir, "d"));
                settingsPerFramework.Add(settings3.Object);

                var settings4 = new Mock<ITaskItem>();
                settings4.SetupGet(e => e.ItemSpec).Returns("d");
                settings4.Setup(e => e.GetMetadata("RestoreAdditionalProjectFallbackFolders")).Returns(Path.Combine(testDir, "t"));
                settingsPerFramework.Add(settings4.Object);

                var settings5 = new Mock<ITaskItem>();
                settings5.SetupGet(e => e.ItemSpec).Returns("e");
                settingsPerFramework.Add(settings5.Object);

                var task = new GetRestoreSettingsTask()
                {
                    BuildEngine = buildEngine,
                    ProjectUniqueName = Path.Combine(testDir, "a.csproj"),
                    RestoreSources = new[] { Path.Combine(testDir, "base") },
                    RestoreFallbackFolders = new[] { Path.Combine(testDir, "base") },
                    RestoreSettingsPerFramework = settingsPerFramework.ToArray()
                };

                // Act
                var result = task.Execute();

                // Assert
                result.Should().BeTrue();
                task.OutputSources.ShouldBeEquivalentTo(new[] { Path.Combine(testDir, "base"), Path.Combine(testDir, "a"), Path.Combine(testDir, "b"), Path.Combine(testDir, "c"), Path.Combine(testDir, "d") });
                task.OutputFallbackFolders.ShouldBeEquivalentTo(new[] { Path.Combine(testDir, "base"), Path.Combine(testDir, "m"), Path.Combine(testDir, "n"), Path.Combine(testDir, "s"), Path.Combine(testDir, "t") });
            }
        }

        [Fact]
        public void GetRestoreSettingsTask_VerifyNullPerFrameworkSettings()
        {
            using (var testDir = TestDirectory.Create())
            {
                // Arrange
                var buildEngine = new TestBuildEngine();
                var testLogger = buildEngine.TestLogger;

                var task = new GetRestoreSettingsTask()
                {
                    BuildEngine = buildEngine,
                    ProjectUniqueName = Path.Combine(testDir, "a.csproj"),
                    RestoreSources = new[] { Path.Combine(testDir, "base") },
                    RestoreFallbackFolders = new[] { Path.Combine(testDir, "base") },
                    RestoreSettingsPerFramework = null
                };

                // Act
                var result = task.Execute();

                // Assert
                result.Should().BeTrue();
                task.OutputSources.ShouldBeEquivalentTo(new[] { Path.Combine(testDir, "base") });
                task.OutputFallbackFolders.ShouldBeEquivalentTo(new[] { Path.Combine(testDir, "base") });
            }
        }

        [Fact]
        public void GetRestoreSettingsTask_VerifyEmptyPerFrameworkSettings()
        {
            using (var testDir = TestDirectory.Create())
            {
                // Arrange
                var buildEngine = new TestBuildEngine();
                var testLogger = buildEngine.TestLogger;

                var task = new GetRestoreSettingsTask()
                {
                    BuildEngine = buildEngine,
                    ProjectUniqueName = Path.Combine(testDir, "a.csproj"),
                    RestoreSources = new[] { Path.Combine(testDir, "base") },
                    RestoreFallbackFolders = new[] { Path.Combine(testDir, "base") },
                    RestoreSettingsPerFramework = new ITaskItem[0]
                };

                // Act
                var result = task.Execute();

                // Assert
                result.Should().BeTrue();
                task.OutputSources.ShouldBeEquivalentTo(new[] { Path.Combine(testDir, "base") });
                task.OutputFallbackFolders.ShouldBeEquivalentTo(new[] { Path.Combine(testDir, "base") });
            }
        }


        [Fact]
        public void GetRestoreSettingsTask_VerifyDisabledSourcesAreExcluded()
        {

            using (var testDir = TestDirectory.Create())
            {
                // Arrange
                var buildEngine = new TestBuildEngine();
                var testLogger = buildEngine.TestLogger;


                var configFile = Path.Combine(testDir, Settings.DefaultSettingsFileName);

                var projectDirectory = Path.GetDirectoryName(configFile);
                Directory.CreateDirectory(projectDirectory);

                File.WriteAllText(configFile, DisableSourceConfig);

                var task = new GetRestoreSettingsTask()
                {
                    BuildEngine = buildEngine,
                    ProjectUniqueName = Path.Combine(testDir, "a.csproj"),
                    RestoreFallbackFolders = new[] { Path.Combine(testDir, "base") },
                    RestoreSettingsPerFramework = new ITaskItem[0]
                };

                // Act
                var result = task.Execute();

                // Assert
                result.Should().BeTrue();
                task.OutputSources.ShouldBeEquivalentTo(new[] { @"https://nuget.org/v2/api" });
            }
        }

        private static string machineWideSettingsConfig = @"<?xml version=""1.0"" encoding=""utf-8""?>
                <configuration>
                </configuration>";

        private static string InnerConfig =
            @"<?xml version=""1.0"" encoding=""utf-8""?>
              <configuration>
                <SectionName>
                  <add key=""inner-key"" value=""inner-value"" />
                </SectionName>
              </configuration>";

        private static string OuterConfig =
            @"<?xml version=""1.0"" encoding=""utf-8""?>
              <configuration>
                <SectionName>
                  <add key=""outer-key"" value=""outer-value"" />
                </SectionName>
              </configuration>";

        private static string DisableSourceConfig =
            @"<?xml version=""1.0"" encoding=""utf-8""?>
             <configuration>
              <packageSources>
                <Clear/>
                <add key=""NuGet"" value=""https://api.nuget.org/v3/index.json"" />
                <add key=""NuGet.v2"" value=""https://nuget.org/v2/api"" />
              </packageSources>
              <disabledPackageSources>
                 <Clear/>
                 <add key=""NuGet"" value=""true"" />
              </disabledPackageSources>
            </configuration>";

    }
}
