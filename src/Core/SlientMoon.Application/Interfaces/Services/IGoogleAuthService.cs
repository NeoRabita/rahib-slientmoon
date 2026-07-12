using System.Threading.Tasks;

namespace SlientMoon.Application.Interfaces.Services
{
    public interface IGoogleAuthService
    {
        Task<Result<GoogleUserPayload>> VerifyTokenAsync(string idToken);
    }

    public class GoogleUserPayload
    {
        public string Email { get; set; }
        public string Name { get; set; }
        public string AvatarUrl { get; set; }
    }

}
