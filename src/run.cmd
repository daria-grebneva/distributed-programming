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

cd VowelConsRater 
start dotnet VowelConsRater.dll
cd ../  

cd VowelConsCounter 
start dotnet VowelConsCounter.dll
cd ../  

start "" /wait "http://127.0.0.1:5001/" 

echo process is running
pause
