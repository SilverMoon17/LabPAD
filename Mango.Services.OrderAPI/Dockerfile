FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src

COPY ./Mango.Services.OrderAPI/Mango.Services.OrderAPI.csproj ./Mango.Services.OrderAPI/
COPY ./Mango.MessageBus/Mango.MessageBus.csproj ./Mango.MessageBus/

RUN dotnet restore "./Mango.Services.OrderAPI/Mango.Services.OrderAPI.csproj"
RUN dotnet restore "./Mango.MessageBus/Mango.MessageBus.csproj"

COPY . .

RUN dotnet build "./Mango.Services.OrderAPI/Mango.Services.OrderAPI.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "./Mango.Services.OrderAPI/Mango.Services.OrderAPI.csproj" -c Release -o /app/publish


FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .

RUN mkdir -p /app/certificates
COPY Mango.Services.OrderAPI/Certificates/aspnetapp.pfx /app/certificates

ENTRYPOINT ["dotnet", "Mango.Services.OrderAPI.dll"]