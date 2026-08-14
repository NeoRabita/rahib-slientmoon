namespace SlientMoon.Domain.Errors
{
    public static class OtpErrors
    {
        public static readonly Error OtpNotFound = Error.NotFound(
            "OtpErrors.OtpNotFound",
            "No pending OTP found. Please request a new one.");

        public static readonly Error RateLimitExceeded = Error.Validation(
            "OtpErrors.RateLimitExceeded",
            "You have exceeded the maximum number of attempts. Please request a new OTP.");

        public static readonly Error InvalidOtp = Error.Validation(
            "OtpErrors.InvalidOtp",
            "The OTP code is incorrect.");
    }
}
