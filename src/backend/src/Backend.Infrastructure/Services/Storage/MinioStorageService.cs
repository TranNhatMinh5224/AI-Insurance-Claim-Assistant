using Backend.Application.Abstractions;
using Microsoft.Extensions.Options;
using Minio;
using Minio.DataModel.Args;
using Minio.Exceptions;

namespace Backend.Infrastructure.Services.Storage;


internal sealed class MinioStorageService : IFileStorageService
{
    private readonly IMinioClient _minioClient;
    private readonly MinioSettings _settings;

    public MinioStorageService(IOptions<MinioSettings> options)
    {
        _settings = options.Value;
        
        var endpoint = _settings.Endpoint.Replace("http://", "").Replace("https://", "");
        
        _minioClient = new MinioClient()
            .WithEndpoint(endpoint)
            .WithCredentials(_settings.AccessKey, _settings.SecretKey)
            .WithSSL(_settings.UseSSL)
            .Build();
    }

    public async Task<string> UploadFileAsync(Stream fileStream, string fileName, string contentType, string bucketName, bool isDraft = true, CancellationToken cancellationToken = default)
    {
        try
        {
            var bktExistArgs = new BucketExistsArgs().WithBucket(bucketName);
            bool found = await _minioClient.BucketExistsAsync(bktExistArgs, cancellationToken);
            if (!found)
            {
                var mkBktArgs = new MakeBucketArgs().WithBucket(bucketName);
                await _minioClient.MakeBucketAsync(mkBktArgs, cancellationToken);
                
                var policy = $@"{{""Version"":""2012-10-17"",""Statement"":[{{""Effect"":""Allow"",""Principal"":{{""AWS"":[""*""]}},""Action"":[""s3:GetBucketLocation"",""s3:ListBucket""],""Resource"":[""arn:aws:s3:::{bucketName}""]}},{{""Effect"":""Allow"",""Principal"":{{""AWS"":[""*""]}},""Action"":[""s3:GetObject""],""Resource"":[""arn:aws:s3:::{bucketName}/*""]}}]}}";
                var setPolicyArgs = new SetPolicyArgs().WithBucket(bucketName).WithPolicy(policy);
                await _minioClient.SetPolicyAsync(setPolicyArgs, cancellationToken);
            }

            if (fileStream.CanSeek)
            {
                fileStream.Position = 0;
            }

            // Gắn tiền tố folder
            string objectName = isDraft ? $"draft/{fileName}" : $"real/{fileName}";

            var putObjectArgs = new PutObjectArgs()
                .WithBucket(bucketName)
                .WithObject(objectName)
                .WithStreamData(fileStream)
                .WithObjectSize(fileStream.Length)
                .WithContentType(contentType);

            await _minioClient.PutObjectAsync(putObjectArgs, cancellationToken);

            return GetFileUrl(fileName, bucketName, isDraft);
        }
        catch (MinioException e)
        {
            throw new Exception($"MinIO Upload Error: {e.Message}");
        }
    }

    public async Task<string> CommitFileAsync(string fileName, string bucketName, CancellationToken cancellationToken = default)
    {
        try
        {
            string sourceObject = $"draft/{fileName}";
            string destObject = $"real/{fileName}";

            // 1. Copy file từ draft sang real
            var copySourceObjectArgs = new CopySourceObjectArgs()
                .WithBucket(bucketName)
                .WithObject(sourceObject);
                
            var copyArgs = new CopyObjectArgs()
                .WithBucket(bucketName)
                .WithObject(destObject)
                .WithCopyObjectSource(copySourceObjectArgs);

            await _minioClient.CopyObjectAsync(copyArgs, cancellationToken);

            // 2. Xóa file ở draft
            var removeArgs = new RemoveObjectArgs()
                .WithBucket(bucketName)
                .WithObject(sourceObject);
                
            await _minioClient.RemoveObjectAsync(removeArgs, cancellationToken);

            // Trả về URL mới (bản real)
            return GetFileUrl(fileName, bucketName, isDraft: false);
        }
        catch (MinioException e)
        {
            throw new Exception($"MinIO Commit Error: {e.Message}");
        }
    }

    public string GetFileUrl(string fileName, string bucketName, bool isDraft = false)
    {
        var host = string.IsNullOrWhiteSpace(_settings.BaseUrl) 
            ? $"http{(_settings.UseSSL ? "s" : "")}://{_settings.Endpoint}"
            : _settings.BaseUrl;
            
        host = host.TrimEnd('/');
        string folder = isDraft ? "draft" : "real";
        
        return $"{host}/{bucketName}/{folder}/{fileName}";
    }
}
