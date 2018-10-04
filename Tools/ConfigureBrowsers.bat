REM -- baseado em https://wiki.mozilla.org/CA:AddRootToFirefox
REM ===================== END - Define your parameters =========================

IF EXIST "%programfiles(x86)%" (set firefox_dir="%programfiles(x86)%\Mozilla Firefox") ELSE (set firefox_dir="%programfiles%\Mozilla Firefox")
set autoconfig_file="%firefox_dir%\defaults\pref\autoconfig.js"
set firefox_cfg="%firefox_dir%\mozilla.cfg"

REM --- Adicionando configuração para utilizar o arquivo correto de configuração
find "general.config.filename" "%autoconfig_file%"
if %errorlevel% == 1 ECHO.pref("general.config.filename", "mozilla.cfg");>>"%autoconfig_file%"

find "%1" "%firefox_cfg%"
if %errorlevel% == 1 ECHO.cert=%1;>>"%firefox_cfg%" & ECHO.certdb.addCertFromBase64(cert, "C,C,C", "");>>"%firefox_cfg%"

exit 0