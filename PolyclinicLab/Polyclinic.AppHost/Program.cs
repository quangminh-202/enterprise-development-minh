var builder = DistributedApplication.CreateBuilder(args);

// Add MongoDB
var mongo = builder.AddMongoDB("mongodb")
                   .WithDataVolume()
                   .AddDatabase("polyclinic");

// Add NATS message broker
var nats = builder.AddNats("nats")
                  .WithDataVolume();

// Add API Host with MongoDB and NATS
builder.AddProject<Projects.Polyclinic_Api_Host>("api")
       .WithReference(mongo)
       .WithReference(nats)
       .WaitFor(mongo)
       .WaitFor(nats)
       .WithExternalHttpEndpoints();

// Add Generator with NATS (with external endpoints for easy access)
builder.AddProject<Projects.Polyclinic_Generator_Nats_Host>("generator")
       .WithReference(nats)
       .WaitFor(nats)
       .WithExternalHttpEndpoints();

builder.Build().Run();