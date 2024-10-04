FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src

COPY ./Mango.Services.Email/Mango.Services.Email.csproj ./Mango.Services.Email/
COPY ./Mango.MessageBus/Mango.MessageBus.csproj ./Mango.MessageBus/

RUN dotnet restore "./Mango.Services.Email/Mango.Services.Email.csproj"
RUN dotnet restore "./Mango.MessageBus/Mango.MessageBus.csproj"

COPY . .

RUN dotnet build "./Mango.Services.Email/Mango.Services.Email.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "./Mango.Services.Email/Mango.Services.Email.csproj" -c Release -o /app/publish


FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .

RUN mkdir -p /app/certificates
COPY Mango.Services.Email/Certificates/aspnetapp.pfx /app/certificates

ENTRYPOINT ["dotnet", "Mango.Services.Email.dll"]