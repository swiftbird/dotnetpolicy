using System.Collections.Generic;
using Policy.Models;
using Azure.Messaging.ServiceBus;
using Azure.Core;
using System.Diagnostics;
using Policy.Handlers;

namespace Policy.Services
{
    public class PolicyService : IPolicyService
    {

        private ServiceBusClient client;
        private ServiceBusSender sender;



        // Use a simple list to store policies in memory (you can replace this with a proper data store)
        private List<PolicyModel> _policies = new List<PolicyModel>();

        public IEnumerable<PolicyModel> GetPolicies()
        {
            return Enumerable.Range(1, 5).Select(index => new PolicyModel
            {
                Id = index,
                PolicyNumber = "123" + index,
                PolicyType = "OldOne",
                PolicyAmount = 12345

            })
            .ToArray();
            //return _policies;
        }

        public async Task<PolicyModel> CreatePolicyAsync(PolicyModel newPolicy)
        {
            _policies.Add(newPolicy);
            _policies.Add(new PolicyModel { Id = 2, PolicyAmount = 1, PolicyNumber = "112233", PolicyType = "typeofthings" });
            Console.WriteLine("Adding a policy to the list: " + newPolicy.Id);
            await sendPolicyUpdatedAsync(newPolicy);
            return newPolicy;
        }

        public async Task sendPolicyUpdatedAsync(PolicyModel newPolicy)
        {
            var clientOptions = new ServiceBusClientOptions()
            {
                TransportType = ServiceBusTransportType.AmqpWebSockets
            };
            //client = new ServiceBusClient("Endpoint=sb://ns-swiftbird.servicebus.windows.net/;SharedAccessKeyName=shaun;SharedAccessKey=21/C5dT3366KPFZXfT8YCzIsWOBw27q1Y+ASbLYraJQ=;EntityPath=claim_updated", clientOptions);
            //sender = client.CreateSender("claim_updated");

            client = new ServiceBusClient("Endpoint=sb://ns-swiftbird.servicebus.windows.net/;SharedAccessKeyName=shaun;SharedAccessKey=4t3sF/+ohuiqGW8860Or8tHb52zu5xmTe+ASbLXt3pk=;EntityPath=policy_updated", clientOptions);
            sender = client.CreateSender("policy_updated");


            // Send a single message
            try
            {
                Console.WriteLine("Creating a new policy with number: " + newPolicy.PolicyNumber);
                await sender.SendMessageAsync(new ServiceBusMessage(newPolicy.PolicyNumber));
                Console.WriteLine("Policy event put on the PolicyUpdated queue, man");
            }
            finally
            {
                // Calling DisposeAsync on client types is required to ensure that network
                // resources and other unmanaged objects are properly cleaned up.
                await sender.DisposeAsync();
                await client.DisposeAsync();
            }

        }

        public async Task InitializeAsync()
        {

            // Registering the listeners
            ClaimUpdatedHandler claimUpdatedHandler = new ClaimUpdatedHandler(this);
            await claimUpdatedHandler.StartSubscriptions();

            PolicyUpdatedHandler policyUpdatedHandler = new PolicyUpdatedHandler(this);
            await policyUpdatedHandler.StartSubscriptions();

            Console.WriteLine("I did the initialization");

        }
    }
}
