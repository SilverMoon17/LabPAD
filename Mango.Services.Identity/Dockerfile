FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ./Mango.Services.Identity.csproj .
RUN dotnet restore "./Mango.Services.Identity.csproj"
COPY . .
RUN dotnet build "./Mango.Services.Identity.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "./Mango.Services.Identity.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .

RUN mkdir -p /app/certificates
COPY ./Certificates/aspnetapp.pfx /app/certificates

ENTRYPOINT ["dotnet", "Mango.Services.Identity.dll"]