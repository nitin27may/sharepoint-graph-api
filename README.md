# SharePoint Online File Operations using Microsoft Graph API

This .NET 9 project demonstrates how to perform file operations in SharePoint Online using the Microsoft Graph API with OAuth 2.0 authentication and delegated access. The application facilitates seamless file interactions, leveraging modern authentication techniques and the capabilities of the Graph API.

## Features
- OAuth 2.0 authentication with delegated access.
- File operations on SharePoint Online, such as:
  - Uploading files
  - Downloading files
  - Deleting files
  - Listing files
- Easy-to-use interface for managing files in SharePoint document libraries.

## Prerequisites

Before running the application, ensure you have the following:
- A Microsoft 365 tenant with SharePoint Online enabled.
- Azure Active Directory (AAD) App Registration:
  - Client ID
  - Tenant ID
  - Client Secret
  - Redirect URI configured for OAuth 2.0 authentication.
- Permissions granted in Azure AD App Registration for Microsoft Graph API:
  - `Sites.ReadWrite.All`
  - `Files.ReadWrite.All`
- Visual Studio 2022 or later.
- .NET 9 SDK installed.

## Getting Started

### 1. Clone the Repository
```bash
git clone https://github.com/nitin27may/sharepoint-graph-api
cd sharepoint-graph-api
```

### 2. Configure the Application
1. Open the `appsettings.json` file and update the placeholders with your Azure AD App Registration details:
    ```json
    {
    "GraphApiSettings": {
        "ClientId": "<YOUR-CLIENT-ID>",
        "TenantId": "<YOUR-TENANT-ID>",
        "ClientSecret": "<YOUR-CLIENT-SECRET>",
        "Scope": "https://graph.microsoft.com/.default ",
        "BaseGraphUri": "https://graph.microsoft.com/v1.0",
        "BaseSpoSiteUri": "<YOUR-BASE-SHAREPOINT-SITE-Name>.sharepoint.com"
        }
    }
    ```

### 3. Build and Run the Application
1. Open the project in Visual Studio.
2. Restore NuGet packages:
   ```bash
   dotnet restore
   ```
3. Build the solution:
   ```bash
   dotnet build
   ```
4. Run the application:
   ```bash
   dotnet run
   ```
Note: We are using [Bruno API client](https://www.usebruno.com/downloads) for the testing Rest API, the files are added in Api Collection Folder

### 4. Authenticate
- This project has `GraphApiCientFactory` which is handling the authentication.
- Upon successful authentication, the application will obtain an access token to interact with SharePoint Online via the Graph API.

## Usage

Please pass `root`as siteName if you are using base SPO site.

### Upload a File
1. Select the document library where you want to upload the file.
2. Provide the file path or use the file picker to select the file.
3. Click `Upload` to upload the file to SharePoint Online.

### Download a File
1. Select the file you want to download from the list of files.
2. Click `Download` to save the file locally.

### Delete a File
1. Select the file you want to delete from the list of files.
2. Click `Delete` to remove the file from SharePoint Online.

### List Files
- The application automatically fetches and displays the files in the selected document library.

## Technologies Used
- **.NET 9**
- **Microsoft Graph API**
- **OAuth 2.0 Authentication**
- **Azure Active Directory**
- **SharePoint Online**

## Contributing
Contributions are welcome! If you find a bug or have a feature request, feel free to open an issue or submit a pull request.

## License
This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for details.

## Contact
For any inquiries or support, please contact [nitin27may@gmail.com](mailto:nitin27may@gmail.com).

