namespace SlientMoon.Domain.Errors
{
    public static class TrackErrors
    {
        public static Error TrackNotFound => Error.NotFound(
            "Track.NotFound",
            "Track not found.");
    }
}
