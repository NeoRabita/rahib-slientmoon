using System;

namespace SlientMoon.Domain.Errors
{
    public static class DomainErrors
    {
        public static Error NotFound(string trackId) => Error.NotFound(
            "Users.NotFound",
            $"Track with Id = '{trackId}' was not found");
    }
}
