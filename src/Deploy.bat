@echo off
if !%1==! goto End

if not exist %1 md %1
if errorlevel 1 goto Error
del /S /Q %1\*.*

echo Deploying Assemblies...
copy /y HFM.exe %1
copy /y HFM.exe.config %1
copy /y HFM.Client.dll %1
copy /y HFM.Core.dll %1
copy /y HFM.Core.DataTypes.dll %1
copy /Y HFM.Core.Plugins.dll %1
copy /y HFM.Forms.dll %1
copy /Y HFM.Log.dll %1
copy /Y HFM.Preferences.dll %1
copy /Y HFM.Proteins.dll %1
copy /Y HFM.Queue.dll %1
copy /Y HTMLparser.dll %1
copy /Y harlam357.Net.dll %1
copy /Y harlam357.Security.dll %1
copy /Y harlam357.Windows.Forms.dll %1
copy /Y ZedGraph.dll %1
copy /Y Castle.Core.dll %1
copy /Y Castle.Windsor.dll %1
copy /Y protobuf-net.dll %1
copy /Y System.Linq.Dynamic.dll %1
copy /Y AutoMapper.dll %1
copy /Y Newtonsoft.Json.Net35.dll %1

if not exist %1\SQLite md %1\SQLite
if not exist %1\SQLite\x86 md %1\SQLite\x86
copy /Y ..\..\..\..\lib\SQLite.NET\bin\System.Data.SQLite.dll %1\SQLite\x86
if not exist %1\SQLite\AMD64 md %1\SQLite\AMD64
copy /Y ..\..\..\..\lib\SQLite.NET\bin\x64\System.Data.SQLite.dll %1\SQLite\AMD64
if not exist %1\SQLite\Mono md %1\SQLite\Mono
copy /Y ..\..\..\..\lib\SQLite.NET\bin\ManagedOnly\System.Data.SQLite.dll %1\SQLite\Mono

if not exist %1\Tools md %1\Tools
copy /Y ..\..\..\HFM.Client.Tool\bin\ReleaseMerge\HFM.Client.exe %1\Tools
copy /Y ..\..\..\HFM.Log.Tool\bin\ReleaseMerge\HFM.Log.exe %1\Tools
copy /Y ..\..\..\HFM.Queue.Tool\bin\ReleaseMerge\HFM.Queue.exe %1\Tools

echo Copying License Files...
if not exist %1\Documentation md %1\Documentation
if not exist %1\Documentation\License md %1\Documentation\License
copy /Y "..\..\..\..\doc\GPLv2.TXT" %1\Documentation\License
copy /Y "..\..\..\..\lib\HTMLparser2\HTMLparser License.txt" %1\Documentation\License
copy /Y "..\..\..\..\lib\ZedGraph\ZedGraph License.txt" %1\Documentation\License
copy /Y "..\..\..\..\lib\Castle Windsor\ASL - Apache Software Foundation License.txt" "%1\Documentation\License\Windsor License.txt"
copy /Y "..\..\..\..\lib\protobuf-net\protobuf-net Licence.txt" %1\Documentation\License
copy /Y "..\..\..\..\lib\protobuf-net\protoc-license.txt" %1\Documentation\License
copy /Y "..\..\..\..\lib\AutoMapper\AutoMapper License.txt" %1\Documentation\License
copy /Y "..\..\..\..\lib\Json.NET\Json.NET License.txt" %1\Documentation\License

echo Copying Support Files and Folders...
if not exist %1\CSS md %1\CSS
if errorlevel 1 goto Error
xcopy /Y CSS %1\CSS

if not exist %1\XSL md %1\XSL
if errorlevel 1 goto Error
xcopy /Y XSL %1\XSL

echo Finished Cleanly.
goto End

:Error
echo An Error Occured While Creating a Deploy Folder.

:End
echo on
