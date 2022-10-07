FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["src/ParatuneClient/ParatuneClient.csproj", "ParatuneClient/"]
RUN dotnet restore "src/ParatuneClient/ParatuneClient.csproj"
COPY . .
WORKDIR "/src/ParatuneClient"
RUN dotnet build "ParatuneClient.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ParatuneClient.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ParatuneClient.dll"]
