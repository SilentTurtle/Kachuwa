Clear-Host
 
 
$baseDirectory = $PSScriptRoot 
$solutionFilesPath = "$baseDirectory\Kachuwa.sln"
$webProjectPath="$baseDirectory\KachuwaApp\KachuwaApp.csproj"
$publishedDir = "$baseDirectory\published"
$loggerPath="$baseDirectory\publog"
$MSBuildLogger="/flp1:Append;LogFile=Build.log;Verbosity=Normal; /flp2:LogFile=BuildErrors.log;Verbosity=Normal;errorsonly"


    if (!(Test-Path -path $loggerPath)) {

        md $loggerPath
    }

 
    if(Test-Path $webProjectPath) 
    {
                    
            # Start-Sleep -s 2
            Write-Host "Waiting 2 second after clean" -ForegroundColor Red -BackgroundColor White
               
                
            Write-Host "publisheing $webProjectPath"
               # dotnet msbuild $webProjectPath /verbosity:quiet /nologo /target:publish /p:RuntimeIdentifiers=win10-x64 /p:Configuration=Release  OutputPath=../artifacts "/flp1:logfile=$loggerPath\msbuild.log;Verbosity=Normal;Append;" "/flp2:logfile=$loggerPath\errors.txt;errorsonly;Append;"
            dotnet msbuild WebApp/WebApp.csproj /verbosity:quiet /nologo /target:publish "/p:RuntimeIdentifiers=win10-x64;Configuration=Release;OutputPath=../artifacts"  "/flp1:logfile=$loggerPath\msbuild.log;Verbosity=Normal;Append;" "/flp2:logfile=$loggerPath\errors.txt;errorsonly;Append;"
            if($LASTEXITCODE -eq 0)
            {
                Write-Host "Published.." -ForegroundColor Green
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
        Write-Host "File does not exist : $webProjectPath"
        Start-Sleep -s 5
              
    }
       
        
    
