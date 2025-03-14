rm -R ~/.nuget/packages/tailwind.hosting
rm -R ~/.nuget/packages/tailwind.hosting.build
rm -R ~/.nuget/packages/tailwind.hosting.cli

rm -R ./examples/Blazor/bin/
rm -R ./examples/Blazor/obj/
rm -R ./nupkgs/*

dotnet clean
dotnet remove examples/Blazor package Tailwind.Hosting
dotnet remove examples/Blazor package Tailwind.Hosting.Build

dotnet build
dotnet pack Tailwind.Hosting
dotnet pack Tailwind.Hosting.Build
dotnet pack Tailwind.Hosting.Cli

dotnet add examples/Blazor package Tailwind.Hosting
dotnet add examples/Blazor package Tailwind.Hosting.Build
