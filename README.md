# OAuth2 to OAuth1 Gateway Azure Function

## Overview

This project implements a robust C# .NET Azure Function that acts as an API gateway to convert OAuth 2.0 tokens into OAuth 1.0a tokens, facilitating secure on-premise authentication.

### Table of Contents

- [Overview](#overview)
- [Features](#features)
- [Technologies Used](#technologies-used)
- [Setup](#setup)
  - [Prerequisites](#prerequisites)
  - [Configuration](#configuration)
  - [Build and Run](#build-and-run)
- [Usage](#usage)
- [Code Overview](#code-overview)
  - [Models](#models)
  - [Services](#services)
  - [Azure Functions](#azure-functions)
  - [Configuration Files](#configuration-files)
- [Related Projects](#related-projects)
- [Contributing](#contributing)
- [Authors and Acknowledgments](#authors-and-acknowledgments)
- [License](#license)

### Features

- Converts OAuth 2.0 tokens to OAuth 1.0a tokens.
- Handles token validation and response formatting.
- Integrates with Azure Functions for seamless deployment and scaling.

### Technologies Used

- **C# .NET Core 3.1**
- **Azure Functions**
- **RestSharp** for HTTP requests

### Setup

#### Prerequisites

- .NET SDK (version 3.1 or later)
- Azure Functions Core Tools
- Azure Subscription

#### Configuration

1. Clone the repository:
    ```bash
    git clone https://github.com/ereali/OAuth2to1-Gateway-AzureFunction.git
    cd OAuth2to1-Gateway-AzureFunction
    ```

2. Update `appsettings.json` with your OAuth API credentials:
    ```json
    {
      "APIConnect": {
        "OAuthSecretKey": "**************************************",
        "OAuthConsumerKey": "**************************************",
        "InstitutionRouteId": "**************************************",
        "MerchantAuthorization": "**************************************"
      }
    }
    ```

3. Update `APIGlobalVars` with your application server hostname:
    ```csharp
    public class APIGlobalVars
    {
        public readonly static string HostName = "hostname";
        public static string IncludeImageYes = "Include";
        public static string IncludeImageNo = "Exclude";
    }
    ```

#### Build and Run

1. Install dependencies and build the project:
    ```bash
    dotnet build
    ```

2. Start the Azure Function locally:
    ```bash
    func start
    ```

### Usage

1. Send an OAuth 2.0 token request to the deployed function endpoint.
2. The function processes the request, converts the token, and responds with an OAuth 1.0a token.
3. Example request body (JSON):
    ```json
    {
      "clientId": "your-client-id",
      "clientSecret": "your-client-secret",
      "grantType": "authorization_code",
      "code": "authorization-code",
      "redirectUri": "your-redirect-uri"
    }
    ```

### Code Overview

#### Models

- **TokenRequest.cs**: Defines the structure for incoming token requests.
    ```csharp
    public class TokenRequest
    {
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string GrantType { get; set; }
        public string Code { get; set; }
        public string RedirectUri { get; set; }
    }
    ```

- **TokenResponse.cs**: Defines the structure for token response.
    ```csharp
    public class TokenResponse
    {
        public string OAuthToken { get; set; }
        public string OAuthTokenSecret { get; set; }
        public string UserId { get; set; }
    }
    ```

#### Services

- **OAuthService.cs**: Handles token validation and conversion logic.
    ```csharp
    public class OAuthService
    {
        public TokenResponse ConvertToOAuth1(TokenRequest request)
        {
            // Conversion logic here
        }
    }
    ```

#### Azure Functions

- **SC-DoorOverride.cs**: Main Azure Function handling HTTP requests and converting tokens using `OAuthService`.
    ```csharp
    public static class SCDoorOverride
    {
        [FunctionName("ConvertToken")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            // Function logic here
        }
    }
    ```

#### Configuration Files

- **host.json**: Configuration for Azure Functions runtime.
    ```json
    {
      "version": "2.0",
      "logging": {
        "applicationInsights": {
          "samplingSettings": {
            "isEnabled": true
          }
        }
      }
    }
    ```

- **local.settings.json**: Local settings for Azure Functions, including environment variables.
    ```json
    {
      "IsEncrypted": false,
      "Values": {
        "AzureWebJobsStorage": "UseDevelopmentStorage=true",
        "FUNCTIONS_WORKER_RUNTIME": "dotnet"
      }
    }
    ```

### Related Projects

- [AWS IoT 1Click Lambda Door Control](https://github.com/ereali/AWS-IoT1Click-Lambda-DoorControl)

### Contributing

**Note**: This project is no longer supported. However, contributions are still welcome. For major changes, please open an issue first to discuss what you would like to change.

### Authors and Acknowledgments

- **Primary Developer**: Edward Reali

### License

This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for details.
