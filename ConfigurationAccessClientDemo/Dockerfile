FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["ConfigurationAccessClientDemo/ConfigurationAccessClientDemo.csproj", "ConfigurationAccessClientDemo/"]
COPY ["ConfigurationHub/ConfigurationHub.csproj", "ConfigurationHub/"]
RUN dotnet restore "ConfigurationAccessClientDemo/ConfigurationAccessClientDemo.csproj"
COPY . .
WORKDIR "/src/ConfigurationAccessClientDemo"
RUN dotnet build "ConfigurationAccessClientDemo.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "ConfigurationAccessClientDemo.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ConfigurationAccessClientDemo.dll"]
