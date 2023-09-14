using HandleNotification;
using HandleNotification.Model;
using HandleNotification.Repositories;
using Microsoft.Extensions.Logging;
using Moq;

namespace ProcessUsersRecordsFunction.UnitTests.FunctionTests
{
    public class HandleNotificationFunctionTests
    {
        private Mock<IAzureCommunicationServiceRepository> azureCommunicationServiceRepository;
        private Mock<ILogger<ProcessMessagesFromServiceBus>> mockLogger;
        private string serviceBusMessage;

        //This setup method resets the objects before each test run
        [SetUp]
        public void Setup()
        {
            // Arrange
            azureCommunicationServiceRepository = new Mock<IAzureCommunicationServiceRepository>();
            mockLogger = new Mock<ILogger<ProcessMessagesFromServiceBus>>();
            serviceBusMessage = "{\"RecordId\":\"\",\"UserId\":\"User1\",\"UserName\":\"rabb1\",\"UserEmail\":\"rabb1@somedomain.com\",\"DataValue\":\"0001A\",\"NotificationFlag\":true}";
        }
        [Test]
        public async Task ServiceBusTriggeredFunction_GetUserRecordsUsingStoredProcedure()
        {
            // Arrange
            var serviceBusTriggeredFunction = new ProcessMessagesFromServiceBus(azureCommunicationServiceRepository.Object, mockLogger.Object);

            // Act
            await serviceBusTriggeredFunction.RunAsync(serviceBusMessage);

            // Assert
            // Verify that the SendEmailNotificatioinAsync method of the azureCommunicationServiceRepository is called.
            azureCommunicationServiceRepository.Verify(r => r.SendEmailNotificatioinAsync(It.IsAny<UserRecords>()), Times.Once());
        }
    }
}
