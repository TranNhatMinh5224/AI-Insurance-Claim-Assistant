namespace Backend.Application.Abstractions;

public interface IFileStorageService
{
    // Nếu isDraft = true, file sẽ lưu vào folder "draft/". Ngược lại lưu vào "real/"
    Task<string> UploadFileAsync(Stream fileStream, string fileName, string contentType, string bucketName, bool isDraft = true, CancellationToken cancellationToken = default);
    
    // Di chuyển file từ folder "draft/" sang "real/"
    Task<string> CommitFileAsync(string fileName, string bucketName, CancellationToken cancellationToken = default);
    
    // Lấy URL tương ứng với trạng thái của file
    string GetFileUrl(string fileName, string bucketName, bool isDraft = false);
}
