using System;
using Azure.Messaging.ServiceBus;
using Policy.Services;

namespace Policy.Handlers
{
	public class ClaimUpdatedHandler
	{

     
        private readonly IPolicyService _policyService;

        public ClaimUpdatedHandler(IPolicyService policyService)
		{
            //_logger = logger;
            _policyService = policyService;
        }

        public async Task StartSubscriptions()
        {
            // handle received messages from the claim queue
            async Task ClaimUpdatedProcessor(ProcessMessageEventArgs args)
            {
                string body = args.Message.Body.ToString();
                Console.WriteLine($"Received from the Claim Queue: {body}");

                // complete the message. message is deleted from the queue. 
                await args.CompleteMessageAsync(args.Message);
            }

            
            // handle any errors when receiving messages
            Task ErrorHandler(ProcessErrorEventArgs args)
            {
                Console.WriteLine(args.Exception.ToString());
                return Task.CompletedTask;
            }

            var clientOptions = new ServiceBusClientOptions()
            {
                TransportType = ServiceBusTransportType.AmqpWebSockets
            };

            ServiceBusClient claimUpdatedClient = new ServiceBusClient("Endpoint=sb://ns-swiftbird.servicebus.windows.net/;SharedAccessKeyName=shaun;SharedAccessKey=21/C5dT3366KPFZXfT8YCzIsWOBw27q1Y+ASbLYraJQ=;EntityPath=claim_updated", clientOptions);
            ServiceBusProcessor claimUpdatedProcessor = claimUpdatedClient.CreateProcessor("claim_updated", new ServiceBusProcessorOptions());

           
            try
            {
                // add handler to process messages
                claimUpdatedProcessor.ProcessMessageAsync += ClaimUpdatedProcessor;

                // add handler to process any errors
                claimUpdatedProcessor.ProcessErrorAsync += ErrorHandler;

                // start processing claim changes
                await claimUpdatedProcessor.StartProcessingAsync();

                Console.WriteLine("Registerd handler for claim updated");

                // stop processing 
                //Console.WriteLine("\nStopping the receiver...");
                //await processor.StopProcessingAsync();
                //Console.WriteLine("Stopped receiving messages");
            }
            finally
            {
                // Calling DisposeAsync on client types is required to ensure that network
                // resources and other unmanaged objects are properly cleaned up.
                //await processor.DisposeAsync();
                //await client.DisposeAsync();
            }
        }
	}
}

