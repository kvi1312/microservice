@echo off
setlocal

REM ===== CONFIGURATION =====
set SERVER=lenovo\SQLSERVER2
set DATABASE=IdentityServer
set USER=sa
set PASSWORD=a2601197

REM ===== FILE PATHS =====
set SCRIPT_DIR=%~dp0

echo ============================================
echo Running stored procedures on %SERVER%\%DATABASE%
echo ============================================

sqlcmd -S "%SERVER%" -U "%USER%" -P "%PASSWORD%" -d "%DATABASE%" -i "%SCRIPT_DIR%Create_Permissions_store_procedure.sql"
IF %ERRORLEVEL% NEQ 0 GOTO :ERROR

sqlcmd -S "%SERVER%" -U "%USER%" -P "%PASSWORD%" -d "%DATABASE%" -i "%SCRIPT_DIR%Delete_Permissions_store_procedure.sql"
IF %ERRORLEVEL% NEQ 0 GOTO :ERROR

sqlcmd -S "%SERVER%" -U "%USER%" -P "%PASSWORD%" -d "%DATABASE%" -i "%SCRIPT_DIR%Get_Permission_ByRoleId_store_procedure.sql"
IF %ERRORLEVEL% NEQ 0 GOTO :ERROR

sqlcmd -S "%SERVER%" -U "%USER%" -P "%PASSWORD%" -d "%DATABASE%" -i "%SCRIPT_DIR%Update_Permissions_store_procedure.sql"
IF %ERRORLEVEL% NEQ 0 GOTO :ERROR

echo.
echo ✅ All stored procedures executed successfully.
goto :END

:ERROR
echo.
echo ❌ An error occurred while executing a script. Check for syntax or connection issues.
goto :END

:END
pause
