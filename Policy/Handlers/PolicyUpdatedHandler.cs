using System;
using Azure.Messaging.ServiceBus;
using Policy.Controllers;
using Policy.Services;

namespace Policy.Handlers
{
    public class PolicyUpdatedHandler
    {


        private readonly IPolicyService _policyService;

        public PolicyUpdatedHandler(IPolicyService policyService)
        {
            //_logger = logger;
            _policyService = policyService;
        }

        public async Task StartSubscriptions()
        {

            // handle received messages from the policy queue
            async Task PolicyUpdatedProcessor(ProcessMessageEventArgs args)
            {
                string body = args.Message.Body.ToString();
                Console.WriteLine($"Received from the Policy Queue: {body}");

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

            ServiceBusClient policyUpdatedClient = new ServiceBusClient("Endpoint=sb://ns-swiftbird.servicebus.windows.net/;SharedAccessKeyName=shaun;SharedAccessKey=4t3sF/+ohuiqGW8860Or8tHb52zu5xmTe+ASbLXt3pk=;EntityPath=policy_updated", clientOptions);
            ServiceBusProcessor policyUpdatedProcessor = policyUpdatedClient.CreateProcessor("policy_updated", new ServiceBusProcessorOptions());

            try
            {
               
                // add handler to process messages
                policyUpdatedProcessor.ProcessMessageAsync += PolicyUpdatedProcessor;

                // add handler to process any errors, but use the same handler as the other one
                policyUpdatedProcessor.ProcessErrorAsync += ErrorHandler;

                // start processing claim changes
                await policyUpdatedProcessor.StartProcessingAsync();

                Console.WriteLine("Registerd handler for policy updated");


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

