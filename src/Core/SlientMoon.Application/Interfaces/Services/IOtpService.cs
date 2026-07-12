using System.Threading.Tasks;

namespace SlientMoon.Application.Interfaces.Services
{
    public interface IOtpService
    {
        Task<string> GenerateOtpAsync(string userId);

        Task<Result> ValidateOtpAsync(string userId, string otp);

        Task RemoveOtpAsync(string userId);
    }
}
