# `dotnet-releaser` User Guide

- [0) General usage](#0-general-usage)
- [1) Commands](#1-commands)
  - [1.1) `dotnet-releaser new`](#11-dotnet-releaser-new)
  - [1.2) `dotnet-releaser build`](#12-dotnet-releaser-build)
  - [1.3) `dotnet-releaser publish`](#13-dotnet-releaser-publish)
- [2) Configuration](#2-configuration)
  - [2.1) General](#21-general)
  - [2.2) MSBuild](#22-msbuild)
  - [2.3) GitHub](#23-github)
  - [2.4) Packaging](#24-packaging)
  - [2.5) NuGet](#25-nuget)
  - [2.6) Homebrew](#26-homebrew)
  - [2.7) Changelog](#27-changelog)

## 0) General usage

Get some help by typing `dotnet-releaser --help`

```
dotnet-releaser 0.1.0 - 2022 (c) Copyright Alexandre Mutel

Usage: dotnet-releaser [command] [options]

Options:
  --version     Show version information.
  -?|-h|--help  Show help information.

Commands:
  build         Build only the project.
  new           Create a dotnet-releaser TOML configuration file for a specified project.
  publish       Build and publish the project.

Run 'dotnet-releaser [command] -?|-h|--help' for more information about a command.
```
## 1) Commands

### 1.1) `dotnet-releaser new`

```
Create a dotnet-releaser TOML configuration file for a specified project.

Usage: dotnet-releaser new [options] <dotnet-releaser.toml>

Arguments:
  dotnet-releaser.toml      TOML configuration file path to create. Default is: dotnet-releaser.toml

Options:
  --project <project_file>  A - relative - path to project file (csproj, vbproj, fsproj)
  --user <GitHub_user/org>  The GitHub user/org where the packages will be published
  --repo <GitHub_repo>      The GitHub repo name where the packages will be published
  --force                   Force overwriting the existing TOML configuration file.
  -?|-h|--help              Show help information.
```

Example:

```shell
$ dotnet-releaser new --project HelloWorld.csproj --user xoofx --repo HelloWorld
``` 

The command above will create a `dotnet-releaser.toml` configuration file. See [](#)


### 1.2) `dotnet-releaser build`

```
Build only the project.

Usage: dotnet-releaser build [options] <dotnet-releaser.toml>

Arguments:
  dotnet-releaser.toml  TOML configuration file

Options:
  --force               Force deleting and recreating the artifacts folder.
  -?|-h|--help          Show help information.
```

Example:

```shell
$ dotnet-releaser build --project HelloWorld.csproj --user xoofx --repo HelloWorld
``` 

### 1.3) `dotnet-releaser publish`

```
Build and publish the project.

Usage: dotnet-releaser publish [options] <dotnet-releaser.toml>

Arguments:
  dotnet-releaser.toml    TOML configuration file

Options:
  --github-token <token>  GitHub Api Token. Required if publish to GitHub is true in the config file
  --nuget-token <token>   NuGet Api Token. Required if publish to NuGet is true in the config file
  --force                 Force deleting and recreating the artifacts folder.
  -?|-h|--help            Show help information.
```

## 2) Configuration

The configuration is all done with a configuration file in the [TOML](https://toml.io/en/) format.

### 2.1) General

The following properties can only be set before any of the sections (e.g `[msbuild]`, `[nuget]`...)

___
> `profile` : string

Defines which packs are created by default. See [packaging](#24-packaging) for more details.

```toml
# This is the default, creating all the OS/CPU/Packages listed on the front readme.md
profile = "default"
# This will make no default packs. You need to configure them manually
profile = "custom"
```
___
> `artifacts_folder` : string

Defines to which folder to output created packages. By default it is setup to `artifacts-dotnet-releaser` relative to where to TOML configuration file is.

```toml
# This is the default, creating all the OS/CPU/Packages listed on the front readme.md
artifacts_folder = "myfolder"
```
___
### 2.2) MSBuild

This section defines:

- The application project to build. This **property is mandatory**. There is no default!
- The MSBuild configuration (e.g `Debug` or `Release`). Default is `Release`
- Additional MSBuild properties

Example:

```toml
# MSBuild section
[msbuild]
project = "../Path/To/My/Project.csproj"
configuration = "Release"
[msbuild.properties]
PublishReadyToRun = false # Disable PublishReadyToRun
```
___
> `msbuild.project` : string

Specifies the path to the project to compile with MSBuild. If this path uses a relative path, it will be relative to the location of your TOML configuration file.

___
> `msbuild.configuration` : string

Specifies the MSBuild `Configuration` property. By default this is set to `Release`.

___
> `[msbuild.properties]`

By default, `dotnet-releaser` is using the following MSBuild defaults for configuring your application as a single file/self contained application:

```toml
# Default values used by `dotnet-releaser`
[msbuild.properties]
PublishTrimmed = true
PublishSingleFile = true
SelfContained = true
PublishReadyToRun = true 
CopyOutputSymbolsToPublishDirectory = false
SkipCopyingSymbolsToOutputDirectory = true 
``` 

But you can completely override these property values.
___
### 2.3) GitHub

In order to publish to GitHub, you need to define at least the actual user and repo of your command line application.

Example:

```toml
[github]
user = "xoofx"
repo = "dotnet-releaser"
# base = "https://github.com"
# version_prefix = "v"
``` 
___
> `github.user` : string

Defines the user or organization on your GitHub server.

___
> `github.repo` : string

Defines the repository under your user or organization on your GitHub server.

___
> `github.base` : string

Defines the base URL for your GitHub server. By default, it is using the public GitHub repository `https://github.com`

___
> `github.version_prefix` : string

Defines the prefix to add to the package version in order to find the associated tag release on GitHub. By default, there is no prefix defined (so the package version must be == the GitHub tag).

Usually, it can require that you setup a `v` on your prefix in case your GitHub tags are prefixed by this letter (e.g `v1.0.0`).

```toml
[github]
user = "xoofx"
repo = "dotnet-releaser"
version_prefix = "v"
``` 
___
### 2.4) Packaging

This is where you define the matrix of all the OS, CPUs and archives/packages you want to cross-compile and generate.

The kind of packaging can be define at the top level with the `profile` property.

> `profile = "default"`

This defines the packaging to use the default OS/CPU/packages.

```toml
## This is the default no need to specify it
profile = "default"
```

For this profile, it will use by default several packs pre-configured.

A `[[pack]]` in the TOML configuration is defined by:

- A RuntimeIdentifier aka a `rid`: See [https://docs.microsoft.com/en-us/dotnet/core/rid-catalog](https://docs.microsoft.com/en-us/dotnet/core/rid-catalog) for all the possible values.
- `kinds` of package:
  - `zip`: Creates a zip archive. 
  - `tar`: Creates a tar.gz archive. 
  - `deb`: Creates a Debian package.
  - `rpm`: Creates a Redhat package. 

```toml
# Default targets for Windows
[[pack]]
rid = ["win-x64", "win-arm", "win-arm64"]
kinds = ["zip"]
# Default targets for Linux/Ubuntu Debian compatible distro
[[pack]]
rid = ["linux-x64", "linux-arm", "linux-arm64"]
kinds = ["deb", "tar"]
# Default targets for Linux/Redhat compatible distro
[[pack]]
rid = ["rhel-x64"]
kinds = ["rpm", "tar"]
# Default targets for macOS
[[pack]]
rid = ["osx-x64", "osx-arm64"]
kinds = ["tar"]
```
You can decide to override a specific `rid`, e.g 

For example, defining this pack with the default profile:

```toml
[[pack]]
rid = ["win-x64"]
kinds = ["zip", "tar"]
```

Will only override the `win-x64` to generate a `zip` and a `tar.gz`

___
> `pack.publish` : bool

You can disable a particular pack to be build/published.

```toml
[[pack]]
publish = false
rid = ["win-x64"]
kinds = ["zip"]
```

By default, all packs declared are `publish = true`.

___
> `profile = "custom"`

If you want to disable the list of default packages, you can use the custom profile.

In that case, you need to define all packs manually as described above. 

You can for example target e.g `win-x86` which is not generated by default.

But you can also extend the default profile by just defining this rid. It's up to what is more convenient for your setup!
### 2.5) NuGet

Allow to publish to a NuGet registry. By default it is on and publishing to the official NuGet public registry.

___
> `nuget.source` : string

By default the publish NuGet registry is used `https://api.nuget.org/v3/index.json`, but you can override it with your own registry:

```toml
[nuget]
source = "https://my.special.registry.nuget.org/v3/index.json"
# publish = false
```
___
> `nuget.publish` : bool

Allow to disable publishing to NuGet:

```toml
[nuget]
publish = false
```
### 2.6) Homebrew

By default, a Homebrew repository and formula will be created if a `tar` file is generated for either a Linux or MacOS platform.

You can disable Homebrew support via

```toml
[brew]
publish = false
```

If your application name is `my-application`, and your GitHub user `xyz`, it will create and update automatically a repository at `https://github.com/xyz/homebrew-my-application`.

This repository will contain:

- a top level `readme.md`
- a folder `Formula`
- a Homebrew Ruby file `Formula/my-application.rb` that contains the formula with all the tar files that can be installed.

See for example the generated [Homebrew repository for grpc-curl](https://github.com/xoofx/homebrew-grpc-curl)

### 2.7) Changelog

`dotnet-releaser` can automatically transfer your changelog from a `changelog.md` to your GitHub release for the specific version of the package published.

By default, it will try to search a `changelog.md` file in the upper directories of your TOML configuration file.

Then it will parse the file to search for a default regex for a header `^##\s+v?((\d+\.)*(\d+))`

For example, if your changelog is setup like this:

```md

# Changelog

## 1.3.1 (27 Oct 2021)

### Fixes
- Fix for this annoying bug...

### Breaking changes
- ...
``` 

If you are publishing the `1.3.1` version of your package, it will extract the markdown after the `## 1.3.1` header:

```md
### Fixes
- Fix for this annoying bug...

### Breaking changes
- ...
```

And this will be uploaded to your tag release.

___
> `changelog.publish` : bool

You can disable entirely changelog support:

```toml
[changelog]
publish = false
```
___
> `changelog.path` : string

Override the default path to the changelog. Can be relative to the TOML configuration file. By default, `dotnet-releaser` tries to look up in higher directory for a file `changelog.md`

```toml
[changelog]
path = "this/is/my/path/to/my/changelog.md"
```

___
> `changelog.version` : string

Overrides the default regex that will be used to match the Markdown header and look for the exact version:

```toml
[changelog]
version = `^##\s+v?((\d+\.)*(\d+))` # This is the default
```