using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Dapr.Client;
using Dapr.Messaging.PublishSubscribe;
using Microsoft.Extensions.Hosting;

namespace Consumer;

public class ConsumerService(DaprClient daprClient, DaprPublishSubscribeClient pubsubClient) : IHostedLifecycleService
{
    public const string PubSub = "test";
    public const string Topic = "test";
    
    private IAsyncDisposable? subscription;

    public Task StartAsync(CancellationToken cancellationToken) => Task.CompletedTask;

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        if (subscription is null) throw new("Wha?");
        
        await subscription.DisposeAsync();
    }

    public async Task StartedAsync(CancellationToken cancellationToken)
    {
        subscription = await pubsubClient.SubscribeAsync(
            PubSub, 
            Topic,
            new(new(TimeSpan.FromSeconds(10), TopicResponseAction.Retry)),
            async (message, _) => {
                
                await Task.CompletedTask;

                var decoded = Encoding.UTF8.GetString(message.Data.Span);
                
                Console.WriteLine($"Received: {decoded}, waiting");
                
                await Task.Delay(TimeSpan.FromSeconds(20), cancellationToken);
                
                Console.WriteLine($"Marking {decoded} as processed.");
                
                return TopicResponseAction.Success;
            },
            cancellationToken
        );
        
        // var current = 0;
        //
        // while (! cancellationToken.IsCancellationRequested)
        // {
        //     ++current;
        //
        //     await daprClient.PublishEventAsync(PubSub, Topic, $"{current}", cancellationToken);
        //     
        //     Console.WriteLine($"Published: {current}");
        //     
        //     await Task.Delay(TimeSpan.FromSeconds(8), cancellationToken);
        //
        //     if (current == 10)
        //     {
        //         current = 0;
        //
        //         Console.WriteLine("---------");
        //     }
        // }
    }

    public Task StartingAsync(CancellationToken cancellationToken) => Task.CompletedTask;

    public Task StoppedAsync(CancellationToken cancellationToken) => Task.CompletedTask;

    public Task StoppingAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
