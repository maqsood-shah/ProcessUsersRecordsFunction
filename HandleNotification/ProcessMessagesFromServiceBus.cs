using HandleNotification.Model;
using HandleNotification.Repositories;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace HandleNotification
{
    public class ProcessMessagesFromServiceBus
    {
        private readonly ILogger<ProcessMessagesFromServiceBus> _logger;
        private readonly IAzureCommunicationServiceRepository _azureCommunicationServiceRepository;

        public ProcessMessagesFromServiceBus(IAzureCommunicationServiceRepository azureCommunicationServiceRepository, ILogger<ProcessMessagesFromServiceBus> logger)
        {
            _azureCommunicationServiceRepository = azureCommunicationServiceRepository;
            _logger = logger;
        }

        [Function(nameof(ProcessMessagesFromServiceBus))]
        public async Task RunAsync([ServiceBusTrigger("%ServiceBusQueueName%", Connection = "AzureWebJobsServiceBus")] string messages)
        {
            // Deserialize the message
            UserRecords? serviceBusMessage = JsonConvert.DeserializeObject<UserRecords>(messages);

            // Handle the message
            if (serviceBusMessage != null && serviceBusMessage.NotificationFlag) await _azureCommunicationServiceRepository.SendEmailNotificatioinAsync(serviceBusMessage);
        }
    }
}
