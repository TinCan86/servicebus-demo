FROM mcr.microsoft.com/dotnet/core/sdk:3.1 as builder

WORKDIR /App

COPY FirstApp.PlusFourAPIDemo.csproj ./

RUN dotnet restore

COPY ./ ./

RUN dotnet build
RUN dotnet publish -c Release

FROM mcr.microsoft.com/dotnet/core/aspnet:3.0 AS runtime

WORKDIR /app

COPY  --from=builder /App/bin/Release/netcoreapp3.1/publish ./
ENTRYPOINT ["dotnet", "FirstApp.PlusFourAPIDemo.dll"]
