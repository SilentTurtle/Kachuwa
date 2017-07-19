Clear-Host
 
 
$baseDirectory = $PSScriptRoot 
$solutionFilesPath = "$baseDirectory\Kachuwa.sln"

$loggerPath="$baseDirectory\buildlog"
$MSBuildLogger="/flp1:Append;LogFile=Build.log;Verbosity=Normal; /flp2:LogFile=BuildErrors.log;Verbosity=Normal;errorsonly"

$msbuild = "dotnet msbuild "

#$nuget="$baseDirectory\nuget\nuget.exe"
#if ((Test-Path -path $nuget)) {
#Write-Host "Restoring nuget packages..."
#    & $nuget restore $solutionFilesPath
#}

if (!(Test-Path -path $loggerPath)) {
md $loggerPath
}

    if ($solutionFilesPath.EndsWith(".sln")) 
    {
        $projectFileAbsPath =$solutionFilesPath # "$baseDirectory\$projectFile"
        
        $filename = [System.IO.Path]::GetFileName($projectFile); 
       
            if(Test-Path $projectFileAbsPath) 
            {
                # Clean the solution
              #  & $msbuild $projectFileAbsPath /target:clean /p:Configuration=Release

               # Start-Sleep -s 2
                Write-Host "Waiting 2 second after clean" -ForegroundColor Red -BackgroundColor White
               
                
                Write-Host "Building $projectFileAbsPath"
                dotnet msbuild $projectFileAbsPath /verbosity:quiet /nologo /target:build /p:Configuration=Release "/flp1:logfile=$loggerPath\msbuild.log;Verbosity=Normal;Append;" "/flp2:logfile=$loggerPath\errors.txt;errorsonly;Append;"
                #& $devenv $projectFileAbsPath /Rebuild
                #/p:PrecompileBeforePublish=true /p:UseMerge=true /p:SingleAssemblyName=AppCode /p:DeployOnBuild=true /p:WebPublishMethod=Package /p:PackageAsSingleFile=true /p:SkipInvalidConfigurations=true /p:PackageLocation="$(build.artifactstagingdirectory)\"
                
                if($LASTEXITCODE -eq 0)
                {
                    Write-Host "Build SUCCESS" -ForegroundColor Green
                   # Clear-Host
                    break
                }
                else
                {
                    Write-Host "Build FAILED" -ForegroundColor Red
                  
                }
            }
            else
            {
                Write-Host "File does not exist : $projectFileAbsPath"
                Start-Sleep -s 5
              
            }
       
        
    }
