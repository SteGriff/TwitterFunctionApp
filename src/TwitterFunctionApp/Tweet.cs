using System.Linq;
using System.Net;
using System.Net.Http;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using System;
using SteGriff.AzureStorageTools;
using System.Configuration;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Table;
using TwitterFunctionApp.Entities;

namespace TwitterFunctionApp
{
    public static class Tweet
    {
        [FunctionName("Tweet")]
        public static async Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = "Tweet/{name}")]HttpRequestMessage req, string name, TraceWriter log)
        {
            log.Info("Running Tweet Function for " + name);

            var consumerKey = ConfigurationManager.AppSettings["ConsumerKey"];
            var consumerSecret = ConfigurationManager.AppSettings["ConsumerSecret"];
            
            var storageProvider = new AzureStorageProvider(ConfigurationManager.ConnectionStrings["PrimaryStorage"].ConnectionString);
            var tableProvider = new AzureTableProvider(storageProvider);
            var table = await tableProvider.GetTableAsync("users");

            var findUser = TableOperation.Retrieve<UserEntity>(name, name);

            var result = await table.ExecuteAsync(findUser);
            if (result == null)
            {
                return req.CreateResponse(HttpStatusCode.BadRequest, "Can't find user " + name);
            }

            var user = (UserEntity)result.Result;
            log.Info(user.ToString());

            string theTweet = "Posted for " + name + " by TwitterFunctionApp at " + DateTime.Now.ToString("hh:MM");

            var twitter = new Classes.TwitterClient(log);
            twitter.PostTweet(theTweet, consumerKey, consumerSecret, user.OAuthToken, user.OAuthVerifier);

            return req.CreateResponse(HttpStatusCode.OK, "Posted for " + name);
        }
    }
}
