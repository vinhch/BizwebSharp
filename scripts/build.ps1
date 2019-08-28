$rootFolder = Resolve-Path "$PSScriptRoot/.."
$projectFile = "$rootFolder/src/BizwebSharp/BizwebSharp.csproj"
$artifactFolder = "$rootFolder/artifact"

if (Test-Path $artifactFolder -PathType Container) {
    Remove-Item $artifactFolder -Recurse
}
New-Item -ItemType Directory -Force -Path $artifactFolder

dotnet clean -c Release $projectFile
dotnet restore $projectFile
dotnet build -c Release $projectFile
dotnet pack --no-build -c Release -o $artifactFolder $projectFile

$nupkg = (Get-ChildItem $artifactFolder/*.nupkg)[0];
Write-Host $nupkg