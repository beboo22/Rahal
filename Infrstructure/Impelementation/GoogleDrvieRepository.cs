using Domain.Abstraction;
using Google.Apis.Drive.v3;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Domain.BaseResponce;
using Domain.Entity;
using Google.Apis.Upload;


namespace Infrstructure.Impelementation
{
    public class GoogleDrvieRepository : IGoogleDriveRepo
    {
        private readonly DriveService _driveService;

        public GoogleDrvieRepository(
            IHostingEnvironment env
            )
        {
            var credentialsFilePath = Path.Combine(env.WebRootPath, "credentials", "rahal-476211-c75bcf996aee.json");

            if (string.IsNullOrWhiteSpace(credentialsFilePath))
                throw new ArgumentException("Credentials file path cannot be null or empty.", nameof(credentialsFilePath));

            if (!File.Exists(credentialsFilePath))
                throw new FileNotFoundException("The credentials file was not found.", credentialsFilePath);

            // Load the service account credentials from the JSON file
            var credential = GoogleCredential.FromFile(credentialsFilePath)
                                             .CreateScoped(DriveService.Scope.Drive);

            _driveService = new DriveService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = "Rahal",
            });


        }

        public async Task<ApiResponse> UploadFile(string folderName, IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return new ApiResponse(400);
            }

            //string folderId = await GetOrCreateFolder(folderName, "1_gHdfM8OvnbKg4nAAxQyslj1QThMlI4p");
            //string folderId = await GetOrCreateFolder(folderName, "12FEBxwIzIJ_EudZuiX3QZEuYdvzcCBUk");

            //var fileMetadata = new Google.Apis.Drive.v3.Data.File
            //{
            //    Name = file.FileName,
            //    Parents = new[] { folderId }
            //};

            var fileMetadata = new Google.Apis.Drive.v3.Data.File
            {
                Name = file.FileName,
                Parents = new[] { "12FEBxwIzIJ_EudZuiX3QZEuYdvzcCBUk" }
            };


            var fileExtension = Path.GetExtension(file.FileName).ToLower();
            string mimeType = fileExtension switch
            {
                ".jpg" or ".jpeg" or ".png" => "image/jpeg",
                ".pdf" => "application/pdf",
                ".mp4" => "video/mp4",
                ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                ".xlsx" => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                _ => "application/octet-stream"
            };

            using var fileStream = file.OpenReadStream();

            var request = _driveService.Files.Create(fileMetadata, fileStream, mimeType);
            request.Fields = "id, webViewLink";
            var result = await request.UploadAsync();

            if (result.Status == UploadStatus.Completed)
            {
                var response = request.ResponseBody;

                var attachment = new Attachments
                {
                    FileId = response.Id,
                    Extend = fileExtension,
                    Type = mimeType,
                    Path = response.WebViewLink

                };

                //await _unitOfWork.Repository<Attachments>().AddAsync(attachment);
                //await _unitOfWork.SaveChangesAsync();
                //var attachmentDto = _mapper.Map<AttachmentsDto>(attachment);
                return new ApiResultResponse<Attachments>(201,attachment);
            }

            return new ApiResponse(500,result.Exception.Message);
        }

        private async Task<string> GetOrCreateFolder(string folderName, string parentFolderId = null)
        {
            var request = _driveService.Files.List();

            request.Q = $"mimeType='application/vnd.google-apps.folder' and name='{folderName}'"
                        + (parentFolderId != null ? $" and '{parentFolderId}' in parents" : "");

            request.Fields = "files(id, name)";

            var result = await request.ExecuteAsync();

            var folder = result.Files.FirstOrDefault();
            if (folder != null)
            {
                return folder.Id;
            }

            // Create a new folder
            var fileMetadata = new Google.Apis.Drive.v3.Data.File
            {
                Name = folderName,
                MimeType = "application/vnd.google-apps.folder",
                Parents = parentFolderId != null ? new[] { parentFolderId } : null
            };

            var createRequest = _driveService.Files.Create(fileMetadata);
            createRequest.Fields = "id";
            var createdFolder = await createRequest.ExecuteAsync();

            //var permission = new Google.Apis.Drive.v3.Data.Permission()
            //{
            //    Type = "user",
            //    Role = "reader",
            //    EmailAddress = "@gmail.com"
            //};

            //var permissionRequest = _driveService.Permissions.Create(permission, createdFolder.Id);
            //permissionRequest.Execute();
            return createdFolder.Id;
        }

        public async Task<ApiResponse> UpdateFileMetadata(string fileId, string newName)
        {
            if (string.IsNullOrEmpty(fileId) || string.IsNullOrEmpty(newName))
            {
                return new ApiResponse(400);
            }

            try
            {
                var getFileRequest = _driveService.Files.Get(fileId);
                getFileRequest.Fields = "id";
                var existingFile = await getFileRequest.ExecuteAsync();
            }
            catch (Exception ex)
            {
                return new ApiResponse(500, ex.Message);
            }

            var fileMetadata = new Google.Apis.Drive.v3.Data.File
            {
                Name = newName
            };

            var request = _driveService.Files.Update(fileMetadata, fileId);
            request.Fields = "id, name, mimeType, webViewLink";
            var updatedFile = await request.ExecuteAsync();

            if (updatedFile != null)
            {
                var attachment = new Attachments
                {
                    FileId = updatedFile.Id,
                    Extend = Path.GetExtension(updatedFile.Name).ToLower(),
                    Type = updatedFile.MimeType,
                    Path = updatedFile.WebViewLink
                };


                //var attachmentDto = _mapper.Map<AttachmentsDto>(attachment);
                return new ApiResultResponse<Attachments>(200,attachment);
            }

            return new ApiResponse(500);
        }

        public async Task<ApiResponse> DeleteFile(string fileId)
        {
            if (string.IsNullOrEmpty(fileId))
            {
                return new ApiResponse(400);
            }

            var request = _driveService.Files.Delete(fileId);
            var response = await request.ExecuteAsync();

            if (response != null)
            {
                //var attachment = await _unitOfWork.Repository<Attachments>().FirstOrDefaultAsync(x => x.FileId == fileId);
                //if (attachment != null)
                //{
                //    await _unitOfWork.Repository<Attachments>().DeleteByIdAsync(attachment.Id);
                //}
                //await _unitOfWork.SaveChangesAsync();
                return new ApiResponse(200);
            }

            return new ApiResponse(500);
        }

        //public async Task<ApiResponse> DeleteFolderAsync(string folderId)
        //{
        //    if (string.IsNullOrEmpty(folderId))
        //    {
        //        return Result.Failure<bool>(GoogleDriveErrors.InvalidFolderId);
        //    }
        //    var listRequest = _driveService.Files.List();
        //    listRequest.Q = $"'{folderId}' in parents";
        //    listRequest.Fields = "files(id)";

        //    var files = await listRequest.ExecuteAsync();

        //    foreach (var file in files.Files)
        //    {
        //        var deleteFileRequest = _driveService.Files.Delete(file.Id);
        //        await deleteFileRequest.ExecuteAsync();
        //    }

        //    var deleteFolderRequest = _driveService.Files.Delete(folderId);
        //    await deleteFolderRequest.ExecuteAsync();

        //    return Result.Success(true);
        //}

        //public async Task<ApiResponse> GetAllFilesInFolder(string folderId)
        //{
        //    var request = _driveService.Files.List();
        //    request.Q = $"'{folderId}' in parents";
        //    request.Fields = "files(id, name, mimeType, webViewLink)";
        //    var result = await request.ExecuteAsync();

        //    var attachments = result.Files.Select(file => new AttachmentsDto
        //    {
        //        FileId = file.Id,
        //        Extend = Path.GetExtension(file.Name).ToLower(),
        //        Type = file.MimeType,
        //    }).ToList();

        //    return Result.Success(attachments);
        //}

        //public async Task<ApiResponse> GetFileById(string fileId)
        //{
        //    if (string.IsNullOrEmpty(fileId))
        //    {
        //        return Result.Failure<(AttachmentsDto, string)>(GoogleDriveErrors.InvalidFileId);
        //    }

        //    var request = _driveService.Files.Get(fileId);
        //    request.Fields = "id, name, mimeType, webViewLink";
        //    var file = await request.ExecuteAsync();

        //    if (file != null)
        //    {
        //        using (var stream = new MemoryStream())
        //        {
        //            await request.DownloadAsync(stream);
        //            var fileBytes = stream.ToArray();

        //            if (fileBytes.Length == 0)
        //            {
        //                return Result.Failure<(AttachmentsDto, string)>(GoogleDriveErrors.FileNotFound);
        //            }

        //            var attachment = new AttachmentsDto
        //            {
        //                FileId = file.Id,
        //                Extend = Path.GetExtension(file.Name).ToLower(),
        //                Type = file.MimeType,
        //            };
        //            var fileBase64 = Convert.ToBase64String(fileBytes);

        //            return Result.Success((attachment, fileBase64));

        //        }
        //    }
        //    else
        //    {
        //        return Result.Failure<(AttachmentsDto, string)>(GoogleDriveErrors.FileNotFound);
        //    }


        //}
    
    }
}
