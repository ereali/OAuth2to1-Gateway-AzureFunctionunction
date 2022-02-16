//Edward Reali - Sooner Card | University of Oklahoma
//Copyright Edward Reali (c) 2022 All Rights Reserved
//Proprietary and confidential

//Methods that handle specfic API calls against TSE

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

        //initialize variables
        public string tempToken { get; set; }
        public string tempTokenSecret { get; set; }
        public string accessToken { get; set; }
        public string accessTokenSecret { get; set; }

        //Init constructor
        public APIService()
        {

        }

        //Get temp token
        public async Task<bool> PostTempToken(string clientHostName, string consumerKey, string consumerSecret)
        {
            //Begins async task to get temp token
            return await Task.Run(async () =>
            {
                //Init uri
                string uri = "https://" + clientHostName + UrlConstants.temporaryToken;

                Console.WriteLine("URI: " + uri);

                //Init restsharp client options
                var clientOptions = new RestClientOptions(uri)
                {
                    ThrowOnAnyError = true,
                    Timeout = 3000
                };

                //Init restsharp client
                var client = new RestClient(clientOptions);

                //Init restsharp request with headers/parameters
                var request = new RestRequest()
                    .AddParameter("text/plain", "", ParameterType.RequestBody);

                // Console.WriteLine($"Consumer Key: {consumerKey}");
                // Console.WriteLine($"Consumer Secret: {consumerSecret}");

                //Perform OAuth1 authentication
                client.Authenticator = OAuth1Authenticator.ForRequestToken(consumerKey, consumerSecret);
                
                //Execute authentication and generate OAuth1 token
                await client.Authenticator.Authenticate(client, request);

                //Get API response
                var tokenResponse = await client.PostAsync(request, cancellationToken: default);
                
                //Output response to console
                Console.WriteLine($"Token Response: {tokenResponse.Content}");

                //Handle response and return true if successful
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
                //Init uri
                string uri = "https://" + clientHostName + UrlConstants.realToken;

                //Init restsharp client options
                var clientOptions = new RestClientOptions(uri)
                {
                    ThrowOnAnyError = true,
                    Timeout = 3000
                };

                //Init restsharp client
                var client = new RestClient(clientOptions);

                //Init restsharp request with headers/parameters
                var request = new RestRequest()
                    .AddParameter("text/plain", "", ParameterType.RequestBody);
                
                //Perform OAuth1 authentication
                client.Authenticator = OAuth1Authenticator.ForAccessToken(consumerKey, consumerSecret, tempTokenIn, tempTokenSecretIn, RestSharp.Authenticators.OAuth.OAuthSignatureMethod.HmacSha1);

                //Execute authentication and generate OAuth1 token
                await client.Authenticator.Authenticate(client, request);

                //Get API response
                var tokenResponse = await client.PostAsync(request, cancellationToken: default);
                
                //Output response to console
                Console.WriteLine($"Token Response: {tokenResponse.Content}");

                //Handle response and return true if successful
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
                //Init uri
                string uri = "https://" + clientHostName + UrlConstants.validateUser;

                Console.WriteLine("URI: " + uri);

                //Init restsharp client options
                var clientOptions = new RestClientOptions(uri)
                {
                    ThrowOnAnyError = true,
                    Timeout = 8000
                };

                //Init restsharp client
                var client = new RestClient(clientOptions);
                
                //Prepare request body for serialization
                var sessionTokenRequest = new SessionTokenRequest
                {
                    Id = requestID,
                    Username = transactUsername,
                    Password = transactPassword,
                    WorkstationId = 0,
                    IsSuperUser = false
                };

                //Console.WriteLine($"{requestID} {transactUsername} {transactPassword}");

                //Serialize request body
                string requestBody = JsonConvert.SerializeObject(sessionTokenRequest);

                Console.WriteLine($"Request Body: {requestBody}");

                //Init restsharp request with headers/parameters
                var request = new RestRequest()
                    .AddHeader("Accept", "application/json")
                    .AddHeader("Content-Type", "application/json")
                    .AddHeader("Prefer", "status=200")
                    .AddHeader("BbTS", "ApplicationName=SC-APIGateway,OsHostName=https://soonerapigateway.azurewebsites.net/api/HttpTrigger1?code=10NsKCf4BYvdXDLr0VUZKnOYcboPva5Sy0Jy8w/SjuWXkESyOosWtg==,IpAddress=13.65.92.72,OsUser=FunctionRunner")
                    //.AddParameter("application/json", requestBody, ParameterType.RequestBody);
                    .AddStringBody(requestBody, "application/json");
                
                //Perform OAuth1 authentication
                //client.Authenticator = OAuth1Authenticator.ForAccessToken(consumerKey, consumerSecret, accessTokenIn, accessTokenSecretIn, RestSharp.Authenticators.OAuth.OAuthSignatureMethod.HmacSha1);

                client.Authenticator = OAuth1Authenticator.ForAccessToken(consumerKey, consumerSecret, accessTokenIn, accessTokenSecretIn, RestSharp.Authenticators.OAuth.OAuthSignatureMethod.HmacSha1);
                //Execute authentication and generate OAuth1 token
                await client.Authenticator.Authenticate(client, request);

                //Try to get API response
                try
                {
                    //Get API response

                    var sessionResponse = await client.PostAsync(request, cancellationToken: default);
                    
                    //Output response to console
                    Console.WriteLine($"Session Response: {sessionResponse.Content}");

                    //Handle response and return a valid session token if successful
                    if (sessionResponse.IsSuccessful)
                    {
                        var sessionToken = await Task.Run(() => JsonConvert.DeserializeObject<SessionTokenResponse>(sessionResponse.Content));
                        return sessionToken;
                    }
                    //Handle response and return a session token error if unsuccessful
                    else
                    {
                        var sessionTokenError = new SessionTokenResponse();
                        return sessionTokenError;
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
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
                //Init uri
                string uri = "https://" + clientHostName + UrlConstants.doorOverride;

                Console.WriteLine("URI: " + uri);

                //Init restsharp client options
                var clientOptions = new RestClientOptions(uri)
                {
                    ThrowOnAnyError = true,
                    Timeout = 5000
                };

                //Init restsharp client
                var client = new RestClient(clientOptions);

                var doorList = doorID.Split(',')
                     .Where(m => int.TryParse(m, out _))
                     .Select(m => int.Parse(m))
                     .ToList();

                //Prepare request body for serialization
                var doorOverrideRequest = new DoorOverrideRequest
                {
                    RequestId = requestID,
                    DoorIds = doorList,
                    StateOverrideValue = doorState,
                    DurationOverrideValue = doorScheduledState
                };

                //Prepare request with headers/parameters
                var request = new RestRequest()
                    .AddHeader("Accept", "application/json")
                    .AddHeader("Prefer", "status=200")
                    .AddHeader("Content-Type", "application/json")
                    .AddHeader("SessionToken", sessionToken)
                    .AddJsonBody(doorOverrideRequest);

                client.Authenticator = OAuth1Authenticator.ForAccessToken(consumerKey, consumerSecret, accessToken, accessTokenSecret, RestSharp.Authenticators.OAuth.OAuthSignatureMethod.HmacSha1);

                //string requestBody = JsonConvert.SerializeObject(doorOverrideRequest);

                // request.AddHeader("Accept", "application/json");
                // request.AddHeader("Prefer", "status=200");
                // request.AddHeader("SessionToken", sessionToken);

                //request.AddParameter("application/json", requestBody, ParameterType.RequestBody);

                await client.Authenticator.Authenticate(client, request);

                //Try to get API response
                try
                {
                    //Get API response
                    var overrideResponse = await client.PostAsync(request, cancellationToken: default);

                    //Output response to console
                    Console.WriteLine($"Override Response: {overrideResponse.Content}");

                    if (overrideResponse.IsSuccessful)
                    {
                        return overrideResponse.Content;
                    }
                    else
                    {
                        return $"Failure: {overrideResponse.Content}, Status Code: {overrideResponse.StatusDescription}, Request Body: {overrideResponse.Content}";
                        //return "failure";
                    }
                }
                catch (Exception e)
                {
                    //error handler in-case real token request fails
                    Console.WriteLine($"Exception: {e.Message}");
                    return "Error executing door override request: " + e.Message;
                }
            });
        }
    }
}
