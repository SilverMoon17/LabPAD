FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ./Mango.Services.AzureBlobService.csproj .
RUN dotnet restore "./Mango.Services.AzureBlobService.csproj"
COPY . .
RUN dotnet build "./Mango.Services.AzureBlobService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "./Mango.Services.AzureBlobService.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .

RUN mkdir -p /app/certificates
COPY ./Certificates/aspnetapp.pfx /app/certificates

ENTRYPOINT ["dotnet", "Mango.Services.AzureBlobService.dll"]