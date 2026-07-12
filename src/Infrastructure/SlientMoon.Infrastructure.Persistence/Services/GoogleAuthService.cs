using Google.Apis.Auth;
using SlientMoon.Application.Interfaces.Services;
using SlientMoon.Domain.Errors;
using System.Threading.Tasks;

namespace SlientMoon.Infrastructure.Persistence.Services
{
    public class GoogleAuthService : IGoogleAuthService
    {
        private const string GoogleClientId = "184413544217-bgo5vnnsna876mm9umi6k35akjf9sg5c.apps.googleusercontent.com";

        public async Task<Result<GoogleUserPayload>> VerifyTokenAsync(string idToken)
        {
            try
            {
                var settings = new GoogleJsonWebSignature.ValidationSettings 
                {
                    Audience = new[] { GoogleClientId }
                };

                var payload = await GoogleJsonWebSignature.ValidateAsync(idToken, settings);

                var googleUser = new GoogleUserPayload
                {
                    Email = payload.Email,
                    Name = payload.Name,
                    AvatarUrl = payload.Picture
                };

                return Result.Success(googleUser);
            }
            catch (InvalidJwtException ex)
            {
                return UserErrors.Unauthorized();
            }
        }
    }
}
