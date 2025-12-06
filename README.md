# SharePoint Online File Operations using Microsoft Graph API

This .NET 10 project demonstrates how to perform file operations in SharePoint Online using the Microsoft Graph API with OAuth 2.0 authentication and application credentials. The application facilitates seamless file interactions, leveraging modern authentication techniques and the capabilities of the Graph API.

## Table of Contents

- [What is Microsoft Graph API?](#what-is-microsoft-graph-api)
- [Microsoft Graph API for SharePoint - Deep Dive](#microsoft-graph-api-for-sharepoint---deep-dive)
- [Features](#features)
- [Prerequisites](#prerequisites)
- [Getting Started](#getting-started)
- [Usage](#usage)
- [Architecture and Code Flow](#architecture-and-code-flow)
- [Understanding the Code](#understanding-the-code)
- [API Reference](#api-reference)
- [Testing with Bruno API Client](#testing-with-bruno-api-client)
- [Troubleshooting](#troubleshooting)
- [Contributing](#contributing)
- [License](#license)

## What is Microsoft Graph API?

Microsoft Graph API is a unified REST API endpoint that provides access to Microsoft 365 data and intelligence. It serves as a single entry point (`https://graph.microsoft.com`) to access data across various Microsoft services including SharePoint, OneDrive, Outlook, Teams, and more.

### Key Advantages

| Feature | Description |
|---------|-------------|
| **Unified Access** | A single API to access data across multiple Microsoft services |
| **Consistent Data Model** | Standardized entities and relationships across all services |
| **Rich SDK Support** | Official SDKs for .NET, JavaScript, Java, Python, and more |
| **Modern Authentication** | OAuth 2.0 and OpenID Connect support with Azure AD |
| **Intelligence & Insights** | AI-powered features and analytics capabilities |

### Graph API Endpoint Structure

```
https://graph.microsoft.com/{version}/{resource}?{query-parameters}
```

- **Version**: `v1.0` (stable) or `beta` (preview features)
- **Resource**: The Microsoft 365 resource you're accessing (e.g., `sites`, `drives`, `users`)
- **Query Parameters**: OData query options like `$select`, `$filter`, `$expand`

## Microsoft Graph API for SharePoint - Deep Dive

This section provides a comprehensive understanding of how Microsoft Graph API interacts with SharePoint Online.

### SharePoint Resource Hierarchy

Understanding SharePoint's hierarchy is crucial for working with the Graph API:

```
┌─────────────────────────────────────────────────────────────────┐
│                    SharePoint Online Tenant                     │
│  (contoso.sharepoint.com)                                       │
├─────────────────────────────────────────────────────────────────┤
│                                                                 │
│  ┌─────────────────────────────────────────────────────────┐   │
│  │                    Sites (Site Collections)             │   │
│  │  /sites/project-alpha, /sites/hr-portal, root site      │   │
│  │                                                         │   │
│  │  ┌─────────────────────────────────────────────────┐   │   │
│  │  │               Drives (Document Libraries)       │   │   │
│  │  │  "Documents", "Shared Documents", "Reports"     │   │   │
│  │  │                                                 │   │   │
│  │  │  ┌─────────────────────────────────────────┐   │   │   │
│  │  │  │          Items (Files & Folders)        │   │   │   │
│  │  │  │  - Files (.docx, .pdf, .xlsx, etc.)     │   │   │   │
│  │  │  │  - Folders (with child items)           │   │   │   │
│  │  │  │  - Metadata (custom properties)         │   │   │   │
│  │  │  └─────────────────────────────────────────┘   │   │   │
│  │  │                                                 │   │   │
│  │  └─────────────────────────────────────────────────┘   │   │
│  │                                                         │   │
│  └─────────────────────────────────────────────────────────┘   │
│                                                                 │
└─────────────────────────────────────────────────────────────────┘
```

### Key SharePoint Concepts in Graph API

#### 1. Sites
A **Site** represents a SharePoint site or site collection. You can access:
- **Root site**: The main SharePoint site (`contoso.sharepoint.com`)
- **Named sites**: Specific sites by path (`/sites/project-alpha`)

**Graph API Pattern:**
```
GET https://graph.microsoft.com/v1.0/sites/{hostname}:{site-path}
GET https://graph.microsoft.com/v1.0/sites/{site-id}
```

#### 2. Drives
A **Drive** represents a document library within a SharePoint site. Each site can have multiple drives (document libraries).

**Graph API Pattern:**
```
GET https://graph.microsoft.com/v1.0/sites/{site-id}/drives
GET https://graph.microsoft.com/v1.0/drives/{drive-id}
```

#### 3. Drive Items
**DriveItems** represent files and folders within a drive. Key operations include:
- List children of a folder
- Upload/download files
- Create folders
- Update metadata
- Delete items

**Graph API Pattern:**
```
GET https://graph.microsoft.com/v1.0/drives/{drive-id}/root:/path/to/item
GET https://graph.microsoft.com/v1.0/drives/{drive-id}/items/{item-id}
```

### Authentication Flow

This application uses the **Client Credentials OAuth 2.0 flow** for server-to-server authentication:

```
┌─────────────┐                                    ┌─────────────────┐
│   Your App  │                                    │    Azure AD     │
│  (Backend)  │                                    │   (Identity)    │
└──────┬──────┘                                    └────────┬────────┘
       │                                                    │
       │  1. Request token with client_id + client_secret   │
       │ ──────────────────────────────────────────────────►│
       │                                                    │
       │  2. Return access_token (JWT)                      │
       │ ◄──────────────────────────────────────────────────│
       │                                                    │
       │                                    ┌───────────────┴───────────────┐
       │                                    │       Microsoft Graph         │
       │                                    └───────────────┬───────────────┘
       │                                                    │
       │  3. API request with Bearer token                  │
       │ ──────────────────────────────────────────────────►│
       │                                                    │
       │  4. Return SharePoint data                         │
       │ ◄──────────────────────────────────────────────────│
       │                                                    │
```

### Graph API URL Patterns Used in This Project

| Operation | Graph API Endpoint | HTTP Method |
|-----------|-------------------|-------------|
| Get Site | `/sites/{hostname}:/sites/{siteName}` | GET |
| Get Drives | `/sites/{site-id}/drives` | GET |
| List Files | `/drives/{drive-id}/items/root:/{path}:/children` | GET |
| Upload File | `/drives/{drive-id}/items/root:/{path}/{filename}:/content` | PUT |
| Read File | `/drives/{drive-id}/items/root:/{path}/{filename}` | GET |
| Delete File | `/drives/{drive-id}/root:/{path}/{filename}` | DELETE |
| Update Metadata | `/drives/{drive-id}/root:/{path}/{filename}` | PATCH |

In this project, we leverage Microsoft Graph API to interact with SharePoint Online document libraries and files.

## Features
- OAuth 2.0 authentication with delegated access
- File operations on SharePoint Online, such as:
  - Uploading files
  - Downloading files
  - Deleting files
  - Listing files
  - Updating file metadata
- Easy-to-use interface for managing files in SharePoint document libraries
- Distributed caching for improved performance

## Prerequisites

Before running the application, ensure you have the following:

### Microsoft 365 and Azure Requirements

- A Microsoft 365 tenant with SharePoint Online enabled
- Azure Active Directory (Entra ID) App Registration with:
  - **Client ID** (Application ID)
  - **Tenant ID** (Directory ID)
  - **Client Secret** (for server-to-server authentication)
- **API Permissions** granted in Azure AD App Registration:
  - `Sites.ReadWrite.All` - Required for accessing and modifying SharePoint sites
  - `Files.ReadWrite.All` - Required for file operations

### Development Environment

| Requirement | Version | Notes |
|-------------|---------|-------|
| .NET SDK | 10.0+ | [Download](https://dotnet.microsoft.com/download/dotnet/10.0) |
| Visual Studio | 2022 or later | Or VS Code with C# extension |
| Bruno API Client | Latest | Optional, for API testing |

### Setting Up Azure AD Application

Follow these steps to register your application in Azure AD (now called Microsoft Entra ID):

#### Step 1: Create App Registration

1. Go to [Azure Portal](https://portal.azure.com/)
2. Navigate to **Microsoft Entra ID** (formerly Azure Active Directory)
3. Select **App registrations** → **New registration**
4. Configure the registration:
   - **Name**: `SharePoint Graph API Client` (or your preferred name)
   - **Supported account types**: Single tenant (recommended for enterprise)
   - **Redirect URI**: Not required for client credentials flow
5. Click **Register**

#### Step 2: Configure API Permissions

1. In your app registration, go to **API permissions**
2. Click **Add a permission** → **Microsoft Graph**
3. Select **Application permissions** (not delegated)
4. Add these permissions:

   | Permission | Type | Description |
   |------------|------|-------------|
   | `Sites.ReadWrite.All` | Application | Read and write items in all site collections |
   | `Files.ReadWrite.All` | Application | Read and write files in all site collections |

5. Click **Grant admin consent for [Your Organization]**

> ⚠️ **Important**: Application permissions require admin consent and provide access to all sites in your tenant. For more granular access, consider using delegated permissions with user context.

#### Step 3: Create Client Secret

1. Go to **Certificates & secrets**
2. Under **Client secrets**, click **New client secret**
3. Add a description and select an expiration period
4. Click **Add**
5. **Copy the secret value immediately** - it won't be shown again!

#### Step 4: Note Your Configuration Values

After setup, you'll need these values for configuration:

```
Client ID:     xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx
Tenant ID:     yyyyyyyy-yyyy-yyyy-yyyy-yyyyyyyyyyyy
Client Secret: your_secret_value_here
```

## Getting Started

### 1. Clone the Repository
```bash
git clone https://github.com/nitin27may/sharepoint-graph-api
cd sharepoint-graph-api
```

### 2. Configure the Application

Update the `Spo.WebApi/appsettings.json` file with your Azure AD App Registration details:

```json
{
  "GraphApiSettings": {
    "TenantId": "<YOUR-TENANT-ID>",
    "ClientId": "<YOUR-CLIENT-ID>",
    "SecretId": "<YOUR-CLIENT-SECRET>",
    "Scope": "https://graph.microsoft.com/.default",
    "BaseGraphUri": "https://graph.microsoft.com/v1.0",
    "BaseSpoSiteUri": "<YOUR-TENANT-NAME>.sharepoint.com"
  }
}
```

**Configuration Parameters Explained:**

| Parameter | Description | Example |
|-----------|-------------|---------|
| `TenantId` | Your Azure AD tenant ID (GUID) | `12345678-1234-1234-1234-123456789012` |
| `ClientId` | Application (client) ID from App Registration | `87654321-4321-4321-4321-210987654321` |
| `SecretId` | Client secret value (not the secret ID) | `abc123...` |
| `Scope` | OAuth scope for Microsoft Graph | `https://graph.microsoft.com/.default` |
| `BaseGraphUri` | Microsoft Graph API base URL | `https://graph.microsoft.com/v1.0` |
| `BaseSpoSiteUri` | Your SharePoint Online root URL | `contoso.sharepoint.com` |

> 💡 **Tip**: Use [User Secrets](https://learn.microsoft.com/en-us/aspnet/core/security/app-secrets) or environment variables in production to keep credentials secure.

### 3. Build and Run the Application

```bash
# Restore NuGet packages
dotnet restore

# Build the solution
dotnet build

# Run the Web API
cd Spo.WebApi
dotnet run
```

The API will be available at `https://localhost:7295` (or the configured port).

### 4. Access Swagger UI

Navigate to `https://localhost:7295/swagger` to explore and test the API endpoints interactively.

## Usage

This section provides detailed instructions on how to use the API endpoints for SharePoint file operations.

### Understanding SharePoint URL Structure

When working with this API, you need to understand how to address SharePoint resources:

```
API URL Pattern:
/GraphApi/{siteName}/{driveName}/{path}/{fileName}

Example:
/GraphApi/project-alpha/Documents/Reports/2024/quarterly-report.pdf
         └─────┬─────┘  └───┬───┘ └─────┬─────┘└────────┬────────┘
            Site Name    Drive    Folder Path      File Name
```

### SharePoint Resource Mapping

| API Parameter | SharePoint Concept | Example Values |
|--------------|-------------------|----------------|
| `siteName` | Site Collection Name | `root`, `project-alpha`, `hr-portal` |
| `driveName` | Document Library Name | `Documents`, `Shared Documents`, `Reports` |
| `path` | Folder Path within Library | `General`, `2024/Reports`, `Archive/Old` |
| `fileName` | File Name with Extension | `report.pdf`, `data.xlsx` |

### Important Notes

> 📌 **Root Site**: Use `root` as the `siteName` to access the main SharePoint site
> 
> 📌 **Default Library**: The default document library in SharePoint is usually `Documents`
> 
> 📌 **Path Format**: Paths are relative to the drive root, use forward slashes `/`

### API Response Format

All file operations return a consistent response format:

```json
{
  "id": "01XXXXXXXXXXXXXX",
  "name": "document.pdf",
  "size": 1024000,
  "webUrl": "https://contoso.sharepoint.com/sites/project-alpha/Documents/document.pdf",
  "createdDateTime": "2024-01-15T10:30:00Z",
  "lastModifiedDateTime": "2024-01-20T14:45:00Z",
  "file": {
    "mimeType": "application/pdf"
  },
  "parentReference": {
    "id": "01YYYYYYYYYYYYYY",
    "path": "/drives/drive-id/root:/FolderName"
  }
}
```

### Available Operations

#### 1. List Files

Retrieves a list of files and folders in a specified path within a document library.

```http
GET /GraphApi/{siteName}/{driveName}/{path}?select={fields}
```

**Parameters:**

| Parameter | Location | Required | Description |
|-----------|----------|----------|-------------|
| `siteName` | Path | Yes | SharePoint site name (`root` for base site) |
| `driveName` | Path | Yes | Document library name |
| `path` | Path | Yes | Folder path within the library |
| `select` | Query | No | Comma-separated list of fields to return |

**Example Request:**
```http
GET /GraphApi/root/Documents/Reports?select=id,name,size,webUrl
```

**Example Response:**
```json
[
  {
    "id": "01ABCDEFG...",
    "name": "report-2024.pdf",
    "size": 2048000,
    "webUrl": "https://contoso.sharepoint.com/Documents/Reports/report-2024.pdf",
    "file": { "mimeType": "application/pdf" }
  },
  {
    "id": "01HIJKLMN...",
    "name": "Archives",
    "webUrl": "https://contoso.sharepoint.com/Documents/Reports/Archives",
    "folder": { "childCount": 5 }
  }
]
```

#### 2. Upload a File

Uploads a file to a specified path within a document library. Uses `multipart/form-data` encoding.

```http
POST /GraphApi/{siteName}/{driveName}/{path}
Content-Type: multipart/form-data
```

**Parameters:**

| Parameter | Location | Required | Description |
|-----------|----------|----------|-------------|
| `siteName` | Path | Yes | SharePoint site name |
| `driveName` | Path | Yes | Document library name |
| `path` | Path | Yes | Target folder path |
| `Name` | Form | Yes | Desired file name (extension auto-appended) |
| `File` | Form | Yes | The file to upload |

**Example cURL:**
```bash
curl -X POST "https://localhost:7295/GraphApi/root/Documents/Reports" \
  -F "Name=quarterly-report" \
  -F "File=@/path/to/report.pdf"
```

**Conflict Behavior:** If a file with the same name exists, a new version with a modified name is created (e.g., `report (1).pdf`).

#### 3. Read/Download a File

Retrieves file metadata. Use the `webUrl` from the response to download the actual file content.

```http
GET /GraphApi/{siteName}/{driveName}/{path}/{fileName}?select={fields}
```

**Parameters:**

| Parameter | Location | Required | Description |
|-----------|----------|----------|-------------|
| `siteName` | Path | Yes | SharePoint site name |
| `driveName` | Path | Yes | Document library name |
| `path` | Path | Yes | Folder path |
| `fileName` | Path | Yes | Name of the file |
| `select` | Query | No | Fields to return |

**Example Request:**
```http
GET /GraphApi/root/Documents/Reports/annual-report.pdf
```

#### 4. Update a File

Replaces an existing file's content. The file must already exist at the specified path.

```http
PUT /GraphApi/{siteName}/{driveName}/{path}
Content-Type: multipart/form-data
```

**Parameters:** Same as Upload operation.

#### 5. Delete a File

Permanently deletes a file from SharePoint.

```http
DELETE /GraphApi/{siteName}/{driveName}/{path}/{fileName}
```

**Parameters:**

| Parameter | Location | Required | Description |
|-----------|----------|----------|-------------|
| `siteName` | Path | Yes | SharePoint site name |
| `driveName` | Path | Yes | Document library name |
| `path` | Path | Yes | Folder path |
| `fileName` | Path | Yes | Name of the file to delete |

**Response:** `204 No Content` on success

#### 6. Update File Metadata

Updates custom metadata properties on a file.

```http
PATCH /GraphApi/{siteName}/{driveName}/{path}/{fileName}
Content-Type: application/json
```

**Request Body:**
```json
{
  "description": "Annual financial report",
  "department": "Finance"
}
```

> ⚠️ **Note**: Only supported metadata fields can be updated. Standard file properties like `name` can be updated, but custom columns require proper configuration in SharePoint.

## Architecture and Code Flow

The application follows a layered architecture pattern with clear separation of concerns. Below is an architectural diagram of the codebase:

```mermaid
flowchart TB
    Client[Client Application]
    SpoWebApi[Spo.WebApi]
    SpoGraphApi[Spo.GraphApi Library]
    GraphApiClient[GraphApiClient]
    GraphApiFactory[GraphApiClientFactory]
    AuthHandler[GraphApiAuthenticationHandler]
    GraphAPI[Microsoft Graph API]
    SharePoint[SharePoint Online]
    
    Client -->|HTTP Requests| SpoWebApi
    SpoWebApi -->|Uses| SpoGraphApi
    SpoGraphApi -->|Creates| GraphApiFactory
    GraphApiFactory -->|Creates| GraphApiClient
    GraphApiClient -->|Authenticated Requests| GraphAPI
    GraphApiClient -->|Uses| AuthHandler
    AuthHandler -->|OAuth 2.0| GraphAPI
    GraphAPI -->|Interacts with| SharePoint
    
    subgraph "Client Layer"
        Client
    end
    
    subgraph "API Layer"
        SpoWebApi
    end
    
    subgraph "Core Library"
        SpoGraphApi
        GraphApiFactory
        GraphApiClient
        AuthHandler
    end
    
    subgraph "External Services"
        GraphAPI
        SharePoint
    end
```

### Request Lifecycle

The following sequence diagram illustrates a typical file operation request:

```mermaid
sequenceDiagram
    participant Client
    participant Controller as GraphApiController
    participant Factory as GraphApiClientFactory
    participant AuthHandler as GraphApiAuthenticationHandler
    participant Cache as Distributed Cache
    participant AzureAD as Azure AD
    participant GraphAPI as Microsoft Graph API
    participant SPO as SharePoint Online

    Client->>Controller: GET /GraphApi/root/Documents/Reports
    Controller->>Factory: Create()
    Factory->>AuthHandler: new GraphApiAuthenticationHandler()
    Factory->>Controller: IGraphApiClient
    
    Controller->>AuthHandler: GetAllFiles() → HTTP Request
    AuthHandler->>Cache: Check for cached token
    
    alt Token not cached
        AuthHandler->>AzureAD: Request token (client credentials)
        AzureAD-->>AuthHandler: Access token
        AuthHandler->>Cache: Store token with expiry
    end
    
    AuthHandler->>GraphAPI: GET /drives/{id}/items/root:/Reports:/children<br/>Authorization: Bearer {token}
    GraphAPI->>SPO: Retrieve files
    SPO-->>GraphAPI: File list
    GraphAPI-->>AuthHandler: JSON response
    AuthHandler-->>Controller: List<FileDetails>
    Controller-->>Client: 200 OK with file list
```

### Component Descriptions

| Component | Responsibility |
|-----------|---------------|
| **GraphApiController** | ASP.NET Core controller exposing REST endpoints, handles HTTP request/response |
| **GraphApiClientFactory** | Creates configured instances of `GraphApiClient` with authentication handler |
| **GraphApiClient** | Core implementation of all SharePoint file operations via Graph API |
| **GraphApiAuthenticationHandler** | HTTP delegating handler that automatically injects Bearer tokens |
| **Distributed Cache** | Caches access tokens and SharePoint site/drive information |

## Understanding the Code

This section explains how the code works to help developers extend or maintain the application.

### Project Structure

```
sharepoint-graph-api/
├── Spo.GraphApi/                      # Core library
│   ├── GraphApiClient.cs              # Main Graph API operations
│   ├── GraphApiClientFactory.cs       # Factory for creating clients
│   ├── IGraphApiClient.cs             # Client interface
│   ├── IGraphApiClientFactory.cs      # Factory interface
│   ├── GraphApiServiceCollectionExtensions.cs  # DI registration
│   ├── Handler/
│   │   └── GraphApiAuthenticationHandler.cs    # OAuth token handling
│   └── Models/
│       ├── GraphApiOptions.cs         # Configuration model
│       ├── FileDetails.cs             # File response model
│       ├── DriveDetails.cs            # Drive/library model
│       ├── SiteDetails.cs             # Site model
│       ├── CustomFile.cs              # Upload request model
│       └── GraphApiException.cs       # Custom exception
│
├── Spo.WebApi/                        # Web API project
│   ├── Controllers/
│   │   └── GraphApiController.cs      # REST API endpoints
│   ├── Program.cs                     # Application entry point
│   ├── appsettings.json               # Configuration
│   └── Dockerfile                     # Container support
│
└── API Collection/                    # Bruno API collection
    ├── Get All Files.bru
    ├── Add a file.bru
    ├── Read File.bru
    ├── Update Files.bru
    ├── Delete a file.bru
    └── environments/
        └── Local.bru
```

### Key Code Components Explained

#### 1. Authentication Handler (`GraphApiAuthenticationHandler.cs`)

The authentication handler is a `DelegatingHandler` that intercepts all HTTP requests and adds the Bearer token:

```csharp
protected override async Task<HttpResponseMessage> SendAsync(
    HttpRequestMessage request, 
    CancellationToken cancellationToken)
{
    // Get token (from cache or Azure AD)
    string accessToken = await GetAccessTokenAsync(cancellationToken);
    
    // Add Authorization header
    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
    
    // Continue with the request
    return await base.SendAsync(request, cancellationToken);
}
```

**Token Caching Strategy:**
- Tokens are cached in distributed cache
- Cache expiry is set to token expiry minus 3 minutes (safety margin)
- Uses `Azure.Identity` library's `ClientSecretCredential` for token acquisition

#### 2. Graph API Client (`GraphApiClient.cs`)

The client implements all file operations. Here's how it constructs Graph API URLs:

```csharp
// For listing files in a folder
var endpoint = $"drives/{driveId}/items/root:/{path}:/children?$select={selectQuery}";

// For uploading a file
var endpoint = $"drives/{driveId}/items/root:/{path}/{fileName}:/content";

// For deleting a file
var endpoint = $"drives/{driveId}/root:/{path}/{fileName}";
```

**Site and Drive Resolution:**
```csharp
// Site lookup pattern
// For root site:
"sites/{baseSpoSiteUri}"

// For named site:
"sites/{baseSpoSiteUri}:/sites/{siteName}"

// Drive lookup
"/sites/{siteId}/drives?$select=id,name,description,webUrl"
```

#### 3. Dependency Injection Setup

Register services in your application:

```csharp
// In Program.cs or Startup.cs
builder.Services.AddDistributedMemoryCache(); // Required for token caching
builder.Services.AddGraphApiServices(builder.Configuration);
```

The extension method registers:
```csharp
public static IServiceCollection AddGraphApiServices(
    this IServiceCollection services, IConfiguration config)
{
    services.Configure<GraphApiOptions>(
        config.GetSection(GraphApiOptions.GraphApiSettings));
    services.AddScoped<IGraphApiClientFactory, GraphApiClientFactory>();
    return services;
}
```

#### 4. Error Handling

Custom exceptions provide detailed error information:

```csharp
public class GraphApiException : Exception
{
    public HttpStatusCode HttpStatusCode { get; }
    public string ErrorMessage { get; }
    
    // Contains the full Graph API error response
}
```

### Extending the Application

To add new Graph API operations:

1. **Add interface method** in `IGraphApiClient.cs`:
```csharp
Task<FolderDetails> CreateFolder(string siteName, string driveName, string path, 
    string folderName, CancellationToken cancellationToken = default);
```

2. **Implement in** `GraphApiClient.cs`:
```csharp
public async Task<FolderDetails> CreateFolder(string siteName, string driveName, 
    string path, string folderName, CancellationToken cancellationToken = default)
{
    var driveDetails = await GetDrive(siteName, driveName, cancellationToken);
    var endpoint = $"drives/{driveDetails.id}/items/root:/{path}/{folderName}";
    
    var folderData = new { 
        name = folderName, 
        folder = new { }, 
        "@microsoft.graph.conflictBehavior" = "rename" 
    };
    
    return await PostAsync<object, FolderDetails>(endpoint, folderData, cancellationToken);
}
```

3. **Add controller endpoint** in `GraphApiController.cs`

## API Reference

### Endpoint Summary

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/GraphApi/{siteName}/{driveName}/{path}` | List files in folder |
| POST | `/GraphApi/{siteName}/{driveName}/{path}` | Upload new file |
| PUT | `/GraphApi/{siteName}/{driveName}/{path}` | Update existing file |
| GET | `/GraphApi/{siteName}/{driveName}/{path}/{fileName}` | Get file metadata |
| DELETE | `/GraphApi/{siteName}/{driveName}/{path}/{fileName}` | Delete file |
| PATCH | `/GraphApi/{siteName}/{driveName}/{path}/{fileName}` | Update file metadata |

### Query Parameters

The `select` query parameter allows you to specify which fields to return:

| Field | Type | Description |
|-------|------|-------------|
| `id` | string | Unique identifier for the item |
| `name` | string | File or folder name |
| `size` | long | File size in bytes |
| `webUrl` | string | URL to access the item in SharePoint |
| `createdDateTime` | datetime | When the item was created |
| `lastModifiedDateTime` | datetime | When the item was last modified |
| `file` | object | File-specific properties (null for folders) |
| `folder` | object | Folder-specific properties (null for files) |
| `parentReference` | object | Parent folder information |

**Default select query:**
```
id,name,size,webUrl,createdDateTime,lastModifiedDateTime,parentReference
```

## Technologies Used

| Technology | Version | Purpose |
|------------|---------|---------|
| **.NET** | 10.0 | Application framework |
| **ASP.NET Core** | 10.0 | Web API framework |
| **Azure.Identity** | 1.17.1 | Azure AD authentication |
| **Microsoft Graph API** | v1.0 | SharePoint access |
| **Distributed Caching** | Built-in | Token and data caching |
| **Swagger/OpenAPI** | 10.0.1 | API documentation |

## Codebase Overview

This section provides insights into the key components of the codebase to help developers understand and extend the functionality.

### Project Structure

- **Spo.GraphApi**: Core library containing interfaces, models, and implementations for Graph API operations
  - `GraphApiClient.cs`: Main implementation of Graph API operations
  - `GraphApiClientFactory.cs`: Factory for creating authenticated clients
  - `GraphApiAuthenticationHandler.cs`: Handles OAuth 2.0 token acquisition
  - `Models/`: Contains data models used throughout the application

- **Spo.WebApi**: ASP.NET Core Web API exposing endpoints
  - `Controllers/GraphApiController.cs`: Exposes REST endpoints
  - `Program.cs`: Application startup and configuration
  - `appsettings.json`: Application settings including Graph API configuration

- **API Collection**: Bruno API client collections for testing
  - Contains pre-configured requests for all API operations

### Key Implementation Details

1. **Authentication Flow**
   - The application uses the Client Credentials OAuth 2.0 flow
   - `GraphApiClientFactory` handles authentication token acquisition and renewal
   - Tokens are cached to improve performance

2. **Performance Optimizations**
   - Site and drive information is cached using distributed caching
   - This reduces redundant calls to Microsoft Graph API

3. **Error Handling**
   - Custom exception types for Graph API-specific errors
   - Consistent error response format across all endpoints

4. **Extension Points**
   - The modular design allows for easy extension of functionality
   - Implement additional Graph API features by extending the `IGraphApiClient` interface

## Testing with Bruno API Client

This project includes a collection of API requests for testing with Bruno API client.

### Setup

1. Install [Bruno API client](https://www.usebruno.com/downloads)
2. Open Bruno and import the `API Collection` folder
3. Configure environment variables in `environments/Local.bru`:

```
vars {
  baseUrl: https://localhost:7295
  siteName: root
  driveName: Documents
  folderPath: TestFolder
}
```

### Available Requests

| Request | Description |
|---------|-------------|
| Get All Files | Lists all files in the configured folder |
| Add a file | Uploads a new file to SharePoint |
| Read File | Gets metadata for a specific file |
| Update Files | Replaces an existing file |
| Delete a file | Removes a file from SharePoint |
| Update Metadata | Updates file properties |

## Troubleshooting

### Common Issues

#### 1. Authentication Errors

**Symptom:** `401 Unauthorized` or `403 Forbidden` responses

**Solutions:**
- Verify `TenantId`, `ClientId`, and `SecretId` in `appsettings.json`
- Ensure admin consent is granted for API permissions
- Check that the client secret hasn't expired
- Verify the `Scope` is set to `https://graph.microsoft.com/.default`

#### 2. Site Not Found

**Symptom:** `404 Not Found` when accessing a site

**Solutions:**
- Verify the `BaseSpoSiteUri` matches your SharePoint tenant URL
- Check that the site name is correct (use `root` for the main site)
- Ensure the app has access to the specific SharePoint site

#### 3. Drive Not Found

**Symptom:** Error stating drive not found

**Solutions:**
- Verify the document library name exactly matches SharePoint (case-sensitive)
- Common library names: `Documents`, `Shared Documents`
- Check that the library exists in the specified site

#### 4. Rate Limiting

**Symptom:** `429 Too Many Requests` responses

**Solutions:**
- Implement retry logic with exponential backoff
- Reduce frequency of API calls
- Use caching (already implemented for sites/drives)

#### 5. Large File Upload Failures

**Symptom:** Timeout or failure when uploading large files

**Solutions:**
- Files over 4MB should use the upload session API (not currently implemented)
- Consider chunked uploads for large files
- Increase request timeouts if needed

### Debugging Tips

1. **Enable detailed logging** in `appsettings.Development.json`:
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Debug",
      "Spo.GraphApi": "Debug"
    }
  }
}
```

2. **Check Graph API responses** - The `GraphApiException` includes the full error response from Microsoft Graph

3. **Use Graph Explorer** - Test queries directly at [Graph Explorer](https://developer.microsoft.com/graph/graph-explorer)

## Additional Resources

### Microsoft Graph API Documentation

- [Microsoft Graph API Overview](https://learn.microsoft.com/graph/overview)
- [Working with SharePoint Sites](https://learn.microsoft.com/graph/api/resources/sharepoint)
- [Drive Resource](https://learn.microsoft.com/graph/api/resources/drive)
- [DriveItem Resource](https://learn.microsoft.com/graph/api/resources/driveitem)
- [Uploading Files to OneDrive/SharePoint](https://learn.microsoft.com/graph/api/driveitem-put-content)

### Azure AD / Entra ID

- [Register an Application](https://learn.microsoft.com/azure/active-directory/develop/quickstart-register-app)
- [Client Credentials Flow](https://learn.microsoft.com/azure/active-directory/develop/v2-oauth2-client-creds-grant-flow)
- [Microsoft Graph Permissions Reference](https://learn.microsoft.com/graph/permissions-reference)

## Contributing

Contributions are welcome! If you find a bug or have a feature request, feel free to open an issue or submit a pull request.

1. Fork the repository
2. Create your feature branch (`git checkout -b feature/amazing-feature`)
3. Commit your changes (`git commit -m 'Add some amazing feature'`)
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

## License

This project is licensed under the MIT License. See the [LICENSE](LICENSE.txt) file for details.

## Contact

For any inquiries or support, please contact [nitin27may@gmail.com](mailto:nitin27may@gmail.com).

---

Built with ❤️ using .NET 10 and Microsoft Graph API.

