for %%v in (v3.5,v4.0) do msbuild /t:rebuild /p:Configuration=Release /p:TargetFrameworkVersion=%%v /m %*
.nuget\nuget.exe pack