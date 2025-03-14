using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;
using RunningApplicationNew.Entity.Dtos;
using RunningApplicationNew.Entity;
using RunningApplicationNew.Helpers;
using System;
using RunningApplicationNew.DataLayer;
using RunningApplicationNew.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.Data;
using RunningApplicationNew.Services;


namespace RunningApplicationNew.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly IJwtHelper _jwtHelper;
        private readonly IEmailHelper _emailhelper;
        private readonly IPhotoService _photoService;


        public UserController(IUserRepository userRepository, IJwtHelper jwtHelper,IEmailHelper emailHelper,IPhotoService photoService)
        {
            _userRepository = userRepository;
            _jwtHelper = jwtHelper;
            _emailhelper = emailHelper;
            _photoService = photoService;
        }
        

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] CreateUserDTO createUserDTO)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Kullanıcıyı e-posta ile kontrol et
            var existingUser = await _userRepository.GetByEmailAsync(createUserDTO.Email);
            if (existingUser != null)
                return Conflict("Bu email zaten kayıtlı.");

            // Şifreyi hash'le
            var hashedPassword = HashPassword(createUserDTO.Password);

            // Yeni kullanıcıyı oluştur
            var newUser = new User
            {
                Name = createUserDTO.Name,
                SurName = createUserDTO.SurName,
                UserName = createUserDTO.UserName,
                Email = createUserDTO.Email,
                PhoneNumber = createUserDTO.PhoneNumber,
                Address = createUserDTO.Address,
                Age = createUserDTO.Age,
                Height = createUserDTO.Height,
                Weight = createUserDTO.Weight,
                PasswordHash = hashedPassword,
                IsActive =true
            };

            await _userRepository.AddAsync(newUser);
            await _userRepository.SaveChangesAsync();

            var token = _jwtHelper.GenerateJwtToken(newUser);
            return Ok(new { Token = token });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginUserDTO loginUserDTO)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            

            // Kullanıcıyı e-posta ile al
            var user = await _userRepository.GetByEmailAsync(loginUserDTO.Email);

         
         
            if (user == null)
                return Unauthorized("Kullanıcı bulunamadı.");

            // Şifre doğrulaması
            if (!VerifyPassword(loginUserDTO.Password, user.PasswordHash))
                return Unauthorized("Geçersiz şifre.");

            // JWT token oluştur
            var token = _jwtHelper.GenerateJwtToken(user);
            return Ok(new { Token = token });
        }

        private string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(hashedBytes);
            }
        }

        private bool VerifyPassword(string password, string hashedPassword)
        {
            return HashPassword(password) == hashedPassword;
        }

        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto changePasswordDTO)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Kullanıcıyı e-posta ile al
            var user = await _userRepository.GetByEmailAsync(changePasswordDTO.Email);
            if (user == null)
                return Unauthorized("Kullanıcı bulunamadı.");

            // Mevcut şifre doğrulaması
            if (!VerifyPassword(changePasswordDTO.CurrentPassword, user.PasswordHash))
                return Unauthorized("Geçersiz mevcut şifre.");

            // Yeni şifreyi hash'le
            var hashedNewPassword = HashPassword(changePasswordDTO.NewPassword);

            // Yeni şifreyi kullanıcıya kaydet
            user.PasswordHash = hashedNewPassword;

            // Veritabanında güncelleme yap
            _userRepository.Update(user);
            await _userRepository.SaveChangesAsync();

            return Ok("Şifre başarıyla değiştirildi.");
        }
        [HttpPost("change-passwordLoggedIn")]
        public async Task<IActionResult> ChangePasswordLoggedIn([FromBody] ChangePasswordLoggedDto changePasswordDTO)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // JWT token'ını almak için Authorization header'ı kontrol et
            var token = Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            if (string.IsNullOrEmpty(token))
                return Unauthorized("Token bulunamadı.");

            // Token'ı doğrula ve email bilgisi al
            var emailFromToken = _jwtHelper.ValidateTokenAndGetEmail(token);
            if (emailFromToken == null)
                return Unauthorized("Geçersiz token.");

            // Token'dan alınan email ile kullanıcıyı al
            var user = await _userRepository.GetByEmailAsync(emailFromToken);
            if (user == null)
                return Unauthorized("Kullanıcı bulunamadı.");

            // Mevcut şifre doğrulaması
            if (!VerifyPassword(changePasswordDTO.CurrentPassword, user.PasswordHash))
                return Unauthorized("Geçersiz mevcut şifre.");

            // Yeni şifreyi hash'le
            var hashedNewPassword = HashPassword(changePasswordDTO.NewPassword);

            // Yeni şifreyi kullanıcıya kaydet
            user.PasswordHash = hashedNewPassword;

            // Veritabanında güncelleme yap
            _userRepository.Update(user);
            await _userRepository.SaveChangesAsync();

            return Ok("Şifre başarıyla değiştirildi.");
        }
        [HttpPut("update-profile")]
        public async Task<IActionResult> UpdateUserProfile([FromBody] UpdateUserDto updateUserDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // JWT token'ını almak için Authorization header'ı kontrol et
            var token = Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            if (string.IsNullOrEmpty(token))
                return Unauthorized("Token bulunamadı.");

            // Token'ı doğrula ve email bilgisi al
            var emailFromToken = _jwtHelper.ValidateTokenAndGetEmail(token);
            if (emailFromToken == null)
                return Unauthorized("Geçersiz token.");

            // Token'dan alınan email ile kullanıcıyı al
            var user = await _userRepository.GetByEmailAsync(emailFromToken);
            if (user == null)
                return Unauthorized("Kullanıcı bulunamadı.");

            // Kullanıcının sadece güncellenebilir alanlarını değiştir
            

            if (updateUserDto.Birthday.HasValue)
                user.Birthday = updateUserDto.Birthday;

            if (updateUserDto.Name != "")
                user.Name = updateUserDto.Name;
            if (updateUserDto.Username != "")
                user.UserName = updateUserDto.Username;
            if (updateUserDto.Active.HasValue)
                user.Active = updateUserDto.Active;
            if (updateUserDto.RunPrefer.HasValue)
                user.Runprefer = updateUserDto.RunPrefer;

            if (updateUserDto.Height.HasValue)
                user.Height = updateUserDto.Height;
            if (updateUserDto.Gender != "")
                user.Gender = updateUserDto.Gender;

            if (updateUserDto.Weight.HasValue)
                user.Weight = updateUserDto.Weight;

            // Veritabanında güncelleme yap
            _userRepository.Update(user);
            await _userRepository.SaveChangesAsync();

            return Ok("Profil bilgileri başarıyla güncellendi.");
        }
        [HttpGet("get-by-email/{email}")]
        [Authorize] // Yetkilendirme gerektirir
        public async Task<IActionResult> GetByEmail([FromRoute] string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return BadRequest("Email adresi sağlanmalıdır.");

            // Kullanıcıyı repository'den al
            var user = await _userRepository.GetByEmailAsync(email);

            if (user == null)
                return NotFound("Kullanıcı bulunamadı.");

            // Dönüş: İhtiyacınıza göre kullanıcı detaylarını filtreleyebilirsiniz
            return Ok(new
            {
                user.Id,
                user.Name,
                user.Email,
                user.PhoneNumber,
                user.Address,
                user.Age,
                user.Height,
                user.Weight,  
            });
            
            

        }
        [HttpGet("get-all-users")]
      
        public async Task<IActionResult> GetAllUsers()
        {
            // Tüm kullanıcıları veritabanından al
            var users = await _userRepository.GetAllAsync();
            if (users == null || !users.Any())
            {
                return NotFound("Hiç kullanıcı bulunamadı.");
            }

            // Kullanıcıları UserDTO formatına dönüştür
            var userDtos = users.Select(user => new UserDTO
            {
                Id= user.Id,
                Name = user.Name,
                SurName = user.SurName,
                PhoneNumber = user.PhoneNumber, 
                Address = user.Address,
                Age = user.Age, 
                Height = user.Height,
                Weight = user.Weight,   
                Username = user.UserName,
                Email = user.Email,
                IsActive = user.IsActive,
                Active = user.Active,
                RunPrefer = user.Runprefer,
                Birthday = user.Birthday,

                ProfilePicturePath = user.ProfilePicturePath
            }).ToList();

            return Ok(userDtos);
        }

        [HttpDelete("delete/{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteUser(int id)
        {
            // Kullanıcıyı ID ile al
            var user = await _userRepository.GetByIdAsync(id);

            if (user == null)
            {
                return NotFound("User not found.");
            }

            // Kullanıcıyı silme işlemi yerine aktiflik durumunu false yaparak devre dışı bırak
            user.IsActive = false;

            // Kullanıcıyı güncelle
            _userRepository.Update(user);
            await _userRepository.SaveChangesAsync();

            return Ok("User has been deactivated (soft delete).");
        }
        [HttpPost("upload-profile-picture")]
        [Authorize]
        public async Task<IActionResult> UploadProfilePicture(IFormFile profilePicture)
        {
            if (profilePicture == null || profilePicture.Length == 0)
                return BadRequest("Profil fotoğrafı yüklenmedi.");

            try
            {
                // JWT'den kullanıcı kimliğini al
                var token = Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
                if (string.IsNullOrEmpty(token))
                    return Unauthorized("Token bulunamadı.");

                // Token'ı doğrula ve email bilgisi al
                var emailFromToken = _jwtHelper.ValidateTokenAndGetEmail(token);
                if (emailFromToken == null)
                    return Unauthorized("Geçersiz token.");

                // Token'dan alınan email ile kullanıcıyı al
                var user = await _userRepository.GetByEmailAsync(emailFromToken);
                if (user == null)
                    return Unauthorized("Kullanıcı bulunamadı.");

                // Cloudinary'ye yükle
                string photoUrl = await _photoService.UploadProfilePhotoAsync(profilePicture, user.Id.ToString());

                // Kullanıcı bilgisini güncelle
                user.ProfilePicturePath = photoUrl;
                _userRepository.Update(user);
                await _userRepository.SaveChangesAsync();

                return Ok(new
                {
                    message = "Profil fotoğrafı başarıyla yüklendi.",
                    imageUrl = photoUrl
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Profil fotoğrafı yüklenirken hata oluştu: {ex.Message}");
            }
        }

        [HttpPut("update-profile-picture")]
        [Authorize]
        public async Task<IActionResult> UpdateProfilePicture(IFormFile profilePicture)
        {
            if (profilePicture == null || profilePicture.Length == 0)
            {
                return BadRequest("Profil fotoğrafı yüklenmedi.");
            }

            // JWT'den kullanıcı kimliğini al
            var token = Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            if (string.IsNullOrEmpty(token))
                return Unauthorized("Token bulunamadı.");

            // Token'ı doğrula ve email bilgisi al
            var emailFromToken = _jwtHelper.ValidateTokenAndGetEmail(token);
            if (emailFromToken == null)
                return Unauthorized("Geçersiz token.");

            // Token'dan alınan email ile kullanıcıyı al
            var user = await _userRepository.GetByEmailAsync(emailFromToken);
            if (user == null)
                return Unauthorized("Kullanıcı bulunamadı.");

            // Profil fotoğrafını kaydetme dizini
            var uploadsFolderPath = @"C:\Users\Ali Uyg\source\repos\RunningApplicationNew\RunningApplicationNew\ProfilePhotos\";
            if (!Directory.Exists(uploadsFolderPath))
            {
                Directory.CreateDirectory(uploadsFolderPath);
            }

            // Mevcut profil fotoğrafını kontrol et ve sil
            if (!string.IsNullOrEmpty(user.ProfilePicturePath))
            {
                var existingFilePath = Path.Combine(uploadsFolderPath, Path.GetFileName(user.ProfilePicturePath));
                if (System.IO.File.Exists(existingFilePath))
                {
                    System.IO.File.Delete(existingFilePath);
                }
            }

            // Yeni dosya için benzersiz bir ad oluştur ve kaydet
            var uniqueFileName = $"{Guid.NewGuid()}_{profilePicture.FileName}";
            var filePath = Path.Combine(uploadsFolderPath, uniqueFileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await profilePicture.CopyToAsync(stream);
            }

            // Kullanıcının profil fotoğrafı yolunu güncelle
            user.ProfilePicturePath = $"/ProfilePhotos/{uniqueFileName}";
            _userRepository.Update(user);
            await _userRepository.SaveChangesAsync();

            return Ok(new { message = "Profil fotoğrafı başarıyla güncellendi.", imagePath = user.ProfilePicturePath });
        }
        [HttpDelete("delete-profile-picture")]
        [Authorize]
        public async Task<IActionResult> DeleteProfilePicture()
        {
            // JWT'den kullanıcı kimliğini al
            var token = Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            if (string.IsNullOrEmpty(token))
                return Unauthorized("Token bulunamadı.");

            // Token'ı doğrula ve email bilgisi al
            var emailFromToken = _jwtHelper.ValidateTokenAndGetEmail(token);
            if (emailFromToken == null)
                return Unauthorized("Geçersiz token.");

            // Token'dan alınan email ile kullanıcıyı al
            var user = await _userRepository.GetByEmailAsync(emailFromToken);
            if (user == null)
                return Unauthorized("Kullanıcı bulunamadı.");

            // Mevcut profil fotoğrafı yolunu kontrol et
            if (!string.IsNullOrEmpty(user.ProfilePicturePath))
            {
                var uploadsFolderPath = @"C:\Users\Ali Uyg\source\repos\RunningApplicationNew\RunningApplicationNew\ProfilePhotos\";
                var existingFilePath = Path.Combine(uploadsFolderPath, Path.GetFileName(user.ProfilePicturePath));

                // Dosya varsa sil
                if (System.IO.File.Exists(existingFilePath))
                {
                    System.IO.File.Delete(existingFilePath);
                }

                // Kullanıcının profil fotoğrafı yolunu temizle
                user.ProfilePicturePath = null;
                _userRepository.Update(user);
                await _userRepository.SaveChangesAsync();

                return Ok(new { message = "Profil fotoğrafı başarıyla silindi." });
            }

            return NotFound("Profil fotoğrafı bulunamadı.");
        }
        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequestDto model)
        {
            if (string.IsNullOrEmpty(model.Email))
            {
                return BadRequest("E-posta adresi gereklidir.");
            }

            var user = await _userRepository.GetByEmailAsync(model.Email);
            if (user == null)
            {
                return BadRequest("Bu e-posta adresiyle kayıtlı kullanıcı bulunamadı.");
            }

            // Şifre sıfırlama token'ı oluştur
            var token = _jwtHelper.GeneratePasswordResetToken(user.Email);

            // Şifre sıfırlama linkini oluştur (token URL'ye eklenir)
            var resetLink = Url.Action(
                "ResetPassword", // Reset password action'ı
                "Account", // Controller adı (örneğin AccountController)
                new { token = token },
                protocol: HttpContext.Request.Scheme);

            // Şifre sıfırlama linkini içeren e-posta gönder
            var emailSubject = "Şifre Sıfırlama Linki";
            var emailBody = $"Merhaba, şifrenizi sıfırlamak için lütfen aşağıdaki linke tıklayın:\n{resetLink}";
            await _emailhelper.SendEmailAsync(model.Email, emailSubject, emailBody);

            return Ok(new { message = "Şifre sıfırlama linki e-posta adresinize gönderildi." });
        }
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequestDto model)
        {
            if (string.IsNullOrEmpty(model.Token) || string.IsNullOrEmpty(model.NewPassword))
            {
                return BadRequest("Token ve yeni şifre gereklidir.");
            }

            // Token'ı doğrula
            var emailFromToken = _jwtHelper.ValidatePasswordResetToken(model.Token);
            if (string.IsNullOrEmpty(emailFromToken))
            {
                return Unauthorized("Geçersiz veya süresi dolmuş token.");
            }

            var user = await _userRepository.GetByEmailAsync(emailFromToken);
            if (user == null)
            {
                return Unauthorized("Kullanıcı bulunamadı.");
            }

            // Şifreyi güncelle
            var hashedPassword = HashPassword(model.NewPassword); // Şifreyi hashleyin
            user.PasswordHash = hashedPassword;
            _userRepository.Update(user);
            await _userRepository.SaveChangesAsync();

            return Ok(new { message = "Şifreniz başarıyla güncellendi." });
        }

        [HttpGet("profile")]
        [Authorize]
        public async Task<IActionResult> GetUserProfile()
        {
            try
            {
                // JWT'den kullanıcı e-posta bilgisini al
                var token = Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
                if (string.IsNullOrEmpty(token))
                    return Unauthorized("Token bulunamadı.");

                // Token'ı doğrula ve email bilgisi al
                var emailFromToken = _jwtHelper.ValidateTokenAndGetEmail(token);
                if (emailFromToken == null)
                    return Unauthorized("Geçersiz token.");

                // Token'dan alınan email ile kullanıcıyı al
                var user = await _userRepository.GetByEmailAsync(emailFromToken);
                if (user == null)
                    return Unauthorized("Kullanıcı bulunamadı.");

                // Kullanıcının profil bilgilerini döndür
                return Ok(new
                {
                    id = user.Id,
                    name = user.Name,
                    surname = user.SurName,
                    userName = user.UserName,
                    email = user.Email,
                    phoneNumber = user.PhoneNumber,
                    address = user.Address,
                    age = user.Age,
                    height = user.Height,
                    weight = user.Weight,
                    gender = user.Gender,
                    birthDay = user.Birthday,
                    profilePictureUrl = user.ProfilePicturePath,
                    runPrefer = user.Runprefer,
                    active = user.Active,
                    isActive = user.IsActive
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Profil bilgileri alınırken bir hata oluştu: {ex.Message}");
            }
        }
    }





}



