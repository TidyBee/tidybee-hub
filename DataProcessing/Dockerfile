FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base

WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["DataProcessing/DataProcessing.csproj", "./"]
RUN dotnet restore "./DataProcessing.csproj"
COPY ["DataProcessing/", "./"]
RUN dotnet build "DataProcessing.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "DataProcessing.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

ENV ASPNETCORE_ENVIRONMENT=Development

ENTRYPOINT ["dotnet", "DataProcessing.dll"]

