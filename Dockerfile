FROM mcr.microsoft.com/dotnet/runtime:7.0-alpine AS base

WORKDIR /app

FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY . .
RUN dotnet build "PresenceBot.sln" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "PresenceBot.sln" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "PresenceBot.dll"]
