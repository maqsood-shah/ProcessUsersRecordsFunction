using log4net;
using Microsoft.Azure.Functions.Worker;
using SendNotificaton.DataContext;
using SendNotificaton.Model;
using SendNotificaton.Repositories;
using TimerTriggerAttribute = Microsoft.Azure.Functions.Worker.TimerTriggerAttribute;

namespace SendNotificaton
{
    public class SendMessagesToServiceBus
    {
        private readonly ILog _logger;
        private readonly IUserRecordsContextRepository _userRecordsContextRepository;
        private readonly IServiceBusRepository _serviceBusRepository;

        public SendMessagesToServiceBus(IUserRecordsContextRepository userRecordsContextRepository, IServiceBusRepository serviceBusRepository)
        {
            _userRecordsContextRepository = userRecordsContextRepository;
            _serviceBusRepository = serviceBusRepository;
            _logger = LogManager.GetLogger(typeof(SendMessagesToServiceBus));
        }

        [Function(nameof(SendMessagesToServiceBus))]
        public void Run(
        [TimerTrigger("%ScheduleExpression%")] TimerInfo timerInfo)
        {
            _logger.Info($"Time TriggeredFunction SendMessagesToServiceBus started.");
            _logger.Info($"Time triggered send notification function execution started... at: {DateTime.Now}");

            // Perform your desired tasks based on TimerInfo
            if (timerInfo.IsPastDue)
            {
                _logger.Info("The TriggeredFunction SendMessagesToServiceBus is running late!");
            }

            List<Record> records = new List<Record>();
            try
            {
                // Call the stored procdure to get records from Database
                records = _userRecordsContextRepository.GetUserRecordsUsingStoredProcedure();
            }
            catch (Exception ex)
            {
                _logger.Error("Error fetching records from databsase using stored procedure" + ex.Message);
                throw;
            }
            try
            {
                // Call the SendMessagestoServiceBusQueueAsync method of ServiceBusRepository class to send List<Record> to service bus
                _serviceBusRepository.SendMessagestoServiceBusQueueAsync(records);
            }
            catch (Exception ex)
            {
                _logger.Error($"Error sending message to Service Bus: {ex.Message}");
                throw;
            }
            _logger.Info($"Time triggered Function SendMessagesToServiceBus succefully executed.");
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
    public class TimerInfo
    {
        public TimerInfo(ScheduleStatus? scheduleStatus,bool isPastDue)
        {
            ScheduleStatus = scheduleStatus;
            IsPastDue = isPastDue;
        }
        public ScheduleStatus? ScheduleStatus { get; set; }
        public bool IsPastDue { get; set; }
        
    }
    public class ScheduleStatus
    {
        public DateTime Last { get; set; }

        public DateTime Next { get; set; }

        public DateTime LastUpdated { get; set; }
    }
}
