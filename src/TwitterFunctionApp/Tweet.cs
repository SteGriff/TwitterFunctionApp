using System.Net;
using System.Net.Http;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using System;
using System.Configuration;
using System.Threading.Tasks;
using TwitterFunctionApp.DAL;
using TwitterFunctionApp.Classes;

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

            string tweet1 = "Posted for " + name + " by TwitterFunctionApp at " + DateTime.Now.ToString("hh:MM:ss.ff") + " (Method 1)";
            var tweetSender = new TweetSender(log, consumerKey, consumerSecret, user.OAuthToken, user.OAuthTokenSecret);
            var result = await tweetSender.Tweet(tweet1);

            string tweet2 = "Posted for " + name + " by TwitterFunctionApp at " + DateTime.Now.ToString("hh:MM:ss.ff") + " (Method 2)";
            var twitter = new Classes.TwitterClient(log);
            twitter.PostTweet(tweet2, consumerKey, consumerSecret, user.OAuthToken, user.OAuthTokenSecret, user.OAuthVerifier);

            return req.CreateResponse(HttpStatusCode.OK, result);
        }
    }
}
