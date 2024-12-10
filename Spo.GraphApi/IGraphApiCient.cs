using Spo.GraphApi.Models;

namespace Spo.GraphApi;

public interface IGraphApiClient
{
    Task<List<FileDetails>> GetAllFiles(string siteName, string driveName, string path, string selectQuery = "id,name,size,webUrl,createdDateTime,lastModifiedDateTime,parentReference", CancellationToken cancellationToken = default);
    Task<FileDetails> AddFile(string siteName, string driveName, string path, CustomFile file, CancellationToken cancellationToken = default);
    Task<FileDetails> UpdateFile(string siteName, string driveName, string path, CustomFile file, CancellationToken cancellationToken = default);
    Task DeleteFile(string siteName, string driveName, string path, string fileName, CancellationToken cancellationToken = default);
    Task<FileDetails> ReadFile(string siteName, string driveName, string path, string fileName, string selectQuery = "id,name,size,webUrl,createdDateTime,lastModifiedDateTime,parentReference", CancellationToken cancellationToken = default);

    Task<FileDetails> UpdateFileMetadata(string siteName, string driveName, string path, string fileName, Dictionary<string, string> metadataUpdates, CancellationToken cancellationToken = default);

}