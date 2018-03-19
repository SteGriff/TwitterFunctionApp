namespace TwitterFunctionApp.Classes
{
    using Microsoft.Azure.WebJobs.Host;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using TwitterFunctionApp.Interfaces;
    using System.Web;

    public class TwitterClient : ITwitter
    {
        private TraceWriter log;

        public TwitterClient(TraceWriter logger)
        {
            log = logger;
        }

        public Uri GetAuthorizeUri(string consumerKey, string consumerSecret, Uri callback)
        {
            log.Info("GetAuthorizeUri " + consumerKey + " :: " + consumerSecret + " :: " + callback.ToString());
            return
                new Uri("https://api.twitter.com/oauth/authorize?oauth_token=" +
                        GetOAuthToken(consumerKey, consumerSecret, callback));
        }

        public TwitterUser GetUser(string consumerKey, string consumerSecret, string oauthToken, string oauthVerifier)
        {
            log.Info("GetUser " + consumerKey + " :: " + consumerSecret + " :: " + oauthToken + " :: " + oauthVerifier);

            var oauthParameters = new OAuthParameterSet(consumerKey, consumerSecret, oauthToken)
                                      {
                                          {OAuthParameter.Verifier, oauthVerifier}
                                      };
            string response;
            using (var webClient = new WebClient())
            {
                response = webClient.DownloadString(GetAccessTokenUri(), oauthParameters);
            }
            if (response == null) throw new InvalidOperationException("Failed to get user");

            log.Info(response);

            Dictionary<string, string> values =
                    response.Split('&').Select(section => section.Split('=')).ToDictionary(
                        bits => bits[0], bits => bits[1]);

            return new TwitterUser
            {
                Token = values["oauth_token"],
                TokenSecret = values["oauth_token_secret"],
                UserId = long.Parse(values["user_id"]),
                ScreenName = values["screen_name"]
            };
        }

        private Uri GetRequestTokenUri(Uri callback)
        {
            log.Info("GetRequestTokenUri " + callback.ToString());

            return
                new Uri("https://api.twitter.com/oauth/request_token?oauth_callback=" + callback.ToString().UrlEncode());
        }

        private Uri GetAccessTokenUri()
        {
            log.Info("GetAccessTokenUri ");

            return new Uri("https://api.twitter.com/oauth/access_token");
        }

        private string GetOAuthToken(string consumerKey, string consumerSecret, Uri callback)
        {
            log.Info("GetOAuthToken " + consumerKey + " :: " + consumerSecret + " :: " + callback.ToString());

            var oauthParameters = new OAuthParameterSet(consumerKey, consumerSecret)
                                      {
                                          {OAuthParameter.Callback, callback.ToString()},
                                      };

            string response;
            using (var webClient = new WebClient())
            {
                response = webClient.UploadString(GetRequestTokenUri(callback), string.Empty, oauthParameters);
            }

            if (response == null) throw new InvalidOperationException("Failed to get OAuth token");

            Dictionary<string, string> values =
                response.Split('&').Select(section => section.Split('=')).ToDictionary(
                    bits => bits[0], bits => bits[1]);
            return values["oauth_token"];
        }

        private Uri VerifyCredentialsUri()
        {
            log.Info("VerifyCredentialsUri");
            return new Uri("https://api.twitter.com/1.1/account/verify_credentials.json");
        }

        public string VerifyCredentials(string consumerKey, string consumerSecret, string oauthToken, string oauthTokenSecret, string oauthVerifier)
        {
            log.Info("VerifyCredentials " + consumerKey + " :: " + consumerSecret + " :: " + oauthToken + " :: " + oauthVerifier + " :: " + oauthTokenSecret);

            var oauthParameters =
                new OAuthParameterSet(consumerKey, consumerSecret, oauthToken, oauthTokenSecret)
                {
                    {OAuthParameter.Verifier, oauthVerifier}
                };

            string response = null;
            using (var webClient = new WebClient())
            {
                var uri = VerifyCredentialsUri();
                var oauth = oauthParameters.GetOAuthHeaderString(uri, "GET");
                log.Info(uri.ToString());
                log.Info(oauth);
                log.Info("Verify Credentials...");
                try
                {
                    response = webClient.DownloadString(uri, oauthParameters);
                }
                catch (Exception ex)
                {
                    log.Error(ex.ToString());
                }
            }

            if (response == null) throw new InvalidOperationException("Failed to post tweet");

            return response;
        }

        private Uri PostTweetUri(string tweetText)
        {
            log.Info("PostTweetUri");
            string encodedTweet = tweetText.UrlEncode();
            return new Uri("https://api.twitter.com/1.1/statuses/update.json?status=" + encodedTweet);
        }

        public string PostTweet(string tweet, string consumerKey, string consumerSecret, string oauthToken, string oauthTokenSecret, string oauthVerifier)
        {
            log.Info("PostTweet " + consumerKey + " :: " + consumerSecret + " :: " + oauthToken + " :: " + oauthVerifier + " :: " + oauthTokenSecret);

            var oauthParameters =
                new OAuthParameterSet(consumerKey, consumerSecret, oauthToken, oauthTokenSecret)
                {
                    {OAuthParameter.Verifier, oauthVerifier}
                };

            string response = null;
            using (var webClient = new WebClient())
            {
                var uri = PostTweetUri(tweet);
                var oauth = oauthParameters.GetOAuthHeaderString(uri, "POST");
                log.Info(uri.ToString());
                log.Info(oauth);
                log.Info("Send tweet...");
                try
                {
                    response = webClient.UploadString(uri, string.Empty, oauthParameters);
                }
                catch (Exception ex)
                {
                    log.Error(ex.ToString());
                }
            }

            if (response == null) throw new InvalidOperationException("Failed to post tweet");

            return response;
        }
    }
}
