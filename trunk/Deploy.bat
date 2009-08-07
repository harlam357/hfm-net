@echo off
if !%1==! goto End

echo Cleaning Deploy Folder...
rd /S /Q %1
if errorlevel 1 goto Error
md %1
if errorlevel 1 goto Error

echo Deploying Assemblies...
copy /y HFM.exe %1
copy /y HFM.exe.config %1
copy /Y HFM.Helpers.dll %1
copy /Y HFM.Instances.dll %1
copy /Y HFM.Instrumentation.dll %1
copy /Y HFM.Preferences.dll %1
copy /Y HFM.Proteins.dll %1
copy /Y HTMLparser.dll %1

echo Copying Support Files and Folders...
copy /Y GPLv2.TXT %1
copy /Y "HTMLparser License.txt" %1

md %1\CSS
xcopy /Y CSS %1\CSS
md %1\XML
xcopy /Y XML %1\XML
md %1\XSL
xcopy /Y XSL %1\XSL

echo Finished Cleanly.
goto End

:Error
echo An Error Occured While Cleaning Deploy Folder.

:End
echo on
