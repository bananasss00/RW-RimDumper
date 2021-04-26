echo off

REM remove unnecessary assemblies
@REM DEL .\*\Assemblies\*.*

REM build dll
dotnet build /property:GenerateFullPaths=true /consoleloggerparameters:NoSummary .vscode\Mod.csproj

if not exist "Assemblies\EPPlus-unity.dll" (
    copy "Source\.packages\EPPlus-unity.dll" "Assemblies\EPPlus-unity.dll"
)