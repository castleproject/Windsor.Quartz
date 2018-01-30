for %%v in (v4.6.1) do msbuild /t:rebuild /p:Configuration=Release /p:TargetFrameworkVersion=%%v /m %*
.nuget\nuget.exe pack