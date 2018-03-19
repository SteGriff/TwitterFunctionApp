using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TwitterFunctionApp.Interfaces
{
    using System;
    using TwitterFunctionApp.Classes;

    public interface ITwitter
    {
        Uri GetAuthorizeUri(string consumerKey, string consumerSecret, Uri callback);
        TwitterUser GetUser(string consumerKey, string consumerSecret, string oauthToken, string oauthVerifier);
    }
}
