@echo off
if !%1==! goto End

if not exist %1 md %1
if errorlevel 1 goto Error
del /S /Q %1\*.*

echo Deploying Assemblies...
copy /Y HFM.Plugins.ClientSettings.FahMon.dll %1

echo Copying Support Files and Folders...
copy /Y readme.txt %1

echo Finished Cleanly.
goto End

:Error
echo An Error Occured While Creating a Deploy Folder.

:End
echo on
