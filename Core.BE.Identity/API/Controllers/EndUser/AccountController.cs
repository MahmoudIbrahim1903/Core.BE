/*using IdentityServer4.Services;
using IdentityServer4.Stores;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.WebUtilities;
using System.Text;
using Emeint.Core.BE.Domain.Exceptions;
using Emeint.Core.BE.Domain.Enums;
using System.Net.Mail;
using System.Text.RegularExpressions;
using Emeint.Core.BE.API.Infrastructure.Services;
using Emeint.Core.BE.Media.Domain.Models;
using Emeint.Core.BE.API.Application.ValueObjects.ViewModels;
using Emeint.Core.BE.Media.API.Application.ValueObject.ViewModels;
using Microsoft.Extensions.Options;
using Emeint.Core.BE.Utilities;
using InvalidOperationException = Emeint.Core.BE.Domain.Exceptions.InvalidOperationException;
using Microsoft.EntityFrameworkCore;

namespace Dryve.Microservices.Identity.API.Controllers.EndUser
{
    /// <summary>
    /// This sample controller implements a typical login/logout/provision workflow for local and external accounts.
    /// The login service encapsulates the interactions with the user data store. This data store is in-memory only and cannot be used for production!
    /// The interaction service provides a way for the UI to communicate with identityserver for validation and context retrieval
    /// </summary>
    [ServiceFilter(typeof(IdentityFilter))]
    [Produces("application/json")]
    [Route("api/identity")]
    public class AccountController : Controller
    {
        //private readonly InMemoryUserLoginService _loginService;
        private readonly ILoginService<ApplicationUser> _loginService;
        private readonly IIdentityServerInteractionService _interaction;
        private readonly IClientStore _clientStore;
        private readonly ILogger<AccountController> _logger;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IVehicleHandler _vehicleHandler;
        private readonly IConfigurationManager _configurationManager;
        private readonly ICountryService _countryService;
        private readonly IPreRegisterRepository _preRegisterRepository;
        private readonly IProfileRepository _profileRepository;
        private readonly INationalIdRepository _nationalIdRepository;
        private readonly IImageRepository _imageRepository;
        private readonly IIdentityService _identityService;
        private readonly string _typeName;
        private readonly ISmsHandler _smsHandler;
        private readonly ILogger<CommonAccountManager> _managerlogger;
        private readonly CommonAccountManager _commonAccountManager;
        private readonly IDocumentRejectionReasonRepository _documentRejectionReasonRepository;
        private readonly IPaymentHandler _paymentHandler;
        private readonly IRatingAndReviewRepository _ratingAndReviewRepository;
        private readonly IHashingManager _hashingManager;
        private readonly IUserDocumentsRepository _userDocumentsRepository;
        private readonly RatingManager _ratingManager;
        private readonly IWebRequestUtility _webRequestUtility;

        public AccountController(

            //InMemoryUserLoginService loginService,
            ILoginService<ApplicationUser> loginService,
            IIdentityServerInteractionService interaction,
            IClientStore clientStore,
            ILogger<AccountController> logger,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IHashingManager hashingManager,
            IConfigurationManager configurationManager,
            ICountryService countryService,
            IPreRegisterRepository preRegisterRepository,
            IProfileRepository profileRepository, IImageRepository imageRepository,
            INationalIdRepository nationalIdRepository,
            IDocumentRejectionReasonRepository documentRejectionReasonRepository,
            IIdentityService identityService,
            ILogger<CommonAccountManager> managerlogger,
            IPaymentHandler paymentHandler, IVehicleHandler vehicleHandler,
            IRatingAndReviewRepository ratingAndReviewRepository,
            IUserDocumentsRepository userDocumentsRepository,
            IWebRequestUtility webRequestUtility, ISmsHandler smsHandler)
        {
            _loginService = loginService;
            _paymentHandler = paymentHandler;
            _interaction = interaction;
            _clientStore = clientStore;
            _logger = logger;
            _userManager = userManager;
            _roleManager = roleManager;
            _configurationManager = configurationManager;
            _countryService = countryService;
            _preRegisterRepository = preRegisterRepository;
            _profileRepository = profileRepository;
            _nationalIdRepository = nationalIdRepository;
            _vehicleHandler = vehicleHandler ?? throw new ArgumentNullException(nameof(vehicleHandler));
            _documentRejectionReasonRepository = documentRejectionReasonRepository;
            _imageRepository = imageRepository;
            _identityService = identityService;
            _typeName = GetType().Name;
            _smsHandler = smsHandler;

            _commonAccountManager = new CommonAccountManager(
               loginService, interaction, clientStore, managerlogger, userManager, roleManager,
                countryService, preRegisterRepository, profileRepository, null, imageRepository, nationalIdRepository,
               documentRejectionReasonRepository, configurationManager, identityService, null,
               userDocumentsRepository, _vehicleHandler, smsHandler, paymentHandler);

            _ratingAndReviewRepository = ratingAndReviewRepository;
            _hashingManager = hashingManager;
            _ratingManager = new RatingManager();
            _userDocumentsRepository = userDocumentsRepository;
            _webRequestUtility = webRequestUtility;
        }

        //public HttpResponseMessage Get()
        //{
        //    HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, "value");
        //    response.Content = new StringContent("hello", Encoding.Unicode);
        //    response.Headers.CacheControl = new CacheControlHeaderValue()
        //    {
        //        MaxAge = TimeSpan.FromMinutes(20)
        //    };
        //    return response;
        //}

        /// <summary>
        /// Used to register a new user with either a CarOwner or Driver Role.
        /// In order to login, use /api/identity/login
        /// </summary>
        /// <param name="user"></param>
        /// <response code="200 (error_code=1)">Internal Server Error</response>
        /// <response code="200 (error_code=10)">Invalid Parameter</response>
        /// <response code="200 (error_code=15)">Invalid Email</response>
        /// <response code="200 (error_code=16)">Invalid Phone number</response>
        /// <response code="200 (error_code=101)">Phone Number Verification Required</response>
        /// <response code="200 (error_code=102)">Email Verification Required</response>
        /// <response code="200 (error_code=103)">Phone Number and Email Verification Required</response>
        /// <response code="200 (error_code=104)">Phone Number Already Exists</response>
        /// <response code="200 (error_code=105)">Registration Failed</response>
        /// <response code="200 (error_code=110)">Sending Email Failed</response> 
        /// <response code="200 (error_code=111)">Sending SMS Failed</response>
        [HttpPost]
        [AllowAnonymous]
        [Route("register")]
        [SkipIdentityFilter]
        //[ValidateAntiForgeryToken]         //TODO: UnComment on production
        public async Task<Response<RegisterResponse>> Register([FromBody] RegisterViewModel user, [FromHeader(Name = "Language")] string language)
        {
            string methodName = System.Reflection.MethodBase.GetCurrentMethod().Name;

            if (user == null)
                throw new InvalidParameterException("user", null);

            string role = user.UserRole.ToLower();
            var country = _countryService.GetCountryByCode(user.CountryCode);
            if (country == null)
                throw new InvalidParameterException("country_code", user.CountryCode);

            if (!country.CountryRoles.Any(r => r.RoleName.ToLower().Trim() == role))
                throw new UnSupportedRoleException(country.NameEn, role);

            if (role != "carowner" && role != "uberdriver" && role != "driver")
                throw new InvalidParameterException("user_role", user.UserRole);

            //var uberDriverProfile = _profileRepository.GetUberProfileByUberId(user.UberId);
            //if (uberDriverProfile != null && uberDriverProfile.UberId == user.UberId)
            //    throw new UberIdAlreadyExistException(user.UberId);

            //{
            //    await _userManager.DeleteAsync(newApplicationUser);
            //}

            // Decode email
            user.Email = System.Net.WebUtility.UrlDecode(user.Email);

            // Validate email
            EmailUtility.ValidateEmail(user.Email);

            // Validate model state
            if (!ModelState.IsValid)
            {
                // the code below should probably be refactored into a GetModelErrors
                // method on your BaseApiController or something like that
                throw new InvalidParameterException("user", "", _commonAccountManager.GetErrorsFromModelState(ModelState));
            }

            _logger.LogInformation($"{_typeName}.{methodName}: valid model state");

            //if (user.UserRole.ToLower() == "uberdriver")
            //{
            //    if (string.IsNullOrEmpty(user.UberId))
            //        throw new MissingParameterException("uber_id");
            //    else
            //    {
            //        bool existingUberDriver = _userManager.Users.Any
            //            (u => (u.DriverProfile != null) && (u.DriverProfile.UberId.Trim().ToLower() == user.UberId.Trim().ToLower()));

            //        if (existingUberDriver)
            //            throw new UberIdAlreadyExistsException(user.UberId);
            //    }
            //}

            // Validate phone number
            //if (user.CountryCode == "EGY")
            //{
            Country userCountry = _countryService.GetCountryByCode(user.CountryCode);
            if (userCountry == null)
                throw new InvalidParameterException("country_code", user.CountryCode);


            user.PhoneNumber = PhoneNumberUtility.ValidateAndAddCountryDialCode(user.PhoneNumber, userCountry.InternationalDialCode, userCountry.PhoneNumberLength, userCountry.Code);

            #region comment valid mobile number
            //var egyptianPhoneNumberPattern = "^\\+201[0|1|2|5]{1}[0-9]{8}"; //"^(?!0+$)(\\+\\d{1,3}[- ]?)?(?!0+$)\\d{10,15}$"
            //var egyptianPhoneNumberRegex = new Regex(egyptianPhoneNumberPattern, RegexOptions.None);
            //Match egyptianPhoneMatch = egyptianPhoneNumberRegex.Match(user.PhoneNumber);
            //if (!egyptianPhoneMatch.Success)
            //    throw new InvalidPhoneNumberException(user.PhoneNumber);

            //userCountry.InternationalDialCode = userCountry.InternationalDialCode.Substring(0, 1);
            //user.PhoneNumber = $"+{userCountry.InternationalDialCode}{user.PhoneNumber}";
            // }

            // Validate phone number
            //if (user.CountryCode == "ARE")
            //{
            //    Country userCountry = _countryRepository.GetCountryByCode(user.CountryCode);
            //    if (userCountry == null)
            //        throw new InvalidParameterException("country_code", user.CountryCode);

            //    user.PhoneNumber = PhoneNumberUtility.ValidateAndAddCountryDialCode(user.PhoneNumber, userCountry.InternationalDialCode, userCountry.PhoneNumberLength, userCountry.Code);

            //    var uaePhoneNumberPattern = "^(?:\\+971|0(0971)?)(?:[234679]|5[01256])\\d{7}$"; //"^(?!0+$)(\\+\\d{1,3}[- ]?)?(?!0+$)\\d{10,15}$"
            //    var uaePhoneNumberRegex = new Regex(uaePhoneNumberPattern, RegexOptions.None);
            //    Match uaePhoneMatch = uaePhoneNumberRegex.Match(user.PhoneNumber);
            //    if (!uaePhoneMatch.Success)
            //        throw new InvalidPhoneNumberException(user.PhoneNumber);
            //}

            #endregion
            // Check if there are users with same phone number
            List<ApplicationUser> usersWithSamePhoneNumber = _userManager.Users.Where(u => u.PhoneNumber == user.PhoneNumber).ToList(); // TODO : validate using Regex according to agreed on format

            // Check if the found account with same phone number is confirmed
            if (usersWithSamePhoneNumber != null && usersWithSamePhoneNumber.Count > 0 && usersWithSamePhoneNumber.Any(u => u.PhoneNumberConfirmed))
                throw new PhoneNumberAlreadyExistsException();

            // TODO: Uncomment if username is not email
            // // Check if there are users with same email
            //List<ApplicationUser> usersWithSameEmail = _userManager.Users.Where(u => u.Email == user.Email).ToList();
            //// Check if the found account with same email is confirmed
            //if (usersWithSameEmail != null && usersWithSameEmail.Count > 0 && usersWithSameEmail.Any(u => u.EmailConfirmed))
            //    throw new EmailAlreadyExistsException();
            bool createNewUser = true;

            PhoneNumberVerification phoneNumberVerification = _configurationManager.GetAccountVerificationPhoneVerification();
            EmailVerification emailVerification = _configurationManager.GetAccountVerificationEmailVerification();

            // Check if email exists
            ApplicationUser existingApplicationUserByEmail = _userManager.Users.FirstOrDefault(u => u.Email == user.Email);
            if (existingApplicationUserByEmail != null)
            {
                if (existingApplicationUserByEmail.PhoneNumberConfirmed || existingApplicationUserByEmail.EmailConfirmed)
                {
                    throw new EmailAlreadyExistsException();
                }
                else if (phoneNumberVerification == PhoneNumberVerification.SignUp && !existingApplicationUserByEmail.PhoneNumberConfirmed)
                {
                    try
                    {
                        if (existingApplicationUserByEmail.PhoneNumber == user.PhoneNumber)
                            throw new PhoneNumberVerificationRequiredException();
                        else // User updated his mobile number before verification
                            createNewUser = false;
                    }
                    catch (PhoneNumberVerificationRequiredException e)
                    {
                        // Return user id with exception for FE to use in verification
                        return new Response<RegisterResponse>()
                        {
                            Data = new RegisterResponse() { UserId = existingApplicationUserByEmail.Id },
                            ErrorCode = e.Code,
                            ErrorMsg = _identityService.Language == Language.en ? e.MessageEn : e.MessageAr
                        };
                    }
                }
                else if (emailVerification == EmailVerification.SignUp && !existingApplicationUserByEmail.EmailConfirmed)
                {
                    throw new EmailVerificationRequiredException();
                }
            }

            ApplicationUser newApplicationUser = null;
            int errorCode = (int)ErrorCodes.Success;
            BaseException requiredActionException = new BaseException();

            // Create
            if (createNewUser)
            {
                var paymentAccountCreated = await _paymentHandler.CreateUserAccount(user.PhoneNumber, $"{user.FirstName} {user.LastName}", user.UserRole);
                if (paymentAccountCreated != null && paymentAccountCreated.Data)
                {
                    newApplicationUser = new ApplicationUser
                    {
                        UserName = user.Email,
                        Email = user.Email,
                        FirstName = user.FirstName.Trim(),
                        LastName = user.LastName.Trim(),
                        PhoneNumber = user.PhoneNumber,
                        SuspensionStatus = UserSuspensionStatus.Active,
                        CountryCode = user.CountryCode,
                        TenantCode = user.TenantCode,
                        RegistrationDate = DateTime.UtcNow,
                        TermsAcceptanceDate = DateTime.UtcNow,
                        RatingAverage = _configurationManager.GetDefaultRating(),
                        ChangePasswordIsRequired = false
                        //AuthenticatorProvider = registerViewModel.AuthenticatorProvider, // TODO: Detect if possible and remove from view model
                    };

                    //var response = await paymentHandler.CreateUserAccount(user.PhoneNumber, user.UserRole);
                    //if (response.Data)
                    //{
                    _logger.LogInformation($"{_typeName}.{methodName}: creating new user");
                    var createUserResult = await _userManager.CreateAsync(newApplicationUser, user.Password);

                    // Create user failed
                    if (!createUserResult.Succeeded || createUserResult.Errors.Any())
                        throw new RegistrationFailedException(_commonAccountManager.GetErrorsFromIdentityResult(createUserResult));

                    _logger.LogInformation($"{_typeName}.{methodName}: user created successfully");
                    _logger.LogInformation($"{_typeName}.{methodName}: adding roles to user");


                    try
                    {
                        await _userManager.AddToRoleAsync(newApplicationUser, user.UserRole);
                    }
                    catch
                    {
                        await _userManager.DeleteAsync(newApplicationUser);
                        await _paymentHandler.DeleteUserAccount(user.PhoneNumber);
                        throw new InvalidParameterException("user_role", user.UserRole);
                    }


                    // Create profile
                    //if (role == "carowner")
                    //{
                    //    // TODO: Handle owner data (vehicle license, etc.)
                    //}
                    //else if (role == "driver")
                    //{
                    //    // TODO: Handle driver data (driving license, etc.)
                    //    throw new InvalidParameterException("user_role", user.UserRole);
                    //}
                    //else

                    if (role == "uberdriver" || role == "driver")
                    {
                        try
                        {
                            var driverProfile = new DriverProfile()
                            {
                                ApplicationUserId = newApplicationUser.Id,
                                //UberId = (role == "uberdriver") ? user.UberId : null,
                                VerificationStatus = DocumentVerificationStatus.NotSubmitted,
                                CreatedBy = newApplicationUser.Id,
                                CreationDate = DateTime.UtcNow,
                                DrivingLicense = new DrivingLicense()
                                {
                                    VerificationStatus = DocumentVerificationStatus.NotSubmitted
                                }
                            };

                            driverProfile.CriminalRecord = new CriminalRecord()
                            {
                                VerificationStatus = DocumentVerificationStatus.NotSubmitted
                            };

                            _profileRepository.Add(driverProfile);
                            _profileRepository.SaveEntities();
                        }
                        catch (DbUpdateException exception)
                        {
                            await _userManager.DeleteAsync(newApplicationUser);
                            await _paymentHandler.DeleteUserAccount(user.PhoneNumber);
                            throw new DbUpdateException("Adding driver Failed", exception.InnerException);
                        }
                    }
                    //else
                    //{
                    //    // TODO: Handle other roles
                    //    throw new InvalidParameterException("user_role", user.UserRole);
                    //}
                    //}
                }
                else
                {
                    _logger.LogInformation($"{_typeName}.{methodName}: Create User Account Error ");
                    await _paymentHandler.DeleteUserAccount(user.PhoneNumber);
                    throw new InternalServerErrorException("Error at creating account in Payment");
                }
            }
            // Update
            else
            {
                existingApplicationUserByEmail.PhoneNumber = user.PhoneNumber;
                existingApplicationUserByEmail.TermsAcceptanceDate = DateTime.UtcNow;
                await _userManager.UpdateAsync(existingApplicationUserByEmail);
            }

            if (phoneNumberVerification == PhoneNumberVerification.SignUp)
            {
                if (createNewUser)
                {
                    try
                    {
                        await SendVerificationSms(newApplicationUser.Id, _identityService.Language.ToString());

                    }
                    catch (Exception exception)
                    {
                        _logger.LogInformation("exception in send sms : ", exception.Message);
                        await _commonAccountManager.UpdateUserLastSentStatus(newApplicationUser, SendingBlockIntervalType.Sms);
                    }
                }
                else
                {
                    try
                    {
                        await SendVerificationSms(existingApplicationUserByEmail.Id, _identityService.Language.ToString());
                    }
                    catch (Exception exception)
                    {
                        _logger.LogInformation("exception in send sms : ", exception.Message);
                        await _commonAccountManager.UpdateUserLastSentStatus(existingApplicationUserByEmail, SendingBlockIntervalType.Sms);
                    }
                }
                //PhoneNumberVerificationRequiredException phoneNumberVerificationRequiredEx = new PhoneNumberVerificationRequiredException();
                //errorCode = phoneNumberVerificationRequiredEx.Code;
                //errorMessage = phoneNumberVerificationRequiredEx.Message;
                requiredActionException = new PhoneNumberVerificationRequiredException();
            }

            if (emailVerification == EmailVerification.SignUp)
            {
                if (createNewUser)
                    await SendVerificationEmail(newApplicationUser.Id, _identityService.Language.ToString());
                else
                    await SendVerificationEmail(existingApplicationUserByEmail.Id, _identityService.Language.ToString());
                if (errorCode == (int)ErrorCodes.Success)
                {
                    //errorCode = (int)IdentityErrorCodes.EmailVerificationRequired;

                    //EmailVerificationRequiredException emailVerificationRequiredEx = new EmailVerificationRequiredException();
                    //errorCode = emailVerificationRequiredEx.Code;
                    //errorMessage = emailVerificationRequiredEx.Message;
                    requiredActionException = new EmailVerificationRequiredException();
                }
                else if (errorCode == (int)IdentityErrorCodes.PhoneNumberVerificationRequired)
                {
                    //errorCode = (int)IdentityErrorCodes.PhoneNumberAndEmailVerificationRequired;

                    //PhoneNumberAndEmailVerificationRequiredException phoneNumberAndEmailVerificationRequiredEx = new PhoneNumberAndEmailVerificationRequiredException();
                    //errorCode = phoneNumberAndEmailVerificationRequiredEx.Code;
                    //errorMessage = phoneNumberAndEmailVerificationRequiredEx.Message;
                    requiredActionException = new PhoneNumberAndEmailVerificationRequiredException();
                }
            }

            return new Response<RegisterResponse>()
            {
                Data = new RegisterResponse() { UserId = createNewUser ? newApplicationUser.Id : existingApplicationUserByEmail.Id },
                ErrorCode = requiredActionException.Code,//errorCode,
                ErrorMsg = _identityService.Language == Language.en ? requiredActionException.MessageEn : requiredActionException.MessageAr
            };
        }

        [HttpPost]
        [Authorize]
        [Route("accept_terms_and_conditions")]
        public async Task<BaseResponse> AcceptTermsAndConditions()
        {

            var user = _userManager.Users.FirstOrDefault(u => u.Id == _identityService.UserId);
            if (user == null)
                throw new UserNotFoundException();

            user.TermsAcceptanceDate = DateTime.UtcNow;
            await _userManager.UpdateAsync(user);

            return new BaseResponse()
            {
                ErrorCode = (int)ErrorCodes.Success
            };


        }

        //[HttpPost]
        //[Route("add_role")]
        //public async Task<BaseResponse> AddRole(string user_id, string role)
        //{
        //    if (string.IsNullOrEmpty(user_id))
        //        throw new MissingParameterException("user_id");

        //    if (string.IsNullOrEmpty(role))
        //        throw new MissingParameterException("role");

        //    var user = await _userManager.FindByIdAsync(user_id); // TODO: Check null user
        //    if (user == null)
        //        throw new InvalidParameterException("user_id", user_id);

        //    try
        //    {
        //        await _userManager.AddToRoleAsync(user, role);
        //    }
        //    catch// (Exception e)
        //    {
        //        //List<string> errors = result.Errors.Select(e => e.Description).ToList();
        //        //string errorString = string.Empty;
        //        //foreach (var error in errors)
        //        //{
        //        //    errorString += error + ".";
        //        //}
        //        throw;
        //    }

        //    return new BaseResponse();
        //}

        /// <summary>
        /// Used to register a new user with either a CarOwner or Driver Role.
        /// In order to login, use /api/identity/login
        /// </summary>
        /// <param name="user"></param>
        /// <response code="200 (error_code=1)">Internal Server Error</response>
        /// <response code="200 (error_code=15)">Invalid Email</response>
        /// <response code="200 (error_code=104)">Phone Number Already Exists</response>
        /// <response code="200 (error_code=105)">Registration Failed</response>
        /// /// <response code="200 (error_code=109)">Email Already Exists</response>
        [HttpPost]
        [AllowAnonymous]
        [SkipIdentityFilter]
        [Route("pre_register")]
        public async Task<BaseResponse> PreRegister([FromBody] PreRegisterViewModel user, [FromHeader(Name = "Language")] string language)
        {
            // Decode email
            user.Email = System.Net.WebUtility.UrlDecode(user.Email);

            // Validate email
            EmailUtility.ValidateEmail(user.Email);

            // Validate model state
            if (!ModelState.IsValid)
                throw new InvalidParameterException("user", "", _commonAccountManager.GetErrorsFromModelState(ModelState));

            // Validate phone number
            //if (user.CountryCode == "EGY")
            //{
            //    Country userCountry = _countryRepository.GetCountryByCode(user.CountryCode);
            //    if (userCountry == null)
            //        throw new InvalidParameterException("ContryCode", user.CountryCode);

            //    user.PhoneNumber = PhoneNumberUtility.ValidateAndAdjustMobileNumber(user.PhoneNumber, userCountry.InternationalDialCode, userCountry.PhoneNumberLength);

            //    var egyptianPhoneNumberPattern = "^01[0|1|2|5]{1}[0-9]{8}"; //"^(?!0+$)(\\+\\d{1,3}[- ]?)?(?!0+$)\\d{10,15}$"
            //    var egyptianPhoneNumberRegex = new Regex(egyptianPhoneNumberPattern, RegexOptions.None);
            //    Match egyptianPhoneMatch = egyptianPhoneNumberRegex.Match(user.PhoneNumber);
            //    if (!egyptianPhoneMatch.Success)
            //        throw new InvalidPhoneNumberException(user.PhoneNumber);
            //}

            // Check if there are users with same phone number
            List<ApplicationUser> usersWithSamePhoneNumber = _userManager.Users.Where(u => u.PhoneNumber == user.PhoneNumber).ToList(); // TODO : validate using Regex according to agreed on format

            // Check if the found account with same phone number is confirmed
            if (usersWithSamePhoneNumber != null && usersWithSamePhoneNumber.Count > 0 && usersWithSamePhoneNumber.Any(u => u.PhoneNumberConfirmed))
                throw new PhoneNumberAlreadyExistsException();

            // Check if email exists
            bool emailExists = _preRegisterRepository.EmailExists(user.Email);
            if (emailExists)
                throw new EmailAlreadyExistsException();

            Random ran = new Random();
            var preRegister = new PreRegister
            {
                Code = ran.Next(1, 10000).ToString(),
                CreatedBy = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                MakeCode = user.MakeCode,
                ModelCode = user.ModelCode,
                ManufactureYear = user.ManufactureYear
            };

            _preRegisterRepository.Add(preRegister);
            _preRegisterRepository.SaveEntities();

            return new BaseResponse
            {
                ErrorCode = (int)ErrorCodes.Success,
            };
        }

        /// <summary>
        /// Used to login and generate a new token
        /// </summary>
        /// <param name="login"></param>
        /// <response code="HTTP 400 Bad Request">Bad Request</response>
        /// <response code="HTTP 401 Unauthorized">Unauthorized</response>
        /// <response code="200 (error_code=101)">Phone Number Verification Required</response>
        /// <response code="200 (error_code=102)">Email Verification Required</response>
        /// <response code="200 (error_code=122)">Invalid username or password</response>
        [HttpPost]
        [AllowAnonymous]
        [SkipIdentityFilter]
        [Route("login")]
        //[ValidateAntiForgeryToken] //TODO: UnComment on production
        public LoginResponse Login([FromForm] LoginViewModel login, [FromHeader(Name = "Language")] string language)
        {
            return _commonAccountManager.Login(login, language);

        }


        /// <summary>
        /// Used to send verification email if required
        /// </summary>
        /// <param name="user_id"></param>
        /// <response code="200 (error_code=1)">Internal Server Error</response>
        /// <response code="200 (error_code=10)">Invalid Parameter</response>
        /// <response code="200 (error_code=11)">Missing Parameter</response>
        /// <response code="200 (error_code=20)">Sending Email Failed</response>
        /// <response code="200 (error_code=129)">User not found</response>
        [HttpGet]
        [Route("send_verification_email")]
        [SkipIdentityFilter]
        public async Task<BaseResponse> SendVerificationEmail(string user_id, [FromHeader(Name = "Language")] string language)
        {
            if (string.IsNullOrEmpty(user_id))
                throw new MissingParameterException("user_id");

            var user = await _userManager.FindByIdAsync(user_id); // TODO: Check null user
            if (user == null)
                throw new UserNotFoundException();

            if (user.EmailConfirmed)
                throw new EmailAlreadyVerifiedException();

            IMailingHandler mailingHandler = new MailingHandler(_configurationManager, _hashingManager, _webRequestUtility);
            var emailCode = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            byte[] tokenGeneratedBytes = Encoding.UTF8.GetBytes(emailCode);
            var codeEncoded = WebEncoders.Base64UrlEncode(tokenGeneratedBytes);

            //Creating the Email Url
            var callbackUrl = Url.Action(
                "VerifyEmail", "Account", //action name, controller name.
                new { user_id = user.Id, code = UrlEncoder.Default.Encode(codeEncoded) },
                protocol: Request.Scheme);

            //Send verification code to user Email    
            var emailSubject = string.Empty;//_configuration.GetValue<string>("Email:EmailSubject");
            var emailBody = string.Empty;
            switch (_identityService.Language)
            {
                case Language.en:
                    emailSubject = Resources_En.EmailSubject;
                    emailBody = $"Dear {user.FirstName} {user.LastName},</br></br>{Resources_En.EmailBody}<a href={callbackUrl}>link</a> </br>";
                    break;
                case Language.ar:
                    emailSubject = Resources_Ar.EmailSubject;
                    emailBody = $"عزيزى {user.FirstName} {user.LastName},</br></br>{Resources_Ar.EmailBody}<a href={callbackUrl}>الرابط</a> </br>";
                    break;
                default:
                    emailSubject = Resources_En.EmailSubject;
                    emailBody = $"Dear {user.FirstName} {user.LastName},</br></br> {Resources_En.EmailBody} <a href={callbackUrl}>link</a> </br>";
                    break;
            }
            //var emailMessage = _configuration.GetValue<string>("EmailBody");

            // TODO: move to Settings
            //try
            //{
            var emailFrom = _configurationManager.GetEmailFrom();
            await _commonAccountManager.CheckSendingBlockInterval(user, SendingBlockIntervalType.Email, "verification code", "كود تفعيل");
            //await _emailSender.SendEmailAsync(emailFrom, user.Email, subject, emailBody);
            //EmailManager emailManager = new EmailManager(_configuration);
            string[] emailsTo = new string[] { user.Email };
            string[] ccEmails = _configurationManager.GetEmailCCEmails();
            List<string> attachements = new List<string>();
            await _commonAccountManager.UpdateUserLastSentStatus(user, SendingBlockIntervalType.Email);
            _logger.LogInformation($"SendVerificationEmail: To " + emailsTo.FirstOrDefault() + " CC: " + ccEmails + " From: " + emailFrom + " Subject: " + emailSubject +
               " Body: " + emailBody + " attachments: " + attachements?.FirstOrDefault());
            var result = await Task.Factory.StartNew(() => mailingHandler.SendEmail(new SendMailViewModel(emailFrom, emailsTo, ccEmails, emailSubject, emailBody, attachements, false, "VerificationEmail", new Dictionary<string, string>(), "DryveServer")));
            // TODO: Handle failure, and update status
            //    if (result.GetType() == typeof(SmtpException))
            //    {
            //        throw result as SmtpException;
            //    }
            //}
            //catch (SmtpException ex)
            //{
            //    if (ex.InnerException != null)
            //        throw new SendEmailFailedException(user.Email, ex.InnerException.Message);
            //    else throw new SendEmailFailedException(user.Email, ex.Message);
            //}

            return await result;
        }

        /// <summary>
        /// Used to verify ownership of email
        /// </summary>
        /// <param name="user_id"></param>
        /// /// <param name="code"></param>
        /// <response code="200 (error_code=1)">Internal Server Error</response>
        /// <response code="200 (error_code=10)">Invalid Parameter</response>
        /// <response code="200 (error_code=11)">Missing Parameter</response>
        /// <response code="200 (error_code=106)">Email Verification Failed</response>
        /// <response code="200 (error_code=129)">User not found</response>
        // If Method name changed, need to be changed in Url.Action() inside SendVerificationEmail()
        [HttpGet]
        [Route("verify_email")]
        [SkipIdentityFilter]
        public async Task<BaseResponse> VerifyEmail(string user_id, string code, [FromHeader(Name = "Language")] string language)
        {
            if (string.IsNullOrEmpty(user_id))
                throw new MissingParameterException("user_id");
            if (string.IsNullOrEmpty(code))
                throw new MissingParameterException("code");

            var user = await _userManager.FindByIdAsync(user_id);
            if (user == null)
                throw new UserNotFoundException();
            if (user.EmailConfirmed)
                throw new EmailAlreadyVerifiedException();

            var codeDecodedBytes = WebEncoders.Base64UrlDecode(code);
            var codeDecoded = Encoding.UTF8.GetString(codeDecodedBytes);
            var result = await _userManager.ConfirmEmailAsync(user, codeDecoded);
            if (result.Succeeded)
            {
                // Remove duplicate emails which are not verified
                List<ApplicationUser> usersWithSameEmail = _userManager.Users.Where(u => u.Email == user.Email && !u.EmailConfirmed).ToList();
                foreach (var userWithSameEmail in usersWithSameEmail)
                {
                    await _userManager.DeleteAsync(userWithSameEmail);
                }

                //return View("ConfirmEmail");
                return new BaseResponse
                {
                    ErrorCode = (int)ErrorCodes.Success,
                };
            }

            throw new EmailVerificationFailedException(_commonAccountManager.GetErrorsFromIdentityResult(result));
        }

        /// <summary>
        /// Used to send verification SMS
        /// </summary>
        /// <param name="user_id"></param>
        /// <response code="200 (error_code=1)">Internal Server Error</response>
        /// <response code="200 (error_code=10)">Invalid Parameter</response>
        /// <response code="200 (error_code=11)">Missing Parameter</response>
        /// <response code="200 (error_code=111)">Sending SMS Failed</response>
        /// <response code="200 (error_code=129)">User not found</response>
        [HttpGet]
        [Route("send_verification_sms")]
        [SkipIdentityFilter]
        public async Task<BaseResponse> SendVerificationSms(string user_id, [FromHeader(Name = "Language")] string language)
        {
            if (string.IsNullOrEmpty(user_id))
                throw new MissingParameterException("user_id");

            var user = await _userManager.FindByIdAsync(user_id); // TODO: Check null user
            if (user == null)
                throw new UserNotFoundException();

            if (user.PhoneNumberConfirmed)
                throw new PhoneNumberAlreadyVerifiedException();

            _commonAccountManager.CheckSendSmsCount(SmsMessageTemplates.PhoneNumberVerification, user);

            //Generate Phonenumber verification code
            var phoneCode = await _userManager.GenerateChangePhoneNumberTokenAsync(user, user.PhoneNumber);

            _logger.LogInformation(
                $"Send Verification Sms >  Email: {user.Email} Code: {phoneCode}  UserId: {user_id}  Phone: {user.PhoneNumber}"); // TODO: Move to settings

            // var smsMessage = _configuration.GetValue<string>("SMS:Body");
            await _commonAccountManager.CheckSendingBlockInterval(user, SendingBlockIntervalType.Sms, "verification code", "كود تفعيل");

            string userPhoneNumber = "";

            bool enableSendingSmsByTwilio = _configurationManager.EnableSendingSmsByTwilio();
            SMSProvider currentSmsProvider = SMSProvider.VictoryLink;
            var userCountry = _countryService.GetCountryByCode(user.CountryCode);
            if (userCountry == null)
                throw new InvalidParameterException("country_code", user.CountryCode);
            userPhoneNumber = PhoneNumberUtility.ValidateAndRemoveCountryDialCode(user.PhoneNumber, userCountry.InternationalDialCode, userCountry.PhoneNumberLength, userCountry.Code);

            if (((user.CountryCode == "EGY" && user.LastSentSmsDate.HasValue) || (user.CountryCode == "ARE"))
                && enableSendingSmsByTwilio)
            {
                currentSmsProvider = SMSProvider.Twilio;
                userPhoneNumber = user.PhoneNumber;
            }

            var smsMessageParameters = new Dictionary<string, string>();
            smsMessageParameters.Add("verification_code", phoneCode);
            var smsResult = await _smsHandler.SendSmsAsync(new SmsViewModel
            {
                CreatedBy = user.Email,
                PhoneNumbers = new List<string> { userPhoneNumber },
                MessageParameters = smsMessageParameters,
                SmsProvider = currentSmsProvider,
                TemplateCode = SmsMessageTemplates.PhoneNumberVerification,
                Language = _identityService.Language.ToString()
            }, true);

            user.IncrementVerificationSmsCount();
            await _commonAccountManager.UpdateUserLastSentStatus(user, SendingBlockIntervalType.Sms);

            return smsResult;
        }



        /// <summary>
        /// Used to verify ownership of phone number
        /// </summary>
        /// <param name="user_id"></param>
        /// /// <param name="code"></param>
        /// <response code="200 (error_code=1)">Internal Server Error</response>
        /// <response code="200 (error_code=10)">Invalid Parameter</response>
        /// <response code="200 (error_code=11)">Missing Parameter</response>
        /// <response code="200 (error_code=107)">Phone Number Verification Failed</response>
        /// <response code="200 (error_code=129)">User not found</response>
        [HttpGet]
        [Route("verify_phone_number")]
        [SkipIdentityFilter]
        public async Task<BaseResponse> VerifyPhoneNumber(string user_id, string code, [FromHeader(Name = "Language")] string language)
        {
            if (string.IsNullOrEmpty(user_id))
                throw new MissingParameterException("user_id");
            if (string.IsNullOrEmpty(code))
                throw new MissingParameterException("code");

            var user = await _userManager.FindByIdAsync(user_id);
            if (user == null)
                throw new UserNotFoundException();

            if (user.PhoneNumberConfirmed)
                throw new PhoneNumberAlreadyVerifiedException();

            //var result = await _userManager.VerifyChangePhoneNumberTokenAsync(user, code, user.PhoneNumber);

            _logger.LogInformation($"VerifyPhoneNumber >  Email: {user.Email} Code: {code}  UserId: {user_id}  Phone: {user.PhoneNumber}");

            var result = await _userManager.ChangePhoneNumberAsync(user, user.PhoneNumber, code);
            if (result.Succeeded)
            {
                // Remove duplicate phone numbers which are not verified
                List<ApplicationUser> usersWithSamePhoneNumber = _userManager.Users.Where(u => u.PhoneNumber == user.PhoneNumber && !u.PhoneNumberConfirmed).ToList();
                foreach (var userWithSamePhoneNumber in usersWithSamePhoneNumber)
                {
                    await _userManager.DeleteAsync(userWithSamePhoneNumber);
                }

                //return View("ConfirmEmail");
                return new BaseResponse
                {
                    ErrorCode = (int)ErrorCodes.Success
                };
            }

            throw new PhoneNumberVerificationFailedException(_commonAccountManager.GetErrorsFromIdentityResult(result));
        }

        //[HttpPost]
        //[AllowAnonymous]
        //[Route("forgot_password")]
        //public async Task<BaseResponse> ForgotPassword([FromBody] ForgotPasswordViewModel forgotPasswordViewModel)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        var user = await _userManager.FindByNameAsync(forgotPasswordViewModel.Email);

        //        if (user == null)
        //            throw new InvalidParameterException("email", forgotPasswordViewModel.Email);

        //        // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=320771
        //        // Send an email with this link
        //        string code = await _userManager.GeneratePasswordResetTokenAsync(user);
        //        byte[] tokenGeneratedBytes = Encoding.UTF8.GetBytes(code);
        //        var codeEncoded = WebEncoders.Base64UrlEncode(tokenGeneratedBytes);

        //        var callbackUrl = Url.Action("ResetPassword", "Account", new { email = user.Email, code = codeEncoded },
        //            protocol: Request.Scheme);
        //        var emailFrom = _configuration.GetValue<string>("Email:From");

        //        await _emailSender.SendEmailAsync(emailFrom, user.Email, "Forgot Password",
        //            "Please reset your password by using this: <a href =\"" + callbackUrl + "\">link</a>");

        //        return new BaseResponse()
        //        {
        //            ErrorCode = (int)ErrorCodes.Success
        //        };
        //    }
        //    else
        //    {
        //        // the code below should probably be refactored into a GetModelErrors
        //        // method on your BaseApiController or something like that

        //        throw new InvalidParameterException(nameof(forgotPasswordViewModel), "", GetErrorsFromModelState());
        //    }
        //}

        /// <summary>
        /// Reset password
        /// </summary>
        /// <param name="forgot_password"></param>
        /// <response code="200 (error_code=1)"></response>
        /// <response code="200 (error_code=10)">Invalid Parameter</response>
        /// <response code="200 (error_code=15)">Invalid Email</response>
        /// <response code="200 (error_code=101)">Phone Number Verification Required</response>
        /// <response code="200 (error_code=102)">Email Verification Required</response>
        /// <response code="200 (error_code=116)">Reset Password Failed</response>
        /// <response code="200 (error_code=129)">User not found</response>
        [HttpPost]
        [AllowAnonymous]
        [SkipIdentityFilter]
        [Route("forgot_password")]
        public async Task<BaseResponse> ForgotPassword([FromBody] ForgotPasswordViewModel forgot_password, [FromHeader(Name = "Language")] string language)
        {

            // Decode email
            var smsResult = await _commonAccountManager.ForgotPassword(forgot_password, ModelState);

            return new BaseResponse
            {
                ErrorCode = smsResult
            };
        }

        /// <summary>
        /// Change password
        /// </summary>
        /// <param name="change_password"></param>
        /// <response code="200 (error_code=1)"></response>
        /// <response code="200 (error_code=10)">Invalid Parameter</response>
        /// <response code="200 (error_code=117)">Change Password Failed</response>
        /// <response code="200 (error_code=129)">User not found</response>
        [HttpPost]
        [Authorize]
        [Route("change_password")]
        public async Task<BaseResponse> ChangePassword([FromBody]ChangePasswordViewModel change_password, [FromHeader(Name = "Language")] string language)
        {

            await _commonAccountManager.ChangePassword(change_password, ModelState);

            return new BaseResponse()
            {
                ErrorCode = (int)ErrorCodes.Success
            };
        }

        /// <summary>
        /// Used to upload national id
        /// </summary>
        /// <param name="national_id"></param>       
        /// <response code="200 (error_code=1)">Internal Server Error</response>
        /// <response code="200 (error_code=10)">Invalid Parameter</response>
        /// <response code="200 (error_code=11)">Missing Parameter</response>
        /// <response code="200 (error_code=115)">National Number must be 14 only numbers for Egypt and starts with 2 or 3 </response>
        [HttpPost]
        [Authorize]
        [Route("update_national_id")]
        public async Task<BaseResponse> UpdateNationalId([FromBody]NationalIdRequestViewModelDEPRECATED national_id, [FromHeader(Name = "Language")] string language)
        {
            if (national_id == null)
                throw new MissingParameterException("national_id");

            if (!ModelState.IsValid)
                throw new InvalidParameterException(nameof(national_id), "", _commonAccountManager.GetErrorsFromModelState(ModelState));

            // Validate images
            bool frontImageExists = _imageRepository.ImageCodeExists(national_id.FrontImage);
            if (!frontImageExists)
                throw new InvalidParameterException("front_image", national_id.FrontImage);

            bool backImageExists = _imageRepository.ImageCodeExists(national_id.BackImage);
            if (!backImageExists)
                throw new InvalidParameterException("back_image", national_id.BackImage);

            if (national_id.SelfieImage != null)
            {
                bool selfieImageExists = _imageRepository.ImageCodeExists(national_id.SelfieImage);
                if (!selfieImageExists)
                    throw new InvalidParameterException("selfie_image", national_id.SelfieImage);
            }

            // Validate national Id number
            if (_identityService.CountryCode == "EGY" /*&& national_id.NationalNumber.Trim().Length != 14*/ /*)   
            {
                var egyptianNationalNumberPattern = "^(2|3)[0-9]{13}$";/*&& national_id.NationalNumber.Trim().Length == 14*/
               /* var egyptianNationalNumberRegex = new Regex(egyptianNationalNumberPattern, RegexOptions.None);
                Match match = egyptianNationalNumberRegex.Match(national_id.NationalNumber);
                if (!match.Success)
                    throw new InvalidNationalIdException();
            }

            DateTime dateOfBirth = new DateTime();

            var user = _userManager.Users.FirstOrDefault(u => u.Id == _identityService.UserId);

            // Date of birth
            if (user != null)
            {
                var userRoles = await _userManager.GetRolesAsync(user);
                if (userRoles != null && !string.IsNullOrEmpty(national_id.DateOfBirth))
                {
                    if (DateTime.TryParseExact(national_id.DateOfBirth, "yyyyMMddHHmmss", null, DateTimeStyles.None,
                                out dateOfBirth) == false)
                        throw new InvalidParameterException("date_of_birth", national_id.DateOfBirth);

                    if (userRoles.Any(role => role == "Driver" || role == "UberDriver"))
                    {
                        int minDriverAge = _configurationManager.GetMinDriverAge();
                        int maxDriverAge = _configurationManager.GetMaxDriverAge();

                        if (dateOfBirth.AddYears(minDriverAge) > DateTime.UtcNow)
                            throw new DriverUnderAgeException(minDriverAge);
                        if (dateOfBirth.AddYears(maxDriverAge) < DateTime.UtcNow)
                            throw new DriverAboveAgeException(maxDriverAge);
                    }
                    user.DateOfBirth = dateOfBirth;
                }

            }

            var nationalIdByUserId = _nationalIdRepository.GetNationalIdByUserId(_identityService.UserId);

            // Validate national number
            var nationalIdByNumber = _nationalIdRepository.GetNationalIdByNumber(national_id.NationalNumber);
            if (nationalIdByNumber != null && nationalIdByNumber.ApplicationUserId != _identityService.UserId)
                throw new NationalNumberAlreadyExistsException(national_id.NationalNumber);

            if (nationalIdByUserId != null)
            {
                // Validate verification status
                if (nationalIdByUserId.VerificationStatus == DocumentVerificationStatus.Verified)
                {
                    if (national_id.SelfieImage != null)
                    {
                        if (nationalIdByUserId.NationalNumber != national_id.NationalNumber ||
                            nationalIdByUserId.FrontImage != national_id.FrontImage ||
                            nationalIdByUserId.BackImage != national_id.BackImage ||
                            nationalIdByUserId.SelfieImage != national_id.SelfieImage)
                            throw new UpdateNationalIdNotAllowedException();

                        if (!String.IsNullOrEmpty(national_id.FirstName) && user.FirstName != national_id.FirstName)
                            throw new UpdateNationalIdNotAllowedException();

                        if (!String.IsNullOrEmpty(national_id.LastName) && user.LastName != national_id.LastName)
                            throw new UpdateNationalIdNotAllowedException();
                    }
                    else
                    {
                        if (nationalIdByUserId.NationalNumber != national_id.NationalNumber ||
                            nationalIdByUserId.FrontImage != national_id.FrontImage ||
                            nationalIdByUserId.BackImage != national_id.BackImage)
                            throw new UpdateNationalIdNotAllowedException();

                        if (!String.IsNullOrEmpty(national_id.FirstName) && user.FirstName != national_id.FirstName)
                            throw new UpdateNationalIdNotAllowedException();

                        if (!String.IsNullOrEmpty(national_id.LastName) && user.LastName != national_id.LastName)
                            throw new UpdateNationalIdNotAllowedException();
                    }
                }
                else if (nationalIdByUserId.VerificationStatus == DocumentVerificationStatus.Rejected)
                {
                    nationalIdByUserId.VerificationStatus = DocumentVerificationStatus.Resubmitted;
                    nationalIdByUserId.ReSubmissionDate = DateTime.UtcNow;
                }
                else
                {
                    nationalIdByUserId.VerificationStatus = DocumentVerificationStatus.Submitted;
                    nationalIdByUserId.SubmissionDate = DateTime.UtcNow;
                }
                nationalIdByUserId.ApplicationUserId = _identityService.UserId;
                nationalIdByUserId.NationalNumber = national_id.NationalNumber;
                nationalIdByUserId.FrontImage = national_id.FrontImage;
                nationalIdByUserId.BackImage = national_id.BackImage;

                if (!String.IsNullOrEmpty(national_id.FirstName))
                    user.FirstName = national_id.FirstName;

                if (!String.IsNullOrEmpty(national_id.LastName))
                    user.LastName = national_id.LastName;

                if (national_id.SelfieImage != null)
                {
                    nationalIdByUserId.SelfieImage = national_id.SelfieImage;
                }
                else
                {
                    nationalIdByUserId.SelfieImage = null;
                }

                _nationalIdRepository.Update(nationalIdByUserId);
            }
            else
            {
                var nationalId = new NationalId
                {
                    CreatedBy = "georgem",
                    CreationDate = DateTime.UtcNow,
                    ApplicationUserId = _identityService.UserId,
                    NationalNumber = national_id.NationalNumber,
                    FrontImage = national_id.FrontImage,
                    BackImage = national_id.BackImage,
                    SelfieImage = national_id.SelfieImage,
                    VerificationStatus = DocumentVerificationStatus.Submitted,
                    SubmissionDate = DateTime.UtcNow
                };

                _nationalIdRepository.Add(nationalId);
            }

            await _userManager.UpdateAsync(user);
            _nationalIdRepository.SaveEntities();

            return new BaseResponse
            {
                ErrorCode = (int)ErrorCodes.Success,
            };
        }

        /// <summary>
        /// Used to upload criminal record
        /// </summary>
        /// <param name="criminal_record"></param>       
        /// <response code="200 (error_code=1)">Internal Server Error</response>
        /// <response code="200 (error_code=10)">Invalid Parameter</response>
        /// <response code="200 (error_code=11)">Missing Parameter</response>
        /// <response code="200 (error_code=145)">Issuance date is in the future</response>
        [HttpPost]
        [Authorize]
        [Route("update_criminal_record")]
        public async Task<BaseResponse> UpdateCriminalRecord([FromBody]CriminalRecordRequestViewModel criminal_record, [FromHeader(Name = "Language")] string language)
        {

            var uberDriverProfile = _profileRepository.GetUberProfileByUserId(_identityService.UserId);
            if (uberDriverProfile == null)
            {
                throw new UserProfileNotFoundException(_identityService.UserId);
            }

            await _commonAccountManager.UpdateCriminalRecord(criminal_record, uberDriverProfile);

            return new BaseResponse
            {
                ErrorCode = (int)ErrorCodes.Success,
            };

        }

        /// <summary>
        /// Used to upload residency record
        /// </summary>
        /// <param name="residency"></param>       
        /// <response code="200 (error_code=1)">Internal Server Error</response>
        /// <response code="200 (error_code=10)">Invalid Parameter</response>
        /// <response code="200 (error_code=11)">Missing Parameter</response>
        [HttpPost]
        [Authorize]
        [Route("update_residency")]
        public async Task<BaseResponse> UpdateResidency([FromBody]ResidencyRequestViewModel residency, [FromHeader(Name = "Language")] string language)
        {
            await _commonAccountManager.UpdateResidency(residency, _identityService.UserId);

            return new BaseResponse
            {
                ErrorCode = (int)ErrorCodes.Success,
            };
        }

        /// <summary>
        /// Used to update driver uber details
        /// </summary>
        /// <param name="driver_uber_details"></param>       
        /// <response code="200 (error_code=1)">Internal Server Error</response>
        /// <response code="200 (error_code=10)">Invalid Parameter</response>
        /// <response code="200 (error_code=11)">Missing Parameter</response>
        [HttpPost]
        [Authorize(Roles = "UberDriver,Driver")]
        [Route("update_uber_profile_details")]
        public async Task<BaseResponse> UpdateUberProfile([FromBody]DriverUberDetailsRequestViewModel driver_uber_details, [FromHeader(Name = "Language")] string language)
        {
            if (driver_uber_details == null)
                throw new MissingParameterException("driver_uber_details");

            // Validate images
            if (!String.IsNullOrEmpty(driver_uber_details.UberScanImageCode))
            {
                bool imageExists = _imageRepository.ImageCodeExists(driver_uber_details.UberScanImageCode);
                if (!imageExists)
                    throw new InvalidParameterException("uber_scan_image_code", driver_uber_details.UberScanImageCode);
            }

            var uberDriverProfile = _profileRepository.GetUberProfileByUserId(_identityService.UserId);

            if (uberDriverProfile != null)
            {
                if (driver_uber_details.IsUberDriver == true)
                {
                    if (driver_uber_details.UberRating != null && (driver_uber_details.UberRating > 5 || driver_uber_details.UberRating < 0))
                        throw new InvalidParameterException("invalid value for rating", Convert.ToString(driver_uber_details.UberRating));

                    uberDriverProfile.UberId = driver_uber_details.UberId;
                    uberDriverProfile.UberNumberOfTrips = driver_uber_details.UberNumberOfTrips;
                    uberDriverProfile.UberRating = driver_uber_details.UberRating;
                    uberDriverProfile.UberScanImage = driver_uber_details.UberScanImageCode;
                    uberDriverProfile.IsUberDriver = driver_uber_details.IsUberDriver;
                }
                else if (driver_uber_details.IsUberDriver == false)
                {
                    uberDriverProfile.IsUberDriver = false;
                }
                if (uberDriverProfile.UberVerificationStatus == DocumentVerificationStatus.Rejected || uberDriverProfile.UberVerificationStatus == DocumentVerificationStatus.Expired)
                {
                    uberDriverProfile.UberReSubmissionDate = DateTime.UtcNow;
                    uberDriverProfile.UberVerificationStatus = DocumentVerificationStatus.Resubmitted;
                }
                else
                {
                    uberDriverProfile.UberSubmissionDate = DateTime.UtcNow;
                    uberDriverProfile.UberVerificationStatus = DocumentVerificationStatus.Submitted;
                }
                uberDriverProfile.PreferredModelPriceCategoryCode = driver_uber_details.PreferredModelPriceCategoryCode;

                _profileRepository.Update(uberDriverProfile);
                _profileRepository.SaveEntities();
            }
            else
            {
                throw new UserProfileNotFoundException(_identityService.UserId);
            }

            return new BaseResponse
            {
                ErrorCode = (int)ErrorCodes.Success,
            };
        }

        /// <summary>
        /// Used to view owner Profile
        /// date format yyyyMMdd
        /// </summary>
        /// <response code="200 (error_code=1)">Internal Server Error</response>
        /// <response code="200 (error_code=10)">Invalid Parameter</response>
        /// <response code="200 (error_code=11)">Missing Parameter</response>
        /// <response code="200 (error_code=14)">Invalid Operation</response>
        /// <response code="200 (error_code=129)">User not found</response>
        [Route("owner_profile")]
        //[Authorize]
        [SkipIdentityFilter]
        [HttpGet]
        public async Task<Response<OwnerProfileResponseViewModel>> OwnerProfile(string user_id, [FromHeader(Name = "Language")] string language)
        {
            // Set user_id with current user id if query param not sent
            if (user_id == null)
                user_id = _identityService.UserId;

            var owner = await _commonAccountManager.OwnerProfile(user_id);
            var nationalIdByUserId = _nationalIdRepository.GetNationalIdByUserId(user_id);

            //var averageResponseHours= await _vehicleHandler.OwnerAverageResponseHours(user_id);

            OwnerProfileResponseViewModel ownerProfileViewModel = new OwnerProfileResponseViewModel
            {
                AverageResponseHours = (owner.OwnerProfile != null) ? owner.OwnerProfile.AverageResponseTime.ToString() : "N/A",
                FirstName = owner.FirstName,
                LastName = owner.LastName,
                Image = new ImageViewModel(owner.ProfileImage, _configurationManager.GetImagesRootFolder()),
                RegistrationDate = owner.RegistrationDate.ToString("yyyyMMddHHmmss"),
                FacebookLink = owner.FacebookLink,
                GoogleLink = owner.GoogleLink,
                RatingAverage = owner.RatingAverage.HasValue ? owner.RatingAverage.Value : 0m
            };

            if (user_id == _identityService.UserId)
                ownerProfileViewModel.ChangePasswordIsRequired = owner.ChangePasswordIsRequired;
            #region TODO Delete 
            ownerProfileViewModel.RatingsAndReviews = new List<RatingViewModel>();
            var rates = _ratingAndReviewRepository.RevieweeRates(user_id, null, "owner").ToList();
            if (rates != null && rates.Count > _configurationManager.GetMinRatingsCount())
            {
                foreach (var rate in rates)
                {
                    ownerProfileViewModel.RatingsAndReviews.Add(new RatingViewModel(rate.Code, rate.Rating, rate.Review, rate.CreatedBy, rate.CreationDate.ToString("yyyyMMddHHmmss"), new ImageViewModel(rate.ReviewerUser.ProfileImage, _configurationManager.GetImagesRootFolder())));
                }
            }
            #endregion
            ownerProfileViewModel.PhoneNumber = owner.PhoneNumber;
            ownerProfileViewModel.Email = owner.Email;

            if (owner.Id == _identityService.UserId)
            {

                ownerProfileViewModel.IsEmailConfirmed = owner.EmailConfirmed;
                ownerProfileViewModel.IsPhoneNumberConfirmed = owner.PhoneNumberConfirmed;

                if (nationalIdByUserId != null)
                {
                    ownerProfileViewModel.NationalId = new NationalIdViewModel()
                    {
                        VerificationStatus = nationalIdByUserId.VerificationStatus,
                        NationalNumber = nationalIdByUserId.NationalNumber,
                        BackImage = new ImageViewModel(nationalIdByUserId.BackImage, _configurationManager.GetImagesRootFolder()),
                        FrontImage = new ImageViewModel(nationalIdByUserId.FrontImage, _configurationManager.GetImagesRootFolder()),
                        SelfieImage = new ImageViewModel(nationalIdByUserId.SelfieImage, _configurationManager.GetImagesRootFolder()),
                        RejectionReason = nationalIdByUserId.DocumentRejectionReason != null ? LocalizationUtility.GetLocalizedText(nationalIdByUserId.DocumentRejectionReason.ReasonEn, nationalIdByUserId.DocumentRejectionReason.ReasonAr, _identityService.Language) : null,
                        RejectionComment = nationalIdByUserId.RejectionComment
                    };
                }
                else
                    ownerProfileViewModel.NationalId = new NationalIdViewModel();
            }

            return new Response<OwnerProfileResponseViewModel>()
            {
                Data = ownerProfileViewModel
            };
        }

        //static DateTimeViewModel CalculateYourAge(DateTime Dob)
        //{
        //    DateTime Now = DateTime.UtcNow;
        //    int years = new DateTime(DateTime.UtcNow.Subtract(Dob).Ticks).Year - 1;
        //    DateTime pastYearDate = Dob.AddYears(years);
        //    int months = 0;
        //    for (int i = 1; i <= 12; i++)
        //    {
        //        if (pastYearDate.AddMonths(i) == Now)
        //        {
        //            months = i;
        //            break;
        //        }
        //        else if (pastYearDate.AddMonths(i) >= Now)
        //        {
        //            months = i - 1;
        //            break;
        //        }
        //    }
        //    int days = Now.Subtract(pastYearDate.AddMonths(months)).Days;
        //    return new DateTimeViewModel()
        //    {
        //        Year = years
        //        //Day = days,
        //        //Month = months
        //    };
        //}

        /// <summary>
        /// Used to view Driver Profile
        /// date format yyyyMMdd
        /// </summary>
        /// <response code="200 (error_code=1)">Internal Server Error</response>
        /// <response code="200 (error_code=10)">Invalid Parameter</response>
        /// <response code="200 (error_code=11)">Missing Parameter</response>
        /// <response code="200 (error_code=14)">Invalid Operation</response>
        /// <response code="200 (error_code=129)">User not found</response>
        [Route("driver_profile")]
        [Authorize]
        [HttpGet]
        public async Task<Response<DriverProfileResponseViewModel>> DriverProfile(string user_id, [FromHeader(Name = "Language")] string language)
        {
            // Set user_id with current user id if query param not sent
            if (user_id == null)
                user_id = _identityService.UserId;

            var user = _userManager.Users.FirstOrDefault(u => u.Id == user_id);
            if (user == null)
                throw new UserNotFoundException();

            // Roles
            var userRoles = await _userManager.GetRolesAsync(user);
            if (userRoles == null)
                throw new InternalServerErrorException("No roles found for user: " + user_id);

            if (!userRoles.Contains("Driver") && !userRoles.Contains("UberDriver"))
                throw new Emeint.Core.BE.Domain.Exceptions.InvalidOperationException("This user_id is not for driver", "هذا المستخدم ليس هو سائق السيارة");

            DriverProfileResponseViewModel driverProfileViewModel = new DriverProfileResponseViewModel
            {
                FirstName = user.FirstName,
                LastName = user.LastName,

                Image = new ImageViewModel(user.ProfileImage, _configurationManager.GetImagesRootFolder()),
                RegistrationDate = user.RegistrationDate.ToString("yyyyMMddHHmmss"),

                //Address = new AddressResponseViewModel(new Address()
                //{
                //    CountryCode = user.CountryCode,
                //    CountryName = user.CountryName,
                //    CityCode = user.CityCode,
                //    CityName = user.CityName,
                //    AreaCode = user.AreaCode,
                //    AreaName = user.AreaName,
                //    Details = user.AddressDetails
                //}),
                //Gender = user.Gender,
                FacebookLink = user.FacebookLink,
                GoogleLink = user.GoogleLink,
                RatingAverage = user.RatingAverage.HasValue ? user.RatingAverage.Value : 0m
            };

            var uberDriverProfile = _profileRepository.GetUberProfileByUserId(user_id);
            if (user_id == _identityService.UserId)
                driverProfileViewModel.ChangePasswordIsRequired = user.ChangePasswordIsRequired;
            #region TODO delete
            driverProfileViewModel.RatingsAndReviews = new List<RatingViewModel>();
            var rates = _ratingAndReviewRepository.RevieweeRates(user_id, null, "driver").ToList();
            if (rates != null && rates.Count > _configurationManager.GetMinRatingsCount())
            {
                foreach (var rate in rates)
                {
                    driverProfileViewModel.RatingsAndReviews.Add(new RatingViewModel(rate.Code, rate.Rating, rate.Review, rate.CreatedBy, rate.CreationDate.ToString("yyyyMMddHHmmss"), new ImageViewModel(rate.ReviewerUser.ProfileImage, _configurationManager.GetImagesRootFolder())));
                }
            }
            #endregion
            driverProfileViewModel.PhoneNumber = user.PhoneNumber;
            driverProfileViewModel.Email = user.Email;
            // Check if previewer is profile owner
            if (user_id == _identityService.UserId)
            {
                driverProfileViewModel.DateOfBirth = user.DateOfBirth.HasValue ? user.DateOfBirth.Value.ToString("yyyyMMddHHmmss") : null;
                driverProfileViewModel.IsEmailConfirmed = user.EmailConfirmed;
                driverProfileViewModel.IsPhoneNumberConfirmed = user.PhoneNumberConfirmed;

                var nationalIdByUserId = _nationalIdRepository.GetNationalIdByUserId(user_id);
                if (nationalIdByUserId != null)
                {
                    driverProfileViewModel.NationalId = new NationalIdViewModel()
                    {
                        VerificationStatus = nationalIdByUserId.VerificationStatus,
                        NationalNumber = nationalIdByUserId.NationalNumber,
                        BackImage = new ImageViewModel(nationalIdByUserId.BackImage, _configurationManager.GetImagesRootFolder()),
                        FrontImage = new ImageViewModel(nationalIdByUserId.FrontImage, _configurationManager.GetImagesRootFolder()),
                        SelfieImage = new ImageViewModel(nationalIdByUserId.SelfieImage, _configurationManager.GetImagesRootFolder()),
                        RejectionReason = nationalIdByUserId.DocumentRejectionReason != null ? LocalizationUtility.GetLocalizedText(nationalIdByUserId.DocumentRejectionReason.ReasonEn, nationalIdByUserId.DocumentRejectionReason.ReasonAr, _identityService.Language) : null,
                        RejectionComment = nationalIdByUserId.RejectionComment
                    };
                }
                else
                    driverProfileViewModel.NationalId = new NationalIdViewModel();

                if (uberDriverProfile != null)
                {
                    if (uberDriverProfile.DrivingLicense != null)
                    {
                        driverProfileViewModel.DrivingLicense = new DrivingLicenseViewModel()
                        {
                            VerificationStatus = uberDriverProfile.DrivingLicense.VerificationStatus,
                            BackImage = new ImageViewModel(uberDriverProfile.DrivingLicense.BackImage, _configurationManager.GetImagesRootFolder()),
                            FrontImage = new ImageViewModel(uberDriverProfile.DrivingLicense.FrontImage, _configurationManager.GetImagesRootFolder()),
                            SelfieImage = !string.IsNullOrEmpty(uberDriverProfile.DrivingLicense.SelfieImage) ? new ImageViewModel(uberDriverProfile.DrivingLicense.SelfieImage, _configurationManager.GetImagesRootFolder()) : null,
                            Number = uberDriverProfile.DrivingLicense.Number,
                            ExpiryDate = uberDriverProfile.DrivingLicense.ExpiryDate.HasValue ? uberDriverProfile.DrivingLicense.ExpiryDate.Value.ToString("yyyyMMddHHmmss") : null,
                            IssueDate = uberDriverProfile.DrivingLicense.IssueDate.HasValue ? uberDriverProfile.DrivingLicense.IssueDate.Value.ToString("yyyyMMddHHmmss") : null,
                            RejectionReason = uberDriverProfile.DrivingLicense.DocumentRejectionReason != null ? LocalizationUtility.GetLocalizedText(uberDriverProfile.DrivingLicense.DocumentRejectionReason.ReasonEn, uberDriverProfile.DrivingLicense.DocumentRejectionReason.ReasonAr, _identityService.Language) : null,
                            RejectionComment = uberDriverProfile.DrivingLicense.RejectionComment
                        };
                    }

                    if (uberDriverProfile.CriminalRecord != null)
                    {
                        driverProfileViewModel.CriminalRecord = new CriminalRecordViewModel()
                        {
                            FrontImage = new ImageViewModel(uberDriverProfile.CriminalRecord.ScannedCopyImage, _configurationManager.GetImagesRootFolder()),
                            BackImage = !string.IsNullOrEmpty(uberDriverProfile.CriminalRecord.ScannedBackImage) ? new ImageViewModel(uberDriverProfile.CriminalRecord.ScannedBackImage, _configurationManager.GetImagesRootFolder()) : null,
                            VerificationStatus = uberDriverProfile.CriminalRecord.VerificationStatus,
                            ExpiryDate = uberDriverProfile.CriminalRecord.ExpiryDate.HasValue ? uberDriverProfile.CriminalRecord.ExpiryDate.Value.ToString("yyyyMMddHHmmss") : null,
                            IssueDate = uberDriverProfile.CriminalRecord.IssueDate.HasValue ? uberDriverProfile.CriminalRecord.IssueDate.Value.ToString("yyyyMMddHHmmss") : null,
                            RejectionReason = uberDriverProfile.CriminalRecord.DocumentRejectionReason != null ? LocalizationUtility.GetLocalizedText(uberDriverProfile.CriminalRecord.DocumentRejectionReason.ReasonEn, uberDriverProfile.CriminalRecord.DocumentRejectionReason.ReasonAr, _identityService.Language) : null,
                            RejectionComment = uberDriverProfile.CriminalRecord.RejectionComment
                        };
                    }
                    else
                    {
                        driverProfileViewModel.CriminalRecord = new CriminalRecordViewModel()
                        {
                            VerificationStatus = DocumentVerificationStatus.NotSubmitted
                        };
                    }

                    var userDocuments = _userDocumentsRepository.GetDocumentsByApplicationUserId(uberDriverProfile.ApplicationUserId);
                    if (userDocuments != null && userDocuments.Count > 0 && uberDriverProfile != null)
                    {
                        var userResidency = new ResidencyViewModel();
                        userResidency.Documents = new List<DocumentViewModel>();

                        foreach (var userDocument in userDocuments)
                            userResidency.Documents.Add(new DocumentViewModel()
                            {
                                ScannedCopyFrontImage = new ImageViewModel(userDocument.ScannedCopyFrontImage, _configurationManager.GetImagesRootFolder()),
                                DocumentCode = userDocument.Code,
                                VerificationStatus = userDocument.VerificationStatus
                            });

                        //userResidency.VerificationStatus = userDocuments.Any(d => d.VerificationStatus == DocumentVerificationStatus.Rejected) ? DocumentVerificationStatus.Rejected :
                        //    userDocuments.Any(d => d.VerificationStatus == DocumentVerificationStatus.Resubmitted) ? DocumentVerificationStatus.Resubmitted :
                        //    userDocuments.Any(d => d.VerificationStatus == DocumentVerificationStatus.Submitted) ? DocumentVerificationStatus.Submitted :
                        //    userDocuments.All(d => d.VerificationStatus == DocumentVerificationStatus.Verified) ? DocumentVerificationStatus.Verified : DocumentVerificationStatus.NotSubmitted;
                        userResidency.VerificationStatus = uberDriverProfile.UserDocumentsVerificationStatus;

                        userResidency.CityCode = user.CityCode;
                        userResidency.AreaCode = user.AreaCode;
                        userResidency.AddressDetails = user.AddressDetails;

                        driverProfileViewModel.Residency = userResidency;
                    }
                    else
                    {
                        var userResidency = new ResidencyViewModel();
                        userResidency.VerificationStatus = DocumentVerificationStatus.NotSubmitted;
                        driverProfileViewModel.Residency = userResidency;
                    }

                }
            }
            //commented as mobile does not display category name till now 
            //var modelPriceCategories = await _vehicleHandler.GetModelPriceCategories(Convert.ToString(_identityService.Language));
            if (uberDriverProfile != null)
            {
                var uberProfile = new UberProfileViewModel();

                uberProfile.PreferredModelPriceCategoryCode = uberDriverProfile.PreferredModelPriceCategoryCode;
                //uberProfile.PreferredModelPriceCategoryName = uberProfile.PreferredModelPriceCategoryCode != null ? modelPriceCategories.Where(m => m.Code == uberProfile.PreferredModelPriceCategoryCode).FirstOrDefault()?.CategoryName : null;
                uberProfile.UberVerificationStatus = uberDriverProfile.UberVerificationStatus;

                if (uberDriverProfile.IsUberDriver.HasValue)
                {
                    if (user_id != _identityService.UserId) // if owner/another driver trying to view this driver 
                    {
                        if (uberDriverProfile.UberVerificationStatus == DocumentVerificationStatus.Verified)
                        {
                            uberProfile.UberNumberOfTrips = uberDriverProfile.UberNumberOfTrips;
                            uberProfile.UberRating = uberDriverProfile.UberRating;
                            uberProfile.UberScanImage = new ImageViewModel(uberDriverProfile.UberScanImage, _configurationManager.GetImagesRootFolder());
                        }
                    }
                    else  // if driver trying to view his profile
                    {
                        uberProfile.UberNumberOfTrips = uberDriverProfile.UberNumberOfTrips;
                        uberProfile.UberRating = uberDriverProfile.UberRating;
                        uberProfile.UberScanImage = new ImageViewModel(uberDriverProfile.UberScanImage, _configurationManager.GetImagesRootFolder());
                    }

                    uberProfile.UberId = uberDriverProfile.UberId;
                    uberProfile.IsUberDriver = uberDriverProfile.IsUberDriver;
                    driverProfileViewModel.UberProfile = uberProfile;
                }
                else
                    driverProfileViewModel.UberProfile = null;
            }

            //StringBuilder url = new StringBuilder();
            //url.Append(_configurationManager.GetVehicleUrl()).Append(_configurationManager.GetDriverProfileUrl());
            ////url.Append("http://localhost:5110/api/vehicle/trip/driver_profile");

            //HttpGetRequest request = new HttpGetRequest(url.ToString(), new { driverId = user_id }, new List<KeyValuePair<string, string>>());
            //var result = await _webRequestUtility.Get<DriverProfileDetailsResponseViewModel>(request, JsonNamingStrategy.SnakeCase);
            //if (result != null)
            //{
            //    driverProfileViewModel.NumberOfTrips = result.TripsCount;
            //    driverProfileViewModel.TripsDaysCount = result.TripsDaysCount;
            //    driverProfileViewModel.TripsOdometer = result.TripsDistanceDriven;
            //}

            driverProfileViewModel.NumberOfTrips = user.DriverProfile.TripsCount;
            driverProfileViewModel.TripsDaysCount = user.DriverProfile.TripsDaysCount;
            driverProfileViewModel.TripsOdometer = user.DriverProfile.TripsDistanceDriven;

            return new Response<DriverProfileResponseViewModel>()
            {
                Data = driverProfileViewModel
            };
        }

        /// <summary>
        /// update owner profile
        /// </summary>
        /// <param name="owner_profile"></param>
        /// <response code="200 (error_code=1)"></response>
        /// <response code="200 (error_code=10)">Invalid Parameter</response>
        /// <response code="200 (error_code=11)">Missing Parameter</response>
        /// <response code="200 (error_code=15)">Invalid Operation</response>
        ///  <response code="200 (error_code=126)">Not Allowed to update Verified Phone Number</response>
        /// <response code="200 (error_code=127)">Not Allowed to update Verified Email</response>
        /// <response code="200 (error_code=129)">User not found</response>
        [HttpPost]
        [Authorize(Roles = "CarOwner")]
        [Route("update_owner_profile")]
        public async Task<BaseResponse> UpdateOwnerProfile([FromBody]OwnerProfileRequestViewModel owner_profile, [FromHeader(Name = "Language")] string language)
        {
            if (owner_profile == null)
                throw new MissingParameterException("owner_profile");

            if (!ModelState.IsValid)
                throw new InvalidParameterException(nameof(owner_profile), "", _commonAccountManager.GetErrorsFromModelState(ModelState));
            var user = _userManager.Users.FirstOrDefault(u => u.Id == _identityService.UserId);

            if (user == null)
                throw new UserNotFoundException();

            // Roles
            var userRoles = await _userManager.GetRolesAsync(user);
            if (userRoles == null)
                throw new InternalServerErrorException("No roles found for user: " + user.Id);

            if (!userRoles.Any(role => role == "CarOwner"))
                throw new Emeint.Core.BE.Domain.Exceptions.InvalidOperationException("This user_id is not for car owner", "هذا المستخدم ليس لصاحب سيارة");

            //check if driver name,phone,image has updated then pblishDriverInfo will be true
            bool publishOwnerInfoUpdates = false;

            // Phone number
            if (user.PhoneNumber != owner_profile.PhoneNumber)
            {
                if (user.PhoneNumberConfirmed)
                    throw new UpdatePhoneNumberNotAllowedException();

                if (user.CountryCode == "EGY")
                {
                    var egyptianPhoneNumberPattern = "^01[0|1|2|5]{1}[0-9]{8}";
                    var egyptianPhoneNumberRegex = new Regex(egyptianPhoneNumberPattern, RegexOptions.None);
                    Match egyptianPhoneMatch = egyptianPhoneNumberRegex.Match(owner_profile.PhoneNumber);
                    if (!egyptianPhoneMatch.Success)
                        throw new InvalidParameterException("mobile_number", owner_profile.PhoneNumber);
                }
                publishOwnerInfoUpdates = true;
                user.PhoneNumber = owner_profile.PhoneNumber;
            }

            // Email
            //if (user.Email != owner_profile.Email)
            //{
            //    if (user.EmailConfirmed)
            //        throw new UpdateEmailNotAllowedException();

            //    EmailUtility.ValidateEmail(owner_profile.Email);

            //    user.Email = owner_profile.Email;
            //}

            //social links
            if (!string.IsNullOrEmpty(owner_profile.FacebookLink))
            {
                if (UrlUtility.IsFacebookProfileUrl(owner_profile.FacebookLink) == false)
                    throw new InvalidUrlException(owner_profile.FacebookLink);
            }
            if (!string.IsNullOrEmpty(owner_profile.GoogleLink))
            {
                if (UrlUtility.IsGoogleProfileUrl(owner_profile.GoogleLink) == false)
                    throw new InvalidUrlException(owner_profile.GoogleLink);
            }

            // Basic info

            bool editFirstNameAllowed = true;
            bool editLastNameAllowed = true;
            //bool editProfileImageAllowed = true;
            //bool editGenderAllowed = true;
            //bool editAddressAllowed = true;

            var nationalId = _nationalIdRepository.GetNationalIdByUserId(user.Id);
            // Validate verification status
            if (nationalId != null)
            {
                if (nationalId.VerificationStatus == DocumentVerificationStatus.Submitted ||
                    nationalId.VerificationStatus == DocumentVerificationStatus.Resubmitted ||
                    nationalId.VerificationStatus == DocumentVerificationStatus.Verified)
                {
                    editFirstNameAllowed = false;
                    editLastNameAllowed = false;
                    //editDateOfBirthAllowed = false;
                    //editGenderAllowed = false;
                }
            }

            // First name
            if (editFirstNameAllowed)
            {
                if (user.FirstName != owner_profile.FirstName)
                    publishOwnerInfoUpdates = true;

                user.FirstName = owner_profile.FirstName;
            }
            else
            {
                if (user.FirstName != owner_profile.FirstName)
                    throw new EditingProfileFieldException("first_name");
            }

            // Last name
            if (editLastNameAllowed)
            {
                if (user.LastName != owner_profile.LastName)
                    publishOwnerInfoUpdates = true;

                user.LastName = owner_profile.LastName;
            }
            else
            {
                if (user.LastName != owner_profile.LastName)
                    throw new EditingProfileFieldException("last_name");
            }

            // Gender
            //if (editGenderAllowed)
            //{
            //    user.Gender = owner_profile.Gender;
            //}
            //else
            //{
            //    if (user.Gender != owner_profile.Gender)
            //        throw new EditingProfileFieldException("gender");
            //}

            // Profile image
            //if (editProfileImageAllowed)
            //{
            if (owner_profile.ImageCode != null)
            {
                bool profileImageExists = _imageRepository.ImageCodeExists(owner_profile.ImageCode);
                if (!profileImageExists)
                    throw new InvalidParameterException("image_code", owner_profile.ImageCode);

                if (user.ProfileImage != owner_profile.ImageCode)
                    publishOwnerInfoUpdates = true;

                user.ProfileImage = owner_profile.ImageCode;
            }
            else //Allow Owner to delete his image
            {
                if (user.ProfileImage != owner_profile.ImageCode)
                    publishOwnerInfoUpdates = true;

                user.ProfileImage = owner_profile.ImageCode;
            }

            //}
            //else
            //{
            //    if (user.ProfileImage != owner_profile.ImageCode)
            //        throw new EditingProfileFieldException("profile_image_code");
            //}

            user.FacebookLink = owner_profile.FacebookLink;
            user.GoogleLink = owner_profile.GoogleLink;

            //check if owner name,phone,image has updated, then call vehicle to update
            if (publishOwnerInfoUpdates == true)
            {
                var ownerImageUrl = new ImageViewModel(owner_profile.ImageCode, _configurationManager.GetImagesRootFolder()).Url;
                await _vehicleHandler.UpdateOwnerInfo(_identityService.UserId, owner_profile.PhoneNumber, ownerImageUrl, owner_profile.FirstName + " " + owner_profile.LastName);
            }

            //Address
            //if (owner_profile.Address == null)
            //    throw new MissingParameterException("address");

            ////user.CountryCode = owner_profile.Address.CountryCode;
            //user.CityCode = owner_profile.Address.CityCode;
            //user.AreaCode = owner_profile.Address.AreaCode;
            //user.AddressDetails = owner_profile.Address.Details;
            //user.Gps

            // National Id
            //if (owner_profile.NationalId != null &&
            //    owner_profile.NationalId.BackImage != null &&
            //    owner_profile.NationalId.FrontImage != null &&
            //    owner_profile.NationalId.SelfieImage != null &&
            //    owner_profile.NationalId.NationalNumber != null)
            //{
            //    if (nationalId == null ||
            //        nationalId.BackImage != owner_profile.NationalId.BackImage ||
            //        nationalId.FrontImage != owner_profile.NationalId.FrontImage ||
            //        nationalId.SelfieImage != owner_profile.NationalId.SelfieImage ||
            //        nationalId.NationalNumber != owner_profile.NationalId.NationalNumber)
            //    {
            //        NationalIdRequestViewModelDEPRECATED nationalIdVM = new NationalIdRequestViewModelDEPRECATED();
            //        nationalIdVM.BackImage = owner_profile.NationalId.BackImage;
            //        nationalIdVM.FrontImage = owner_profile.NationalId.FrontImage;
            //        nationalIdVM.SelfieImage = owner_profile.NationalId.SelfieImage;
            //        nationalIdVM.DateOfBirth = owner_profile.NationalId.DateOfBirth;
            //        nationalIdVM.ExpiryDate = owner_profile.NationalId.ExpiryDate;
            //        nationalIdVM.NationalNumber = owner_profile.NationalId.NationalNumber;

            //       await UpdateNationalId(nationalIdVM, _identityService.Language.ToString());
            //    }
            //}

            await _userManager.UpdateAsync(user);

            return new BaseResponse()
            {
                ErrorCode = (int)ErrorCodes.Success
            };
        }

        /// <summary>
        /// update driver profile
        /// </summary>
        /// <param name="driver_profile"></param>
        /// <response code="200 (error_code=1)"></response>
        /// <response code="200 (error_code=10)">Invalid Parameter</response>
        /// <response code="200 (error_code=11)">Missing Parameter</response>
        /// <response code="200 (error_code=15)">Invalid Operation</response>
        ///  <response code="200 (error_code=126)">Not Allowed to update Verified Phone Number</response>
        /// <response code="200 (error_code=127)">Not Allowed to update Verified Email</response>
        /// <response code="200 (error_code=129)">User not found</response>
        [HttpPost]
        [Authorize(Roles = "UberDriver,Driver")]
        [Route("update_driver_profile")]
        public async Task<BaseResponse> UpdateDriverProfile([FromBody]DriverProfileRequestViewModel driver_profile, [FromHeader(Name = "Language")] string language)
        {
            if (driver_profile == null)
                throw new MissingParameterException("driver_profile");

            if (!ModelState.IsValid)
                throw new InvalidParameterException(nameof(driver_profile), "", _commonAccountManager.GetErrorsFromModelState(ModelState));

            var user = _userManager.Users.FirstOrDefault(u => u.Id == _identityService.UserId);
            if (user == null)
                throw new UserNotFoundException();

            DateTime expiryDate = DateTime.UtcNow;
            if (driver_profile.DrivingLicense != null && !string.IsNullOrEmpty(driver_profile.DrivingLicense.ExpiryDate))
            {
                if (DateTime.TryParseExact(driver_profile.DrivingLicense.ExpiryDate, "yyyyMMddHHmmss", null, DateTimeStyles.None, out expiryDate) == false)
                    throw new InvalidParameterException("expiryDate", driver_profile.DrivingLicense.ExpiryDate);

                //Remove Expiry Validation
                //if (expiryDate < DateTime.Today)
                //    throw new DrivingLicenseExpiredException();
            }

            // Roles
            var userRoles = await _userManager.GetRolesAsync(user);
            if (userRoles == null)
                throw new InternalServerErrorException("No roles found for user: " + user.Id);

            if (!userRoles.Any(role => role == "Driver" || role == "UberDriver"))
                throw new Emeint.Core.BE.Domain.Exceptions.InvalidOperationException("This user_id is not for driver", "هذا المستخدم ليس  لسائق سيارة");

            //check if driver name,phone,image has updated then pblishDriverInfo will be true
            bool publishDriverInfoUpdates = false;
            // Phone number
            if (user.PhoneNumber != driver_profile.PhoneNumber)
            {
                if (user.PhoneNumberConfirmed)
                    throw new UpdatePhoneNumberNotAllowedException();

                if (user.CountryCode == "EGY")
                {
                    var egyptianPhoneNumberPattern = "^01[0|1|2|5]{1}[0-9]{8}";
                    var egyptianPhoneNumberRegex = new Regex(egyptianPhoneNumberPattern, RegexOptions.None);
                    Match egyptianPhoneMatch = egyptianPhoneNumberRegex.Match(driver_profile.PhoneNumber);
                    if (!egyptianPhoneMatch.Success)
                        throw new InvalidParameterException("mobile_number", driver_profile.PhoneNumber);
                }

                publishDriverInfoUpdates = true;
                user.PhoneNumber = driver_profile.PhoneNumber;
            }

            // Email
            //if (user.Email != driver_profile.Email)
            //{
            //    if (user.EmailConfirmed)
            //        throw new UpdateEmailNotAllowedException();

            //    EmailUtility.ValidateEmail(driver_profile.Email);

            //    user.Email = driver_profile.Email;
            //}

            //social links
            if (!string.IsNullOrEmpty(driver_profile.FacebookLink))
            {
                if (UrlUtility.IsFacebookProfileUrl(driver_profile.FacebookLink) == false)
                    throw new InvalidUrlException(driver_profile.FacebookLink);
            }
            if (!string.IsNullOrEmpty(driver_profile.GoogleLink))
            {
                if (UrlUtility.IsGoogleProfileUrl(driver_profile.GoogleLink) == false)
                    throw new InvalidUrlException(driver_profile.GoogleLink);
            }
            // Basic info

            bool editFirstNameAllowed = true;
            bool editLastNameAllowed = true;
            bool editProfileImageAllowed = true;
            bool editDateOfBirthAllowed = true;
            //bool editAddressAllowed = true;
            //bool editGenderAllowed = true;

            var nationalId = _nationalIdRepository.GetNationalIdByUserId(user.Id);
            // Validate verification status
            if (nationalId != null)
            {
                if (nationalId.VerificationStatus == DocumentVerificationStatus.Submitted ||
                    nationalId.VerificationStatus == DocumentVerificationStatus.Resubmitted ||
                    nationalId.VerificationStatus == DocumentVerificationStatus.Verified)
                {
                    editFirstNameAllowed = false;
                    editLastNameAllowed = false;
                    editDateOfBirthAllowed = false;

                    //allow driver to update his photo if only he did not upload it before
                    if (user.ProfileImage != null)
                        editProfileImageAllowed = false;

                    //editGenderAllowed = false;
                }
                else if (nationalId?.VerificationStatus == DocumentVerificationStatus.NotSubmitted ||
                         nationalId?.VerificationStatus == DocumentVerificationStatus.Rejected ||
                         nationalId?.VerificationStatus == DocumentVerificationStatus.Expired)
                {
                    editProfileImageAllowed = true;
                    editFirstNameAllowed = true;
                    editLastNameAllowed = true;
                    editDateOfBirthAllowed = true;
                }
            }


            // First name
            if (editFirstNameAllowed)
            {
                if (user.FirstName != driver_profile.FirstName)
                    publishDriverInfoUpdates = true;

                user.FirstName = driver_profile.FirstName;
            }
            else
            {
                if (user.FirstName != driver_profile.FirstName)
                    throw new EditingProfileFieldException("first_name");
            }

            // Last name
            if (editLastNameAllowed)
            {
                if (user.LastName != driver_profile.LastName)
                    publishDriverInfoUpdates = true;

                user.LastName = driver_profile.LastName;
            }
            else
            {
                if (user.LastName != driver_profile.LastName)
                    throw new EditingProfileFieldException("last_name");
            }

            // Gender
            //if (editGenderAllowed)
            //{
            //    user.Gender = driver_profile.Gender;
            //}
            //else
            //{
            //    if (user.Gender != driver_profile.Gender)
            //        throw new EditingProfileFieldException("gender");
            //}

            // Date of birth
            DateTime dateOfBirth;
            if (driver_profile.NationalId != null)
            {
                if (DateTime.TryParseExact(driver_profile.NationalId.DateOfBirth, "yyyyMMddHHmmss", null, DateTimeStyles.None, out dateOfBirth) == false)
                    throw new InvalidParameterException("date_of_birth", driver_profile.NationalId.DateOfBirth);

                if (editDateOfBirthAllowed)
                {
                    user.DateOfBirth = DateTime.ParseExact(driver_profile.NationalId.DateOfBirth, "yyyyMMddHHmmss", null); // TODO: What if invalid format?
                }
                else
                {
                    if (user.DateOfBirth != dateOfBirth)
                        throw new EditingProfileFieldException("date_of_birth");
                }
            }

            // Profile image
            if (editProfileImageAllowed || user.ProfileImage == null)
            {
                bool profileImageExists = _imageRepository.ImageCodeExists(driver_profile.ImageCode);
                if (!profileImageExists)
                    throw new InvalidParameterException("image_code", driver_profile.ImageCode);

                if (user.ProfileImage != driver_profile.ImageCode)
                    publishDriverInfoUpdates = true;

                user.ProfileImage = driver_profile.ImageCode;
            }
            else
            {
                if (user.ProfileImage != driver_profile.ImageCode)
                    throw new EditingProfileFieldException("profile_image_code");
            }

            // TODO: check if URL
            user.FacebookLink = driver_profile.FacebookLink;
            user.GoogleLink = driver_profile.GoogleLink;

            //check if driver name,phone,image has updated, then call vehicle to update
            if (publishDriverInfoUpdates == true)
            {
                var driverImageUrl = new ImageViewModel(driver_profile.ImageCode, _configurationManager.GetImagesRootFolder()).Url;
                await _vehicleHandler.UpdateDriverInfo(_identityService.UserId, driver_profile.PhoneNumber, driverImageUrl, driver_profile.FirstName + " " + driver_profile.LastName);
            }

            // Address
            //if (driver_profile.Address == null)
            //    throw new MissingParameterException("address");

            ////user.CountryCode = driver_profile.Address.CountryCode;
            //user.CityCode = driver_profile.Address.CityCode;
            //user.AreaCode = driver_profile.Address.AreaCode;
            //user.AddressDetails = driver_profile.Address.Details;
            //user.Gps

            // National Id
            //if (driver_profile.NationalId != null)
            //{
            //    if (nationalId.BackImage != driver_profile.NationalId.BackImage ||
            //        nationalId.FrontImage != driver_profile.NationalId.FrontImage ||
            //        nationalId.SelfieImage != driver_profile.NationalId.SelfieImage ||
            //        nationalId.NationalNumber != driver_profile.NationalId.NationalNumber)
            //    {
            //        NationalIdRequestViewModelDEPRECATED nationalIdVM = new NationalIdRequestViewModelDEPRECATED();

            //        await UpdateNationalId(nationalIdVM, _identityService.Language.ToString());
            //    }
            //}

            // driving license
            //await UpdateDrivingLicense(driver_profile.DrivingLicense, _identityService.Language.ToString());

            await _userManager.UpdateAsync(user);

            return new BaseResponse()
            {
                ErrorCode = (int)ErrorCodes.Success
            };
        }

        /// <summary>
        /// Used to upload driving license,Expiry date format is yyyyMMdd
        /// </summary>
        /// <param name="driving_license"></param>       
        /// <response code="200 (error_code=10)">Invalid Parameter</response>
        /// <response code="200 (error_code=11)">Missing Parameter</response>
        ///// <response code="200 (error_code=128)">Driving License Number Already Exists</response>
        /// <response code="200 (error_code=136)">Driving License Number Already Exists</response>
        [HttpPost]
        [Authorize(Roles = "UberDriver, Driver")]
        [Route("update_driving_license")]
        public async Task<BaseResponse> UpdateDrivingLicense([FromBody] DrivingLicenseRequestViewModel driving_license, [FromHeader(Name = "Language")] string language)
        {
            if (driving_license == null)
                throw new MissingParameterException("driving_license");

            if (!ModelState.IsValid)
                throw new InvalidParameterException(nameof(driving_license), "", _commonAccountManager.GetErrorsFromModelState(ModelState));

            // Validate images
            bool frontImageExists = _imageRepository.ImageCodeExists(driving_license.FrontImageCode);
            if (!frontImageExists)
                throw new InvalidParameterException("front_image", driving_license.FrontImageCode);

            bool backImageExists = _imageRepository.ImageCodeExists(driving_license.BackImageCode);
            if (!backImageExists)
                throw new InvalidParameterException("back_image", driving_license.BackImageCode);

            if (!string.IsNullOrEmpty(driving_license.SelfieImage))
            {
                bool selfieImageExists = _imageRepository.ImageCodeExists(driving_license.SelfieImage);
                if (!selfieImageExists)
                    throw new InvalidParameterException("selfie_image", driving_license.SelfieImage);
            }

            var uberDriverProfile = _profileRepository.GetUberProfileByUserId(_identityService.UserId);

            DateTime expiryDate = DateTime.UtcNow;
            if (!string.IsNullOrEmpty(driving_license.ExpiryDate))
            {
                if (DateTime.TryParseExact(driving_license.ExpiryDate, "yyyyMMddHHmmss", null, DateTimeStyles.None, out expiryDate) == false)
                    throw new InvalidParameterException("expiryDate", driving_license.ExpiryDate);

                //if (expiryDate < DateTime.Today)
                //    throw new InvalidParameterException("expiryDate", driving_license.ExpiryDate);
            }

            DateTime issueDate = DateTime.UtcNow;
            if (!string.IsNullOrEmpty(driving_license.IssueDate))
            {
                if (DateTime.TryParseExact(driving_license.IssueDate, "yyyyMMddHHmmss", null, DateTimeStyles.None, out issueDate) == false)
                    throw new InvalidParameterException("issueDate", driving_license.IssueDate);

                if (issueDate > DateTime.Today)
                    throw new InvalidParameterException("issueDate", driving_license.IssueDate);
            }

            //TODO:Check Format of Driving Number

            //check if Driving License is Already Exists
            bool drivingLicenseNumberExist = _profileRepository.DrivingLicenseNumberExists(driving_license.Number, uberDriverProfile.ApplicationUserId);
            if (!drivingLicenseNumberExist)
            {
                uberDriverProfile.DrivingLicense.Number = driving_license.Number;
            }
            else
            {
                throw new DrivingLicenseDuplicateNumberException();
            }

            if (uberDriverProfile != null)
            {
                if (uberDriverProfile.DrivingLicense != null)
                {
                    if (uberDriverProfile.DrivingLicense.VerificationStatus == DocumentVerificationStatus.Verified)
                    {
                        if (uberDriverProfile.DrivingLicense.Number != driving_license.Number ||
                            uberDriverProfile.DrivingLicense.ExpiryDate != (string.IsNullOrEmpty(driving_license.ExpiryDate) ? (DateTime?)null : expiryDate) ||
                            uberDriverProfile.DrivingLicense.IssueDate != (string.IsNullOrEmpty(driving_license.IssueDate) ? (DateTime?)null : issueDate) ||
                            uberDriverProfile.DrivingLicense.BackImage != driving_license.BackImageCode ||
                            uberDriverProfile.DrivingLicense.FrontImage != driving_license.FrontImageCode ||
                            uberDriverProfile.DrivingLicense.SelfieImage != driving_license.SelfieImage)
                            throw new UpdateDrivingLicenseNotAllowedException();
                    }
                    // Update
                    else if (uberDriverProfile.DrivingLicense.VerificationStatus == DocumentVerificationStatus.Rejected)
                    {
                        uberDriverProfile.DrivingLicense.VerificationStatus = DocumentVerificationStatus.Resubmitted;
                        uberDriverProfile.DrivingLicense.ReSubmissionDate = DateTime.UtcNow;
                    }
                    else
                    {
                        uberDriverProfile.DrivingLicense.VerificationStatus = DocumentVerificationStatus.Submitted;
                        uberDriverProfile.DrivingLicense.SubmissionDate = DateTime.UtcNow;
                    }
                    uberDriverProfile.ApplicationUserId = _identityService.UserId;
                    uberDriverProfile.DrivingLicense.FrontImage = driving_license.FrontImageCode;
                    uberDriverProfile.DrivingLicense.BackImage = driving_license.BackImageCode;
                    uberDriverProfile.DrivingLicense.SelfieImage = driving_license.SelfieImage;
                    uberDriverProfile.DrivingLicense.Number = driving_license.Number;
                    uberDriverProfile.DrivingLicense.ExpiryDate = string.IsNullOrEmpty(driving_license.ExpiryDate) ? (DateTime?)null : expiryDate;
                    uberDriverProfile.DrivingLicense.IssueDate = string.IsNullOrEmpty(driving_license.IssueDate) ? (DateTime?)null : issueDate;

                }
                // Add
                else
                {
                    uberDriverProfile.DrivingLicense = new DrivingLicense
                    {
                        BackImage = driving_license.BackImageCode,
                        FrontImage = driving_license.FrontImageCode,
                        SelfieImage = driving_license.SelfieImage,
                        Number = driving_license.Number,
                        ExpiryDate = string.IsNullOrEmpty(driving_license.ExpiryDate) ? (DateTime?)null : expiryDate,
                        IssueDate = string.IsNullOrEmpty(driving_license.IssueDate) ? (DateTime?)null : issueDate,
                        VerificationStatus = DocumentVerificationStatus.Submitted,
                        SubmissionDate = DateTime.UtcNow
                    };
                }

                _profileRepository.Update(uberDriverProfile);
                _profileRepository.SaveEntities();
            }
            else
            {
                throw new UserProfileNotFoundException(_identityService.UserId);
            }

            return new BaseResponse
            {
                ErrorCode = (int)ErrorCodes.Success,
            };
        }


        /// <summary>
        /// Used to delete user's own account
        /// </summary>
        //[Route("delete_my_account")]
        //[Authorize]
        //[HttpGet]
        //public async Task<Response<BaseResponse>> DeleteMyAccount()
        //{
        //    var user = _userManager.Users.FirstOrDefault(u => u.Id == _identityService.UserId);
        //    if (user == null)
        //        throw new UserNotFoundException();

        //    await _userManager.DeleteAsync(user);

        //    // TODO: Remove Roles
        //    // TODO: Remove Profile
        //    // TODO: Remove National Id

        //    return new Response<BaseResponse>();
        //}

        [HttpPost]
        [Authorize(Roles = "CarOwner, UberDriver, Driver")]
        [Route("rate_review_user")]
        public async Task<BaseResponse> RatingReviewUser([FromBody]RateReviewUserViewModel user_rating_review)
        {
            if (user_rating_review == null)
                throw new MissingParameterException("user_rating_and_review");
            if (string.IsNullOrEmpty(user_rating_review.RevieweeId))
                throw new MissingParameterException("reviewee_id");
            if (string.IsNullOrEmpty(user_rating_review.HostNumber))
                throw new MissingParameterException("host_number");

            var revieweeUser = _userManager.Users.FirstOrDefault(u => u.Id == user_rating_review.RevieweeId);
            if (revieweeUser == null)
                throw new UserNotFoundException();

            var reviewerUserId = _identityService.UserId;
            if (user_rating_review.RevieweeId == reviewerUserId)
                throw new InvalidOperationException("You Can't Rate yourself .", "لا يمكن تقييم نفسك .");
            var reviewerUser = _userManager.Users.FirstOrDefault(u => u.Id == reviewerUserId);
            var response = await _vehicleHandler.ValidateUserRating(user_rating_review.HostNumber, reviewerUserId, user_rating_review.RevieweeId);
            if (response.Data != null && response.Data.IsValid)
            {
                try
                {
                    //decimal allRates = 0m;
                    decimal ratingAverage = 0m;
                    _ratingAndReviewRepository.Add(new RatingAndReview()
                    {
                        CreatedBy = $"{reviewerUser.FirstName} {reviewerUser.LastName}",
                        Rating = user_rating_review.Rate,
                        Review = user_rating_review.Review,
                        RevieweeUser = revieweeUser,
                        ReviewerUser = reviewerUser,
                        HostNumber = user_rating_review.HostNumber,
                        RevieweeRole = response.Data.RevieweeRole
                    });

                    var rates = _ratingAndReviewRepository.RevieweeRates(revieweeUser.Id, null, response.Data.RevieweeRole).ToList();
                    if (rates != null && rates.Any(r => r.HostNumber == user_rating_review.HostNumber && r.ReviewerUser.Id == _identityService.UserId))
                        throw new InvalidOperationException("You can't rate the same user more than once.", "لا يمكنك تقييم نفس المستخدم أكثر من مرة .");
                    if (rates != null)
                    {
                        ratingAverage = rates.Count() >= _configurationManager.GetMinRatingsCount() ?
                            _ratingManager.CalculateRatingAverage(rates.Sum(r => (int)r.Rating), (int)user_rating_review.Rate, rates.Count())
                            : _configurationManager.GetDefaultRating();
                        if (response.Data.RevieweeRole == "owner")
                        {
                            var ownerProfile = _profileRepository.GetOwnerProfileById(user_rating_review.RevieweeId);
                            if (ownerProfile != null)
                                ownerProfile.RatingAverage = ratingAverage;
                            else
                                _profileRepository.Add(new OwnerProfile()
                                {
                                    ApplicationUserId = user_rating_review.RevieweeId,
                                    CreatedBy = _identityService.DisplayName,
                                    CreationDate = DateTime.UtcNow,
                                    RatingAverage = ratingAverage
                                });
                        }
                        else if (response.Data.RevieweeRole == "driver")
                        {
                            var driverProfile = _profileRepository.GetDriverProfileById(user_rating_review.RevieweeId);
                            if (driverProfile != null)
                                driverProfile.RatingAverage = ratingAverage;
                        }
                    }
                    //if (rates != null && rates.Count >= _configurationManager.GetMinRatingsCount())
                    //{

                    //    foreach (var rating in rates)
                    //    {
                    //        allRates += (int)rating.Rating;
                    //    }
                    //    ratingAverage = _ratingManager.CalculateRatingAverage(allRates, (int)user_rating_review.Rate, rates.Count);
                    //}
                    //else
                    //{
                    //    ratingAverage = _configurationManager.GetDefaultRating();
                    //}

                    //revieweeUser.RatingAverage = ratingAverage;
                    _ratingAndReviewRepository.SaveEntities();
                    return new BaseResponse()
                    {
                        ErrorCode = (int)ErrorCodes.Success
                    };
                }

                catch (Exception exception)
                {
                    _logger.LogError("An error has occurred while applying rating and review");
                    throw new InternalServerErrorException(exception.Message);
                }
            }

            throw new InvalidOperationException("make sure that you are involved in this trip ", "تأكد من أنك مشترك في هذه الرحلة");
        }



        [Route("reviewee_ratings_and_reviews")]
        //[Authorize]
        [HttpPost]
        [SkipIdentityFilter]
        public Response<RatingsAndReviewsResponseViewModel> RevieweeRatingsAndReviews([FromBody]RevieweeRatesRequestViewModel reviewee_rates_request, [FromHeader(Name = "Language")] string language)
        {
            RatingsAndReviewsResponseViewModel ratingsAndReviewsResponse = new RatingsAndReviewsResponseViewModel();
            List<RatingAndReview> ratingsAndReviews = new List<RatingAndReview>();

            if (reviewee_rates_request == null)
                throw new MissingParameterException("reviewee_rates_request");

            string userId = reviewee_rates_request.UserId;
            if (string.IsNullOrEmpty(userId))
                userId = _identityService.UserId;

            var rates = _ratingAndReviewRepository.RevieweeRates(userId, null, reviewee_rates_request.Role);
            ratingsAndReviewsResponse.Count = (rates.Count() > _configurationManager.GetMinRatingsCount()) ? rates.Count() : 0;


            if (ratingsAndReviewsResponse.Count > _configurationManager.GetMinRatingsCount())
            {
                if (reviewee_rates_request.Pagination == null)
                    ratingsAndReviews = rates.ToList();
                else
                {
                    var pageNumber = (int)((reviewee_rates_request?.Pagination?.PageNumber - 1) * reviewee_rates_request?.Pagination?.PageSize);
                    ratingsAndReviews = rates.Skip(pageNumber).Take(reviewee_rates_request.Pagination.PageSize.Value).ToList();
                }

                foreach (var rate in ratingsAndReviews)
                {
                    ratingsAndReviewsResponse.RatingsAndReviews.Add(new RatingViewModel(rate.Code, rate.Rating, rate.Review, rate.CreatedBy, rate.CreationDate.ToString("yyyyMMddHHmmss"), new ImageViewModel(rate.ReviewerUser.ProfileImage, _configurationManager.GetImagesRootFolder())));
                }
            }

            return new Response<RatingsAndReviewsResponseViewModel>()
            {
                Data = ratingsAndReviewsResponse
            };
        }

        [HttpGet]
        [Authorize]
        [Route("switch_role")]
        public async Task<BaseResponse> SwitchRole(string new_role, string uber_driver_id = null)
        {
            var countryCode = _identityService.CountryCode;
            var country = _countryService.GetCountryByCode(countryCode);
            if (country == null)
                throw new InvalidParameterException("country_code", countryCode);
            if (!country.AllowMultipleRoles)
                throw new MultipleRolesNotAllowedException(country.NameEn, country.NameAr);
            if (!country.CountryRoles.Any(r => r.RoleName == new_role))
                throw new UnSupportedRoleException(country.NameEn, new_role);

            if (new_role != "CarOwner" && new_role != "UberDriver" && new_role != "Driver")
                throw new InvalidParameterException("user_role", new_role);
            if (_identityService.UserRoles.Contains(new_role))
                throw new UserAlreadyExistException($" with this role: {new_role}");


            var currUser = _userManager.Users.Include(u => u.Roles).FirstOrDefault(u => u.Id == _identityService.UserId);
            if (currUser == null)
                throw new UserNotFoundException();

            if (new_role == "UberDriver" || new_role == "Driver")
            {
                //if(new_role == "UberDriver")
                //{
                //    if (string.IsNullOrEmpty(uber_driver_id))
                //        throw new MissingParameterException("uber_id");
                //    else
                //    {
                //        bool existingUberDriver = _userManager.Users.Any
                //            (u => (u.DriverProfile != null) && (u.DriverProfile.UberId.Trim().ToLower() == uber_driver_id.Trim().ToLower()));

                //        if (existingUberDriver)
                //            throw new UberIdAlreadyExistsException(uber_driver_id);
                //    }
                //}


                _profileRepository.Add(new DriverProfile()
                {
                    ApplicationUserId = currUser.Id,
                    UberId = uber_driver_id,
                    VerificationStatus = DocumentVerificationStatus.NotSubmitted,
                    CreatedBy = currUser.Id,
                    CreationDate = DateTime.UtcNow,
                    DrivingLicense = new DrivingLicense()
                    {
                        VerificationStatus = DocumentVerificationStatus.NotSubmitted
                    },
                    CriminalRecord = new CriminalRecord()
                    {
                        VerificationStatus = DocumentVerificationStatus.NotSubmitted
                    }
                });
            }

            #region Adding role in Payment 
            var response = await _paymentHandler.AddUserAccountRole(currUser.PhoneNumber, new_role);
            if (response != null && response.Data)
            {
                _profileRepository.SaveEntities();
                await _userManager.AddToRoleAsync(currUser, new_role);
            }
            else
                throw new InvalidOperationException("internal servcer error", "خطأ في الخادم");
            #endregion


            //TODO To be updated with payment
            //await _paymentHandler.CreateUserAccount(currUser.PhoneNumber, new_role);

            return new BaseResponse
            {
                ErrorCode = (int)ErrorCodes.Success
            };
        }

        [HttpGet]
        [Route("multiple_roles/is_allowed")]
        [Authorize]
        public async Task<Response<bool>> MultipleRolesAllowed()
        {
            var country = _countryService.GetCountryByCode(_identityService.CountryCode);
            if (country == null)
                throw new InvalidParameterException("country_code", _identityService.CountryCode);

            return new Response<bool>
            {
                Data = country.AllowMultipleRoles,
                ErrorCode = (int)ErrorCodes.Success
            };
        }

        [HttpGet]
        [Route("update_user_payment_method")]
        [Authorize]
        public async Task<BaseResponse> UpdateUserPaymentMethod(PaymentMethods payment_method, string credit_card_code)
        {
            await _commonAccountManager.UpdateUserPaymentMethod(_identityService.UserId, payment_method, credit_card_code);
            return new BaseResponse()
            {
                ErrorCode = (int)ErrorCodes.Success,
            };
        }

    }
}            */
