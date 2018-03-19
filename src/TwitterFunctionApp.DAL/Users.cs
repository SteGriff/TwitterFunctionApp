using Microsoft.WindowsAzure.Storage.Table;
using SteGriff.AzureStorageTools;
using System;
using System.Threading.Tasks;
using TwitterFunctionApp.DAL.Entities;

namespace TwitterFunctionApp.DAL
{
    public class Users
    {
        string _connectionString;

        public Users(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<string> InsertUserAsync(UserEntity userEntity)
        {
            try
            {
                var storageProvider = new AzureStorageProvider(_connectionString);
                var tableProvider = new AzureTableProvider(storageProvider);
                var table = await tableProvider.GetTableAsync("users");
                
                var insert = TableOperation.Insert(userEntity);
                await table.ExecuteAsync(insert);
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
            return "";
        }

        public async Task<UserEntity> GetUserAsync(string name)
        {
            var storageProvider = new AzureStorageProvider(_connectionString);
            var tableProvider = new AzureTableProvider(storageProvider);
            var table = await tableProvider.GetTableAsync("users");

            var findUser = TableOperation.Retrieve<UserEntity>(name, name);

            var result = await table.ExecuteAsync(findUser);
            if (result == null)
            {
                return null;
            }

            var user = (UserEntity)result.Result;
            return user;
        }
    }
}
