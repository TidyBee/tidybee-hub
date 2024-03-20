FROM --platform=$BUILDPLATFORM AS base
ARG TARGETARCH
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ApiGateway/ApiGateway.csproj ./
RUN dotnet restore "./ApiGateway.csproj" -a $TARGETARCH
COPY ApiGateway/ ./
RUN dotnet build "ApiGateway.csproj" -c Release -o /app/build -a $TARGETARCH

FROM build AS publish
RUN dotnet publish "ApiGateway.csproj" -c Release -o /app/publish -a $TARGETARCH

FROM mcr.microsoft.com/dotnet/aspnet:7.0-bookworm-slim AS final
LABEL org.opencontainers.image.source=https://github.com/TidyBee/tidybee-hub
WORKDIR /app
COPY --from=publish /app/publish .
ENV ASPNETCORE_ENVIRONMENT=Container
USER $APP_UID
ENTRYPOINT ["dotnet", "ApiGateway.dll"]
