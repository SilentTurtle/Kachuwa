
SET builddir=%~dp0
SET EX="C:\Program Files\IIS Express\iisexpress.exe"
CALL %EX% /path:%builddir%WebApp\bin\Release\netcoreapp1.1 /port:8880
pause
