FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG TARGETARCH
WORKDIR /src
COPY TidyEvents/TidyEvents.csproj ./
RUN dotnet restore "./TidyEvents.csproj" -a $TARGETARCH
COPY TidyEvents/ ./
RUN dotnet build "TidyEvents.csproj" -c Release -o /app/build -a $TARGETARCH

FROM build AS publish
RUN dotnet publish "TidyEvents.csproj" -c Release -o /app/publish -a $TARGETARCH

FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/aspnet:8.0-bookworm-slim AS final
LABEL org.opencontainers.image.source=https://github.com/TidyBee/tidybee-hub
WORKDIR /app
COPY --from=publish /app/publish .
ENV ASPNETCORE_ENVIRONMENT=Container
ENTRYPOINT ["dotnet", "TidyEvents.dll"]
