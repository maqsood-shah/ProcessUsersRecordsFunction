using Azure.Messaging.ServiceBus;
using Newtonsoft.Json;
using System.Text;
using SendNotificaton.Model;
using SendNotificaton.DataContext;
using log4net;

namespace SendNotificaton.Repositories
{
    public class ServiceBusRepository : IServiceBusRepository
    {
        private readonly ILog _logger = LogManager.GetLogger(typeof(ServiceBusRepository));
        private readonly string _connectionString;
        private readonly string _queueName;

        public ServiceBusRepository(string connectionString, string queueName)
        {
            _connectionString = connectionString;
            _queueName = queueName;
        }
        public async Task SendMessagestoServiceBusQueueAsync(List<Record> RecordsResult)
        {
            // The servicebus client that owns the connection and it used to create senders.
            ServiceBusClient client;

            // The servicebus sender used to publish messages to the queue
            ServiceBusSender sender;

            // set the transport type to AmqpWebSockets so that the ServiceBusClient uses the port 443. 
            // If you use the default AmqpTcp, you will need to make sure that the ports 5671 and 5672 are open
            var clientOptions = new ServiceBusClientOptions()
            {
                TransportType = ServiceBusTransportType.AmqpWebSockets
            };

            client = new ServiceBusClient(_connectionString, clientOptions);
            sender = client.CreateSender(_queueName);

            try
            {
                //using ServiceBusMessageBatch messageBatch = await sender.CreateMessageBatchAsync();

                foreach (var record in RecordsResult)
                {
                    //Convert UserRecords class object to jsonString so that we shouldn't get serialization error
                    string jsonConvertedServiceBusMessage = JsonConvert.SerializeObject(Map(record));
                    byte[] body = Encoding.ASCII.GetBytes(jsonConvertedServiceBusMessage);
                    //Send message to service bus
                    await sender.SendMessageAsync(new ServiceBusMessage(body));
                }

                _logger.Info($"{RecordsResult.Count} messages has been published to the queue.");

            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
            }
            finally
            {
                // Calling DisposeAsync on client types is required to ensure that network
                // resources and other unmanaged objects are properly cleaned up.
                await sender.DisposeAsync();
                await client.DisposeAsync();
            }
        }
        public UserRecords Map(Record record)
        {
            return new UserRecords
            {
                UserId = record.UserId,
                UserName = record.UserName,
                UserEmail = record.UserEmail,
                DataValue = record.DataValue,
                NotificationFlag = record.NotificationFlag
            };
        }
    }
}
