@echo off
setlocal

REM Set the desired output dimensions (e.g., 1024x1024)
set "WIDTH=1024"
set "HEIGHT=1024"

REM Check if an image file was dropped onto the batch file
if "%~1"=="" (
    echo Drag and drop an image file onto this batch file to resize it.
    pause
    exit /b
)

REM Resize the image using ImageMagick
magick "%~1" -resize %WIDTH%x%HEIGHT% "%~dpn1_resized.png"
REM save as 8-bit PNG
magick "%~dpn1_resized.png" -depth 8 "%~dpn1_resized.png"
REM overwrite original file
move /Y "%~dpn1_resized.png" "%~1"


echo Image resized successfully! Output saved as "%~dpn1_resized.png"
pause
