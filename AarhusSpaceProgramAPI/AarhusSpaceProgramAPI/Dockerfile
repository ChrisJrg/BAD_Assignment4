FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 5265

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["AarhusSpaceProgramAPI/AarhusSpaceProgramAPI.csproj", "AarhusSpaceProgramAPI/"]
RUN dotnet restore "AarhusSpaceProgramAPI/AarhusSpaceProgramAPI.csproj"
COPY . .
WORKDIR "/src/AarhusSpaceProgramAPI"
RUN dotnet build "./AarhusSpaceProgramAPI.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./AarhusSpaceProgramAPI.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AarhusSpaceProgramAPI.dll"]
