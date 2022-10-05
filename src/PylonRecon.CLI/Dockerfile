FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["src/PylonRecon.CLI/PylonRecon.CLI.csproj", "PylonRecon.CLI/"]
RUN dotnet restore "src/PylonRecon.CLI/PylonRecon.CLI.csproj"
COPY . .
WORKDIR "/src/PylonRecon.CLI"
RUN dotnet build "PylonRecon.CLI.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "PylonRecon.CLI.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "PylonRecon.CLI.dll"]
