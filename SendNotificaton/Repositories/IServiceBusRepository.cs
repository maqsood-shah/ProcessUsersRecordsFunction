using SendNotificaton.DataContext;
using SendNotificaton.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SendNotificaton.Repositories
{
    public interface IServiceBusRepository
    {
        Task SendMessagestoServiceBusQueueAsync(List<Record> usrRecordsResult);
        UserRecords Map(Record record);
    }
}
