namespace SlientMoon.Domain.Errors
{
    public static class FavoriteErrors
    {
        public static readonly Error CourseNotFound = Error.NotFound(
            "Favorites.CourseNotFound",
            "The specified course was not found.");

        public static readonly Error AlreadyExists = Error.Conflict(
            "Favorites.AlreadyExists",
            "This course is already in your favorites.");

        public static readonly Error NotFound = Error.NotFound(
            "Favorites.NotFound",
            "The favorite record was not found.");

        public static readonly Error Forbidden = Error.Failure(
            "Favorites.Forbidden",
            "You do not have permission to access or delete this resource.");
    }
}
