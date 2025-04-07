# 1. Migration
dot net ef migrations add ""
dotnet ef database update

# 2. Command
- Run docker compose : docker-compose -f docker-compose.yml -f docker-compose.override.yml up -d --remove-orphans
- account portainer http://local:9000 : admin - Nguyenkhai2611!
- account kibana http://localhost:5601 : elastic - admin
- rabbitMq http://localhost:15672 : guest - guest
- dotnet ef migrations add "Init_OrderDb" --project Ordering.Infrastructure --startup-project Ordering.API --output-dir Persistence\Migrations
- dotnet ef database update --project Ordering.Infrastructure --startup-project Ordering.API 
