@ECHO OFF
echo Installing MyService...
%SystemRoot%\Microsoft.NET\Framework64\v4.0.30319\InstallUtil.exe /i %~dp0SISPOSProxy.Service.exe
echo Done.
pause