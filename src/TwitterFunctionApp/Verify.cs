using System.Linq;
using System.Net;
using System.Net.Http;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using System.Configuration;
using TwitterFunctionApp.DAL;

namespace TwitterFunctionApp
{
    public static class Verify
    {
        [FunctionName("Verify")]
        public static async System.Threading.Tasks.Task<HttpResponseMessage> RunAsync([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = "Verify/{name}")]HttpRequestMessage req, string name, TraceWriter log)
        {
            log.Info("Verify " + name);

            var consumerKey = ConfigurationManager.AppSettings["ConsumerKey"];
            var consumerSecret = ConfigurationManager.AppSettings["ConsumerSecret"];

            var userData = new Users(ConfigurationManager.ConnectionStrings["PrimaryStorage"].ConnectionString);
            var user = await userData.GetUserAsync(name);
            if (user == null)
            {
                return req.CreateResponse(HttpStatusCode.BadRequest, "Can't find user " + name);
            }

            log.Info(user.ToString());

            var twitter = new Classes.TwitterClient(log);
            var response = twitter.VerifyCredentials(consumerKey, consumerSecret, user.OAuthToken, user.OAuthTokenSecret, user.OAuthVerifier);

            return req.CreateResponse(HttpStatusCode.OK, response);
        }
    }
}
