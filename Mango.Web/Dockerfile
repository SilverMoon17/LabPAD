# Use the official .NET Core runtime as the base image
FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
# Use the official .NET Core SDK as the build image
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Mango.Web.csproj", "./"]
RUN dotnet restore "./Mango.Web.csproj"
COPY . .
RUN dotnet build "Mango.Web.csproj" -c Release -o /app/build
FROM build AS publish
RUN dotnet publish "Mango.Web.csproj" -c Release -o /app/publish
# Build the final image using the base image and the published output
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

RUN mkdir -p /app/certificates
COPY Certificates/aspnetapp.pfx /app/certificates

ENTRYPOINT ["dotnet", "Mango.Web.dll"]