@echo Installing self-signed certificate for client
@echo  You only need to do on this machine once.
@echo.

@REM Create and install the certificate
@makecert -sr LocalMachine -ss My -a sha1 -n CN=%COMPUTERNAME% -sky exchange -pe  %COMPUTERNAME%.cer

@REM Install it into "Trusted People" store of this machine
@certmgr.exe -add %COMPUTERNAME%.cer -r CurrentUser -s TrustedPeople

@echo  %COMPUTERNAME%.cer is created.
@echo.
@echo If the server is on different machine, import this certificate into the server Trusted People store.
