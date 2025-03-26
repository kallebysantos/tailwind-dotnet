![image](https://github.com/user-attachments/assets/ee3ec17a-2742-49a9-a77c-182e2a87e6dc)

------
# .NET 4.8 + Tailwindcss example

## Getting Started

> [!WARNING] 
> **Visual Studio users**: The VS IDE may "Not respond" if you try to install the package first, without add this pre-configuration.
> Using from Rider IDE is possible to do reverse path, install first then configure.

### Pre-Configurarion

First of all add the following section to your `.csproj` file
> If you're using Visual Studio you may need to edit it from notepad

```xml
<?xml version="1.0" encoding="utf-8"?>
<Project ToolsVersion="12.0" DefaultTargets="Build" xmlns="http://schemas.microsoft.com/developer/msbuild/2003">
  <PropertyGroup Label="Tailwind.Hosting.Build Props">
    <TailwindVersion>latest</TailwindVersion>
    <TailwindInputCssFile>Content/styles.css</TailwindInputCssFile>
    <TailwindOutputCssFile>Content/Site.css</TailwindOutputCssFile>
  </PropertyGroup>

  <!-- ...default config -->
</Project>
```

After that, create a `Content/styles.css` file with the following content:
```css
@import "tailwindcss";
```

### Installing the Tailwindcss integration
Now you need to add the following package:
- `Tailwind.Hosting.Build`

Check the `.csproj` file again and make sure that the `<PropertyGroup>` section introduced before is the first thing loaded, it should be something like this:
```xml
<?xml version="1.0" encoding="utf-8"?>
<Project ToolsVersion="12.0" DefaultTargets="Build" xmlns="http://schemas.microsoft.com/developer/msbuild/2003">
  <PropertyGroup Label="Tailwind.Hosting.Build Props">
    <!-- ...tailwind config -->
  </PropertyGroup>
  <Import Project="..\packages\Tailwind.Hosting.Build.1.2.2\build\Tailwind.Hosting.Build.props" Condition="Exists('..\packages\Tailwind.Hosting.Build.1.2.2\build\Tailwind.Hosting.Build.props')" />
  
<!-- ...default config -->
</Project>
```

### Hot-Reload
Unfortunetely `net4.8` projects doesn't have a good hot-reload support and since `Tailwind.Hosting` uses the `HOSTING STARTUP ASSEMBLIES` .NET Core feature, you may need an extra step to setup with. I suggest the following 2 alternatives

#### 1: Use the IDE's quick play button and fire (when it works)

![image](https://github.com/user-attachments/assets/eff766ef-aab2-4e14-a374-2fb0223463c6)

#### 2: Use a separated .NET Core in multiple startup
You can also create a separated .NET Core project with `Tailwind.Hosting` installed like [described here](https://github.com/kallebysantos/tailwind-dotnet#installing-the-tailwindcss-integration). 

Then add this project to startup:

*Visual Studio 2022*
```
Project right click > Configure Startup Projects > Multiple startup projects
```
*Rider*
```
Project right click > Run Multiple projects
```

Its recommended to edit the .NET Core's `.csproj` file to point the same `<PropertyGroup>` variables of the `WebForms` project. This way you ensures that it will find the `Content/styles.css` and `Content/Site.css`

