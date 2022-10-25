; viewedsetup.nsi

;--------------------------------

; The name of the installer
Name "VieweD"

; The file to write
OutFile "viewedsetup.exe"

; Request application privileges for Windows Vista
RequestExecutionLevel user

; Build Unicode installer
Unicode True

; The default installation directory
InstallDir $DESKTOP\VieweD

SetCompressor /SOLID lzma

;--------------------------------

; Pages

Page directory
Page instfiles

;--------------------------------

; The stuff to install
Section "" ;No components page, name is not important

  ; Set output path to the installation directory.
  SetOutPath $INSTDIR
  
  ; Main exe files (excluding the installer)
  File /x viewedsetup.* *.*

  ; Extra library files
  ; x64
  SetOutPath $INSTDIR\x64
  File x64\*.*
  ; x86
  SetOutPath $INSTDIR\x86
  File x86\*.*

  ; Program Data

  ; Engines Plugin Readme
  SetOutPath $INSTDIR\Plugins
  File /r Plugins\readme.txt

  ; FFXI Engine Data
  SetOutPath $INSTDIR\data\ffxi
  File /r data\ffxi\*.*
  SetOutPath $INSTDIR\Plugins\ffxi
  File /r Plugins\ffxi\*.*

  SetOutPath $INSTDIR
  
SectionEnd ; end the section
