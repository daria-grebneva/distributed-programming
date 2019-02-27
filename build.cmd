@echo off
if "%1" == "" goto WriteArgument

set DIR_NAME=%1

if exist %DIR_NAME% goto ErrWithBuildNumber

mkdir %DIR_NAME%

cd src/Frontend/                         
dotnet build --configuration Release --output %DIR_NAME%
move %DIR_NAME% ../../%DIR_NAME%/Frontend 
cd ../../
              
cd src/Backend/                         
dotnet build --configuration Release --output %DIR_NAME%  
move %DIR_NAME% ../../%DIR_NAME%/Backend
cd ../../

cd src/TextListener/                         
dotnet build --configuration Release --output %DIR_NAME%  
move %DIR_NAME% ../../%DIR_NAME%/TextListener
cd ../../

cd src/                    
xcopy run.cmd ..\%DIR_NAME%\
xcopy stop.cmd ..\%DIR_NAME%\
xcopy Config ..\%DIR_NAME%\Config\

echo finished build

cd ../
cd %DIR_NAME%/Backend
start dotnet Backend.dll
cd ../../ 

cd %DIR_NAME%/Frontend 
start dotnet Frontend.dll
cd ../../    

cd %DIR_NAME%/TextListener 
start dotnet TextListener.dll
cd ../../  

start "" /wait "http://127.0.0.1:5001/" 

echo process is running
exit
          
:WriteArgument
(
   echo write your version parameter                                
   pause
   exit   
)

:ErrWithBuildNumber
(
   echo build already exists                                
   pause
   exit 
)
      
