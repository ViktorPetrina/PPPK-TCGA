using Minio;
using Minio.DataModel.Args;

namespace DataLayer.Repository
{
    public class MinioRepository : ISimpleFileRepository
    {
        private readonly IMinioClient _client;

        private const string BUCKET_NAME = "scans";

        public MinioRepository(string endpoint, string accessKey, string secretKey)
        {
            _client = new MinioClient()
                .WithEndpoint(endpoint)
                .WithCredentials(accessKey, secretKey)
                .Build();
        }

        public async Task Create(string objectName, string filePath, string contentType)
        {
            var fileInfo = new FileInfo(filePath);

            if (!fileInfo.Exists)
            {
                throw new FileNotFoundException("File not found.", filePath);
            }

            using (var fileStream = fileInfo.OpenRead())
            {
                await _client.PutObjectAsync(new PutObjectArgs()
                    .WithBucket(BUCKET_NAME)
                    .WithObject(objectName)
                    .WithStreamData(fileStream)
                    .WithObjectSize(fileStream.Length)
                    .WithContentType(contentType));
            }
        }

        public async Task<IEnumerable<string>> ReadAll()
        {
            var objects = _client.ListObjectsEnumAsync(new ListObjectsArgs().WithBucket(BUCKET_NAME));
            IList<string> result = new List<string>();

            await foreach (var item in objects)
            {
                result.Add(ReadTSVFile(BUCKET_NAME, item.Key).Result);
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
