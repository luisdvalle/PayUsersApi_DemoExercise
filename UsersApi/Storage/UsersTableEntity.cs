using Microsoft.WindowsAzure.Storage.Table;

namespace UsersApi.Storage
{
    public class UsersTableEntity : TableEntity
    {
        public string Name { get; set; }
        public string Email => RowKey;
        public double MonthlySalary { get; set; }
        public double MonthlyExpenses { get; set; }

        public UsersTableEntity(string email)
            : base(UsersApiConstants.UsersTableEntityPartitionKey,
                email)
        {
        }

        public UsersTableEntity()
        {
        }
    }
}
