set Luban=..\Luban.ClientServer\Luban.ClientServer.exe
set Config=..\Config
rem set OutPut=Gen

set OutPut=..\..\Client\Assets\Resources\ResJsonConfig\Gen

%Luban% -j cfg --^
 -d %Config%\Defines\__root__.xml ^
 --input_data_dir %Config%\Datas ^
 --output_code_dir %OutPut%\Code ^
 --output_data_dir %OutPut%\Data ^
 --gen_types code_cs_unity_json,data_json ^
 -s all 
pause