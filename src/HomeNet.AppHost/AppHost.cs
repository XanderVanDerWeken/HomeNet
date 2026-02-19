using Projects;

var builder = DistributedApplication.CreateBuilder(args);

var dbName = "homenetdb";

var pg = builder.AddPostgres("homenet")
    .WithEnvironment("POSTGRES_DB", dbName)
    .WithBindMount("../../sql/postgres.initdb.sql", "/docker-entrypoint-initdb.d/initdb.sql")
    .WithDataVolume()
    .WithPgAdmin()
    .WithLifetime(ContainerLifetime.Persistent);
var db = pg.AddDatabase(dbName);

builder.AddProject<HomeNet_Web>("web")
    .WaitFor(db)
    .WithReference(db);

builder.Build().Run();
