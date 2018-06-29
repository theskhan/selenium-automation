RMDIR /S /Q publish

dotnet publish --output ../../publish/Starter ./SeleniumAutomationStarter/SeleniumAutomationStarter/SeleniumAutomationStarter.csproj
dotnet publish --output ../../publish/Executor ./SeleniumAutomation/SeleniumAutomation/SeleniumAutomation.csproj

copy .\commands\*.bat .\publish