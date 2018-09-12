# IRB1600 Converter
This program allows you to convert an SVG image into a program for the IRB 1600.

# Usage:
## SVG to program (auto scale and offsets):
IRB1600_Converter.exe program --input Image.svg --output Program.txt --width 100 --height 100 --auto

## SVG to program:
IRB1600_Converter.exe program --input Image.svg --output Program.txt --width 100 --height 100 --margin 10 --scale 2 --offsetx 100 --offsety 100

## Program to SVG:
IRB1600_Converter.exe svg --input Program.txt --output Result.svg --width 594mm --height 420mm --viewboxwidth 594 --viewboxheight 420
