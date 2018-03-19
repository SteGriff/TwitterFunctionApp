using System.Net;
using System.Net.Http;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using System;
using System.Configuration;
using System.Threading.Tasks;
using TwitterFunctionApp.DAL;

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

            var userData = new Users(ConfigurationManager.ConnectionStrings["PrimaryStorage"].ConnectionString);
            var user = await userData.GetUserAsync(name);
            if (user == null)
            {
                return req.CreateResponse(HttpStatusCode.BadRequest, "Can't find user " + name);
            }

            log.Info(user.ToString());

            string theTweet = "Posted for " + name + " by TwitterFunctionApp at " + DateTime.Now.ToString("hh:MM");

            var twitter = new Classes.TwitterClient(log);
            twitter.PostTweet(theTweet, consumerKey, consumerSecret, user.OAuthToken, user.OAuthVerifier);

            return req.CreateResponse(HttpStatusCode.OK, "Posted for " + name);
        }
    }
}
