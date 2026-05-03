FROM mcr.microsoft.com/dotnet/runtime:9.0 AS base
USER $APP_UID
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["ConcurrentResumeProcessor.ProcessorLib/ConcurrentResumeProcessor.ProcessorLib.csproj", "ConcurrentResumeProcessor.ProcessorLib/"]
RUN dotnet restore "ConcurrentResumeProcessor.ProcessorLib/ConcurrentResumeProcessor.ProcessorLib.csproj"
COPY . .
WORKDIR "/src/ConcurrentResumeProcessor.ProcessorLib"
RUN dotnet build "./ConcurrentResumeProcessor.ProcessorLib.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./ConcurrentResumeProcessor.ProcessorLib.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ConcurrentResumeProcessor.ProcessorLib.dll"]
