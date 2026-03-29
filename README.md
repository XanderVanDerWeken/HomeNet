```bash
dotnet ef migrations add InitialSchema \
  --project src/HomeNet.Infrastructure \
  --startup-project src/HomeNet.Web \
  --context CardDbContext \
  --output-dir Persistence/Modules/Cards/Migrations

dotnet ef migrations add InitialSchema \
  --project src/HomeNet.Infrastructure \
  --startup-project src/HomeNet.Web \
  --context PersonDbContext \
  --output-dir Persistence/Modules/Persons/Migrations

dotnet ef migrations add InitialSchema \
  --project src/HomeNet.Infrastructure \
  --startup-project src/HomeNet.Web \
  --context UserDbContext \
  --output-dir Persistence/Modules/Auth/Migrations
```

```bash
dotnet ef database update \
  --project src/HomeNet.Infrastructure \
  --startup-project src/HomeNet.Web \
  --context CardDbContext

dotnet ef database update \
  --project src/HomeNet.Infrastructure \
  --startup-project src/HomeNet.Web \
  --context PersonDbContext

dotnet ef database update \
  --project src/HomeNet.Infrastructure \
  --startup-project src/HomeNet.Web \
  --context UserDbContext
```