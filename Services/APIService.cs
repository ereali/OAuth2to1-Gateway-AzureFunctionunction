//#load "SessionTokenRequest.csx"
//#load "SessionTokenResponse.csx"
//#load "DoorOverrideRequest.csx"

//comment test

using System.Net;
using Newtonsoft.Json;
using RestSharp;
using RestSharp.Authenticators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using DoorOverrideAPI.Models;
using DoorOverrideAPI.Helpers;

namespace DoorOverrideAPI.Services
{
    public class APIService
    {

        public string tempToken { get; set; }
        public string tempTokenSecret { get; set; }
        public string accessToken { get; set; }
        public string accessTokenSecret { get; set; }

        public APIService()
        {

        }

        public async Task<bool> PostTempToken(string clientHostName, string consumerKey, string consumerSecret)
        {
            return await Task.Run(async () =>
            {
                string uri = "https://" + clientHostName + UrlConstants.temporaryToken;

                var clientOptions = new RestClientOptions(uri)
                {
                    ThrowOnAnyError = true,
                    Timeout = 3000
                };

                var client = new RestClient(clientOptions);

                var request = new RestRequest(Method.Post.ToString());

                request.AddParameter("text/plain", "", ParameterType.RequestBody);

                var signature = OAuth1Authenticator.ForRequestToken(consumerKey, consumerSecret, RestSharp.Authenticators.OAuth.OAuthSignatureMethod.HmacSha1);

                await signature.Authenticate(client, request);

                var tokenResponse = await client.ExecuteAsync(request);

                if (tokenResponse.IsSuccessful)
                {
                    //Make exception clause if not responding
                    tempToken = tokenResponse.Content.Split("&")[0].Split("=")[1];
                    tempTokenSecret = tokenResponse.Content.Split("&")[1].Split("=")[1];

                    return true;
                }
                else
                {
                    //error handler in-case real token request fails
                    return false;
                }

            });
        }

        public async Task<bool> PostAccessToken(string clientHostName, string consumerKey, string consumerSecret, string tempTokenIn, string tempTokenSecretIn)
        {
            return await Task.Run(async () =>
            {
                string uri = "https://" + clientHostName + UrlConstants.realToken;

                var clientOptions = new RestClientOptions(uri)
                {
                    ThrowOnAnyError = true,
                    Timeout = 3000
                };

                var client = new RestClient(clientOptions);

                var request = new RestRequest(Method.Post.ToString());

                request.AddParameter("text/plain", "", ParameterType.RequestBody);

                var signature = OAuth1Authenticator.ForAccessToken(consumerKey, consumerSecret, tempTokenIn, tempTokenSecretIn, RestSharp.Authenticators.OAuth.OAuthSignatureMethod.HmacSha1);

                await signature.Authenticate(client, request);

                var tokenResponse = await client.ExecuteAsync(request);

                if (tokenResponse.IsSuccessful)
                {
                    //Make exception clause if not responding
                    accessToken = tokenResponse.Content.Split("&")[0].Split("=")[1];
                    accessTokenSecret = tokenResponse.Content.Split("&")[1].Split("=")[1];

                    return true;
                }
                else
                {
                    //error handler in-case real token request fails
                    return false;
                }
            });
        }

        public async Task<SessionTokenResponse> PostSessionToken(string clientHostName, string consumerKey, string consumerSecret, string requestID, string transactUsername, string transactPassword, string accessTokenIn, string accessTokenSecretIn)
        {
            return await Task.Run(async () =>
            {
                var uri = "https://" + clientHostName + UrlConstants.validateUser;

                var clientOptions = new RestClientOptions(uri)
                {
                    ThrowOnAnyError = true,
                    Timeout = 8000
                };

                var client = new RestClient(clientOptions);

                var request = new RestRequest(Method.Post.ToString());

                var signature = OAuth1Authenticator.ForAccessToken(consumerKey, consumerSecret, accessTokenIn, accessTokenSecretIn, RestSharp.Authenticators.OAuth.OAuthSignatureMethod.HmacSha1);

                var sessionTokenRequest = new SessionTokenRequest
                {
                    Id = requestID,
                    Username = transactUsername,
                    Password = transactPassword,
                    WorkstationId = 0,
                    IsSuperUser = false
                };

                string requestBody = JsonConvert.SerializeObject(sessionTokenRequest);

                request.AddHeader("Accept", "application/json");
                request.AddHeader("Content-Type", "application/json");
                request.AddHeader("Prefer", "status=200");
                request.AddHeader("BbTS", "ApplicationName=SC-APIGateway,OsHostName=https://soonerapigateway.azurewebsites.net/api/HttpTrigger1?code=10NsKCf4BYvdXDLr0VUZKnOYcboPva5Sy0Jy8w/SjuWXkESyOosWtg==,IpAddress=13.65.92.72,OsUser=FunctionRunner");

                request.AddParameter("application/json", requestBody, ParameterType.RequestBody);

                await signature.Authenticate(client, request);

                var sessionResponse = await client.ExecuteAsync(request);

                try
                {
                    var sessionToken = await Task.Run(() => JsonConvert.DeserializeObject<SessionTokenResponse>(sessionResponse.Content));

                    if (sessionResponse.IsSuccessful)
                    {
                        return sessionToken;
                    }
                    else
                    {
                        var sessionTokenError = new SessionTokenResponse();
                        return sessionTokenError;
                    }
                }
                catch (Exception e)
                {
                    //error handler in-case real token request fails
                    var sessionTokenError = new SessionTokenResponse();
                    return sessionTokenError;
                }

            });
        }

        //Handles door override post api call
        public async Task<String> PostDoorOverride(string clientHostName, string consumerKey, string consumerSecret, string requestID, string doorID, int doorState, int doorScheduledState, string sessionToken, string accessTokenIn, string accessTokenSecretIn)
        {
            return await Task.Run(async () =>
            {
                string uri = "https://" + clientHostName + UrlConstants.doorOverride;

                var clientOptions = new RestClientOptions()
                {
                    ThrowOnAnyError = true,
                    Timeout = 5000
                };

                var client = new RestClient(clientOptions);

                var request = new RestRequest(Method.Post.ToString());

                var signature = OAuth1Authenticator.ForAccessToken(consumerKey, consumerSecret, accessToken, accessTokenSecret, RestSharp.Authenticators.OAuth.OAuthSignatureMethod.HmacSha1);

                var doorList = doorID.Split(',')
                     .Where(m => int.TryParse(m, out _))
                     .Select(m => int.Parse(m))
                     .ToList();

                var doorOverrideRequest = new DoorOverrideRequest
                {
                    RequestId = requestID,
                    DoorIds = doorList,
                    StateOverrideValue = doorState,
                    DurationOverrideValue = doorScheduledState
                };

                string requestBody = JsonConvert.SerializeObject(doorOverrideRequest);

                request.AddHeader("Accept", "application/json");
                request.AddHeader("Prefer", "status=200");
                request.AddHeader("SessionToken", sessionToken);

                request.AddParameter("application/json", requestBody, ParameterType.RequestBody);

                await signature.Authenticate(client, request);

                var overrideResponse = await client.ExecuteAsync(request);

                try
                {
                    //var doorOverrideResponse = await Task.Run(() => JsonConvert.DeserializeObject<DoorOverrideResponse>(overrideResponse.Content));

                    if (overrideResponse.IsSuccessful)
                    {
                        return overrideResponse.Content;
                    }
                    else
                    {
                        return $"Response: {overrideResponse.Content}, Status Code: {overrideResponse.StatusDescription}, Request Body: {requestBody}";
                        //return "failure";
                    }
                }
                catch (Exception e)
                {
                    //error handler in-case real token request fails

                    return "failure";
                }
            });
        }
    }
}
