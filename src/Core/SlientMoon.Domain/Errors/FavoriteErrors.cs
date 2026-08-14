namespace SlientMoon.Domain.Errors
{
    public static class FavoriteErrors
    {
        public static readonly Error AlreadyExists = Error.Conflict(
            "Favorites.AlreadyExists",
            "This course is already in your favorites.");

        public static readonly Error Forbidden = Error.Failure(
            "Favorites.Forbidden",
            "You do not have permission to access or delete this resource.");
    }
}
