@echo off
cls

IF NOT EXIST .paket\ ( mkdir .paket )

IF NOT EXIST .paket\paket.exe (

IF NOT EXIST .paket\paket.bootstrapper.exe (
copy ..\paket.bootstrapper.exe .paket\
if errorlevel 1 (
  exit /b %errorlevel%
)
)
.paket\paket.bootstrapper.exe

)

.paket\paket.exe install
if errorlevel 1 (
  exit /b %errorlevel%
)

