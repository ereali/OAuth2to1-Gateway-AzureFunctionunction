# OAuth 2.0 to 1.0a Gateway Azure Function to facilitate on-premise authentication

## Overview

This project implements a robust C# .NET Azure Function that acts as an API gateway to convert OAuth 2.0 tokens into OAuth 1.0a tokens, facilitating secure on-premise authentication. 
This is primarily for use with the Transact System Enterprise (TSE) suite of APIs but can be adapted to fit several use-cases.

## Key Components

### 1. Models

- **TokenRequest.cs**: 
  - Structure for incoming token requests.
  - Properties include `clientId`, `clientSecret`, `grantType`, `code`, and `redirectUri`.

- **TokenResponse.cs**:
  - Structure for outgoing token responses.
  - Properties include `oauthToken`, `oauthTokenSecret`, and `userId`.

### 2. Services

- **OAuthService.cs**:
  - Core logic for token conversion.
  - Methods:
    - `ValidateToken(TokenRequest request)`: Validates the incoming OAuth 2.0 token request.
    - `ConvertToOAuth1(TokenRequest request)`: Converts the validated OAuth 2.0 token to OAuth 1.0a token.
    - `GenerateTokenResponse(TokenRequest request)`: Generates the token response for the client.

### 3. Azure Functions

- **SC-DoorOverride.cs**:
  - Main entry point for the Azure Function.
  - Handles HTTP requests:
    - Validates the request payload.
    - Calls `OAuthService` to convert tokens.
    - Sends back the converted token as an HTTP response.
  - Endpoint: `/api/convert-token`

### 4. Configuration

- **host.json**: Configuration for Azure Functions runtime.
  - Example settings:
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
  - Example:
    ```json
    {
      "IsEncrypted": false,
      "Values": {
        "AzureWebJobsStorage": "UseDevelopmentStorage=true",
        "FUNCTIONS_WORKER_RUNTIME": "dotnet"
      }
    }
    ```

- **azure-pipelines.yml**: CI/CD pipeline configuration.
  - Stages include build, test, and deploy.

## Getting Started

### Prerequisites

- .NET SDK (version 3.1 or later)
- Azure Functions Core Tools
- Azure Subscription

### Setup

1. Clone the repository:
   ```bash
   git clone https://github.com/ereali/OAuth-2.0---1.0a-gateway-Azure-Function.git
   ```
2. Navigate to the project directory:
   ```bash
   cd OAuth-2.0---1.0a-gateway-Azure-Function
   ```
3. Install dependencies and build the project:
   ```bash
   dotnet build
   ```

### Running Locally

1. Start the Azure Function locally:
   ```bash
   func start
   ```
2. Test the function using tools like Postman:
   - URL: `http://localhost:7071/api/convert-token`
   - Method: POST
   - Body (JSON):
     ```json
     {
       "clientId": "your-client-id",
       "clientSecret": "your-client-secret",
       "grantType": "authorization_code",
       "code": "authorization-code",
       "redirectUri": "your-redirect-uri"
     }
     ```

### Deployment

1. Deploy the function to Azure:
   ```bash
   func azure functionapp publish <FunctionAppName>
   ```

## Usage

- Send an OAuth 2.0 token request to the deployed function endpoint.
- The function processes the request, converts the token, and responds with an OAuth 1.0a token.
- Example response:
  ```json
  {
    "oauthToken": "oauth-token-value",
    "oauthTokenSecret": "oauth-token-secret",
    "userId": "user-id"
  }
  ```

## License

This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for details.
© 2024 Edward Reali
