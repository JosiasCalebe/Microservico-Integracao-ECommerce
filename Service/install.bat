@echo off
    setlocal EnableDelayedExpansion
    set SERVICE_NAME=ServiceIntegracao
    set SERVICE_FULL_NAME=ServiceIntegracao.exe
rem Verifica se o servi�o est� rodando
    echo Verificando se o %SERVICE_NAME% est� instalado
    sc query %SERVICE_NAME% | find /i "%SERVICE_NAME%"
rem Se existe o servi�o
    if "%ERRORLEVEL%"=="0" (
       goto :Fim
    )
    echo Instalando o servi�o %SERVICE_NAME%
    sc create %SERVICE_NAME% start= auto binPath= "C:\WindowsServicesApi\%SERVICE_FULL_NAME%" DisplayName= "%SERVICE_NAME%"
rem Inicia o servi�o
    echo Iniciando o servi�o %SERVICE_NAME%
    sc start %SERVICE_NAME%
    goto :Fim
rem
rem Finaliza��o
:Fim
    endLocal