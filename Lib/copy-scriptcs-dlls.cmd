set "scriptcs-folder=..\..\scriptcs\"
set "release=release"

xcopy  /Y "%scriptcs-folder%\src\ScriptCs.Engine.Roslyn\bin\%release%" ScriptCs\
xcopy  /Y "%scriptcs-folder%\src\ScriptCs.Core\bin\%release%" ScriptCs\