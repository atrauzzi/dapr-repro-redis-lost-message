using Consumer;
using Dapr.Client;
using Dapr.Messaging.PublishSubscribe.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHealthChecks();
builder.Services.AddDaprClient();
builder.Services.AddDaprPubSubClient();
builder.Services.AddHostedService<ConsumerService>();

var app = builder.Build();

app.MapHealthChecks("/healthz");

var daprClient = app.Services.GetRequiredService<DaprClient>();

await daprClient.WaitForSidecarAsync();

await daprClient.PublishEventAsync("test", "test", "SINGLE");

await app.RunAsync();


