Clear-Host
 
 
$baseDirectory = $PSScriptRoot 
$solutionFilesPath = "$baseDirectory\Kachuwa.sln"

dotnet clean $projectFileAbsPath --framework netcoreapp1.1  #--configuration * 

dotnet clean $projectFileAbsPath --framework netcoreapp1.1  --configuration Debug