@echo off

cd Backend
start "Backend" dotnet Backend.dll
cd ../ 

cd Frontend 
start "Frontend" dotnet Frontend.dll
cd ../    

cd TextListener 
start "TextListener" dotnet TextListener.dll
cd ../  

cd TextRankCalc 
start "TextRankCalc" dotnet TextRankCalc.dll
cd ../  

cd TextStatistics
start "TextStatistics" dotnet TextStatistics.dll
cd ../  

cd TextProcessingLimiter
start "TextProcessingLimiter" dotnet TextProcessingLimiter.dll
cd ../  
                                         
set file=%CD%\Config\config.ini
for /f "tokens=1,2 delims=:" %%a in (%file%) do (
	for /l %%i in (1, 1, %%b) do start "%%a" /d %%a dotnet %%a.dll
)

start "" /wait "http://127.0.0.1:5001/" 

echo process is running
pause
