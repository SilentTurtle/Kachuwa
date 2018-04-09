Clear-Host
 
 
$baseDirectory = $PSScriptRoot 
$solutionFilesPath = "$baseDirectory\Kachuwa.sln"

dotnet clean $projectFileAbsPath --framework netcoreapp2.0  #--configuration * 

dotnet clean $projectFileAbsPath --framework netcoreapp2.0  --configuration Debug