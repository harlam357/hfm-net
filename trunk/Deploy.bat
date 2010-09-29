@echo off
if !%1==! goto End

if not exist %1 md %1
if errorlevel 1 goto Error
del /S /Q %1\*.*

echo Deploying Assemblies...
copy /y HFM.exe %1
copy /y HFM.exe.config %1
copy /y HFM.Framework.dll %1
copy /Y HFM.Helpers.dll %1
copy /Y HFM.Instances.dll %1
copy /Y HFM.Log.dll %1
copy /Y HFM.Plugins.dll %1
copy /Y HFM.Preferences.dll %1
copy /Y HFM.Proteins.dll %1
copy /Y HFM.Queue.dll %1
copy /Y HTMLparser.dll %1
copy /Y harlam357.Net.dll %1
copy /Y harlam357.Security.dll %1
copy /Y harlam357.Windows.Forms.dll %1
copy /Y ZedGraph.dll %1
copy /Y Castle.Core.dll %1
copy /Y Castle.DynamicProxy2.dll %1
copy /Y Castle.MicroKernel.dll %1
copy /Y Castle.Windsor.dll %1
copy /Y protobuf-net.dll %1
copy /Y System.Linq.Dynamic.dll %1

if not exist %1\SQLite md %1\SQLite
if not exist %1\SQLite\x86 md %1\SQLite\x86
copy /Y ..\..\..\SQLite.NET\bin\System.Data.SQLite.dll %1\SQLite\x86
if not exist %1\SQLite\AMD64 md %1\SQLite\AMD64
copy /Y ..\..\..\SQLite.NET\bin\x64\System.Data.SQLite.dll %1\SQLite\AMD64
if not exist %1\SQLite\Mono md %1\SQLite\Mono
copy /Y ..\..\..\SQLite.NET\bin\ManagedOnly\System.Data.SQLite.dll %1\SQLite\Mono

echo Copying Support Files and Folders...
copy /Y GPLv2.TXT %1
copy /Y "HTMLparser License.txt" %1
copy /Y "ZedGraph License.txt" %1
copy /Y "Windsor License.txt" %1
copy /Y "protobuf-net Licence.txt" %1
copy /Y "protoc-license.txt" %1

if not exist %1\CSS md %1\CSS
if errorlevel 1 goto Error
xcopy /Y CSS %1\CSS

if not exist %1\XML md %1\XML
if errorlevel 1 goto Error
xcopy /Y XML %1\XML

if not exist %1\XSL md %1\XSL
if errorlevel 1 goto Error
xcopy /Y XSL %1\XSL

echo Finished Cleanly.
goto End

:Error
echo An Error Occured While Creating a Deploy Folder.

:End
echo on
