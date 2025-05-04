using Minio.DataModel.Args;
using Minio.DataModel;
using Minio;
using Backend.Models;

namespace Backend.Services
{
    public class MinioService(IMinioClient minioClient, IConfiguration configuration)
    {
        private readonly IMinioClient _minioClient = minioClient;
        private readonly string[] _allowedBuckets = configuration.GetSection("AllowedBuckets").Get<string[]>() ?? [];

        public string GetAllowedBuckets()
        {
            var allowedBuckets = ""; //Ovdje sad koristim globalno sve buckete definirane, ali vjerovatno bi isla definicija po useru.
            for (var i = 0; i < _allowedBuckets.Length; i++)
            {
                allowedBuckets += _allowedBuckets[i];
                if (i + 1 != _allowedBuckets.Length)
                {
                    allowedBuckets += ",";
                }
            }
            return allowedBuckets;
        }

        public async Task<bool> BucketExistsAsync(string bucketName)
        {
            var args = new BucketExistsArgs()
                .WithBucket(bucketName);
            return await _minioClient.BucketExistsAsync(args);
        }

        public async Task CreateBucketAsync(string bucketName)
        {
            if (!await BucketExistsAsync(bucketName) && _allowedBuckets.Contains(bucketName))
            {
                var args = new MakeBucketArgs()
                    .WithBucket(bucketName);
                await _minioClient.MakeBucketAsync(args);
            }
        }

        public async Task<IEnumerable<string>> ListBucketsAsync()
        {
            var buckets = new List<string>();
            var bucketList = await _minioClient.ListBucketsAsync();

            foreach (var bucket in bucketList.Buckets)
            {
                buckets.Add(bucket.Name);
            }

            return buckets;
        }

        public async Task UploadFileAsync(string bucketName, string objectName, Stream data, long size, string contentType)
        {
            var args = new PutObjectArgs()
                .WithBucket(bucketName)
                .WithObject(objectName)
                .WithStreamData(data)
                .WithObjectSize(size)
                .WithContentType(contentType);

            await _minioClient.PutObjectAsync(args);
        }

        public async Task<Stream> GetFileAsync(string bucketName, string objectName)
        {
            var memoryStream = new MemoryStream();

            var args = new GetObjectArgs()
                .WithBucket(bucketName)
                .WithObject(objectName)
                .WithCallbackStream(stream =>
                {
                    stream.CopyTo(memoryStream);
                    memoryStream.Position = 0;
                });

            await _minioClient.GetObjectAsync(args);

            return memoryStream;
        }

        public async Task<ObjectStat> GetObjectStatAsync(string bucketName, string objectName)
        {
            var args = new StatObjectArgs()
                .WithBucket(bucketName)
                .WithObject(objectName);

            return await _minioClient.StatObjectAsync(args);
        }

        public async Task DeleteFileAsync(string bucketName, string objectName)
        {
            var args = new RemoveObjectArgs()
                .WithBucket(bucketName)
                .WithObject(objectName);

            if (_allowedBuckets.Contains(bucketName))
            {
                await _minioClient.RemoveObjectAsync(args);
            }
        }

        public async Task<IEnumerable<FileData>> ListFilesAsync(string bucketName, string? prefix = null, bool returnWithoutPrefix = false)
        {

            if (_allowedBuckets.Contains(bucketName) == false)
            {
                return [];
            }

            var files = new List<FileData>();
            var listArgs = new ListObjectsArgs()
                .WithBucket(bucketName)
                .WithPrefix(prefix ?? "")
                .WithRecursive(true);

            var items = _minioClient.ListObjectsEnumAsync(listArgs);
            
            await foreach(var item in items)
            {
                if (returnWithoutPrefix && prefix != null)
                {
                    item.Key = item.Key.Remove(0, prefix.Length);
                }
                files.Add(new FileData()
                {
                    Name = item.Key,
                    ModifiedOn = item.LastModifiedDateTime,
                    FileSize = $"{((double)item.Size / 1000)} kB"
                });
            }

            return files.OrderBy(x => x.Name);
        }
    }
}
