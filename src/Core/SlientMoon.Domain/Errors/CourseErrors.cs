namespace SlientMoon.Domain.Errors
{
    public static class CourseErrors
    {
        public static readonly Error NotFound = Error.NotFound(
            "Course.NotFound",
            "Auth.CourseNotFound");
    }
}