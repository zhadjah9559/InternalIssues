using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace InternalIssues.Services
{
    public interface IImageService
    {
        Task<byte[]> EncodeFileAsync(IFormFile formFile);

        string DecodeFile(byte[] imageData, string contentType);

        string RecordContentType(IFormFile formFile);
    }
}
