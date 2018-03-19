using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using System;
using System.Net;
using System.Net.Http;
using System.Configuration;

namespace TwitterFunctionApp
{
    public static class Twitter
    {
        [FunctionName("Twitter")]
        public static HttpResponseMessage Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "Twitter")]HttpRequestMessage req, TraceWriter log)
        {
            log.Info("Start twitter auth");

            var consumerKey = ConfigurationManager.AppSettings["ConsumerKey"];
            var consumerSecret = ConfigurationManager.AppSettings["ConsumerSecret"];
            var twitterCallback = ConfigurationManager.AppSettings["TwitterCallback"];

            var twitter = new Classes.TwitterClient(log);
            var location = twitter.GetAuthorizeUri(consumerKey, consumerSecret, new Uri(twitterCallback));

            HttpResponseMessage response = req.CreateResponse(HttpStatusCode.Moved);
            response.Headers.Location = location;
            return response;
        }
    }
}
