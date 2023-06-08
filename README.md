```
dotnet user-secrets set "Settings:ProhibitedUserIds:0" "telegramUserId"
dotnet user-secrets set "Settings:PhoneName" "Kamushek-S21-Ultra"

dotnet user-secrets set "RouterOptions:RouterUri" "routerUri"

dotnet user-secrets set "RouterAuthOptions:Login" "login"
dotnet user-secrets set "RouterAuthOptions:Password" "password"

dotnet user-secrets set "Telegram:ApiKey" "apiKey"
```

# Database
## update (from src)
```bash
dotnet ef database update -s PresenceBot -p PresenceBot.Infrastructure
``` 
## add migration
```bash
dotnet ef migrations add MigrationName -s PresenceBot -p PresenceBot.Infrastructure -o Database/Migrations
```
## remove migrations from db
```bash
dotnet ef database update 0 -s PresenceBot -p PresenceBot.Infrastructure
```
## remove migration
```bash
dotnet ef migrations remove -s PresenceBot -p PresenceBot.Infrastructure
```