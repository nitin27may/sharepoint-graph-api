using Microsoft.AspNetCore.Mvc;
using Spo.GraphApi;
using Spo.GraphApi.Models;

namespace Spo.WebApi.Controllers;

[ApiController]
[Route("[controller]")]
public class GraphApiController : ControllerBase
{
    private readonly ILogger<GraphApiController> _logger;
    private readonly IGraphApiClient _graphApiClient;
    public GraphApiController(IGraphApiClientFactory graphApiClientFactory, ILogger<GraphApiController> logger)
    {
        _graphApiClient = graphApiClientFactory.Create();
        _logger = logger;
    }

    [HttpGet("{siteName}/{driveName}/{*path}")]
    public async Task<IActionResult> GetAllFiles(string siteName, string driveName, string path, [FromQuery] string select = "id,name,size,webUrl,createdDateTime,lastModifiedDateTime,parentReference", CancellationToken cancellationToken = default)
    {
        if (path != null)
        {
            path = System.Net.WebUtility.UrlDecode(path);
        }
        try
        {
            var files = await _graphApiClient.GetAllFiles(siteName, driveName, path, select, cancellationToken);
            return Ok(files);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Message = ex.Message });
        }
    }

    [HttpPost("{siteName}/{driveName}/{*path}")]
    public async Task<IActionResult> AddFile(string siteName, string driveName, string path, [FromForm] CustomFile file, CancellationToken cancellationToken)
    {
        if (path != null)
        {
            path = System.Net.WebUtility.UrlDecode(path);
        }
        try
        {
            var fileExtension = Path.GetExtension(file.File.FileName);

            // Append the extension to the file name if not already included
            if (!file.Name.EndsWith(fileExtension, StringComparison.OrdinalIgnoreCase))
            {
                file.Name += fileExtension;
            }
            var addedFile = await _graphApiClient.AddFile(siteName, driveName, path, file, cancellationToken);
            return CreatedAtAction(nameof(ReadFile), new { siteName, driveName, path, fileName = addedFile.Name }, addedFile);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Message = ex.Message });
        }
    }

    [HttpPut("{siteName}/{driveName}/{*path}")]
    public async Task<IActionResult> UpdateFile(string siteName, string driveName, string path, [FromForm] CustomFile file, CancellationToken cancellationToken)
    {
        if (path != null)
        {
            path = System.Net.WebUtility.UrlDecode(path);
        }
        try
        {
            var updatedFile = await _graphApiClient.UpdateFile(siteName, driveName, path, file, cancellationToken);
            return Ok(updatedFile);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Message = ex.Message });
        }
    }

    [HttpDelete("{siteName}/{driveName}/{path}/{fileName}")]
    public async Task<IActionResult> DeleteFile(string siteName, string driveName, string path, string fileName, CancellationToken cancellationToken)
    {
        if (path != null)
        {
            path = System.Net.WebUtility.UrlDecode(path);
        }
        try
        {
            await _graphApiClient.DeleteFile(siteName, driveName, path, fileName, cancellationToken);
            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Message = ex.Message });
        }
    }

    [HttpGet("{siteName}/{driveName}/{path}/{fileName}")]
    public async Task<IActionResult> ReadFile(string siteName, string driveName, string path, string fileName, [FromQuery] string select = "id,name,size,webUrl,createdDateTime,lastModifiedDateTime,parentReference", CancellationToken cancellationToken = default)
    {
        if (path != null)
        {
            path = System.Net.WebUtility.UrlDecode(path);
        }
        try
        {
            var file = await _graphApiClient.ReadFile(siteName, driveName, path, fileName, select, cancellationToken);
            return Ok(file);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Message = ex.Message });
        }
    }

    [HttpPatch("{siteName}/{driveName}/{path}/{fileName}")]
    public async Task<IActionResult> UpdateFileMetadata(string siteName, string driveName, string path, string fileName, [FromBody] Dictionary<string, string> metadataUpdates, CancellationToken cancellationToken)
    {
        if (path != null)
        {
            path = System.Net.WebUtility.UrlDecode(path);
        }
        try
        {
            if (metadataUpdates == null || metadataUpdates.Count == 0)
                return BadRequest(new { Message = "Metadata updates cannot be null or empty." });

            var updatedFile = await _graphApiClient.UpdateFileMetadata(siteName, driveName, path, fileName, metadataUpdates, cancellationToken);
            return Ok(updatedFile);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Message = ex.Message });
        }
    }
}