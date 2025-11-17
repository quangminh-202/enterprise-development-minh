var builder = DistributedApplication.CreateBuilder(args);

var mongo = builder.AddMongoDB("mongodb")
                   .WithDataVolume()
                   .AddDatabase("polyclinic");

builder.AddProject<Projects.Polyclinic_Api_Host>("api")
       .WithReference(mongo)
       .WaitFor(mongo)
       .WithExternalHttpEndpoints();

builder.Build().Run();