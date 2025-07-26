FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy project files cho layer caching tốt hơn
COPY ["IdentityServer/IdentityServer/IdentityServer.csproj", "IdentityServer/IdentityServer/"]
COPY ["IdentityServer/Microservice.IDP.Infrastructure/Microservice.IDP.Infrastructure.csproj", "IdentityServer/Microservice.IDP.Infrastructure/"]
COPY ["IdentityServer/Microservices.IDP.Presentation/Microservices.IDP.Presentation/Microservices.IDP.Presentation.csproj", "IdentityServer/Microservices.IDP.Presentation/Microservices.IDP.Presentation/"]

# Restore packages
RUN dotnet restore "IdentityServer/IdentityServer/IdentityServer.csproj"

# Copy source code
COPY . .

# Build
WORKDIR "/src/IdentityServer/IdentityServer"
RUN dotnet build "IdentityServer.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "IdentityServer.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "IdentityServer.dll"]