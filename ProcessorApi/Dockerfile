FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["ProcessorApi/ProcessorApi.csproj", "ProcessorApi/"]
COPY ["ProcessorLib/ProcessorLib.csproj", "ProcessorLib/"]
RUN dotnet restore "ProcessorApi/ProcessorApi.csproj"
COPY . .
WORKDIR "/src/ProcessorApi"
RUN dotnet build "./ProcessorApi.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./ProcessorApi.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ProcessorApi.dll"]