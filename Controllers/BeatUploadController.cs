using Microsoft.AspNetCore.Mvc;
using Beatshop.Interfaces;
using Beatshop.Models;
using Amazon;
using Amazon.S3;
using Amazon.S3.Transfer;
using System.Net;
using System.Security.Claims;

namespace Beatshop.Controllers;

[Route("api/beatupload")]
[ApiController]
public class BeatUploadController : ControllerBase
{
    private IBeatUploadRepository _beatRepository;
    private IAmazonServiceRepository _amazonServiceRepository;

    public BeatUploadController(IBeatUploadRepository beatUploadRepository, IAmazonServiceRepository amazonServiceRepository)
    {
        _beatRepository = beatUploadRepository;
        _amazonServiceRepository = amazonServiceRepository;
    }

    [HttpGet]
    public async Task<List<Beat>> Get()
    {
        return await _beatRepository.GetAllAsync();
    }
    [HttpPost]
    public async Task<IActionResult> Post([FromForm(Name = "trackName")] string trackName,
        [FromForm(Name = "trackFile")] IFormFile trackFile,
        [FromForm(Name = "description")] string trackDescription, [FromForm(Name = "genre")] string trackGenre)
    {
        long sizeTrackInBytes = trackFile.Length;
        string extensionTrack = Path.GetExtension(trackFile.FileName);

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

        var responseFromAmazon = await _amazonServiceRepository.UploadFileAsync(trackFile, memoryStream);

        var track = new Beat
        {
            Name = trackFile.FileName,
            Description = trackDescription,
            Genre = trackGenre,
            CreatedById = userId,
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


