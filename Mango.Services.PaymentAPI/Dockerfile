FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src

COPY ./Mango.Services.PaymentAPI/Mango.Services.PaymentAPI.csproj ./Mango.Services.PaymentAPI/
COPY ./Mango.MessageBus/Mango.MessageBus.csproj ./Mango.MessageBus/

RUN dotnet restore "./Mango.Services.PaymentAPI/Mango.Services.PaymentAPI.csproj"
RUN dotnet restore "./Mango.MessageBus/Mango.MessageBus.csproj"

COPY . .

RUN dotnet build "./Mango.Services.PaymentAPI/Mango.Services.PaymentAPI.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "./Mango.Services.PaymentAPI/Mango.Services.PaymentAPI.csproj" -c Release -o /app/publish


FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS final
WORKDIR /app

COPY --from=publish /app/publish .

RUN mkdir -p /app/certificates
COPY Mango.Services.PaymentAPI/Certificates/aspnetapp.pfx /app/certificates

ENTRYPOINT ["dotnet", "Mango.Services.PaymentAPI.dll"]