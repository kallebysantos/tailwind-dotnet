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
This repository haves a pack toolset for tailwindcss integration with .Net that is based in 2 main packages, one for Hosting Startup and other for MsBuild.

## Instalation
Just install the packages with `dotnet add Tailwind.Hosting` and `dotnet add Tailwind.Hosting.Build`
Then add the following to your `Properties/launchSettings.json`
```json
"environmentVariables": {
    "ASPNETCORE_ENVIRONMENT": "Development",
    "ASPNETCORE_HOSTINGSTARTUPASSEMBLIES": "Tailwind.Hosting"
}
```
The available `MS build ` variables are:

| Property | Value |
|---|---|
| TailwindVersion | `latest` or custom value like `3.3.5`  |
| TailwindWatch | true |
| TailwindInputCssFile | {Project Folder}/wwwroot/styles.css |
| TailwindOutputCssFile | {Project Folder}/wwwroot/app.css |
| TailwindConfigFile | {Project Folder}/tailwind.config.js |

> These variables shoud be placed in your `.csproj` file.
