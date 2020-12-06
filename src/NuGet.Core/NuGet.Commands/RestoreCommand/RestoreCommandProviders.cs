// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using NuGet.Common;
using NuGet.DependencyResolver;
using NuGet.Protocol;
using NuGet.Protocol.Core.Types;
using NuGet.Repositories;

namespace NuGet.Commands
{
    /// <summary>
    /// Feed providers
    /// </summary>
    public class RestoreCommandProviders
    {
        /// <summary>
        /// Providers used by the restore command. These can be shared across restores.
        /// </summary>
        /// <param name="globalPackages">Path to the global packages folder.</param>
        /// <param name="fallbackPackageFolders">Path to any fallback package folders.</param>
        /// <param name="localProviders">This is typically just a provider for the global packages folder.</param>
        /// <param name="remoteProviders">All dependency providers.</param>
        /// <param name="packageFileCache">Nuspec and package file cache.</param>
        /// <param name="restoreCommandCache">Graph cache shared between restores.</param>
        public RestoreCommandProviders(
            NuGetv3LocalRepository globalPackages,
            IReadOnlyList<NuGetv3LocalRepository> fallbackPackageFolders,
            IReadOnlyList<IRemoteDependencyProvider> localProviders,
            IReadOnlyList<IRemoteDependencyProvider> remoteProviders,
            LocalPackageFileCache packageFileCache,
            RestoreCommandCache restoreCommandCache)
        {
            GlobalPackages = globalPackages ?? throw new ArgumentNullException(nameof(globalPackages));
            LocalProviders = localProviders ?? throw new ArgumentNullException(nameof(localProviders));
            RemoteProviders = remoteProviders ?? throw new ArgumentNullException(nameof(remoteProviders));
            FallbackPackageFolders = fallbackPackageFolders ?? throw new ArgumentNullException(nameof(fallbackPackageFolders));
            PackageFileCache = packageFileCache ?? throw new ArgumentNullException(nameof(packageFileCache));
            RestoreCommandCache = restoreCommandCache ?? throw new ArgumentNullException(nameof(restoreCommandCache));
        }

        /// <summary>
        /// A <see cref="NuGetv3LocalRepository"/> repository may be passed in as part of the request.
        /// This allows multiple restores to share the same cache for the global packages folder
        /// and reduce disk hits.
        /// </summary>
        public NuGetv3LocalRepository GlobalPackages { get; }

        public IReadOnlyList<NuGetv3LocalRepository> FallbackPackageFolders { get; }

        public IReadOnlyList<IRemoteDependencyProvider> LocalProviders { get; }

        public IReadOnlyList<IRemoteDependencyProvider> RemoteProviders { get; }

        public LocalPackageFileCache PackageFileCache { get; }

        public RestoreCommandCache RestoreCommandCache { get; }

        public static RestoreCommandProviders Create(
            string globalFolderPath,
            IEnumerable<string> fallbackPackageFolderPaths,
            IEnumerable<SourceRepository> sources,
            SourceCacheContext cacheContext,
            LocalPackageFileCache packageFileCache,
            RestoreCommandCache restoreCommandCache,
            ILogger log)
        {
            var globalPackages = new NuGetv3LocalRepository(globalFolderPath, packageFileCache);
            var globalPackagesSource = Repository.Factory.GetCoreV3(globalFolderPath, FeedType.FileSystemV3);

            var localProviders = new List<IRemoteDependencyProvider>()
            {
                // Do not throw or warn for global cache
                new SourceRepositoryDependencyProvider(
                    globalPackagesSource,
                    log,
                    cacheContext,
                    ignoreFailedSources: true,
                    ignoreWarning: true,
                    fileCache: packageFileCache)
            };

            // Add fallback sources as local providers also
            var fallbackPackageFolders = new List<NuGetv3LocalRepository>();

            foreach (var path in fallbackPackageFolderPaths)
            {
                var fallbackRepository = new NuGetv3LocalRepository(path, packageFileCache);
                var fallbackSource = Repository.Factory.GetCoreV3(path, FeedType.FileSystemV3);

                var provider = new SourceRepositoryDependencyProvider(
                    fallbackSource,
                    log,
                    cacheContext,
                    ignoreFailedSources: false,
                    ignoreWarning: false,
                    fileCache: packageFileCache);

                fallbackPackageFolders.Add(fallbackRepository);
                localProviders.Add(provider);
            }

            var remoteProviders = new List<IRemoteDependencyProvider>();

            foreach (var source in sources)
            {
                var provider = new SourceRepositoryDependencyProvider(
                    source,
                    log,
                    cacheContext,
                    cacheContext.IgnoreFailedSources,
                    ignoreWarning: false,
                    fileCache: packageFileCache);

                remoteProviders.Add(provider);
            }

            return new RestoreCommandProviders(
                globalPackages,
                fallbackPackageFolders,
                localProviders,
                remoteProviders,
                packageFileCache,
                restoreCommandCache);
        }
    }
}
