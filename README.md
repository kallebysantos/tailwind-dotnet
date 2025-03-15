<p align="center">
  <a href="https://tailwindcss.com" target="_blank">
    <picture>
      <source media="(prefers-color-scheme: dark)" srcset="https://raw.githubusercontent.com/tailwindlabs/tailwindcss/HEAD/.github/logo-dark.svg">
      <source media="(prefers-color-scheme: light)" srcset="https://raw.githubusercontent.com/tailwindlabs/tailwindcss/HEAD/.github/logo-light.svg">
      <img alt="Tailwind CSS" src="https://raw.githubusercontent.com/tailwindlabs/tailwindcss/HEAD/.github/logo-light.svg" width="350" height="70" style="max-width: 100%;">
    </picture>
  </a>
</p>

<p align="center">
  <a href="https://tailwindcss.com">Tailwindcss</a> Integration for .Net.
</p>

------
- [Installation](#installing-the-tailwindcss-integration)
- [Condiguration](#condiguration)
- [Examples](#examples)

This repository haves a pack toolset for tailwindcss integration with .Net that is based in 2 main packages, one for Hosting Startup and other for MsBuild.

## Getting Started

### Requirements:

You only need .NET, nothing more! - No `npm` neither `postcss` stuff

### Creating a new project

First of all let's create a new .Net 8 Blazor app
```bash
dotnet new blazor --empty -o BlazorWind -f net8.0
```
>Of course you can use this same guide for `mvc`, `razor` or even the legacy `webforms`

### Installing the Tailwindcss integration

Now you need to add 2 packages:

1. `Tailwind.Hosting`: Add support for *HotReload* when you execute `dotnet watch`
2. `Tailwind.Hosting.Build`: Integrates with the *MsBuild* compiler, so it will automatically setup the tailwindcss as well generate publish ready output on `dotnet publish`
```bash
dotnet add package Tailwind.Hosting
dotnet add package Tailwind.Hosting.Build
```
Finally to enable *HotReload* you only need to add the following to your `Properties/launchSettings.json`
```json
    "environmentVariables": {
        "ASPNETCORE_ENVIRONMENT": "Development",
        "ASPNETCORE_HOSTINGSTARTUPASSEMBLIES": "Tailwind.Hosting"
    }
````

### Tailwindcss setup

This last one is related with tailwind itself. Since `v4` you only need to add the following to a `wwwroot/styles.css` file:
```css
@import "tailwindcss";
```

## Configuration
The Tailwind integration will automatically use the .NET conventions and the available `MsBuild` variables are:
| Property | Value |
|---|---|
| TailwindVersion | `latest` or custom value like `3.3.5`  |
| TailwindWatch | true |
| TailwindInputCssFile | {Project Folder}/wwwroot/styles.css |
| TailwindOutputCssFile | {Project Folder}/wwwroot/app.css |
| TailwindConfigFile | {Project Folder}/tailwind.config.js |
| TailwindMinifyOnPublish | true |
| TailwindExcludeInputFileOnPublish | true |

All variables can be overwritten from `.csproj`
```xml
<PropertyGroup Label="Tailwind.Hosting.Build Props">
    <TailwindVersion>latest</TailwindVersion>
    <TailwindWatch>true</TailwindWatch>

    <TailwindInputCssFile>wwwroot/styles.css</TailwindInputCssFile>
    <TailwindOutputCssFile>wwwroot/app.css</TailwindOutputCssFile>
    <TailwindConfigFile>tailwind.config.js</TailwindConfigFile>

    <TailwindMinifyOnPublish>true</TailwindMinifyOnPublish>
    <TailwindExcludeInputFileOnPublish>true</TailwindExcludeInputFileOnPublish>
</PropertyGroup>
```

## Examples
You can find more in `examples` folder:
- [Blazor example](examples/Blazor/Blazor.csproj)

