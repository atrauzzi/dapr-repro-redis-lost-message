using Dapr.Client;
using Dapr.Messaging.PublishSubscribe.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHealthChecks();
builder.Services.AddDaprClient();
builder.Services.AddDaprPubSubClient();

var app = builder.Build();

app.MapHealthChecks("/healthz");

var daprClient = app.Services.GetRequiredService<DaprClient>();

await daprClient.WaitForSidecarAsync();

for (int i = 1; i <= 1; i++)
{
    await daprClient.PublishEventAsync("test", "test", $"Event-{i}");
}

await app.RunAsync();


