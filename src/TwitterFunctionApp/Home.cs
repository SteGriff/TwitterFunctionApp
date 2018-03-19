using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using System.Net.Http.Headers;
using System.Text;

namespace TwitterFunctionApp
{
    public static class Home
    {
        [FunctionName("Home")]
        public static async Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "Home")]HttpRequestMessage req, TraceWriter log)
        {
            var html = @"<html><body><a href=""/api/Twitter"">Authorise Twitter</a></body></html>";
            var response = req.CreateResponse(HttpStatusCode.OK);
            response.Content = new StringContent(html, Encoding.UTF8, "text/html");
            //response.Content.Headers.ContentType = new MediaTypeHeaderValue("text/html");
            return response;
        }
    }
}
