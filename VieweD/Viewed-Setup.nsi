; viewedsetup.nsi
;--------------------------------
; Includes
!include "MUI2.nsh"
!include "logiclib.nsh"

;--------------------------------
; Custom defines
!define NAME "VieweD"
!define APPFILE "viewed.exe"
!define VERSION "1.0.3.6"
!define SLUG "${NAME} v${VERSION}"
!define BIN_DEBUG "bin\Debug\net8.0-windows\"

!define /date MyTIMESTAMP "%Y-%m-%d"
  
; The name of the installer
Name "${NAME}"

; The file to write
OutFile "${NAME}-Setup-${MyTIMESTAMP}.exe"

; Request application privileges for Windows Vista
RequestExecutionLevel user

; Build Unicode installer
Unicode True

; The default installation directory
InstallDir $DESKTOP\VieweD

SetCompressor /SOLID lzma

;--------------------------------
; UI
  
!define MUI_ICON "resources\icons\found-it!.ico"
!define MUI_HEADERIMAGE
!define MUI_HEADERIMAGE_BITMAP "resources\nsis-header.bmp"
!define MUI_WELCOMEPAGE_TITLE "${SLUG} Setup"
!define MUI_ABORTWARNING
  
;--------------------------------
; Installer pages
!insertmacro MUI_PAGE_WELCOME
; !insertmacro MUI_PAGE_LICENSE "license.txt"
!insertmacro MUI_PAGE_DIRECTORY
!insertmacro MUI_PAGE_COMPONENTS
!insertmacro MUI_PAGE_INSTFILES
!insertmacro MUI_PAGE_FINISH

 ; Uninstaller pages
!insertmacro MUI_UNPAGE_CONFIRM
!insertmacro MUI_UNPAGE_INSTFILES
  
; Set UI language
!insertmacro MUI_LANGUAGE "English"

;--------------------------------
; Include dotNET core check header
; Source: https://github.com/danpaul88/nsis-dotnetcore
!include "dotnetcore.nsh"

; The stuff to install
Section "VieweD Files (required)" MainApp ;No components page, name is not important
  SectionIn RO

  ; Check for .NET 8
  !insertmacro CheckDotNetCore 8.0

  ; Set output path to the installation directory.
  SetOutPath $INSTDIR
  
  ; Main exe files (excluding the installer)
  ; File /x viewedsetup.* *.*
  ; Main executable
  File ${BIN_DEBUG}*.exe
  ; Libraries
  File ${BIN_DEBUG}*.dll
  ; Library configurations
  File ${BIN_DEBUG}*.json
  ; Debug file
  File ${BIN_DEBUG}*.pdb

  ; VLC Library Files
  SetOutPath $INSTDIR\libvlc
  File /r ${BIN_DEBUG}libvlc\*.*

  ; Engines Plugin Readme
  SetOutPath $INSTDIR\data
  File data\welcome.rtf

  ; Default Plugins
  ; FFXI Engine Data
  SetOutPath $INSTDIR\data\ffxi\filter
  File /r data\ffxi\filter\*.*

  SetOutPath $INSTDIR\data\ffxi\lookup
  File /r data\ffxi\lookup\*.*

  SetOutPath $INSTDIR\data\ffxi\rules
  File /r data\ffxi\rules\ffxi.xml

  ; AA Engine Data
  SetOutPath $INSTDIR\data\aa\filter
  File /r data\aa\filter\*.*

  SetOutPath $INSTDIR\data\aa\keys
  File /r data\aa\keys\*.*

  SetOutPath $INSTDIR\data\aa\lookup
  File /r data\aa\lookup\*.txt

  SetOutPath $INSTDIR\data\aa\rules
  File /r data\aa\rules\*.txt

  SetOutPath $INSTDIR
  
  WriteRegStr HKCU "Software\${NAME}" "" $INSTDIR
  WriteUninstaller "$INSTDIR\Uninstall.exe"
SectionEnd ; end the section

;--------------------------------
; Section - Shortcut
Section "Desktop Shortcut" DeskShort
  CreateShortCut "$DESKTOP\${NAME}.lnk" "$INSTDIR\${APPFILE}"
SectionEnd

;--------------------------------
; Remove empty parent directories
  Function un.RMDirUP
  
    !define RMDirUP '!insertmacro RMDirUPCall'

    !macro RMDirUPCall _PATH
          push '${_PATH}'
          Call un.RMDirUP
    !macroend

    ; $0 - current folder
    ClearErrors

    Exch $0
    ;DetailPrint "ASDF - $0\.."
    RMDir "$0\.."

    IfErrors Skip
    ${RMDirUP} "$0\.."
    Skip:

    Pop $0

  FunctionEnd
  
;--------------------------------
; Section - Uninstaller

Section "Uninstall"

  ;Delete Shortcut
  Delete "$DESKTOP\${NAME}.lnk"

  ;Delete Uninstall
  Delete "$INSTDIR\Uninstall.exe"

  ;Delete Folder
  RMDir /r "$INSTDIR"
  ${RMDirUP} "$INSTDIR"

  DeleteRegKey /ifempty HKCU "Software\${NAME}"

SectionEnd

;--------------------------------
; Descriptions

  ;Language strings
  LangString DESC_DeskShort ${LANG_ENGLISH} "Create Shortcut on Dekstop."
  LangString DESC_MainApp ${LANG_ENGLISH} "The main program files required to run VieweD"

  ;Assign language strings to sections
  !insertmacro MUI_FUNCTION_DESCRIPTION_BEGIN
    !insertmacro MUI_DESCRIPTION_TEXT ${DeskShort} $(DESC_DeskShort)
    !insertmacro MUI_DESCRIPTION_TEXT ${MainApp} $(DESC_MainApp)
  !insertmacro MUI_FUNCTION_DESCRIPTION_END
