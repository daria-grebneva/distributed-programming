@echo off

cd Backend
start dotnet Backend.dll
cd ../ 

cd Frontend 
start dotnet Frontend.dll
cd ../    

cd TextListener 
start dotnet TextListener.dll
cd ../  

cd TextRankCalc 
start dotnet TextRankCalc.dll
cd ../  
                                          
set file=%CD%\Config\config.ini
for /f "tokens=1,2 delims=:" %%a in (%file%) do (
	for /l %%i in (1, 1, %%b) do start /d %%a dotnet %%a.dll
)

start "" /wait "http://127.0.0.1:5001/" 

echo process is running
pause
