using RunningApplicationNew.Entity;

namespace RunningApplicationNew.Helpers
{
    public interface IJwtHelper
    {
        string GenerateJwtToken(User user);
        string ValidateTokenAndGetEmail(string token);// JWT token üretim metodu
        string GeneratePasswordResetToken(string email);
        string ValidatePasswordResetToken(string token);
    }
}
