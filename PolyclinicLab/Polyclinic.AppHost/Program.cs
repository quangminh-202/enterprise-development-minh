var builder = DistributedApplication.CreateBuilder(args);

// Add MongoDB
var mongo = builder.AddMongoDB("mongodb")
                   .WithDataVolume()
                   .AddDatabase("polyclinic");

// NATS parameters
var natsUserName = builder.AddParameter("NatsLogin");
var natsPassword = builder.AddParameter("NatsPassword");

// Add NATS message broker with JetStream
var nats = builder.AddNats("polyclinic-nats", userName: natsUserName, password: natsPassword, port: 4222)
                  .WithJetStream()
                  .WithArgs("-m", "8222")
                  .WithHttpEndpoint(port: 8222, targetPort: 8222);

// Add NATS NUI (Web UI for NATS management)
builder.AddContainer("nats-nui", "ghcr.io/nats-nui/nui")
       .WithReference(nats)
       .WaitFor(nats)
       .WithHttpEndpoint(port: 31311, targetPort: 31311);

// NATS configuration parameters
var natsStream = builder.AddParameter("NatsStream");
var rawSubject = builder.AddParameter("RawSubject");
var validatedSubject = builder.AddParameter("ValidatedSubject");

// Add Generator with NATS
builder.AddProject<Projects.Polyclinic_Generator_Nats_Host>("generator")
       .WithReference(nats)
       .WaitFor(nats)
       .WithEnvironment("Nats:StreamName", natsStream)
       .WithEnvironment("Nats:RawSubject", rawSubject)
       .WithExternalHttpEndpoints();

// Add API Host with MongoDB and NATS (includes Validator)
var apiHost = builder.AddProject<Projects.Polyclinic_Api_Host>("api")
       .WithReference(mongo)
       .WithReference(nats)
       .WaitFor(mongo)
       .WaitFor(nats)
       .WithEnvironment("Nats:StreamName", natsStream)
       .WithEnvironment("Nats:RawSubject", rawSubject)
       .WithEnvironment("Nats:ValidatedSubject", validatedSubject)
       .WithExternalHttpEndpoints();

builder.Build().Run();
