@ECHO OFF
pushd "%~dp0"
ECHO.
ECHO.
ECHO.
ECHO This script deletes all temporary build files in their
ECHO corresponding BIN and OBJ Folder contained in the following projects
ECHO.
ECHO YALV
ECHO YALV.Samples
ECHO YalvLib
ECHO UnitTests\YalvLib.UnitTests
ECHO UnitTests\YalvLib.IntegrationTests
ECHO.
ECHO Debug and Release folders
ECHO.
REM Ask the user if hes really sure to continue beyond this point XXXXXXXX
set /p choice=Are you sure to continue (Y/N)?
if not '%choice%'=='Y' Goto EndOfBatch
REM Script does not continue unless user types 'Y' in upper case letter
ECHO.
ECHO XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX
ECHO.
ECHO XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX
ECHO.
ECHO Removing vs settings folder with *.sou file
ECHO.
RMDIR /S /Q .vs
RMDIR /S /Q TestResults

ECHO Deleting BIN and OBJ Folders in YALV folder
ECHO.
RMDIR /S /Q YALV\bin
RMDIR /S /Q YALV\obj

ECHO Deleting BIN and OBJ Folders in YALV.Samples folder
ECHO.
RMDIR /S /Q YALV.Samples\bin
RMDIR /S /Q YALV.Samples\obj

ECHO Deleting BIN and OBJ Folders in YalvLib folder
ECHO.
RMDIR /S /Q YalvLib\bin
RMDIR /S /Q YalvLib\obj

ECHO Deleting BIN and OBJ Folders in UnitTests\YalvLib.UnitTests folder
ECHO.
RMDIR /S /Q UnitTests\YalvLib.IntegrationTests\bin
RMDIR /S /Q UnitTests\YalvLib.IntegrationTests\obj

ECHO Deleting BIN and OBJ Folders in YalvLib.Tests.Integration folder
ECHO.
RMDIR /S /Q UnitTests\YalvLib.UnitTests\YalvLib.UnitTests\bin
RMDIR /S /Q UnitTests\YalvLib.UnitTests\YalvLib.UnitTests\obj
            
PAUSE

:EndOfBatch
