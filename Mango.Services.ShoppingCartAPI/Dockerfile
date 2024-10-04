FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src

COPY ./Mango.Services.ShoppingCartAPI/Mango.Services.ShoppingCartAPI.csproj ./Mango.Services.ShoppingCartAPI/
COPY ./Mango.MessageBus/Mango.MessageBus.csproj ./Mango.MessageBus/

RUN dotnet restore "./Mango.Services.ShoppingCartAPI/Mango.Services.ShoppingCartAPI.csproj"
RUN dotnet restore "./Mango.MessageBus/Mango.MessageBus.csproj"

COPY . .

RUN dotnet build "./Mango.Services.ShoppingCartAPI/Mango.Services.ShoppingCartAPI.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "./Mango.Services.ShoppingCartAPI/Mango.Services.ShoppingCartAPI.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .

RUN mkdir -p /app/certificates
COPY Mango.Services.ShoppingCartAPI/Certificates/aspnetapp.pfx /app/certificates

ENTRYPOINT ["dotnet", "Mango.Services.ShoppingCartAPI.dll"]