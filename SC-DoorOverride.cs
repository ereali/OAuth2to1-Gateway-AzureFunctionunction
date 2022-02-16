using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using DoorOverrideAPI.Services;

namespace DoorOverrideAPI
{
    public static class DoorOverrideAPI
    {
        [FunctionName("DoorOverrideAPI")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("Azure HTTP Function running in C# beginning door override request");

            string doorID = req.Headers["doorID"];
            string doorStateInit = req.Headers["doorState"];
            string doorScheduledStateInit = req.Headers["doorScheduledState"];  // "1" or "2"
            string requestID = req.Headers["requestID"]; //guid string

            int doorState = Convert.ToInt32(doorStateInit);
            int doorScheduledState = Convert.ToInt32(doorScheduledStateInit);

            log.LogInformation($"Door ID(s): {doorID}");
            log.LogInformation($"Door State: {doorState.ToString()}");
            log.LogInformation($"Door Scheduled Override Duration: {doorScheduledState.ToString()}");
            log.LogInformation($"Request ID: {requestID}");

            string clientHostName = GetEnvironmentVariable("HOST_NAME");
            log.LogInformation(clientHostName);

            string consumerKey = GetEnvironmentVariable("CONSUMER_KEY");
            string consumerSecret = GetEnvironmentVariable("CONSUMER_SECRET");
            string transactUsername = GetEnvironmentVariable("TRANSACT_USERNAME");
            string transactPassword = GetEnvironmentVariable("TRANSACT_PASSWORD");

            string doorOverride = "";

            if (doorID.ToString() == null || doorState.ToString() == null || doorScheduledState.ToString() == null || requestID == null)
            {
                log.LogInformation("Door ID, door state, door scheduled state, or request ID is null");
                //return new BadRequestResult();
            }
            else
            {
                log.LogInformation("Door ID, door state, door scheduled state, and request ID is valid");

                var apiService = new APIService();

                var tempTokenSuccess = await apiService.PostTempToken(clientHostName, consumerKey, consumerSecret);

                log.LogInformation($"Temp Token: {apiService.tempToken}");
                log.LogInformation($"Temp Token Result: {tempTokenSuccess.ToString()}");
                //log.LogInformation($"Temp Token Secret: {apiService.tempTokenSecret}");

                var tempToken = apiService.tempToken;
                var tempTokenSecret = apiService.tempTokenSecret;

                var accessTokenSuccess = await apiService.PostAccessToken(clientHostName, consumerKey, consumerSecret, tempToken, tempTokenSecret);

                log.LogInformation($"Access Token: {apiService.accessToken}");
                log.LogInformation($"Access Token Result: {accessTokenSuccess.ToString()}");

                //log.LogInformation(transactUsername);
                //log.LogInformation(transactPassword);

                var sessionToken = await apiService.PostSessionToken(clientHostName, consumerKey, consumerSecret, requestID, transactUsername, transactPassword, apiService.accessToken, apiService.accessTokenSecret);

                log.LogInformation($"Result: {sessionToken.Message}");
                log.LogInformation($"Session Token: {sessionToken.SessionToken}");

                doorOverride = await apiService.PostDoorOverride(clientHostName, consumerKey, consumerSecret, requestID, doorID, doorState, doorScheduledState, sessionToken.SessionToken, apiService.accessToken, apiService.accessTokenSecret);

            }

            string responseMessage = string.IsNullOrEmpty(doorOverride)
            ? "This HTTP triggered function executed successfully. Pass a name in the query string or in the request body for a personalized response."
            : $"{doorOverride}";

            log.LogInformation($"Response: {doorOverride}");
            return new OkObjectResult(responseMessage);
        }

        public static string GetEnvironmentVariable(string name)
        {
            return Environment.GetEnvironmentVariable(name, EnvironmentVariableTarget.Process);
        }
    }
}
