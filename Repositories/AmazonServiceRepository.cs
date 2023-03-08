using System;
using Amazon.S3;
using Amazon;
using Amazon.S3.Transfer;
using Beatshop.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Amazon.S3.Model;
using System.Net;

namespace Beatshop.Repositories;

public class AmazonServiceRepository : IAmazonServiceRepository
{
    private readonly AmazonS3Client _s3Client;

    public AmazonServiceRepository(AmazonS3Client s3Client)
    {
        _s3Client = s3Client;
    }

    public async Task<PutObjectResponse> UploadFileAsync(IFormFile formFile, MemoryStream memoryStream)
    {
        var request = new PutObjectRequest
        {
            BucketName = "beats.storage",
            Key = formFile.FileName,
            InputStream = memoryStream,
            ContentType = formFile.ContentType
        };

        var response = await _s3Client.PutObjectAsync(request);

        return response;
    }
}

