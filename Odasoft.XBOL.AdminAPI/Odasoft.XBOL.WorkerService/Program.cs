using Odasoft.XBOL.Business.Extensions;
using Odasoft.XBOL.Data.Extensions;
using Odasoft.XBOL.WorkerService.Extensions;

var builder = Host.CreateApplicationBuilder(args);

// Infrastructure
builder.Services.ConfigureOptions(builder.Configuration);
builder.Services.ConfigureDatabase(builder.Configuration);

// Application
builder.Services.ConfigureRepositories();
builder.Services.ConfigureServices();

// Worker
builder.Services.ConfigureBackgroundJobs(builder.Configuration);

var host = builder.Build();
host.Run();
