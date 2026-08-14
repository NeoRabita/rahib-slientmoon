namespace SlientMoon.Domain.Errors
{
    public static class StorageErrors
    {
        public static Error FileEmpty => Error.Validation(
            "Storage.FileEmpty",
            "Yüklənəcək fayl boş ola bilməz.");

        public static Error InvalidFileName => Error.Validation(
            "Storage.InvalidFileName",
            "Keçərsiz və ya boş fayl adı.");

        public static Error UnsupportedType => Error.Validation(
            "Storage.UnsupportedType",
            "Dəstəklənməyən saxlanma tipi.");

        public static Error FileNotFound(string fileName) => Error.NotFound(
            "Storage.FileNotFound",
            $"'{fileName}' adlı fayl sistemdə tapılmadı.");

        public static Error UploadFailed(string details) => Error.Failure(
            "Storage.UploadFailed",
            $"Fayl yüklənərkən xəta baş verdi: {details}");

        public static Error DeleteFailed(string details) => Error.Failure(
            "Storage.DeleteFailed",
            $"Fayl silinərkən xəta baş verdi: {details}");
    }
}