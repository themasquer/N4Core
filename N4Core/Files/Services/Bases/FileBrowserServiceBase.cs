#nullable disable

using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using N4Core.Culture;
using N4Core.Culture.Utils.Bases;
using N4Core.Files.Configs;
using N4Core.Files.Entities;
using N4Core.Files.Enums;
using N4Core.Files.Messages;
using N4Core.Files.Models;
using N4Core.Mappers.Utils.Bases;
using N4Core.Repositories.Bases;
using N4Core.Responses.Bases;
using N4Core.Services.Bases;
using N4Core.Session.Utils.Bases;
using N4Core.Types.Extensions;
using System.Text;

namespace N4Core.Files.Services.Bases
{
    public abstract class FileBrowserServiceBase : CrudServiceBase<FileBrowserItem, FileBrowserItemModel>
    {
        public FileBrowserServiceConfig Config { get; protected set; }

        protected string _filterSessionKey;

        protected FileBrowserServiceBase(UnitOfWorkBase unitOfWork, RepoBase<FileBrowserItem> repo, CultureUtilBase cultureUtil, SessionUtilBase sessionUtil,
            MapperUtilBase<FileBrowserItem, FileBrowserItemModel, FileBrowserItemModel> mapperUtil)
            : base(unitOfWork, repo, cultureUtil, sessionUtil, mapperUtil)
        {
            _filterSessionKey = "FileBrowserFilterSessionKey";
            Config = new FileBrowserServiceConfig();
            Messages = new FileBrowserMessagesModel(Language);
        }

        public void Set(Action<FileBrowserServiceConfig> config)
        {
            config.Invoke(Config);
            Set(language: Config.Language, usePageSession: Config.UsePageSession, recordsPerPageCounts: Config.RecordsPerPageCounts);
            Messages = new FileBrowserMessagesModel(Language);
        }

        public virtual async Task<Response> SyncFileBrowserItemsWithDatabase(bool removeExisting = false, CancellationToken cancellationToken = default)
        {
            List<FileBrowserItemModel> items = await GetFileBrowserItems(false, false, cancellationToken);
            List<FileBrowserItem> entities = await _repo.Query().OrderBy(q => q.Path).ToListAsync(cancellationToken);
            FileBrowserItem fileEntity;
            int count = 0;
            if (removeExisting)
            {
                // remove all entities
                _repo.Delete();
                entities.Clear();
            }
            else
            {
                // remove entities
                foreach (FileBrowserItem entity in entities)
                {
                    if (!items.Any(i => i.Path == entity.Path))
                    {
                        _repo.Delete(entity);
                        count++;
                    }
                }
            }
            // add or update entities
            foreach (FileBrowserItemModel item in items)
            {
                if (!entities.Any(e => e.Path == item.Path))
                {
                    _repo.Create(_mapperUtil.Map(item));
                    count++;
                }
                else
                {
                    fileEntity = entities.SingleOrDefault(e => e.Path == item.Path && e.Extension != null);
                    if (fileEntity is not null && (fileEntity.Length != item.Length || fileEntity.CreateDate != item.CreateDate))
                    {
                        fileEntity.CreateDate = item.CreateDate;
                        fileEntity.Length = item.Length;
                        _repo.Update(fileEntity);
                        count++;
                    }
                }
            }
            if (count > 0)
                await _unitOfWork.SaveAsync(cancellationToken);
            return Success(removeExisting ? (Messages as FileBrowserMessagesModel).ResetSuccessfully : $"{count} {(Messages as FileBrowserMessagesModel).SyncedSuccessfully}");
        }

        protected virtual async Task<List<FileBrowserItemModel>> GetFileBrowserItems(bool fromDatabase, bool includeFileContents, CancellationToken cancellationToken = default)
        {
            List<FileBrowserItemModel> items;
            FileBrowserItemModel item;
            DirectoryInfo directory;
            FileInfo file;
            List<string> entries;
            string extension;
            if (fromDatabase)
            {
                items = await Query().OrderBy(q => q.Path).ToListAsync(cancellationToken);
                if (includeFileContents)
                {
                    for (int i = 0; i < items.Count; i++)
                    {
                        extension = items[i].Extension;
                        if (!string.IsNullOrWhiteSpace(extension) && Config.TextFiles.ContainsKey(extension))
                            items[i].Content = File.ReadAllText(items[i].Path, Encoding.GetEncoding(1254));
                    }
                }
            }
            else
            {
                items = new List<FileBrowserItemModel>();
                entries = Config.GetDirectoriesAndFiles();
                foreach (string entry in entries)
                {
                    extension = GetExtension(entry).ToLower();
                    if (!string.IsNullOrWhiteSpace(extension) && (Config.TextFiles.ContainsKey(extension) || Config.ImageFiles.ContainsKey(extension) || Config.OtherFiles.ContainsKey(extension))) // file
                    {
                        item = new FileBrowserItemModel();
                        file = new FileInfo(entry);
                        item.CreateDate = file.LastWriteTime;
                        item.Extension = extension;
                        item.ParentPath = file.Directory.FullName.Remove(0, file.Directory.FullName.IndexOf(Config.DirectoryPath));
                        item.Path = file.FullName.Remove(0, file.FullName.IndexOf(Config.DirectoryPath));
                        item.Length = file.Length;
                        item.Title = file.Name;
                        item.Level = item.Path.Remove(0, Config.DirectoryPath.Length + 1).Split("\\").Count();
                        if (includeFileContents && Config.TextFiles.ContainsKey(item.Extension))
                        {
                            item.Content = File.ReadAllText(item.Path, Encoding.GetEncoding(1254));
                        }
                        items.Add(item);
                    }
                    else // directory
                    {
                        item = new FileBrowserItemModel();
                        directory = new DirectoryInfo(entry);
                        item.CreateDate = directory.LastWriteTime;
                        item.ParentPath = directory.Parent.FullName.Remove(0, directory.Parent.FullName.IndexOf(Config.DirectoryPath));
                        item.Path = directory.FullName.Remove(0, directory.FullName.IndexOf(Config.DirectoryPath));
                        item.Title = directory.Name;
                        item.Level = item.Path.Remove(0, Config.DirectoryPath.Length + 1).Split("\\").Count();
                        items.Add(item);
                    }
                }
            }
            return items;
        }

        public virtual async Task<FileBrowserModel> GetContents(FileBrowserModel model, CancellationToken cancellationToken = default)
        {
            UpdateFilterSession(model);
            model.Operation = FileBrowserOperations.Home;
            model.StartLink = Config.StartLink;
            List<FileBrowserItemModel> items;
            if (!Config.HasDirectories)
                return model;
            items = await GetFileBrowserItems(Config.Database, model.Filter.Find && model.Filter.HasExpression, cancellationToken);
            if (items.Any())
            {
                UpdateFile(model, items);
                if (model.HasFileContent)
                {
                    model.Operation = FileBrowserOperations.Content;
                    if (model.Filter.HasExpression)
                        model.FileContentType = Config.TextFiles[".txt"];
                }
                else if (model.Filter.Find && (model.Filter.HasExpression || (model.HasPath && model.Path != model.StartLink)))
                {
                    model.Operation = FileBrowserOperations.Filter;
                    UpdateFilteredItems(model, items);
                }
                else
                {
                    if (model.Filter.Find)
                        model.Path = model.StartLink;
                    if (model.HasPath)
                    {
                        model.Operation = FileBrowserOperations.Content;
                        UpdateDirectories(model, items);
                        UpdateFiles(model, items);
                    }
                }
                UpdateFirstLevelPaths(model, items);
                model.HierarchicalDirectoryLinks = AddHierarchicalLinks(model.Path, items);
                if (model.FileType != FileTypes.Other)
                    model.Title = AddLinks(model.Path);
            }
            return model;
        }

        protected virtual void UpdateFilterSession(FileBrowserModel model)
        {
            FileBrowserFilterModel filter = _sessionUtil.Get<FileBrowserFilterModel>(_filterSessionKey);
            bool filterSession = filter is not null && model.Filter is not null && model.Filter.FilterSession;
            if (!filterSession)
            {
                if (model.Filter is null)
                    model.Filter = new FileBrowserFilterModel();
                model.Filter.PageNumber = model.PageNumber;
                model.Filter.RecordsPerPageCount = model.RecordsPerPageCount;
                model.Filter.Language = Language;
                model.Filter.PlaceHolder = Language == Languages.Türkçe ? "İfade giriniz..." : "Enter expression...";
                if (model.Filter.HasExpression && model.Filter.Expression.Length < 2)
                {
                    model.Filter.PlaceHolder = Language == Languages.Türkçe ? "İfade en az 2 harf olmalıdır!" : "Expression must be minimum 2 letters!";
                    model.Filter.Expression = string.Empty;
                }
                model.Filter.Path = model.Path;
                if (model.Filter.Find)
                {
                    if (model.Filter.Paging && filter is not null)
                    {
                        model.Filter.Path = filter.Path;
                        model.Path = model.Filter.Path;
                    }
                }
                else
                {
                    if (model.Filter.FindInCode && filter is not null)
                        model.Filter.Path = filter.Path;
                }
                _sessionUtil.Set(model.Filter, _filterSessionKey);
            }
            else
            {
                filter.Find = model.Filter.Find;
                model.Filter = filter;
                model.PageNumber = filter.PageNumber;
                model.RecordsPerPageCount = filter.RecordsPerPageCount;
                if (model.Filter.Find)
                    model.Path = model.Filter.Path;
            }
        }

        protected virtual void UpdateFirstLevelPaths(FileBrowserModel model, List<FileBrowserItemModel> items)
        {
            model.FirstLevelPath = GetFirstLevelPath(model.Path);
            model.FirstLevelPaths = items.Where(i => i.Extension == null && i.Level == 1).Select(i => new SelectListItem()
            {
                Value = GetPath(i.Path),
                Text = i.Title
            }).ToList();
        }

        protected string GetFirstLevelPath(string path)
        {
            path = string.IsNullOrWhiteSpace(path) ? Config.StartLink : path;
            string firstLevelPath = path;
            string[] pathItems = path.Split("\\");
            if (pathItems.Length >= 2)
                firstLevelPath = pathItems[0] + "\\" + pathItems[1];
            return firstLevelPath;
        }

        protected virtual string AddLinks(string path)
        {
            path = path ?? Config.StartLink;
            string[] pathItems = path.Split("\\");
            string link;
            string linkItem;
            List<string> linkItems = new List<string>();
            for (int i = 0; i < pathItems.Length; i++)
            {
                if (!string.IsNullOrWhiteSpace(pathItems[i]))
                {
                    linkItem = string.Empty;
                    for (int j = 0; j <= i; j++)
                    {
                        linkItem += pathItems[j] + "\\";
                    }
                    linkItem = linkItem.TrimEnd('\\');
                    link = "<a " + Config.AtagStyleUnderline + " " + Config.AtagHref + linkItem + "\">" + pathItems[i] + "</a>";
                    linkItems.Add(link);
                }
            }
            return string.Join("\\", linkItems);
        }

        protected virtual string AddLink(string path, bool useBadgesForFolders = true)
        {
            path = path ?? Config.StartLink;
            string[] pathItems = path.Split("\\");
            string link = string.Empty;
            string span;
            if (pathItems.Any())
            {
                link = "<a class=\"pathlink\" " + Config.AtagStyleNone + " " + Config.AtagHref + path + "&filter.filtersession=true\">";
                for (int i = 0; i < pathItems.Length; i++)
                {
                    if (!string.IsNullOrWhiteSpace(pathItems[i]))
                    {
                        if (i < pathItems.Length - 1)
                        {
                            if (useBadgesForFolders)
                                span = "<span class=\"badge bg-dark\">" + pathItems[i] + "</span>&nbsp;\\&nbsp;";
                            else
                                span = pathItems[i] + "\\";
                        }
                        else
                        {
                            span = pathItems[i];
                        }
                        link += span;
                    }
                }
                link += "</a>";
            }
            return link;
        }

        protected virtual string GetWwwrootPath(string path, bool includeFile = true)
        {
            string wwwrootPath = Config.DirectoryPath;
            string extension;
            path = path ?? Config.StartLink;
            if (path != Config.StartLink)
            {
                wwwrootPath += path.Remove(0, Config.StartLink.Length);
                if (!includeFile)
                {
                    extension = GetExtension(wwwrootPath);
                    if (!string.IsNullOrWhiteSpace(extension) && (Config.TextFiles.ContainsKey(extension) || Config.ImageFiles.ContainsKey(extension) || Config.OtherFiles.ContainsKey(extension)))
                        wwwrootPath = wwwrootPath.Remove(wwwrootPath.LastIndexOf("\\"));
                }
            }
            return wwwrootPath;
        }

        protected virtual void UpdateFile(FileBrowserModel model, List<FileBrowserItemModel> items)
        {
            string wwwrootPath = GetWwwrootPath(model.Path);
            FileBrowserItemModel item = items.SingleOrDefault(q => q.Path == wwwrootPath && q.Extension != null);
            if (item is not null)
            {
                if (Config.TextFiles.ContainsKey(item.Extension) && !model.Filter.Find)
                {
                    model.FileContentType = Config.TextFiles[item.Extension];
                    model.FileType = FileTypes.Text;
                    model.FileCodeContent = File.ReadAllText(wwwrootPath, Encoding.GetEncoding(1254));
                    model.FileContent = model.FileCodeContent;
                    if (model.Filter.HasExpression)
                    {
                        model.FileContent = model.FileCodeContent.Find(model.Filter.Expression, model.Filter.MatchCase, model.Filter.MatchWord);
                        if (model.FileContent is null)
                            model.FileContent = Language == Languages.English ? "Expression not found!" : "İfade bulunamadı!";
                    }
                }
                else if (Config.ImageFiles.ContainsKey(item.Extension) && !model.Filter.Find)
                {
                    model.FileContentType = Config.ImageFiles[item.Extension];
                    model.FileType = FileTypes.Image;
                    model.FileBinaryContent = File.ReadAllBytes(wwwrootPath);
                    model.FileContent = "data:" + model.FileContentType + ";base64," + Convert.ToBase64String(model.FileBinaryContent);
                }
                else if (Config.OtherFiles.ContainsKey(item.Extension) && !model.Filter.Find)
                {
                    model.FileContentType = Config.OtherFiles[item.Extension];
                    model.FileType = FileTypes.Other;
                    model.FileBinaryContent = File.ReadAllBytes(wwwrootPath);
                    model.Title = item.Title;
                }
            }
        }

        protected virtual void UpdateDirectories(FileBrowserModel model, List<FileBrowserItemModel> items)
        {
            string wwwrootPath = GetWwwrootPath(model.Path);
            model.Contents = items.Where(i => i.ParentPath == wwwrootPath && i.Extension == null).Select(i => new FileBrowserItemModel()
            {
                Title = i.Title,
                Folders = (model.Path ?? Config.StartLink) + "\\" + i.Title,
            }).ToList();
        }

        protected virtual void UpdateFiles(FileBrowserModel model, List<FileBrowserItemModel> items)
        {
            string wwwrootPath = GetWwwrootPath(model.Path);
            model.Contents?.AddRange(items.Where(i => i.ParentPath == wwwrootPath && i.Extension != null).Select(i => new FileBrowserItemModel()
            {
                Title = i.Title,
                Folders = (model.Path ?? Config.StartLink) + "\\" + i.Title,
                IsFile = true
            }).ToList());
        }

        protected virtual void UpdateFilteredItems(FileBrowserModel model, List<FileBrowserItemModel> items)
        {
            model.FilteredItems = items.Where(i => i.Extension != null && i.Path.StartsWith(GetWwwrootPath(model.Path, false)) &&
                Config.TextFiles.ContainsKey(i.Extension)).ToList();
            if (model.Filter.HasExpression)
            {
                model.FilteredItems = model.FilteredItems.Where(i => i.Content.Find(model.Filter.Expression, model.Filter.MatchCase, model.Filter.MatchWord) is not null).ToList();
            }
            model.OperationMessage = Messages.RecordNotFound;
            if (model.FilteredItems.Any())
            {
                model.FilteredItems = Paginate(model.FilteredItems.Select(i => new FileBrowserItemModel()
                {
                    Title = AddLink(GetPath(i.Path))
                }).ToList(), model);
                model.OperationMessage = model.TotalRecordsCount == 1 ? model.TotalRecordsCount + " " + Messages.RecordFound : model.TotalRecordsCount + " " + Messages.RecordsFound;
            }
        }

        protected virtual string AddHierarchicalLinks(string path, List<FileBrowserItemModel> items)
        {
            string ulTag = string.Empty;
            string wwwrootPath = GetWwwrootPath(path, false);
            List<FileBrowserItemModel> directories = items.Where(i => i.Extension == null && i.Level <= (Config.HideHierarchicalDirectoryAfterLevel ?? byte.MaxValue)).ToList();
            if (directories.Any())
            {
                for (int i = 0; i < directories.Count; i++)
                {
                    if (i == 0)
                    {
                        ulTag += $"<ul><li>";
                    }
                    else
                    {
                        if (directories[i].Level < directories[i - 1].Level)
                        {
                            for (int j = directories[i].Level; j < directories[i - 1].Level + 1; j++)
                            {
                                ulTag += "</ul>";
                            }
                        }
                        if (directories[i].Level == directories[i - 1].Level)
                        {
                            ulTag += $"<li{(wwwrootPath.StartsWith(directories[i].Path) ? " " + Config.LiClassCurrent : "")}>";
                        }
                        else
                        {
                            ulTag += $"<ul><li{(wwwrootPath.StartsWith(directories[i].Path) ? " " + Config.LiClassCurrent : "")}>";
                        }
                    }
                    ulTag += $"<a {(wwwrootPath.StartsWith(directories[i].Path) ? Config.AtagStyleUnderline : Config.AtagStyleNone)} {Config.AtagHref + GetPath(directories[i].Path)}\">" +
                        $"{(wwwrootPath.StartsWith(directories[i].Path) ? "<b>" + directories[i].Title + "</b>" : directories[i].Title)}</a></li>";
                }
                ulTag += "</ul>";
            }
            return ulTag;
        }

        protected virtual string GetPath(string wwwrootPath)
        {
            wwwrootPath = wwwrootPath ?? Config.DirectoryPath;
            return wwwrootPath != Config.DirectoryPath ? Config.StartLink + wwwrootPath.Remove(0, Config.DirectoryPath.Length) : Config.StartLink;
        }

        protected virtual string GetExtension(string path)
        {
            string extension = string.Empty;
            path = path ?? Config.StartLink;
            string[] pathItems = path.Split("\\");
            if (pathItems.Length > 1)
            {
                extension = Path.GetExtension(pathItems.Last());
            }
            return extension;
        }
    }
}
