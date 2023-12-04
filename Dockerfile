FROM mcr.microsoft.com/dotnet/sdk:7.0.403-alpine3.18@sha256:fbbbdfc34566e2f4fcb2147292b76b9b847911a348c404a7a24b1656465e6bea
WORKDIR /app/
COPY . .
RUN dotnet publish --configuration Release --output out
CMD ["./out/tidybee-hub"]