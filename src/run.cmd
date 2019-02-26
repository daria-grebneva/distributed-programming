@echo off

cd Backend
start dotnet Backend.dll
cd ../ 

cd Frontend 
start dotnet Frontend.dll
cd ../    

start "" /wait "http://127.0.0.1:5001/" 

echo process is running
pause
