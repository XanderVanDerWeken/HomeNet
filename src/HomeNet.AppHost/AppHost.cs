using Projects;

var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<HomeNet_Web>("web");

builder.Build().Run();
