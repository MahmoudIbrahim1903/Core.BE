//using System.Collections.Generic;
//using IdentityServer4.Services;
//using IdentityServer4.Stores;
//using Microsoft.AspNetCore.Identity;
//using Microsoft.AspNetCore.Mvc;
//using Emeint.Core.BE.Identity.Domain.Model;
//using Emeint.Core.BE.Identity.Services;
//using Microsoft.Extensions.Logging;
//using System.Linq;
//using System.Threading.Tasks;
//using Emeint.Core.BE.Domain.Exceptions;
//using Emeint.Core.BE.Identity.Domain.Exceptions;
//using Emeint.Core.BE.Identity.Infrastructure.Repositories;
//using Emeint.Core.BE.API.Infrastructure.Services;
//using Emeint.Core.BE.Media.Domain.Models;
//using Emeint.Core.BE.API.Application.ValueObjects.ViewModels;
//using Emeint.Core.BE.Domain.Enums;
//using Emeint.Core.BE.Identity.API.Application.ValueObjects.ViewModel.Common.AccountViewModel;
//using Emeint.Core.BE.Identity.Domain.Enums;
//using Emeint.Core.BE.Identity.Infrastructure.Data;
//using InvalidOperationException = Emeint.Core.BE.Domain.Exceptions.InvalidOperationException;
//using System;
//using Microsoft.AspNetCore.Mvc.ModelBinding;
//using Emeint.Core.BE.Identity.Domain.Configurations;
//using Emeint.Core.BE.Utilities;
//using Emeint.Core.BE.Identity.Infrastructure.Services.Contracts;
//using Emeint.Core.BE.Identity.Resources;
//using Emeint.Core.BE.Identity.API.Application.ValueObjects.ViewModel.Common.ApplicationVersion;

//namespace Emeint.Core.BE.Identity.API.Controllers.Common
//{
//    public class CommonAccountManager
//    {
//        #region Fields   

//        //private readonly InMemoryUserLoginService _loginService;
//        private readonly ILoginService<ApplicationUser> _loginService;
//        private readonly IClientStore _clientStore;
//        private readonly ILogger<CommonAccountManager> _logger;
//        private readonly UserManager<ApplicationUser> _userManager;
//        private readonly RoleManager<ApplicationRole> _roleManager;
//        private readonly IIdentityConfigurationManager _configurationManager;
//        private readonly ICountryService _countryService;
//        private readonly IProfileRepository _profileRepository;
//        private readonly IApplicationVersionRepository _applicationVersionRepository;
//        private readonly IImageRepository _imageRepository;
//        private readonly IIdentityService _identityService;
//        private readonly string _typeName;
//        private readonly ApplicationDbContext _context;
//        #endregion

//        #region Constructor

//        public CommonAccountManager(
//            //InMemoryUserLoginService loginService,
//            ILoginService<ApplicationUser> loginService,
//            IClientStore clientStore,
//            ILogger<CommonAccountManager> logger,
//            UserManager<ApplicationUser> userManager,
//            RoleManager<ApplicationRole> roleManager,
//            ICountryService countryService,
//            IProfileRepository profileRepository, IApplicationVersionRepository applicationVersionRepository, IImageRepository imageRepository,
//             IIdentityConfigurationManager configurationManager,
//            IIdentityService identityService, ApplicationDbContext context)
//        {
//            _loginService = loginService;
//            _clientStore = clientStore;
//            _logger = logger;
//            _userManager = userManager;
//            _roleManager = roleManager;
//            _configurationManager = configurationManager;
//            _countryService = countryService;
//            _profileRepository = profileRepository;
//            _applicationVersionRepository = applicationVersionRepository;
//            _imageRepository = imageRepository;
//            _identityService = identityService;
//            _typeName = GetType().Name;
//            _context = context;
//        }

//        #endregion

//        #region Methods
//        /*  public async Task CheckSendingBlockInterval(ApplicationUser user, SendingBlockIntervalType sendingBlockIntervalType)
//          {
//              bool hasBlockInterval = false;
//              DateTime? lastSentDate = null;
//              int sendingBlockInterval = 0;

//              if (sendingBlockIntervalType == SendingBlockIntervalType.Email)
//              {
//                  hasBlockInterval = int.TryParse(_configurationManager.GetSendEmailBlockIntervalByMinutes(), out sendingBlockInterval);
//                  lastSentDate = user.LastSentEmailDate;
//              }
//              else if (sendingBlockIntervalType == SendingBlockIntervalType.Sms)
//              {
//                  hasBlockInterval = int.TryParse(_configurationManager.GetSendSmsBlockIntervalByMinutes(), out sendingBlockInterval);
//                  lastSentDate = user.LastSentSmsDate;
//              }


//              if (hasBlockInterval && sendingBlockInterval != 0 && lastSentDate != null)
//              {
//                  if ((DateTime.UtcNow - lastSentDate).Value.TotalMinutes < sendingBlockInterval)
//                  {
//                      // throw new InvalidSendingIntervalException(_configurationManager.GetSendSmsBlockIntervalByMinutes());
//                  }
//              }
//          }

//          public async Task UpdateUserLastSentStatus(ApplicationUser user, SendingBlockIntervalType sendingBlockIntervalType)
//          {
//              if (sendingBlockIntervalType == SendingBlockIntervalType.Sms)
//                  user.LastSentSmsDate = DateTime.UtcNow;

//              else if (sendingBlockIntervalType == SendingBlockIntervalType.Email)
//                  user.LastSentEmailDate = DateTime.UtcNow;

//              await _userManager.UpdateAsync(user);
//          }   */


//        public async Task<int> ForgotPassword(ForgotPasswordViewModel forgot_password, ModelStateDictionary modelState)
//        {

//            if (!modelState.IsValid)
//                throw new InvalidParameterException(nameof(forgot_password), "", GetErrorsFromModelState(modelState));

//            forgot_password.Email = System.Net.WebUtility.UrlDecode(forgot_password.Email);

//            var user = await _userManager.FindByNameAsync(forgot_password.Email);

//            if (user == null)
//            {
//                var phoneNumber = PhoneNumberUtility.ValidateAndAddCountryDialCode(forgot_password.Email, "20", 11, "EGY");// TODO get country code from header
//                user = _userManager.Users.FirstOrDefault(u => u.PhoneNumber == phoneNumber);
//            }
//            if (user == null)
//                throw new UserNotFoundException();


//            PhoneNumberVerification phoneNumberVerification = _configurationManager.GetAccountVerificationPhoneVerification();
//            if (phoneNumberVerification == PhoneNumberVerification.SignUp && !user.PhoneNumberConfirmed)
//                throw new PhoneNumberVerificationRequiredException();

//            EmailVerification emailVerification = _configurationManager.GetAccountVerificationEmailVerification();
//            if (emailVerification == EmailVerification.SignUp && !user.EmailConfirmed)
//                throw new EmailVerificationRequiredException();

//            //Generate random strong password
//            var newPassword = GeneratePasswordUtility.Generate();

//            //Check user has password
//            if (user.PasswordHash != null)
//            {
//                //Remove user current password
//                var removePasswordResult = await _userManager.RemovePasswordAsync(user);

//                //remove password failed
//                if (removePasswordResult.Errors.Any())
//                    throw new ResetPasswordFailedException(forgot_password.Email, GetErrorsFromIdentityResult(removePasswordResult));
//            }

//            //add new password with new random password
//            var addPasswordResult = await _userManager.AddPasswordAsync(user, newPassword);

//            //add new password failed
//            if (addPasswordResult.Errors.Any())
//                throw new ResetPasswordFailedException(forgot_password.Email, GetErrorsFromIdentityResult(addPasswordResult));

//            if (user.PhoneNumberConfirmed == false)
//                throw new PhoneNumberVerificationRequiredException();

//            /*  try
//              {
//                  user.ChangePasswordIsRequired = true;
//                  await _userManager.UpdateAsync(user);
//              }
//              catch (Exception exception)
//              {
//                  _logger.LogError("An error has occured while Forgot Password , please try again later : Exception : ", exception.Message);
//              }    

//              await CheckSendingBlockInterval(user, SendingBlockIntervalType.Sms); */
//            //send sms with new generated password
//            string userPhoneNumber = "";

//            // SMSProvider currentSmsProvider = SMSProvider.VictoryLink;
//            var userCountry = _countryService.GetCountryByCode(user.CountryCode);
//            if (userCountry == null)
//                throw new InvalidParameterException("country_code", user.CountryCode);
//            userPhoneNumber = PhoneNumberUtility.ValidateAndRemoveCountryDialCode(user.PhoneNumber, userCountry.InternationalDialCode, userCountry.PhoneNumberLength, userCountry.Code);

//            /* if (((user.CountryCode == "EGY" && user.LastSentSmsDate.HasValue) || (user.CountryCode == "ARE"))
//                 && enableSendingSmsByTwilio)
//             {
//                 //currentSmsProvider = SMSProvider.Twilio;
//                 userPhoneNumber = user.PhoneNumber;
//             } */

//            var smsMessageParameters = new Dictionary<string, string>();
//            smsMessageParameters.Add("new_password", newPassword);
//            //var smsResult = await _smsHandler.SendSmsAsync(new SmsViewModel
//            //{
//            //    CreatedBy = user.Email,
//            //    PhoneNumbers = new List<string> { userPhoneNumber },
//            //    MessageParameters = smsMessageParameters,
//            //    SmsProvider = currentSmsProvider,
//            //    TemplateCode = SmsMessageTemplates.ResetPassword,
//            //    Language = _identityService.Language.ToString()
//            //}, true);

//            /* await UpdateUserLastSentStatus(user, SendingBlockIntervalType.Sms);     */

//            return 0;//smsResult.ErrorCode;
//        }
//        public string GetErrorsFromModelState(ModelStateDictionary modelState)
//        {
//            List<string> errors = new List<string>();
//            string error = string.Empty;
//            int counter = 0;

//            foreach (var state in modelState)
//            {
//                if (counter == 0)
//                {
//                    if (state.Value.Errors.Count > 0)
//                        error = state.Value.Errors[0].ErrorMessage;
//                }
//                else
//                {
//                    if (state.Value.Errors.Count > 0)
//                        error = " " + state.Value.Errors[0].ErrorMessage;
//                }

//                errors.Add(error);

//                counter++;
//            }

//            return string.Concat(errors);
//        }
//        public async Task ChangePassword(ChangePasswordViewModel change_password, ModelStateDictionary modelState)
//        {
//            if (!modelState.IsValid)
//                throw new InvalidParameterException(nameof(change_password), "", GetErrorsFromModelState(modelState));

//            var user = await _userManager.FindByNameAsync(_identityService.Email);

//            if (user == null)
//                throw new UserNotFoundException();

//            //Change current password with new password
//            var result = await _userManager.ChangePasswordAsync(user, change_password.CurrentPassword, change_password.NewPassword);

//            //Change password failed
//            if (result.Errors.Any())
//                throw new ChangePasswordFailedException(_identityService.Email, GetErrorsFromIdentityResult(result));
//            /* try
//             {
//                 user.ChangePasswordIsRequired = false;
//                 await _userManager.UpdateAsync(user);
//             }
//             catch (Exception exception)
//             {
//                 _logger.LogError("An error has occured while reset password , please try again later : Exception : ", exception.Message);
//             } */
//        }
//        public string GetErrorsFromIdentityResult(IdentityResult identityResult)
//        {
//            // Refer to the list of errors here : IdentityErrorDescriber
//            List<string> errors = new List<string>();
//            foreach (var error in identityResult.Errors)
//            {
//                if (error.Code == "PasswordMismatch")
//                {
//                    switch (_identityService.Language)
//                    {
//                        case Language.en:
//                            error.Description = Resources_En.PasswordMismatch;
//                            break;
//                        case Language.ar:
//                            error.Description = Resources_Ar.PasswordMismatch;
//                            break;
//                        default:
//                            error.Description = Resources_En.PasswordMismatch;
//                            break;
//                    }
//                }
//                errors.Add(error.Description);
//            }
//            return string.Concat(errors);
//        }

//        public LoginResponse Login([FromForm] LoginViewModel login, [FromHeader(Name = "Language")] string language)
//        {
//            return new LoginResponse
//            {
//                error_msg = string.Empty,
//                access_token =
//                    "mock_token_eyJhbGciOiJSUzI1NiIsImtpZCI6IkQ3QTU0NDk0Q0EyM0ZDOTJERjQzNjgxQTUzOTVCQTVFMTJBNTc1RTciLCJ0eXAiOiJKV1QiLCJ4NXQiOiIxNlZFbE1val9KTGZRMmdhVTVXNlhoS2xkZWMifQ.eyJuYmYiOjE1MTc4NDc1NDcsImV4cCI6MTUxNzg1MTE0NywiaXNzIjoibnVsbCIsImF1ZCI6WyJudWxsL3Jlc291cmNlcyIsInZlaGljbGVzIl0sImNsaWVudF9pZCI6InhhbWFyaW4iLCJzdWIiOiJlZmU1YTlhYy0wMjA5LTRjMTktOTFkNC1mM2NiNjZmYjc4MjMiLCJhdXRoX3RpbWUiOjE1MTc4NDc1NDcsImlkcCI6ImxvY2FsIiwicHJlZmVycmVkX3VzZXJuYW1lIjoiYW1hbGFAZW1laW50Lm5ldCIsIm5hbWUiOiJBbWFsIiwibGFzdF9uYW1lIjoiR2FkIiwiZW1haWwiOiJhbWFsYUBlbWVpbnQubmV0IiwiZW1haWxfdmVyaWZpZWQiOnRydWUsInBob25lX251bWJlciI6IjAxMDAyMDIwMTI1IiwicGhvbmVfbnVtYmVyX3ZlcmlmaWVkIjp0cnVlLCJyb2xlIjoiQ2FyT3duZXIiLCJDb3VudHJ5Q29kZSI6IkVHWSIsIlRlbmFudENvZGUiOiJOVC0xIiwic2NvcGUiOlsidmVoaWNsZXMiXSwiYW1yIjpbIlBhc3N3b3JkIl19.UWqMdFlC9rHxnTUaqoKMMfgVPHn7DJ0XRwzw3vF1Wj2I0SC9kyAvS9MBPh3YNub7RqdoaPHKNl9VhFiRm-GGNR4dG-wj40bMMOxCshD_1gq-dfvAln3gE7duWv7G6eMx5_EvSM04e5BUE4BSH9BlS3v4p2bnD8cqzLX5aGDIula3etmeT2dbhJlUmFrYGNsxIOCY9omCT5ydZxK2fqUavTU7lUKTKG_rGFwMWwi0ixPfYQMMvl3EtVqcmR4Qa_K57WgjCFrAFPzo5rvrad76guQ8bZRIkTl2G5iI4DXj2owjcYRC3Fg_bl_fBen41dODjaUEium2taNbGJuTfj3Rbg",
//                expires_in = 3600,
//                token_type = "Bearer"
//            };
//        }

//        //public async Task<List<ApplicationUser>> Search(AdminSearchUsersCriteria searchCriteria, params string[] roles)
//        //{
//        //    if (searchCriteria == null)
//        //        throw new MissingParameterException("searchCriteria");
//        //    List<IdentityRole> identityRoles = new List<IdentityRole>();
//        //    foreach (var item in roles)
//        //    {
//        //        var identityRole = _roleManager.Roles.FirstOrDefault(r => r.Name == item);
//        //        identityRoles.Add(identityRole);
//        //    }
//        //    //Exclude old drivers from validating criminal record-- so get date from configuration to ByPassOldDrivers
//        //    var usersWithRoles = _profileRepository.SearchUsers(searchCriteria, identityRoles, _configurationManager.GetDateToByPassOldDriversDocuments());
//        //    return usersWithRoles;
//        //}

//        public ApplicationUser GetUserByPhoneNumber(string phone_number)
//        {
//            var user = _userManager.Users.FirstOrDefault(u => u.PhoneNumber == phone_number);

//            if (user == null)
//                throw new UserNotFoundException();
//            return user;
//        }
//        public ApplicationUser GetUserById(string userId)
//        {
//            var user = _userManager.Users.FirstOrDefault(u => u.Id == userId);

//            if (user == null)
//                throw new UserNotFoundException();
//            return user;
//        }

//        public async Task<string> ChangeEndUserPassword(ApplicationUser user, string newPassword)
//        {
//            //Check user has password
//            if (user.PasswordHash != null)
//            {
//                //Remove user current password
//                var removePasswordResult = await _userManager.RemovePasswordAsync(user);

//                //remove password failed
//                if (removePasswordResult.Errors.Any())
//                    throw new ResetPasswordFailedException(user.Email, GetErrorsFromIdentityResult(removePasswordResult));
//            }

//            //add new password  
//            var addPasswordResult = await _userManager.AddPasswordAsync(user, newPassword);

//            //add new password failed
//            if (addPasswordResult.Errors.Any())
//                throw new ResetPasswordFailedException(user.Email, GetErrorsFromIdentityResult(addPasswordResult));

//            /*  try
//              {
//                  user.ChangePasswordIsRequired = true;
//                  await _userManager.UpdateAsync(user);
//              }
//              catch (Exception exception)
//              {
//                  _logger.LogError("An error has occured while reset password , please try again later : Exception : ", exception.Message);
//              }                     */
//            return newPassword;
//        }

//        //public async Task UpdateUserData(UpdateUserDataViewModel updateUserData, ApplicationUser user)
//        //{
//        //    bool nameChanged = false;
//        //    if (!string.IsNullOrEmpty(updateUserData.FirstName) && updateUserData.FirstName != user.FirstName)
//        //    {
//        //        user.FirstName = updateUserData.FirstName;
//        //        nameChanged = true;
//        //    }

//        //    if (!string.IsNullOrEmpty(updateUserData.LastName) && updateUserData.LastName != user.LastName)
//        //    {
//        //        user.LastName = updateUserData.LastName;
//        //        nameChanged = true;
//        //    }

//        //    if (nameChanged)
//        //    {
//        //        await _paymentHandler.UpdateUserName(user.PhoneNumber, $"{user.FirstName} {user.LastName}");
//        //        var userRoles = await _userManager.GetRolesAsync(user);
//        //        if (userRoles == null)
//        //            throw new InternalServerErrorException("No roles found for user: " + user.Id);
//        //        if (userRoles.Any(role => role == "Driver" || role == "UberDriver"))
//        //        {
//        //            var driverImageUrl = new ImageViewModel(user.ProfileImage, _configurationManager.GetImagesRootFolder()).Url;
//        //            await _vehicleHandler.UpdateDriverInfo(user.Id, user.PhoneNumber, driverImageUrl, updateUserData.FirstName + " " + updateUserData.LastName);
//        //        }
//        //        if (userRoles.Any(role => role == "CarOwner"))
//        //        {
//        //            var ownerImageUrl = new ImageViewModel(user.ProfileImage, _configurationManager.GetImagesRootFolder()).Url;
//        //            await _vehicleHandler.UpdateOwnerInfo(user.Id, user.PhoneNumber, ownerImageUrl, updateUserData.FirstName + " " + updateUserData.LastName);
//        //        }
//        //    }

//        //    //if (!string.IsNullOrEmpty(updateUserData.NewEmail))
//        //    //{
//        //    //    updateUserData.NewEmail = System.Net.WebUtility.UrlDecode(updateUserData.NewEmail);
//        //    //    EmailUtility.ValidateEmail(updateUserData.NewEmail);
//        //    //    if (_userManager.Users.Any(u => u.Email.Trim() == updateUserData.NewEmail.Trim() && user.Id != u.Id))
//        //    //        throw new EmailAlreadyExistsException();

//        //    //    user.Email = updateUserData.NewEmail;
//        //    //}

//        //    user.UpdateLastModified(_identityService.DisplayName);
//        //    _profileRepository.SaveEntities();
//        //}

//        //public AdminUsersBaseResponseViewModel GetUsers(UsersSearchCriteria searchCriteria)
//        //{
//        //    var searchCriteriaViewModel = new AdminSearchUsersCriteria();
//        //    if (searchCriteria?.SearchCriteria != null)
//        //    {
//        //        searchCriteriaViewModel.SearchCriteria.Name = searchCriteria.SearchCriteria.Name;
//        //        searchCriteriaViewModel.SearchCriteria.PhoneNumber = searchCriteria.SearchCriteria.PhoneNumber;
//        //        searchCriteriaViewModel.SearchCriteria.Status = searchCriteria.SearchCriteria.Status;
//        //        searchCriteriaViewModel.SearchCriteria.Rating = searchCriteria.SearchCriteria.Rating;
//        //        searchCriteriaViewModel.SearchCriteria.IsUber = searchCriteria.SearchCriteria.IsUber;
//        //        searchCriteriaViewModel.SearchCriteria.RegisterFrom = searchCriteria.SearchCriteria.RegisterFrom;
//        //        searchCriteriaViewModel.SearchCriteria.RegisterTo = searchCriteria.SearchCriteria.RegisterTo;
//        //        searchCriteriaViewModel.SearchCriteria.NationalIdNumber = searchCriteria.SearchCriteria.NationalIdNumber;
//        //    }

//        //    var users = new List<AdminUserViewModel>();
//        //    var applicationUsers = Search(searchCriteriaViewModel, "Driver", "UberDriver", "CarOwner").Result;

//        //    var dateToByPassOldDrivers = _configurationManager.GetDateToByPassOldDriversDocuments();

//        //    var requiredRoles = _roleManager.Roles.Where(r => r.Name == "Driver" || r.Name == "UberDriver" || r.Name == "CarOwner").ToList();
//        //    foreach (var user in applicationUsers)
//        //    {
//        //        Dictionary<string, UserOverallStatus?> rolesStatuses = new Dictionary<string, UserOverallStatus?>();
//        //        foreach (var role in user.Roles)
//        //        {
//        //            string userRole = requiredRoles.FirstOrDefault(r => r.Id == role.RoleId).Name;
//        //            var status = GetUserOverallStatus(user, userRole, dateToByPassOldDrivers);
//        //            rolesStatuses.Add(userRole, status);
//        //        }
//        //        users.Add(new AdminUserViewModel(user, rolesStatuses));
//        //    }

//        //    #region Old Code
//        //    //if (user.Roles.Count > 1)
//        //    //{
//        //    //    foreach (var role in user.Roles)
//        //    //    {
//        //    //        string userRole = userRoles.FirstOrDefault(r => r.Id == role.RoleId).Name;
//        //    //        var userStatus = GetUserOverallStatus(user, userRole, dateToByPassOldDrivers);
//        //    //        users.Add(new AdminUserViewModel(user, userStatus, userRole));
//        //    //    }
//        //    //}
//        //    //else
//        //    //{
//        //    //    var onlyUserRole = user.Roles.FirstOrDefault();
//        //    //    string onlyUserRoleName = userRoles.FirstOrDefault(r => r.Id == onlyUserRole.RoleId).Name;
//        //    //    var userStatus = GetUserOverallStatus(user, onlyUserRoleName, dateToByPassOldDrivers);
//        //    //    users.Add(new AdminUserViewModel(user, userStatus, onlyUserRoleName));
//        //    //} 
//        //    #endregion


//        //    var pageSize = searchCriteria?.Pagination?.PageSize ?? 10;
//        //    if (pageSize == 0)
//        //    {
//        //        pageSize = 10;
//        //    }
//        //    var pageNumber = searchCriteria?.Pagination?.PageNumber != null ? (int)((searchCriteria?.Pagination?.PageNumber) * pageSize) : 0;


//        //    if (users.Count != 0)
//        //    {
//        //        if (searchCriteria?.Sorting != null)
//        //        {
//        //            switch (searchCriteria.Sorting.SortBy)
//        //            {
//        //                case UsersSortBy.Name:
//        //                    if (searchCriteria.Sorting.Direction == SortDirection.Desc)
//        //                        users = users.OrderByDescending(u => u.FirstName).ToList();
//        //                    else
//        //                        users = users.OrderBy(u => u.FirstName).ToList();
//        //                    break;
//        //                case UsersSortBy.Rating:
//        //                    if (searchCriteria.Sorting.Direction == SortDirection.Desc)
//        //                        users = users.OrderByDescending(u => u.RatingAverage).ToList();
//        //                    else
//        //                        users = users.OrderBy(u => u.RatingAverage).ToList();
//        //                    break;
//        //                case UsersSortBy.RegistrationDate:
//        //                    if (searchCriteria.Sorting.Direction == SortDirection.Desc)
//        //                        users = users.OrderByDescending(u => u.RegistrationDate).ToList();
//        //                    else
//        //                        users = users.OrderBy(u => u.RegistrationDate).ToList();
//        //                    break;
//        //                case UsersSortBy.TripsCount:
//        //                    if (searchCriteria.Sorting.Direction == SortDirection.Desc)
//        //                        users = users.OrderByDescending(u => u.DriverTripsCount).ToList();
//        //                    else
//        //                        users = users.OrderBy(u => u.DriverTripsCount).ToList();
//        //                    break;
//        //                case UsersSortBy.CarsCount:
//        //                    //if (searchCriteria.Sorting.Direction == SortDirection.Desc)
//        //                    //    users = users.OrderByDescending(u => u.RatingAverage).ToList();
//        //                    //else
//        //                    //    users = users.OrderBy(u => u.RatingAverage).ToList();
//        //                    break;
//        //                case UsersSortBy.Earnings:
//        //                    //if (searchCriteria.Sorting.Direction == SortDirection.Desc)
//        //                    //    users = users.OrderByDescending(u => u.RatingAverage).ToList();
//        //                    //else
//        //                    //    users = users.OrderBy(u => u.RatingAverage).ToList();
//        //                    break;
//        //                case UsersSortBy.Payments:
//        //                    //if (searchCriteria.Sorting.Direction == SortDirection.Desc)
//        //                    //    users = users.OrderByDescending(u => u.RatingAverage).ToList();
//        //                    //else
//        //                    //    users = users.OrderBy(u => u.RatingAverage).ToList();
//        //                    break;
//        //                default:
//        //                    {
//        //                        users = users.OrderByDescending(o => o.RegistrationDate).ToList();

//        //                    }
//        //                    break;
//        //            }
//        //        }
//        //        else
//        //        {
//        //            users = users.OrderByDescending(o => o.RegistrationDate).ToList();
//        //        }

//        //    }

//        //    int count = users.Count;
//        //    if (searchCriteria.Pagination != null)
//        //    {
//        //        users = users.Skip(pageNumber).Take(pageSize).ToList();
//        //    }

//        //    var response = new AdminUsersBaseResponseViewModel()
//        //    {
//        //        AllUsers = users,
//        //        Length = count
//        //    };

//        //    return response;
//        //}

//        public async Task<Response<RegisterResponse>> Register(RegisterViewModel user, string language)
//        {
//            string methodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
//            // Decode email
//            // user.Email = System.Net.WebUtility.UrlDecode(user.Email);

//            // Validate email
//            // EmailUtility.ValidateEmail(user.Email);

//            // Validate model state
//            if (user == null)
//                throw new MissingParameterException("User");

//            _logger.LogInformation($"{_typeName}.{methodName}: valid model state");

//            Country userCountry = _countryService.GetCountryByCode(user.CountryCode);
//            if (userCountry == null)
//                throw new InvalidParameterException("country_code", user.CountryCode);

//            /*user.PhoneNumber = PhoneNumberUtility.ValidateAndAddCountryDialCode(user.PhoneNumber, userCountry.InternationalDialCode
//                , userCountry.PhoneNumberLength, userCountry.Code);*/


//            // Check if there are users with same phone number
//            // TODO : validate using Regex according to agreed on format
//            /*  List<ApplicationUser> usersWithSamePhoneNumber = _userManager.Users.Where(u => u.PhoneNumber == user.PhoneNumber).ToList();*/

//            // Check if the found account with same phone number is confirmed
//            /*
//            if (usersWithSamePhoneNumber != null && usersWithSamePhoneNumber.Count > 0 && usersWithSamePhoneNumber.Any(u => u.PhoneNumberConfirmed))
//                throw new PhoneNumberAlreadyExistsException();       */

//            bool createNewUser = true;

//            /* PhoneNumberVerification phoneNumberVerification =
//                 _configurationManager.GetAccountVerificationPhoneVerification();
//             EmailVerification emailVerification = _configurationManager.GetAccountVerificationEmailVerification();    */

//            // Check if email exists
//            /* ApplicationUser existingApplicationUserByEmail = _userManager.Users.FirstOrDefault(u => u.Email == user.Email);
//            / if (existingApplicationUserByEmail != null)
//             {
//                 if (existingApplicationUserByEmail.PhoneNumberConfirmed || existingApplicationUserByEmail.EmailConfirmed)
//                 {
//                     throw new EmailAlreadyExistsException();
//                 }
//                 /* else if (phoneNumberVerification == PhoneNumberVerification.SignUp &&
//                      !existingApplicationUserByEmail.PhoneNumberConfirmed)     */
//            /*  {
//                   try
//                   {
//                       if (existingApplicationUserByEmail.PhoneNumber == user.PhoneNumber)
//                           throw new PhoneNumberVerificationRequiredException();
//                       else // User updated his mobile number before verification
//                           createNewUser = false;
//                   }
//                   catch (PhoneNumberVerificationRequiredException e)
//                   {
//                       // Return user id with exception for FE to use in verification
//                       return new Response<RegisterResponse>()
//                       {
//                           Data = new RegisterResponse() { UserId = existingApplicationUserByEmail.Id },
//                           ErrorCode = e.Code,
//                           ErrorMsg = _identityService.Language == Language.en ? e.MessageEn : e.MessageAr
//                       };
//                   }
//               }     */
//            /* else if (emailVerification == EmailVerification.SignUp && !existingApplicationUserByEmail.EmailConfirmed)
//             {
//                 throw new EmailVerificationRequiredException();
//             }  
//        } */

//            ApplicationUser newApplicationUser = null;
//            int errorCode = (int)ErrorCodes.Success;
//            BaseException requiredActionException = new BaseException();

//            // Create
//            if (createNewUser)
//            {
//                string externalId = user.ExternalId?.Trim();
//                newApplicationUser = new ApplicationUser
//                {
//                    // To Do make it according to configurations
//                    Id = string.IsNullOrEmpty(externalId) ? null /* to be generated */ : externalId,
//                    UserName = user.UserName?.Trim(),
//                    Email = user.Email,
//                    PhoneNumber = user.PhoneNumber,
//                    FirstName = user.FirstName?.Trim(),
//                    LastName = user.LastName?.Trim(),
//                    CountryCode = user.CountryCode,
//                    TenantCode = user.TenantCode,
//                    RegistrationDate = DateTime.UtcNow,
//                    //DateOfBirth = user.DateOfBirth,
//                    ApplicationVersion = user.ApplicationVersion,
//                    Language = language,
//                    //Gender = user.Gender,
//                    Platform = user.Platform
//                };

//                _logger.LogInformation($"{_typeName}.{methodName}: creating new user");
//                var createUserResult = await _userManager.CreateAsync(newApplicationUser, user.Password);

//                // Create user failed
//                if (!createUserResult.Succeeded || createUserResult.Errors.Any())
//                    throw new Exception(createUserResult.Errors.FirstOrDefault().Description);
//                // throw new RegistrationFailedException(_commonAccountManager.GetErrorsFromIdentityResult(createUserResult));

//                _logger.LogInformation($"{_typeName}.{methodName}: user created successfully");
//                _logger.LogInformation($"{_typeName}.{methodName}: adding roles to user");

//                try
//                {
//                    await _userManager.AddToRolesAsync(newApplicationUser, user.UserRoles);
//                }
//                catch (Exception ex)
//                {
//                    await _userManager.DeleteAsync(newApplicationUser);
//                    throw new InternalServerErrorException(ex.Message);
//                }
//            }
//            // Update
//            /* else
//             {
//                 existingApplicationUserByEmail.PhoneNumber = user.PhoneNumber;
//                 /* existingApplicationUserByEmail.TermsAcceptanceDate = DateTime.UtcNow;   */
//            /*await _userManager.UpdateAsync(existingApplicationUserByEmail); */
//            /*  }   */

//            /* if (phoneNumberVerification == PhoneNumberVerification.SignUp)
//             {
//                 if (createNewUser)
//                 {
//                     try
//                     {
//                         // await SendVerificationSms(newApplicationUser.Id, _identityService.Language.ToString());
//                     }
//                     catch (Exception exception)
//                     {
//                         _logger.LogInformation("exception in send sms : ", exception.Message);
//                         // await _commonAccountManager.UpdateUserLastSentStatus(newApplicationUser, SendingBlockIntervalType.Sms);
//                     }
//                 }
//                 else
//                 {
//                     //try
//                     //{
//                     //    await SendVerificationSms(existingApplicationUserByEmail.Id, _identityService.Language.ToString());
//                     //}
//                     //catch (Exception exception)
//                     //{
//                     //    _logger.LogInformation("exception in send sms : ", exception.Message);
//                     //    await _commonAccountManager.UpdateUserLastSentStatus(existingApplicationUserByEmail, SendingBlockIntervalType.Sms);
//                     //}
//                 }   
//                 requiredActionException = new PhoneNumberVerificationRequiredException();
//             }*/

//            //if (emailVerification == Emeint.Core.BE.Identity.Domain.Enums.EmailVerification.SignUp)
//            //{
//            //    if (createNewUser)
//            //        await SendVerificationEmail(newApplicationUser.Id, _identityService.Language.ToString());
//            //    else
//            //        await SendVerificationEmail(existingApplicationUserByEmail.Id, _identityService.Language.ToString());
//            //    if (errorCode == (int)ErrorCodes.Success)
//            //    {

//            //        requiredActionException = new EmailVerificationRequiredException();
//            //    }
//            //    else if (errorCode == (int)IdentityErrorCodes.PhoneNumberVerificationRequired)
//            //    {

//            //        requiredActionException = new PhoneNumberAndEmailVerificationRequiredException();
//            //    }
//            //}

//            return new Response<RegisterResponse>()
//            {
//                Data = new RegisterResponse() { UserId = newApplicationUser.Id },
//                ErrorCode = requiredActionException.Code,
//                ErrorMsg = _identityService.Language == Language.en ? requiredActionException.MessageEn : requiredActionException.MessageAr
//            };

//        }
//    }

//    #endregion
//}




