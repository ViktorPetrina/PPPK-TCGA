using Microsoft.Extensions.Configuration;
using Minio;
using Minio.ApiEndpoints;
using Minio.DataModel.Args;
using System.Net.Mime;
using System.Security.AccessControl;

namespace DataLayer.Repository
{
    // TODO: 
    // rijesiti konfiguraciju, ako se dohvaca po currentDirectoryju onda se treba mijenjati za svaki projekt
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

        public async Task<IEnumerable<string>> ReadAll()
        {
            var bucketName = _configuration["MinIO:BucketName"];
            var objects = _client.ListObjectsEnumAsync(new ListObjectsArgs().WithBucket(bucketName));
            IList<string> result = new List<string>();

            await foreach (var item in objects)
            {
                result.Add(ReadTSVFile(bucketName, item.Key).Result);
                Console.WriteLine("Added tsv file");
            }

            return result;
        }

        private async Task<string> ReadTSVFile(string bucketName, string objectName)
        {
            try
            {
                var memoryStream = new MemoryStream();

                var args = new GetObjectArgs()
                                .WithBucket(bucketName)
                                .WithObject(objectName)
                                .WithCallbackStream(stream =>
                                {
                                    stream.CopyTo(memoryStream);
                                });

                await _client.GetObjectAsync(args);

                memoryStream.Position = 0;
                using (var reader = new StreamReader(memoryStream)) 
                { 
                    var content = await reader.ReadToEndAsync();
                    return content;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error reading {objectName}: {ex.Message}");
                return String.Empty;
            }
        }

    }
}
