FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:7.0 AS build
ARG TARGETARCH
WORKDIR /src
COPY DataProcessing/DataProcessing.csproj ./
RUN dotnet restore "./DataProcessing.csproj" -a $TARGETARCH
COPY DataProcessing/ ./
RUN dotnet build "DataProcessing.csproj" -c Release -o /app/build -a $TARGETARCH

FROM build  AS publish
RUN dotnet publish "DataProcessing.csproj" -c Release -o /app/publish -a $TARGETARCH

FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/aspnet:7.0-bookworm-slim AS final
LABEL org.opencontainers.image.source=https://github.com/TidyBee/tidybee-hub
WORKDIR /app
COPY --from=publish /app/publish .
ENV ASPNETCORE_ENVIRONMENT=Container
ENTRYPOINT ["dotnet", "DataProcessing.dll"]
