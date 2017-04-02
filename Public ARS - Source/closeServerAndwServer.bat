@echo off
FOR /F "tokens=4 delims= " %%P IN ('netstat -a -n -o ^| findstr :MY_SERVER_PORT') DO @ECHO TaskKill.exe /PID %%P
FOR /F "tokens=4 delims= " %%P IN ('netstat -a -n -o ^| findstr :MY_WSERVER_PORT') DO @ECHO TaskKill.exe /PID %%P
taskkill /f /im server.exe
taskkill /f /im wServer.exe