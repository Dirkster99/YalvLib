@ECHO OFF
pushd "%~dp0"
ECHO.
ECHO.
ECHO.
ECHO This script deletes all temporary build files in their
ECHO corresponding BIN and OBJ Folder contained in the following projects
ECHO.
ECHO ModernYalv
ECHO MRU
ECHO Edi.Core
ECHO YALV
ECHO YALV.Samples
ECHO YalvLib
ECHO YalvLib.Tests
ECHO YalvLib.Tests.Integration
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

ECHO Deleting BIN and OBJ Folders in ModernYalv folder
ECHO.
RMDIR /S /Q ModernYalv\bin
RMDIR /S /Q ModernYalv\obj

ECHO Deleting BIN and OBJ Folders in MRU folder
ECHO.
RMDIR /S /Q MRU\bin
RMDIR /S /Q MRU\obj

ECHO Deleting BIN and OBJ Folders in Edi.Core folder
ECHO.
RMDIR /S /Q Edi.Core\bin
RMDIR /S /Q Edi.Core\obj

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

ECHO Deleting BIN and OBJ Folders in YalvLib.Tests folder
ECHO.
RMDIR /S /Q YalvLib.Tests\bin
RMDIR /S /Q YalvLib.Tests\obj

ECHO Deleting BIN and OBJ Folders in YalvLib.Tests.Integration folder
ECHO.
RMDIR /S /Q YalvLib.Tests.Integration\bin
RMDIR /S /Q YalvLib.Tests.Integration\obj

PAUSE

:EndOfBatch
