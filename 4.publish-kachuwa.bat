dotnet msbuild KachuwaApp/KachuwaApp.csproj /verbosity:quiet /nologo /target:publish /p:MvcRazorCompileOnPublish=false;RuntimeIdentifiers=win10-x64;Configuration=Release;OutputPath=../artifacts
pause