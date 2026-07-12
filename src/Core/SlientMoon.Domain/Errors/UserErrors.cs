using System;

namespace SlientMoon.Domain.Errors
{
    public static class UserErrors
    {
        public static Error NotFound(Guid userId) => Error.NotFound(
            "Users.NotFound",
            $"The user with the Id = '{userId}' was not found");

        public static Error Unauthorized() => Error.Failure(
            "Users.Unauthorized",
            "You are not authorized to perform this action.");

        public static readonly Error NotFoundByEmail = Error.NotFound(
            "Users.NotFoundByEmail",
            "The user with the specified email was not found");

        public static readonly Error InvalidCredentials = Error.Failure(
            "Users.InvalidCredentials",
            "Invalid email or password");


        public static readonly Error EmailNotUnique = Error.Conflict(
            "Users.EmailNotUnique",
            "The provided email is not unique");

        public static readonly Error EmailNotVerified = Error.Failure(
            "Users.EmailNotVerified",
            "The provided email is not verified");

        public static readonly Error EmailAlreadyVerified = Error.Failure(
           "EMAIL_ALREADY_VERIFIED",
           "This email address has already been verified");

    }

}
