using System.Threading.Tasks;

namespace SlientMoon.Application.Interfaces.Services
{
    public interface IOtpService
    {
        Task<string> GenerateOtpAsync(string userId, string email);

        Task<Result> ValidateOtpAsync(string userId, string otp);

        Task RemoveOtpAsync(string userId);
    }
}
