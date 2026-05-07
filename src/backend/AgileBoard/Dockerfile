FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["AgileBoard.csproj", "./"]
RUN dotnet restore "AgileBoard.csproj"
COPY . .
RUN dotnet build "AgileBoard.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "AgileBoard.csproj" -c Release -o /src/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /src/publish .
ENTRYPOINT ["dotnet", "AgileBoard.dll"]
