
SET builddir=%~dp0
SET EX="C:\Program Files\IIS Express\iisexpress.exe"
CALL %EX% /path:%builddir%WebApp /port:8880
