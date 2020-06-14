FROM mcr.microsoft.com/dotnet/core/sdk:3.1 as builder

WORKDIR /App

COPY FirstApp.PlusFourService.csproj ./

RUN dotnet restore

COPY Program.cs ./

RUN dotnet build
RUN dotnet publish -c Release

FROM mcr.microsoft.com/dotnet/core/runtime:3.1

COPY --from=builder /App/bin/Release/netcoreapp3.1/publish App/

WORKDIR /App

ENTRYPOINT ["dotnet", "FirstApp.PlusFourService.dll"]
