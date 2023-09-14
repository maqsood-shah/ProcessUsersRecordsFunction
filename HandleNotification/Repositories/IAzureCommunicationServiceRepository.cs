using HandleNotification.Model;

namespace HandleNotification.Repositories
{
    public interface IAzureCommunicationServiceRepository
    {
        Task SendEmailNotificatioinAsync(UserRecords userRecord);
    }
}
