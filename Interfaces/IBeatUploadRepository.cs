using System;
using Beatshop.Models;
using Microsoft.AspNetCore.Mvc;

namespace Beatshop.Interfaces;

public interface IBeatUploadRepository
{
    Task<bool> CreateAsync(Beat track);

    Task<List<Beat>> GetAllAsync();
}

