using Microsoft.WindowsAzure.Storage.Table;

namespace UsersApi.Storage
{
    public class AccountsTableEntity : TableEntity
    {
        public string Email => RowKey;
        public string Status { get; set; }

        public AccountsTableEntity(string email)
            : base(UsersApiConstants.AccountsTableEntityPartitionKey,
                email)
        {
            Status = "Active";
        }

        public AccountsTableEntity()
        {
        }
    }
}
