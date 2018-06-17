@echo off
echo Uninstalling MyService...
%SystemRoot%\Microsoft.NET\Framework64\v4.0.30319\InstallUtil.exe /u %~dp0SISPOSProxy.Service.exe
echo Done.
pause