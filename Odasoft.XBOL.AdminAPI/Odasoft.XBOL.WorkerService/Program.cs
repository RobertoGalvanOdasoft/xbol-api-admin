using Odasoft.XBOL.WorkerService.Extensions;

var builder = Host.CreateApplicationBuilder(args);

// Infrastructure (SMTP, Hangfire, etc.)
builder.Services.ConfigureOptions(builder.Configuration);

// Email & Templating
builder.Services.ConfigureEmail();

// Worker
builder.Services.ConfigureBackgroundJobs(builder.Configuration);

var host = builder.Build();
host.Run();
