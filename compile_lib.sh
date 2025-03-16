PROJECT=$1

rm -R ~/.nuget/packages/tailwind.hosting
rm -R ~/.nuget/packages/tailwind.hosting.build
rm -R ~/.nuget/packages/tailwind.hosting.cli

rm -R "${PROJECT:?}/bin/"
rm -R "${PROJECT:?}/obj/"
rm -R ./nupkgs/*

dotnet clean
dotnet remove "$PROJECT" package Tailwind.Hosting
dotnet remove "$PROJECT" package Tailwind.Hosting.Build

dotnet build
dotnet pack Tailwind.Hosting
dotnet pack Tailwind.Hosting.Build
dotnet pack Tailwind.Hosting.Cli

dotnet add "$PROJECT" package Tailwind.Hosting
dotnet add "$PROJECT" package Tailwind.Hosting.Build
