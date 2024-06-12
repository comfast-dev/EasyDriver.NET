## Deployment & publish
Url: https://learn.microsoft.com/en-us/nuget/quickstart/create-and-publish-a-package-using-the-dotnet-cli
1. Update version in csproj
2. `dotnet pack`
3. `dotnet nuget push .\EasyDriver\bin\Debug\EasyDriver.x.x.x.nupkg --api-key xxxxxxx --source https://api.nuget.org/v3/index.json`