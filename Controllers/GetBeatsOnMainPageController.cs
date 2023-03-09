using System;
using Beatshop.Interfaces;
using Beatshop.Models;
using Microsoft.AspNetCore.Mvc;

namespace Beatshop.Controllers;

[Route("/api/get_beats_on_main_page")]
[ApiController]
public class GetBeatsOnMainPage : ControllerBase
{
	private IBeatUploadRepository _beatUploadRepository;

	public GetBeatsOnMainPage(IBeatUploadRepository beatUploadRepository)
	{
		_beatUploadRepository = beatUploadRepository;
	}

	[HttpGet]
	public async Task<List<Beat>> Get()
	{
		return await _beatUploadRepository.GetBeats();
	}
}

