call "C:\Program Files (x86)\Microsoft Visual Studio 12.0\VC\vcvarsall.bat" x86

MSBuild ../Src/CShell.sln /t:Build /p:Configuration=Release
