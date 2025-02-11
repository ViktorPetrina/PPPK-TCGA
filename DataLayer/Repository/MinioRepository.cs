using Microsoft.Extensions.Configuration;
using Minio;
using Minio.DataModel.Args;
using System.Net.Mime;
using System.Security.AccessControl;

namespace DataLayer.Repository
{
    public class MinioRepository : ISimpleFileRepository
    {
        private readonly IMinioClient _client;
        private readonly IConfiguration _configuration;

        public MinioRepository()
        {
            var basePath = Path.GetFullPath
            (
                Path.Combine(Directory.GetCurrentDirectory(), @"..\..\..\..\DataLayer")
            );

            _configuration = new ConfigurationBuilder()
                .SetBasePath(basePath)
                .AddJsonFile("appsettings.json")
                .Build();

            var endpoint = _configuration["MinIO:Endpoint"];
            var accessKey = _configuration["MinIO:AccessKey"];
            var secretKey = _configuration["MinIO:SecretKey"];

            _client = new MinioClient()
                .WithEndpoint(endpoint)
                .WithCredentials(accessKey, secretKey)
                .Build();
        }

        public async Task Create(string objectName, string filePath, string contentType)
        {
            var bucketName = _configuration["MinIO:BucketName"];

            var fileInfo = new FileInfo(filePath);

            if (!fileInfo.Exists)
            {
                throw new FileNotFoundException("File not found.", filePath);
            }

            using (var fileStream = fileInfo.OpenRead())
            {
                await _client.PutObjectAsync(new PutObjectArgs()
                    .WithBucket(bucketName)
                    .WithObject(objectName)
                    .WithStreamData(fileStream)
                    .WithObjectSize(fileStream.Length)
                    .WithContentType(contentType));
            }
        }

        public async Task ReadAll(string objectName, string filePath)
        {
            var bucketName = _configuration["MinIO:BucketName"];

            await _client.GetObjectAsync(new GetObjectArgs()
                .WithBucket(bucketName)
                .WithObject(objectName)
                .WithFile(filePath));

            
        }
    }
}
