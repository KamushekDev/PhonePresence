FROM mcr.microsoft.com/dotnet/runtime:7.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
#COPY ["src/PhonePresenceBot/PhonePresenceBot.csproj", "src/PhonePresenceBot/"]
#RUN dotnet restore "src/PhonePresenceBot/PhonePresenceBot.csproj"
COPY . .
#WORKDIR "/src/src/PhonePresenceBot"
RUN dotnet build "PhonePresenceBot.sln" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "PhonePresenceBot.sln" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "PhonePresenceBot.dll"]
