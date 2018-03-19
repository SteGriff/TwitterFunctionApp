using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using System.Configuration;
using SteGriff.AzureStorageTools;
using Microsoft.WindowsAzure.Storage.Table;
using TwitterFunctionApp.Entities;

namespace TwitterFunctionApp
{
    public static class OAuth
    {
        [FunctionName("OAuth")]
        public static async Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = "OAuth")]HttpRequestMessage req, TraceWriter log)
        {
            log.Info("OAuth Response");

            var requestValues = req.GetQueryNameValuePairs();
            
            string oauthToken = requestValues
                .FirstOrDefault(q => string.Compare(q.Key, "oauth_token", true) == 0)
                .Value;

            string oauthVerifier = requestValues
                .FirstOrDefault(q => string.Compare(q.Key, "oauth_verifier", true) == 0)
                .Value;

            var consumerKey = ConfigurationManager.AppSettings["ConsumerKey"];
            var consumerSecret = ConfigurationManager.AppSettings["ConsumerSecret"];
            var twitterCallback = ConfigurationManager.AppSettings["TwitterCallback"];

            var twitter = new Classes.TwitterClient(log);
            var user = twitter.GetUser(consumerKey, consumerSecret, oauthToken, oauthVerifier);
            
            var storageProvider = new AzureStorageProvider(ConfigurationManager.ConnectionStrings["PrimaryStorage"].ConnectionString);
            var tableProvider = new AzureTableProvider(storageProvider);
            var table = await tableProvider.GetTableAsync("users");

            var userEntity = new UserEntity(user.UserName, user.UserName);
            var insert = TableOperation.Insert(userEntity);
            await table.ExecuteAsync(insert);
            
            return req.CreateResponse(HttpStatusCode.OK);
        }
    }
}
