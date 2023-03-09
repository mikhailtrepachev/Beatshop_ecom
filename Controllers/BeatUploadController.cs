using Microsoft.AspNetCore.Mvc;
using Beatshop.Interfaces;
using Beatshop.Models;
using System.Net;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace Beatshop.Controllers;

[Route("api/beatupload")]
[ApiController]
[Authorize]
public class BeatUploadController : ControllerBase
{
    private IBeatUploadRepository _beatRepository;
    private IAmazonServiceRepository _amazonServiceRepository;

    public BeatUploadController(IBeatUploadRepository beatUploadRepository,
        IAmazonServiceRepository amazonServiceRepository)
    {
        _beatRepository = beatUploadRepository;
        _amazonServiceRepository = amazonServiceRepository;
    }
    [HttpPost]
    public async Task<IActionResult> Post([FromForm(Name = "trackName")] string trackName,
        [FromForm(Name = "trackFile")] IFormFile trackFile,
        [FromForm(Name = "description")] string trackDescription, [FromForm(Name = "genre")] string trackGenre)
    {
        long sizeTrackInBytes = trackFile.Length;
        string extensionTrack = Path.GetExtension(trackFile.FileName);

        var creationDate = DateTime.Now.ToString("MM/dd/yyyy HH:mm");

        //TODO: Create a check for the presence of a file with the same name

        var newFileName = trackName + extensionTrack;

        if (trackFile == null || trackFile.Length == 0)
        {
            return BadRequest("Please, provide a track file.");
        }

        if (trackFile.Length > 80 * 1024 * 1024)
        {
            return BadRequest("The track size should not be more than 80mb!");
        }

        using var memoryStream = new MemoryStream();
        await trackFile.CopyToAsync(memoryStream);

        //Get user's id
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if(userId == null)
        {
            return BadRequest("Identity could not be identified");
        }

        var responseFromAmazon = await _amazonServiceRepository.UploadFileAsync(trackFile, memoryStream, newFileName);

        var track = new Beat
        {
            Id = Guid.NewGuid().ToString(),
            Name = newFileName,
            Description = trackDescription,
            Genre = trackGenre,
            CreatedById = userId,
            CreationDate = creationDate,
        };

        var responseFromDatabase = await _beatRepository.CreateAsync(track);

        if (responseFromAmazon.HttpStatusCode == HttpStatusCode.OK)
        {
            return Ok("File has been uploaded successfuly!");
        }
        else
        {
            return BadRequest("Failed to connect to file storage!");
        }
    }
}


