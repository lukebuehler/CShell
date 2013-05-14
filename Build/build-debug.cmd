call "C:\Program Files (x86)\Microsoft Visual Studio 10.0\VC\vcvarsall.bat" x86

MSBuild ../Src/CShell.sln /t:Build /p:Configuration=Debug
