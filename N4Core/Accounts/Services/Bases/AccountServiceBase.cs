#nullable disable

using Microsoft.EntityFrameworkCore;
using N4Core.Accounts.Configs;
using N4Core.Accounts.Entities;
using N4Core.Accounts.Enums;
using N4Core.Accounts.Messages;
using N4Core.Accounts.Models;
using N4Core.Accounts.Utils.Bases;
using N4Core.Culture;
using N4Core.Culture.Utils.Bases;
using N4Core.Repositories.Bases;
using N4Core.Responses;
using N4Core.Responses.Bases;
using N4Core.Services.Models;
using System.Security.Claims;

namespace N4Core.Accounts.Services.Bases
{
    public abstract class AccountServiceBase : IDisposable
    {
        public AccountServiceConfig Config { get; private set; }
        public Languages Language { get; private set; }
        public ViewModel ViewModel { get; private set; }
        public AccountMessagesModel Messages { get; private set; }

        protected readonly UnitOfWorkBase _unitOfWork;
        protected readonly RepoBase<AccountUser> _userRepo;
        protected readonly RepoBase<AccountGroup> _groupRepo;
        protected readonly AccountUtilBase _accountUtil;
        protected readonly CultureUtilBase _cultureUtil;

        protected AccountServiceBase(UnitOfWorkBase unitOfWork, RepoBase<AccountUser> userRepo, RepoBase<AccountGroup> groupRepo, 
            AccountUtilBase accountUtil, CultureUtilBase cultureUtil)
        {
            _unitOfWork = unitOfWork;
            _userRepo = userRepo;
            _groupRepo = groupRepo;
            _accountUtil = accountUtil;
            _cultureUtil = cultureUtil;
            Config = new AccountServiceConfig();
            Language = _cultureUtil.GetLanguage();
            ViewModel = new ViewModel(Language);
            Messages = new AccountMessagesModel(Language);
        }

        public void Set(Action<AccountServiceConfig> config)
        {
            config.Invoke(Config);
            Language = Config.Language.HasValue ? Config.Language.Value : _cultureUtil.GetLanguage();
            ViewModel = new ViewModel(Language);
            Messages = new AccountMessagesModel(Language);
        }

        public virtual async Task<Response<AccountUserModel>> GetUser(string userName, string password, CancellationToken cancellationToken = default)
        {
            var existingUser = await _userRepo.Query().Include(q => q.Role).SingleOrDefaultAsync(q => q.UserName == userName && q.Password == password && q.IsActive, cancellationToken);
            if (existingUser is null)
                return new ErrorResponse<AccountUserModel>(Messages.UserNotFound);
            var userModel = new AccountUserModel()
            {
                Guid = existingUser.Guid,
                Id = existingUser.Id,
                UserName = existingUser.UserName,
                RoleNames = new List<string>() { existingUser.Role.RoleName }
            };
            return new SuccessResponse<AccountUserModel>(Messages.UserFound, userModel);
        }

        public virtual async Task<Response<AccountUserModel>> GetUser(AccountLoginModel model, CancellationToken cancellationToken = default) => 
            await GetUser(model.UserName, model.Password, cancellationToken);

        public virtual async Task<Response<ClaimsPrincipal>> GetPrincipal(AccountLoginModel model, CancellationToken cancellationToken = default)
        {
            var response = await GetUser(model, cancellationToken);
            if (response.IsSuccessful)
                return new SuccessResponse<ClaimsPrincipal>(response.Message, _accountUtil.GetPrincipal(response.Data, Config.AuthenticationScheme));
            return new ErrorResponse<ClaimsPrincipal>(response.Message);
        }

        public virtual async Task<Response> RegisterUser(AccountRegisterModel model, CancellationToken cancellationToken = default)
        {
            if (await _userRepo.Query().AnyAsync(q => q.UserName == model.UserName.Trim(), cancellationToken))
                return new ErrorResponse(Messages.UserFoundWithSameUserName);
            var entity = new AccountUser()
            {
                UserName = model.UserName.Trim(),
                Password = model.Password.Trim(),
                EMail = model.EMail?.Trim(),
                IsActive = true,
                RoleId = (int)Roles.User,
                GroupUsers = model.GroupIds?.Select(gId => new AccountGroupUser()
                {
                    GroupId = gId
                }).ToList()
            };
            _userRepo.Create(entity);
            await _unitOfWork.SaveAsync(cancellationToken);
            return new SuccessResponse(Messages.UserRegistered);
        }

        public virtual async Task<List<AccountGroupModel>> GetGroups(CancellationToken cancellationToken = default)
        {
            return await _groupRepo.Query().OrderBy(s => s.GroupName).Select(s => new AccountGroupModel()
            {
                Id = s.Id,
                Guid = s.Guid,
                GroupName = s.GroupName
            }).ToListAsync(cancellationToken);
        }

        public void Dispose()
        {
            _userRepo.Dispose();
            _unitOfWork.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
