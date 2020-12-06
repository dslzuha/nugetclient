Param(
    [Parameter(Mandatory = $True)]
    [string] $nugetClientFilePath,
    [Parameter(Mandatory = $True)]
    [string] $sourceRootFolderPath,
    [Parameter(Mandatory = $True)]
    [string] $resultsFolderPath,
    [Parameter(Mandatory = $True)]
    [string] $logsFolderPath,
    [int] $iterationCount
)

. "$PSScriptRoot\..\PerformanceTestUtilities.ps1"

$repoUrl = "https://github.com/OrchardCMS/OrchardCore.git"
$testCaseName = GenerateNameFromGitUrl $repoUrl
$resultsFilePath = [System.IO.Path]::Combine($resultsFolderPath, "$testCaseName.csv")

RunPerformanceTestsOnGitRepository -nugetClientFilePath $nugetClientFilePath -sourceRootFolderPath $sourceRootFolderPath -testCaseName $testCaseName -repoUrl $repoUrl -commitHash "991ff7b536811c8ff2c603e30d754b858d009fa2" -resultsFilePath $resultsFilePath -logsFolderPath $logsFolderPath -iterationCount $iterationCount