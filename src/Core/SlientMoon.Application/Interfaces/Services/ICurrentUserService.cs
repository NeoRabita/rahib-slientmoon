namespace SlientMoon.Application.Interfaces.Services
{
    public interface ICurrentUserService
    {
        public string? UserId { get; }
        public bool IsAuthenticated { get; }
    }
}
