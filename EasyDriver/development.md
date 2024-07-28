## Deployment & publish

Url: https://learn.microsoft.com/en-us/nuget/quickstart/create-and-publish-a-package-using-the-dotnet-cli

1. Run integration tests
2. Update changelog.md with new version
3. Publish commands

```
dotnet build
dotnet pack
dotnet nuget push .\EasyDriver\bin\Debug\EasyDriver.x.x.x.nupkg --api-key xxxxxxx --source https://api.nuget.org/v3/index.json
```