using Domain.BaseResponce;
using Domain.Entity;
using Microsoft.AspNetCore.Http;

namespace Domain.Abstraction
{
    public interface IGoogleDriveRepo
    {
        Task<ApiResponse> UploadFile(string folderName, IFormFile file);
        Task<ApiResponse> UpdateFileMetadata(string fileId, string newName);
        Task<ApiResponse> DeleteFile(string fileId);
        //Task<ApiResponse> DeleteFolderAsync(string folderId);
        //Task<ApiResponse> GetFileById(string fileId);
        //Task<ApiResponse> GetAllFilesInFolder(string folderId);
    }




}
