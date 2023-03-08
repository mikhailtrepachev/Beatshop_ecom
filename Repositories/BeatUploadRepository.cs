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

	public async Task<bool> CreateAsync(Beat track)
	{
        _dbContext.Add(track);
        await _dbContext.SaveChangesAsync();
        return true;
	}

	public async Task<List<Beat>> GetAllAsync()
	{
		return await _dbContext.Beats
					.ToListAsync();
	}
}

