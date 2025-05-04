@echo off

:: Check if minio.exe exists
where minio.exe >nul 2>&1
if %ERRORLEVEL% neq 0 (
    echo Error: minio.exe not found in PATH.
    echo Make sure it is installed and accessible.
    pause
    exit /b 1
)

:: Check if current directory is writable
echo. > test.tmp 2>nul
if %ERRORLEVEL% neq 0 (
    echo Error: Cannot write to current directory.
    pause
    exit /b 1
)
del test.tmp >nul 2>&1

:: Start MinIO
echo Starting MinIO server in: %CD%
minio.exe server ./

:: Pause if MinIO crashes
if %ERRORLEVEL% neq 0 (
    echo Error: MinIO server failed (Code: %ERRORLEVEL%)
    pause
    exit /b 1
)