@echo off
    setlocal EnableDelayedExpansion
    set SERVICE_NAME=ServiceIntegracao
    set SERVICE_FULL_NAME=ServiceIntegracao.exe
rem Verifica se o serviço está rodando
    echo Verificando se o %SERVICE_NAME% está instalado
    sc query %SERVICE_NAME% | find /i "%SERVICE_NAME%"
rem Se existe o serviço
    if "%ERRORLEVEL%"=="0" (
       goto :Fim
    )
    echo Instalando o serviço %SERVICE_NAME%
    sc create %SERVICE_NAME% start= auto binPath= "C:\WindowsServicesApi\%SERVICE_FULL_NAME%" DisplayName= "%SERVICE_NAME%"
rem Inicia o serviço
    echo Iniciando o serviço %SERVICE_NAME%
    sc start %SERVICE_NAME%
    goto :Fim
rem
rem Finalização
:Fim
    endLocal