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

cd src/TextRankCalc/                         
dotnet build --configuration Release --output %DIR_NAME%  
move %DIR_NAME% ../../%DIR_NAME%/TextRankCalc
cd ../../ 

cd src/VowelConsRater/                         
dotnet build --configuration Release --output %DIR_NAME%  
move %DIR_NAME% ../../%DIR_NAME%/VowelConsRater
cd ../../ 

cd src/VowelConsCounter/                         
dotnet build --configuration Release --output %DIR_NAME%  
move %DIR_NAME% ../../%DIR_NAME%/VowelConsCounter
cd ../../ 

cd src/TextStatistics/                         
dotnet build --configuration Release --output %DIR_NAME%  
move %DIR_NAME% ../../%DIR_NAME%/TextStatistics
cd ../../ 

cd src/TextProcessingLimiter/                         
dotnet build --configuration Release --output %DIR_NAME%  
move %DIR_NAME% ../../%DIR_NAME%/TextProcessingLimiter          
cd ../../ 

cd src/                    
xcopy run.cmd ..\%DIR_NAME%\
xcopy stop.cmd ..\%DIR_NAME%\
xcopy Config ..\%DIR_NAME%\Config\

echo finished build
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
      
