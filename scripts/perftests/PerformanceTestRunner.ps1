Param(
    [Parameter(Mandatory = $true)]
    [string]$resultsDirectoryPath,
    [string[]]$nugetClients,
    [string]$testDirectoryPath,
    [string]$logsDirectoryPath,
    [switch]$SkipCleanup
)

    . "$PSScriptRoot\PerformanceTestUtilities.ps1"

    if (![string]::IsNullOrEmpty($testDirectoryPath) -And $(GetAbsolutePath $resultsDirectoryPath).StartsWith($(GetAbsolutePath $testDirectoryPath))) 
    {
        Log "$resultsDirectoryPath cannot be a subdirectory of $testDirectoryPath" "red"
        exit(1)
    }
    
    if ([string]::IsNullOrEmpty($testDirectoryPath)) 
    {
        $testDirectoryPath = [System.IO.Path]::Combine($env:TEMP, "np")
    }

    Log "Clients: $nugetClients" "green"

    try 
    {
        foreach($nugetClient in $nugetClients)
        {
            try 
            {
                Log "Running tests for $nugetClient" "green"

                if ([string]::IsNullOrEmpty($nugetClient) -Or !$(Test-Path $nugetClient)) 
                {
                    Log "The NuGet client at '$nugetClient' cannot be resolved." "yellow"
                    exit(1)
                }

                $testDirectoryPath = GetAbsolutePath $testDirectoryPath
                $resultsDirectoryPath = GetAbsolutePath $resultsDirectoryPath
                Log "Discovering the test cases." 
                $testFiles = $(Get-ChildItem $PSScriptRoot\testCases "Test-*.ps1" ) | ForEach-Object { $_.FullName }
                Log "Discovered test cases: $testFiles" "green"
                $testFiles | ForEach-Object {
                    $testCase = $_
                    try 
                    {
                    . $_ -nugetClient $nugetClient -sourceRootDirectory $([System.IO.Path]::Combine($testDirectoryPath, "source")) -resultsDirectoryPath $resultsDirectoryPath -logsPath $([System.IO.Path]::Combine($testDirectoryPath, "logs")) 
                    } 
                    catch 
                    {
                        Log "Problem running the test case $testCase with error $_" "red"
                    }
                }
            } 
            catch 
            {
                Log "Problem running the tests with $nugetClient" "red"
            }
        }
    }
    finally 
    {
        if (!$SkipCleanup) {
            Remove-Item -r -force $testDirectoryPath -ErrorAction Ignore > $null
        }
    }
