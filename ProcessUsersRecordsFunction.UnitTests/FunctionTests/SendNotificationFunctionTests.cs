using Moq;
using SendNotificaton;
using SendNotificaton.DataContext;
using SendNotificaton.Repositories;


namespace ProcessUsersRecordsFunction.UnitTests.FunctionTests
{
    public class SendNotificationFunctionTests
    {
        private Mock<IServiceBusRepository> mockServiceBusRepository;
        private Mock<IUserRecordsContextRepository> mockUserContextRecordsRepository;
        private TimerInfo mockTimerInfo;
        private List<Record> mockRecords;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            mockRecords = new List<Record>
               {
                    new Record { RecordId = "14020b70-8c44-4802-bb35-0e67156c07e8",UserId="User001",UserName="rabb1",UserEmail="rabb1@somedomain.com",DataValue="DataValu1",NotificationFlag=true},
                    new Record { RecordId = "21cb21c2-a460-4cdf-b5c3-9c7587469d4e",UserId="User002",UserName="rabb2",UserEmail="rabb2@somedomain.com",DataValue="DataValu2",NotificationFlag=false}

               };
            mockUserContextRecordsRepository = new Mock<IUserRecordsContextRepository>();
            mockUserContextRecordsRepository.Setup(repo => repo.GetUserRecordsUsingStoredProcedure()).Returns(mockRecords);
            
        }
        //This setup method resets the objects before each test run
        [SetUp]
        public void Setup()
        {
            // Arrange
            mockServiceBusRepository = new Mock<IServiceBusRepository>();
            mockTimerInfo = new TimerInfo(new ScheduleStatus(), false);
        }

        [Test]
        public void TimeTriggeredFunction_Should_Call_Methods_Inside_SendNotificationFunction()
        {
            var timeTriggeredFunction = new SendMessagesToServiceBus(mockUserContextRecordsRepository.Object, mockServiceBusRepository.Object);

            // Act
            timeTriggeredFunction.Run(mockTimerInfo);

            // Assert
            // Verify that the GetUserRecordsUsingStoredProcedure method of the UserContextRecordsRepository is called.
            mockUserContextRecordsRepository.Verify(r => r.GetUserRecordsUsingStoredProcedure(), Times.Once());

            // Verify that the SendMessagestoServiceBusQueueAsync method of the ServiceBusRepository is called with the expected parameters.
            mockServiceBusRepository.Verify(repo => repo.SendMessagestoServiceBusQueueAsync(It.IsAny<List<Record>>()), Times.Once);
        }
    }
}