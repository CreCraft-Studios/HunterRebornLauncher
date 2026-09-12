@echo off
title Hunter Reborn Setup

net session >null 2>&1
if %errorlevel% neq 0 (
	echo Setup file must run with Administrator.
	echo Right-click the batch file and select "Run as administrator"
	pause

	exit /b 1
)

echo Editing hosts file
echo This can take time as each entry needs to wait for the previous entry to finish

set loc=%SystemRoot%\System32\drivers\etc\hosts

echo #theHunter Classic domain redirects>>"%loc%"
call :add "api.thehunter.com"
call :add "gameserver.thehunter.com"
call :add "apiv2.thehunter.com"
call :add "thehunter.com"
call :add "www.thehunter.com"
call :add "static.thehunter.com"
call :add "signup.thehunter.com"
call :add "avatar.thehunter.com"
call :add "photo.thehunter.com"
call :add "map.thehunter.com"
call :add "tracking2.avalanchestudios.com"
call :add "static.thehunterbeta.com"
call :add "apiv2.thehunterbeta.com"
call :add "oldtracking.avalanchestudios.com"
call :add "metrics.avalanchestudios.com"

echo Setup Complete
pause
exit /b 0

:add
set "domain=%~1"

findstr /i /r /c:"^[ ]*127\.0\.0\.1[ ]*%domain%[ ]*$" "%loc%" >nul 2>&1

if %errorlevel% neq 0 (
	echo 127.0.0.1 %domain%>>"%loc%"
	echo Added domain %domain%
	timeout /t 1 /nobreak >nul
) else (
	echo %domain% already exists
)
