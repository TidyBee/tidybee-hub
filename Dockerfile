FROM mcr.microsoft.com/dotnet/sdk:7.0.203-alpine3.17-amd64@sha256:912d0c585a53734e98216cdee5ae49383399fa9bbf744c791e3f104846d0ed8e AS build
WORKDIR /app/
COPY Controllers ./Controllers
COPY Middlewares ./Middlewares
COPY Models ./Models
COPY Properties ./Properties
COPY Program.cs appsettings.Development.json appsettings.json tidybee-hub.csproj ./
RUN dotnet publish -c Release -o out
FROM mcr.microsoft.com/dotnet/runtime:7.0.5-alpine3.17-amd64@sha256:7ce58f868cbc39bd6480f7b629ec538e4357b92a845bc9d0cfd59dc2e24f9b36
WORKDIR /app/
COPY --from=build /app/out/ .
ENTRYPOINT ["dotnet", "tidybee-hub.dll"]
