; viewedsetup.nsi

;--------------------------------

; The name of the installer
Name "VieweD Setup"

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
  SetOutPath $INSTDIR\runtimes\win-x64\native
  File runtimes\win-x64\native\*.*
  ; x86
  SetOutPath $INSTDIR\runtimes\win-x86\native
  File runtimes\win-x86\native\*.*

  ; Program Data
  SetOutPath $INSTDIR\data\ffxi\lookup
  File /r data\lookup\*.*
  SetOutPath $INSTDIR\data\ffxi\parse
  File /r data\parse\*.*
  SetOutPath $INSTDIR\data\ffxi\filter
  File /r data\filter\*.*

  SetOutPath $INSTDIR\Plugins
  File /r Plugins\*.*
  SetOutPath $INSTDIR\Plugins\ffxi
  File /r Plugins\ffxi\*.*

  SetOutPath $INSTDIR
  
SectionEnd ; end the section
