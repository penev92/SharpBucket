﻿using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using RestSharp;
using SharpBucket.Authentication;
using SharpBucket.Utility;

namespace SharpBucket
{
    /// <summary>
    /// A client for the BitBucket API. It supports V1 and V2 of the API.
    /// More info:
    /// https://confluence.atlassian.com/display/BITBUCKET/Use+the+Bitbucket+REST+APIs
    /// </summary>
    public abstract class SharpBucket : ISharpBucket
    {
        private Authenticate authenticator;

        /// <summary>
        /// The base URL exposing the BitBucket API.
        /// </summary>
        protected string BaseUrl { get; }

        private RequestExecutor RequestExecutor { get; }

        internal SharpBucket(string baseUrl, RequestExecutor requestExecutor)
        {
            this.BaseUrl = baseUrl;
            this.RequestExecutor = requestExecutor;
            NoAuthentication();
        }

        /// <summary>
        /// Do not use authentication with the BitBucket API. Only public data can be retrieved.
        /// </summary>
        public void NoAuthentication()
        {
            authenticator = new NoAuthentication(BaseUrl) { RequestExecutor = this.RequestExecutor };
        }

        /// <summary>   
        /// Use basic authentication with the BitBucket API. OAuth authentication is preferred over
        /// basic authentication, due to security reasons.
        /// </summary>
        /// <param name="username">Your BitBucket user name.</param>
        /// <param name="password">Your BitBucket password.</param>
        public void BasicAuthentication(string username, string password)
        {
            authenticator = new BasicAuthentication(username, password, BaseUrl) { RequestExecutor = this.RequestExecutor };
        }

        /// <summary>
        /// Use 2 legged OAuth 1.0a authentication. This is similar to basic authentication, since
        /// it requires the same number of steps. It is still safer to use than basic authentication, 
        /// since you can revoke the API keys.
        /// More info:
        /// https://confluence.atlassian.com/display/BITBUCKET/OAuth+on+Bitbucket
        /// </summary>
        /// <param name="consumerKey">Your consumer API key obtained from the BitBucket web page.</param>
        /// <param name="consumerSecretKey">Your consumer secret API key also obtained from the BitBucket web page.</param>
        [Obsolete("Use OAuth1TwoLeggedAuthentication instead")]
        public void OAuth2LeggedAuthentication(string consumerKey, string consumerSecretKey)
        {
            authenticator = new OAuthentication2Legged(consumerKey, consumerSecretKey, BaseUrl) { RequestExecutor = this.RequestExecutor };
        }

        /// <summary>
        /// Use 2 legged OAuth 1.0a authentication. This is similar to basic authentication, since
        /// it requires the same number of steps. It is still safer to use than basic authentication, 
        /// since you can revoke the API keys.
        /// More info:
        /// https://confluence.atlassian.com/display/BITBUCKET/OAuth+on+Bitbucket
        /// </summary>
        /// <param name="consumerKey">Your consumer API key obtained from the BitBucket web page.</param>
        /// <param name="consumerSecretKey">Your consumer secret API key also obtained from the BitBucket web page.</param>
        public void OAuth1TwoLeggedAuthentication(string consumerKey, string consumerSecretKey)
        {
            authenticator = new OAuth1TwoLeggedAuthentication(consumerKey, consumerSecretKey, BaseUrl) { RequestExecutor = this.RequestExecutor };
        }

        /// <summary>
        /// Use 3 legged OAuth 1.0a authentication. This is the most secure one, but for simple uses it might
        /// be a bit too complex.
        /// More info:
        /// https://confluence.atlassian.com/display/BITBUCKET/OAuth+on+Bitbucket
        /// </summary>
        /// <param name="consumerKey">Your consumer API key obtained from the BitBucket web page.</param>
        /// <param name="consumerSecretKey">Your consumer secret API key also obtained from the BitBucket web page.</param>
        /// <param name="callback">Callback URL to which BitBucket will send the pin.</param>
        /// <returns></returns>
        [Obsolete("Use OAuth1ThreeLeggedAuthentication instead")]
        public OAuthentication3Legged OAuth3LeggedAuthentication(
            string consumerKey,
            string consumerSecretKey,
            string callback = "oob")
        {
            var oauthentication3Legged = new OAuthentication3Legged(consumerKey, consumerSecretKey, callback, BaseUrl) { RequestExecutor = this.RequestExecutor };
            authenticator = oauthentication3Legged;
            return oauthentication3Legged;
        }

        /// <summary>
        /// Use 3 legged OAuth 1.0a authentication. This is the most secure one, but for simple uses it might
        /// be a bit too complex.
        /// More info:
        /// https://confluence.atlassian.com/display/BITBUCKET/OAuth+on+Bitbucket
        /// </summary>
        /// <param name="consumerKey">Your consumer API key obtained from the BitBucket web page.</param>
        /// <param name="consumerSecretKey">Your consumer secret API key also obtained from the BitBucket web page.</param>
        /// <param name="callback">Callback URL to which BitBucket will send the pin.</param>
        /// <returns></returns>
        public OAuth1ThreeLeggedAuthentication OAuth1ThreeLeggedAuthentication(
            string consumerKey,
            string consumerSecretKey,
            string callback = "oob")
        {
            var oauth1ThreeLeggedAuthentication = new OAuth1ThreeLeggedAuthentication(consumerKey, consumerSecretKey, callback, BaseUrl) { RequestExecutor = this.RequestExecutor };
            authenticator = oauth1ThreeLeggedAuthentication;
            return oauth1ThreeLeggedAuthentication;
        }

        /// <summary>
        /// Use 3 legged OAuth 1.0a authentication. Use this method if you have already obtained the OAuthToken
        /// and OAuthSecretToken. This method can be used so you do not have to go through the whole 3 legged
        /// process every time. You can save the tokens you receive the first time and reuse them in another session.
        /// </summary>
        /// <param name="consumerKey">Your consumer API key obtained from the BitBucket web page.</param>
        /// <param name="consumerSecretKey">Your consumer secret API key also obtained from the BitBucket web page.</param>
        /// <param name="oauthToken">Your OAuth token that was obtained on a previous session.</param>
        /// <param name="oauthTokenSecret">Your OAuth secret token that was obtained on a previous session.</param>
        /// <returns></returns>
        [Obsolete("Use OAuth1ThreeLeggedAuthentication instead")]
        public OAuthentication3Legged OAuth3LeggedAuthentication(
            string consumerKey,
            string consumerSecretKey,
            string oauthToken,
            string oauthTokenSecret)
        {
            var oauthentication3Legged = new OAuthentication3Legged(
                consumerKey,
                consumerSecretKey,
                oauthToken,
                oauthTokenSecret,
                BaseUrl)
            {
                RequestExecutor = this.RequestExecutor
            };
            authenticator = oauthentication3Legged;
            return oauthentication3Legged;
        }

        /// <summary>
        /// Use 3 legged OAuth 1.0a authentication. Use this method if you have already obtained the OAuthToken
        /// and OAuthSecretToken. This method can be used so you do not have to go through the whole 3 legged
        /// process every time. You can save the tokens you receive the first time and reuse them in another session.
        /// </summary>
        /// <param name="consumerKey">Your consumer API key obtained from the BitBucket web page.</param>
        /// <param name="consumerSecretKey">Your consumer secret API key also obtained from the BitBucket web page.</param>
        /// <param name="oauthToken">Your OAuth token that was obtained on a previous session.</param>
        /// <param name="oauthTokenSecret">Your OAuth secret token that was obtained on a previous session.</param>
        /// <returns></returns>
        public void OAuth1ThreeLeggedAuthentication(
            string consumerKey,
            string consumerSecretKey,
            string oauthToken,
            string oauthTokenSecret)
        {
            authenticator = new OAuth1ThreeLeggedAuthentication(
                consumerKey,
                consumerSecretKey,
                oauthToken,
                oauthTokenSecret,
                BaseUrl)
            {
                RequestExecutor = this.RequestExecutor
            };
        }

        /// <summary>
        /// Use Oauth2 authentication. This is the newest version and is preferred.
        /// </summary>
        /// <param name="consumerKey"></param>
        /// <param name="consumerSecretKey"></param>
        /// <returns></returns>
        [Obsolete("Use OAuth2ClientCredentials instead")]
        public OAuthentication2 OAuthentication2(string consumerKey, string consumerSecretKey)
        {
            var oaAuthentication2 = new OAuthentication2(consumerKey, consumerSecretKey, BaseUrl) { RequestExecutor = this.RequestExecutor };
            authenticator = oaAuthentication2;
            oaAuthentication2.GetToken();
            return oaAuthentication2;
        }

        /// <summary>
        /// Use Oauth2 authentication. This is the newest version and is preferred.
        /// </summary>
        /// <param name="consumerKey"></param>
        /// <param name="consumerSecretKey"></param>
        /// <returns></returns>
        public void OAuth2ClientCredentials(string consumerKey, string consumerSecretKey)
        {
            authenticator = new OAuth2ClientCredentials(consumerKey, consumerSecretKey, BaseUrl) { RequestExecutor = this.RequestExecutor };
        }

        /// <summary>
        /// Allows the use of a mock IRestClient, for testing.
        /// </summary>
        /// <param name="client"></param>
        internal void MockAuthentication(IRestClient client)
        {
            authenticator = new MockAuthentication(client, BaseUrl) { RequestExecutor = this.RequestExecutor };
        }

        private string Send(object body, Method method, string relativeUrl, IDictionary<string, object> requestParameters = null)
        {
            return authenticator.GetResponse(relativeUrl, method, body, requestParameters);
        }

        private async Task<string> SendAsync(object body, Method method, string overrideUrl, IDictionary<string, object> requestParameters, CancellationToken token)
        {
            return await authenticator.GetResponseAsync(overrideUrl, method, body, requestParameters, token);
        }

        private T Send<T>(object body, Method method, string relativeUrl, IDictionary<string, object> requestParameters = null)
            where T : new()
        {
            return authenticator.GetResponse<T>(relativeUrl, method, body, requestParameters);
        }

        private async Task<T> SendAsync<T>(object body, Method method, string overrideUrl, IDictionary<string, object> requestParameters, CancellationToken token)
            where T : new()
        {
            return await authenticator.GetResponseAsync<T>(overrideUrl, method, body, requestParameters, token);
        }

        private Uri GetRedirectLocation(string overrideUrl, IDictionary<string, object> requestParameters)
        {
            return authenticator.GetRedirectLocation(overrideUrl, requestParameters);
        }

        private async Task<Uri> GetRedirectLocationAsync(string overrideUrl, IDictionary<string, object> requestParameters, CancellationToken token)
        {
            return await authenticator.GetRedirectLocationAsync(overrideUrl, requestParameters, token);
        }

        string ISharpBucketRequester.Get(string relativeUrl, object requestParameters)
        {
            //Convert to dictionary to avoid refactoring the Send method.
            var parameterDictionary = requestParameters.ToDictionary();
            return Send(null, Method.GET, relativeUrl, parameterDictionary);
        }

        async Task<string> ISharpBucketRequester.GetAsync(string overrideUrl, object requestParameters, CancellationToken token)
        {
            //Convert to dictionary to avoid refactoring the Send method.
            var parameterDictionary = requestParameters.ToDictionary();
            return await SendAsync(null, Method.GET, overrideUrl, parameterDictionary, token);
        }

        T ISharpBucketRequester.Get<T>(string relativeUrl, object requestParameters)
        {
            //Convert to dictionary to avoid refactoring the Send method.
            var parameterDictionary = requestParameters.ToDictionary();
            return Send<T>(null, Method.GET, relativeUrl, parameterDictionary);
        }

        async Task<T> ISharpBucketRequester.GetAsync<T>(string overrideUrl, object requestParameters, CancellationToken token)
        {
            //Convert to dictionary to avoid refactoring the Send method.
            var parameterDictionary = requestParameters.ToDictionary();
            return await SendAsync<T>(null, Method.GET, overrideUrl, parameterDictionary, token);
        }

        Uri ISharpBucketRequester.GetRedirectLocation(string overrideUrl, object requestParameters)
        {
            //Convert to dictionary to avoid refactoring the Send method.
            var parameterDictionary = requestParameters.ToDictionary();
            return GetRedirectLocation(overrideUrl, parameterDictionary);
        }

        async Task<Uri> ISharpBucketRequester.GetRedirectLocationAsync(string overrideUrl, object requestParameters, CancellationToken token)
        {
            //Convert to dictionary to avoid refactoring the Send method.
            var parameterDictionary = requestParameters.ToDictionary();
            return await GetRedirectLocationAsync(overrideUrl, parameterDictionary, token);
        }

        T ISharpBucketRequester.Post<T>(T body, string relativeUrl)
        {
            return Send<T>(body, Method.POST, relativeUrl);
        }

        async Task<T> ISharpBucketRequester.PostAsync<T>(T body, string overrideUrl, CancellationToken token)
        {
            return await SendAsync<T>(body, Method.POST, overrideUrl, null, token);
        }

        T ISharpBucketRequester.Put<T>(T body, string relativeUrl)
        {
            return Send<T>(body, Method.PUT, relativeUrl);
        }

        async Task<T> ISharpBucketRequester.PutAsync<T>(T body, string overrideUrl, CancellationToken token)
        {
            return await SendAsync<T>(body, Method.PUT, overrideUrl, null, token);
        }

        void ISharpBucketRequester.Delete(string relativeUrl)
        {
            Send(null, Method.DELETE, relativeUrl);
        }

        async Task ISharpBucketRequester.DeleteAsync(string overrideUrl, CancellationToken token)
        {
            await SendAsync(null, Method.DELETE, overrideUrl, null, token);
        }
    }
}