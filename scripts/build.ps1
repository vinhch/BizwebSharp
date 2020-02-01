function Join-Paths {
    if ($args.Count -eq 0) {
        throw "Must supply some arguments."
    }
    $output = $args[0]
    if ($args.Count -gt 1) {
        foreach($path in $args[1..($args.Count - 1)]) {
            $output = Join-Path $output -ChildPath $path
        }
    }
    return $output
}

function Write-SeparateLine([string]$msg) {
    Write-Host "`n------------------------------ $msg ------------------------------`n"
}

$rootFolder = Join-Paths $PSScriptRoot ".."
$projectFile = Join-Paths $rootFolder "src" "BizwebSharp" "BizwebSharp.csproj"
$artifactFolder = Join-Paths $rootFolder "artifact"

if (Test-Path $artifactFolder -PathType Container) {
    Remove-Item $artifactFolder -Recurse
}
$null = New-Item -ItemType Directory -Force -Path $artifactFolder

# build configuration {Debug|Release}
$buildConfig = "Release"

Write-SeparateLine "Clean projects"
$null = dotnet clean -c $buildConfig $projectFile

Write-SeparateLine "Restore dependencies"
dotnet restore $projectFile

Write-SeparateLine "Build and pack"
dotnet build -c $buildConfig $projectFile
dotnet pack --no-build -c Release -o $artifactFolder $projectFile

$nupkg = (Get-ChildItem (Join-Paths $artifactFolder "*.nupkg"))[0];
Write-Host $nupkg
