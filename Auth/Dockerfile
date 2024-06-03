FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:7.0 AS build
ARG TARGETARCH
WORKDIR /src
COPY Auth/Auth.csproj ./
RUN dotnet restore "./Auth.csproj" -a $TARGETARCH
COPY Auth/ ./
RUN dotnet build "Auth.csproj" -c Release -o /app/build -a $TARGETARCH

FROM build AS publish
RUN dotnet publish "Auth.csproj" -c Release -o /app/publish -a $TARGETARCH

FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/aspnet:7.0-bookworm-slim AS final
LABEL org.opencontainers.image.source=https://github.com/TidyBee/tidybee-hub
WORKDIR /app
COPY --from=publish /app/publish .
COPY Auth/Database /app/Database
ENV ASPNETCORE_ENVIRONMENT=Container
ENTRYPOINT ["dotnet", "Auth.dll"]
