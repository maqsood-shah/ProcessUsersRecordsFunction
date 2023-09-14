using SendNotificaton.DataContext;
using System.Collections.Generic;

namespace SendNotificaton.Repositories
{
    public interface IUserRecordsContextRepository
    {
        List<Record> GetUserRecordsUsingStoredProcedure();
    }
}
