﻿namespace DotNetReleaser;

/// <summary>
/// Constants associated with the MSBuild target file app-releaser.targets
/// </summary>
static class ReleaserConstants
{
    public const string DotNetReleaserFileName = "dotnet-releaser.targets";

    public const string DotNetReleaserPackAndGetNuGetPackOutput = nameof(DotNetReleaserPackAndGetNuGetPackOutput);
    public const string DotNetReleaserGetPackageInfo = nameof(DotNetReleaserGetPackageInfo);
    public const string DotNetReleaserPublishAndCreateDeb = nameof(DotNetReleaserPublishAndCreateDeb);
    public const string DotNetReleaserPublishAndCreateRpm = nameof(DotNetReleaserPublishAndCreateRpm);
    public const string DotNetReleaserPublishAndCreateTar = nameof(DotNetReleaserPublishAndCreateTar);
    public const string DotNetReleaserPublishAndCreateZip = nameof(DotNetReleaserPublishAndCreateZip);
    public const string DotNetReleaserPublishAndCreateSetup = nameof(DotNetReleaserPublishAndCreateSetup);

    public const string ItemSpecKind = "Kind";
    public const string PackageId = nameof(PackageId);
    public const string PackageVersion = nameof(PackageVersion);
    public const string PackageDescription = nameof(PackageDescription);
    public const string PackageLicenseExpression = nameof(PackageLicenseExpression);
    public const string PackageOutputType = nameof(PackageOutputType);
    public const string PackageProjectUrl = nameof(PackageProjectUrl);
}