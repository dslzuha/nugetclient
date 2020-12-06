// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NuGet.Common;
using NuGet.Packaging;
using NuGet.Packaging.Signing;
using NuGet.Protocol;
using static NuGet.Commands.VerifyArgs;

namespace NuGet.Commands
{
    /// <summary>
    /// Command Runner used to run the business logic for nuget verify command
    /// </summary>
    public class VerifyCommandRunner : IVerifyCommandRunner
    {
        private const int SuccessCode = 0;
        private const int FailureCode = 1;

        public async Task<int> ExecuteCommandAsync(VerifyArgs verifyArgs)
        {
            if (verifyArgs.Verifications.Count == 0)
            {
                verifyArgs.Logger.LogError(string.Format(CultureInfo.CurrentCulture, Strings.VerifyCommand_VerificationTypeNotSupported));
                return FailureCode;
            }

            var errorCount = 0;

            if (ShouldExecuteVerification(verifyArgs, Verification.Signatures))
            {
                var packagesToVerify = LocalFolderUtility.ResolvePackageFromPath(verifyArgs.PackagePath);
                LocalFolderUtility.EnsurePackageFileExists(verifyArgs.PackagePath, packagesToVerify);

                var allowListEntries = verifyArgs.CertificateFingerprint.Select(fingerprint =>
                    new CertificateHashAllowListEntry(VerificationTarget.Primary, fingerprint)).ToList();

                var verificationProviders = SignatureVerificationProviderFactory.GetSignatureVerificationProviders(new SignatureVerificationProviderArgs(allowListEntries));
                var verifier = new PackageSignatureVerifier(verificationProviders, SignedPackageVerifierSettings.VerifyCommandDefaultPolicy);

                foreach (var package in packagesToVerify)
                {
                    try
                    {
                        errorCount += await VerifySignatureForPackageAsync(package, verifyArgs.Logger, verifier);
                    }
                    catch (InvalidDataException e)
                    {
                        verifyArgs.Logger.LogError(string.Format(CultureInfo.CurrentCulture, Strings.VerifyCommand_PackageIsNotValid, package));
                        ExceptionUtilities.LogException(e, verifyArgs.Logger);
                    }
                }
            }

            return errorCount == 0 ? SuccessCode : FailureCode;
        }

        private async Task<int> VerifySignatureForPackageAsync(string packagePath, ILogger logger, PackageSignatureVerifier verifier)
        {
            var result = 0;
            using (var packageReader = new PackageArchiveReader(packagePath))
            {
                var verificationResult = await verifier.VerifySignaturesAsync(packageReader, CancellationToken.None);

                var packageIdentity = packageReader.GetIdentity();

                logger.LogInformation(Environment.NewLine + string.Format(CultureInfo.CurrentCulture,
                    Strings.VerifyCommand_VerifyingPackage,
                    packageIdentity.ToString()));
                logger.LogInformation($"{packagePath}{Environment.NewLine}");

                var logMessages = verificationResult.Results.SelectMany(p => p.Issues).Select(p => p.ToLogMessage()).ToList();
                await logger.LogMessagesAsync(logMessages);

                if (logMessages.Any(m => m.Level >= LogLevel.Warning))
                {
                    var errors = logMessages.Count(m => m.Level == LogLevel.Error);
                    var warnings = logMessages.Count(m => m.Level == LogLevel.Warning);

                    logger.LogInformation(string.Format(CultureInfo.CurrentCulture, Strings.VerifyCommand_FinishedWithErrors, errors, warnings));

                    result = errors;
                }

                if (verificationResult.Valid)
                {
                    logger.LogInformation(Environment.NewLine + Strings.VerifyCommand_Success);
                }
                else
                {
                    logger.LogError(Environment.NewLine + Strings.VerifyCommand_Failed);
                }

                return result;
            }
        }

        private bool ShouldExecuteVerification(VerifyArgs args, Verification v)
        {
            return args.Verifications.Any(verification => verification == Verification.All || verification == v);
        }
    }
}
