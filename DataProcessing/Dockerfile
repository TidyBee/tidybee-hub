FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY DataProcessing/DataProcessing.csproj ./
RUN dotnet restore "./DataProcessing.csproj"
COPY DataProcessing/ ./
RUN dotnet build "DataProcessing.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "DataProcessing.csproj" -c Release -o /app/publish

FROM base AS final
LABEL org.opencontainers.image.source=https://github.com/TidyBee/tidybee-hub
WORKDIR /app
COPY --from=publish /app/publish .
ENV ASPNETCORE_ENVIRONMENT=Development
ENTRYPOINT ["dotnet", "DataProcessing.dll"]
