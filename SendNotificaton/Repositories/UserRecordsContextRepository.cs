using System.Collections.Generic;
using SendNotificaton.DataContext;

namespace SendNotificaton.Repositories
{
    public class UserRecordsContextRepository : IUserRecordsContextRepository
    {
        private readonly UsersRecordsContext _dbContext;
        public UserRecordsContextRepository(UsersRecordsContext dbContext)
        {
            _dbContext = dbContext;
        }
        public List<Record> GetUserRecordsUsingStoredProcedure()
        {
            return _dbContext.GetUsersRecordsFromStoredProcedure();
        }
    }
}
