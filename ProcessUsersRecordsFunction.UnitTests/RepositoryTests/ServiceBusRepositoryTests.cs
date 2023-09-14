using Azure.Messaging.ServiceBus;
using Moq;
using Newtonsoft.Json;
using SendNotificaton.DataContext;
using SendNotificaton.Model;
using System.Text;

namespace ProcessUsersRecordsFunction.UnitTests.RepositoryTests
{
    public class ServiceBusRepositoryTests
    {
        private List<Record> mockRecords;
        string MockConnectionString = "Endpoint=sb://mock-servicebus.servicebus.windows.net/;SharedAccessKeyName=mock-key-name;SharedAccessKey=mock-key-value";
        string QueueName = "mock_queue_name";
        Mock<ServiceBusClient> serviceBusClientMock;
        Mock<ServiceBusSender> serviceBusSenderMock;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            // Arrange
            mockRecords = new List<Record>
               {
                    new Record { Id=1, RecordId = "14020b70-8c44-4802-bb35-0e67156c07e8",UserId="User001",UserName="user1",UserEmail="user1@somedomain.com",DataValue="DataValu1",NotificationFlag=true,CreatedTime= Convert.ToDateTime("2023-09-01 16:17:19.313")},
                    new Record { Id=2,RecordId = "21cb21c2-a460-4cdf-b5c3-9c7587469d4e",UserId="User002",UserName="user2",UserEmail="user2@somedomain.com",DataValue="DataValu2",NotificationFlag=true,CreatedTime= Convert.ToDateTime("2023-09-01 16:17:25.313")}
               };
        }
        [SetUp]
        public void Setup()
        {
            // Arrange
            var clientOptions = new ServiceBusClientOptions()
            {
                TransportType = ServiceBusTransportType.AmqpWebSockets
            };
            serviceBusClientMock = new Mock<ServiceBusClient>(MockConnectionString, clientOptions);
            serviceBusSenderMock = new Mock<ServiceBusSender>();
            serviceBusClientMock.Setup(client => client.CreateSender(QueueName))
                .Returns(serviceBusSenderMock.Object);
        }

        [Test]
        public async Task Send_MessageListToServiceBusQueue_ValidRecords_Success()
        {
            foreach (var record in mockRecords)
            {
                string jsonConvertedServiceBusMessage = JsonConvert.SerializeObject(Map(record));
                byte[] body = Encoding.ASCII.GetBytes(jsonConvertedServiceBusMessage);

                serviceBusSenderMock
               .Setup(m => m.SendMessagesAsync(
                   It.Is<IEnumerable<ServiceBusMessage>>(value => value.Select(s => s.Body.ToArray()).FirstOrDefault() == body),
                   It.IsAny<CancellationToken>()))
               .Returns(Task.CompletedTask)
               .Verifiable();

                await serviceBusSenderMock.Object.SendMessageAsync(new ServiceBusMessage(body));
            }
        }

        [Test]
        public void Send_Null_MessageListToServiceBusQueue_Exception()
        {
            var mock = new Mock<ServiceBusSender>()
            {
                CallBase = true
            };
            Assert.ThrowsAsync<ArgumentNullException>(async () => await mock.Object.SendMessagesAsync(messages: null));
        }

        private UserRecords Map(Record record)
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
