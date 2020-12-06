﻿// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Moq;
using NuGet.ProjectManagement;
using NuGet.VisualStudio;
using NuGet.VisualStudio.Telemetry;
using Xunit;

namespace NuGet.PackageManagement.VisualStudio.Test
{
    public class RestoreTelemetryServiceTests
    {
        [Theory]
        [InlineData(RestoreOperationSource.OnBuild, NuGetOperationStatus.Succeeded)]
        [InlineData(RestoreOperationSource.Explicit, NuGetOperationStatus.Succeeded)]
        [InlineData(RestoreOperationSource.OnBuild, NuGetOperationStatus.NoOp)]
        [InlineData(RestoreOperationSource.Explicit, NuGetOperationStatus.NoOp)]
        [InlineData(RestoreOperationSource.OnBuild, NuGetOperationStatus.Failed)]
        [InlineData(RestoreOperationSource.Explicit, NuGetOperationStatus.Failed)]
        public void RestoreTelemetryService_EmitRestoreEvent_OperationSucceed(RestoreOperationSource source, NuGetOperationStatus status)
        {
            // Arrange
            var telemetrySession = new Mock<ITelemetrySession>();
            TelemetryEvent lastTelemetryEvent = null;
            telemetrySession
                .Setup(x => x.PostEvent(It.IsAny<TelemetryEvent>()))
                .Callback<TelemetryEvent>(x => lastTelemetryEvent = x);

            string operationId = Guid.NewGuid().ToString();

            var noopProjectsCount = 0;

            if (status == NuGetOperationStatus.NoOp)
            {
                noopProjectsCount = 1;
            }

            var stausMessage = status == NuGetOperationStatus.Failed ? "Operation Failed" : string.Empty;
            var restoreTelemetryData = new RestoreTelemetryEvent(
                operationId: operationId,
                projectIds: new[] { Guid.NewGuid().ToString() },
                source: source,
                startTime: DateTimeOffset.Now.AddSeconds(-3),
                status: status,
                packageCount: 2,
                noOpProjectsCount: noopProjectsCount,
                endTime: DateTimeOffset.Now,
                duration: 2.10);
            var service = new RestoreTelemetryService(telemetrySession.Object);

            // Act
            service.EmitRestoreEvent(restoreTelemetryData);

            // Assert
            VerifyTelemetryEventData(restoreTelemetryData, lastTelemetryEvent);
        }

        private void VerifyTelemetryEventData(RestoreTelemetryEvent expected, TelemetryEvent actual)
        {
            Assert.NotNull(actual);
            Assert.Equal(TelemetryConstants.RestoreActionEventName, actual.Name);
            Assert.Equal(10, actual.Properties.Count);

            Assert.Equal(expected.Source.ToString(), actual.Properties[TelemetryConstants.OperationSourcePropertyName].ToString());

            Assert.Equal(expected.NoOpProjectsCount, (int)actual.Properties[TelemetryConstants.NoOpProjectsCountPropertyName]);

            TestTelemetryUtility.VerifyTelemetryEventData(expected, actual);
        }
    }
}
