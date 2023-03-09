using Microsoft.EntityFrameworkCore;
using Beatshop.Data;
using Beatshop.Interfaces;
using Beatshop.Models;

namespace Beatshop.Repositories;

public class BeatUploadRepository : IBeatUploadRepository
{
	private readonly ApplicationDbContext _dbContext;

	public BeatUploadRepository(ApplicationDbContext dbContext)
	{
		_dbContext = dbContext;
	}

	//insert a new Beat model inside the database
	public async Task<bool> CreateAsync(Beat track)
	{
        _dbContext.Add(track);
        await _dbContext.SaveChangesAsync();
        return true;
	}

    //return 20 beats for the main page. TODO: rename the method depending on the logic
    public async Task<List<Beat>> GetBeats()
	{
		return await _dbContext.Beats
					.OrderByDescending(o => o.CreationDate)
					.Take(20)
					.ToListAsync();
	}
}

