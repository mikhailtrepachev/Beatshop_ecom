using System;
using Amazon.S3.Model;
using Microsoft.AspNetCore.Mvc;

namespace Beatshop.Interfaces;

public interface IAmazonServiceRepository
{
    Task<PutObjectResponse> UploadFileAsync(IFormFile formFile, MemoryStream memoryStream);
}

