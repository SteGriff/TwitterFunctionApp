using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using System.Configuration;
using TwitterFunctionApp.DAL;
using TwitterFunctionApp.DAL.Entities;
using System;

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

            var usersData = new Users(ConfigurationManager.ConnectionStrings["PrimaryStorage"].ConnectionString);
            var userEntity = new UserEntity(user.UserName, user.UserName)
            {
                OAuthToken = oauthToken,
                OAuthVerifier = oauthVerifier,
                UpdatedDate = DateTime.Now
            };

            var result = await usersData.InsertUserAsync(userEntity);

            if (string.IsNullOrEmpty(result))
            {
                return req.CreateResponse(HttpStatusCode.OK);
            }
            else
            {
                return req.CreateResponse<string>(HttpStatusCode.InternalServerError, result);
            }
        }
    }
}
