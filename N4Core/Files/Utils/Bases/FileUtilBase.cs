#nullable disable

using Microsoft.AspNetCore.Http;
using N4Core.Files.Bases;
using N4Core.Files.Managers;
using N4Core.Files.Models;
using N4Core.Files.Models.Bases;
using N4Core.Records.Bases;

namespace N4Core.Files.Utils.Bases
{
    public abstract class FileUtilBase : DirectoryManager
    {
        protected char _acceptedExtensionsSeperator = ',';
        protected string _acceptedExtensions = ".jpg, .jpeg, .png";
        protected double _acceptedLengthInMegaBytes = 1;

        public void Set(double acceptedLengthInMegaBytes, string acceptedExtensions, params string[] fileDirectories)
        {
            _acceptedLengthInMegaBytes = acceptedLengthInMegaBytes;
            _acceptedExtensions = acceptedExtensions;
            SetDirectories(fileDirectories);
        }

        public virtual void UpdateFile(IFormFile formFile, RecordFile file)
        {
            file.FileContent = string.Empty;
            file.FilePath = null;
            file.FileData = null;
            if (formFile is not null)
            {
                if (HasDirectories)
                {
                    file.FilePath = "/" + string.Join("/", Directories) + "/";
                }
                else
                {
                    using (MemoryStream memoryStream = new MemoryStream())
                    {
                        formFile.CopyTo(memoryStream);
                        file.FileData = memoryStream.ToArray();
                    }
                }
                file.FileContent = Path.GetExtension(formFile.FileName).ToLower();
            }
        }

        public virtual bool? CheckFile(IFormFile formFile)
        {
            bool? result = null;
            if (formFile is not null && !string.IsNullOrWhiteSpace(_acceptedExtensions))
            {
                string fileExtension = Path.GetExtension(formFile.FileName);
                string[] acceptedFileExtensionsArray = _acceptedExtensions.Split(_acceptedExtensionsSeperator);
                foreach (string acceptedFileExtensionsItem in acceptedFileExtensionsArray)
                {
                    if (acceptedFileExtensionsItem.Trim().Equals(fileExtension.Trim(), StringComparison.OrdinalIgnoreCase))
                    {
                        result = true;
                        break;
                    }
                }
                if (result == true)
                {
                    if (formFile.Length > _acceptedLengthInMegaBytes * Math.Pow(1024, 2))
                        result = false;
                }
            }
            return result;
        }

        public virtual void UpdateImgSrc<TRecord>(TRecord record) where TRecord : Record, new()
        {
            RecordFileModel fileModel;
            RecordFile file;
            if (record is RecordFile)
            {
                file = record as RecordFile;
                fileModel = record as RecordFileModel;
                fileModel.FileImgSrc = GetImgSrc(file);
            }
        }

        public virtual void UpdateImgSrc<TRecord>(List<TRecord> records) where TRecord : Record, new()
        {
            foreach (var record in records)
            {
                UpdateImgSrc(record);
            }
        }

        public virtual void UploadFile(IFormFile formFile, RecordFile file)
        {
            SaveFile(formFile, file);
        }

        public virtual string GetImgSrc(RecordFile file)
        {
            string imgSrc = string.Empty;
            if (file is not null)
            {
                if (file.FileData is not null && !string.IsNullOrWhiteSpace(file.FileContent))
                    imgSrc = GetContentType(file.FileContent) + Convert.ToBase64String(file.FileData);
                else if (file.Id != 0 && !string.IsNullOrWhiteSpace(file.FileContent) && !string.IsNullOrWhiteSpace(file.FilePath))
                    imgSrc = file.FilePath + file.Id + file.FileContent;
            }
            return imgSrc;
        }

        public virtual void SaveFile(IFormFile formFile, RecordFile file)
        {
            if (formFile is not null && HasDirectories)
            {
                using (FileStream fileStream = new FileStream(Path.Combine(CreatePath(file.Id + file.FileContent)), FileMode.Create))
                {
                    formFile.CopyTo(fileStream);
                }
            }
        }

        public virtual void DeleteFile(int id)
        {
            if (DirectoryPath != string.Empty)
            {
                var filePath = Directory.GetFiles(DirectoryPath).SingleOrDefault(file => Path.GetFileNameWithoutExtension(file).Equals(id.ToString()));
                if (!string.IsNullOrWhiteSpace(filePath) && File.Exists(filePath))
                    File.Delete(filePath);
            }
        }

        public virtual void DeleteFile<TRecord>(TRecord record) where TRecord : Record, new()
        {
            if (record is not null && record is RecordFile)
            {
                var file = record as RecordFile;
                file.FileData = null;
                file.FileContent = null;
                file.FilePath = null;
                DeleteFile(record.Id);
            }
        }

        public virtual string GetContentType(string fileNameOrExtension, bool includeData = true, bool inclueBase64 = true)
        {
            if (string.IsNullOrWhiteSpace(fileNameOrExtension))
                return string.Empty;
            Dictionary<string, string> mimeTypes = new Dictionary<string, string>
            {
                { ".txt", "text/plain" },
                { ".pdf", "application/pdf" },
                { ".doc", "application/vnd.ms-word" },
                { ".docx", "application/vnd.ms-word" },
                { ".xls", "application/vnd.ms-excel" },
                { ".xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" },
                { ".csv", "text/csv" },
                { ".png", "image/png" },
                { ".jpg", "image/jpeg" },
                { ".jpeg", "image/jpeg" },
                { ".gif", "image/gif" }
            };
            string contentType;
            string fileExtension = Path.GetExtension(fileNameOrExtension).ToLower();
            contentType = mimeTypes[fileExtension];
            if (includeData)
                contentType = "data:" + contentType;
            if (inclueBase64)
                contentType = contentType + ";base64,";
            return contentType;
        }

        public virtual FileToDownloadModel GetFile(int entityId, string fileToDownloadFileNameWithoutExtension = null, bool useOctetStreamContentType = false)
        {
            FileToDownloadModel file = null;
            if (DirectoryPath != string.Empty)
            {
                string fileNameWithoutPath = GetFileNameWithoutPath(entityId.ToString(), DirectoryPath);
                if (string.IsNullOrWhiteSpace(fileNameWithoutPath))
                    return null;
                string fileExtension = Path.GetExtension(fileNameWithoutPath);
                file = new FileToDownloadModel()
                {
                    FileStream = new FileStream(Path.Combine(DirectoryPath, fileNameWithoutPath), FileMode.Open),
                    FileContentType = useOctetStreamContentType ? "application/octet-stream" : GetContentType(fileNameWithoutPath, false, false),
                    FileName = string.IsNullOrWhiteSpace(fileToDownloadFileNameWithoutExtension) ? entityId + fileExtension : fileToDownloadFileNameWithoutExtension + fileExtension
                };
            }
            return file;
        }

        public virtual string GetFileNameWithoutPath(string fileNameWithoutExtension, string filePath)
        {
            string[] files = Directory.GetFiles(filePath);
            if (files is null || files.Length == 0)
                return null;
            string file = files.FirstOrDefault(f => Path.GetFileNameWithoutExtension(f) == fileNameWithoutExtension);
            if (file is null)
                return null;
            return Path.GetFileName(file);
        }

        public virtual string CreatePath(string fileName)
        {
            if (DirectoryPath != string.Empty)
                return DirectoryPath + @"\" + fileName;
            return string.Empty;
        }
    }
}
