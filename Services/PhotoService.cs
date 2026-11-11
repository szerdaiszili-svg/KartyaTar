namespace KartyaTar2.Services;

// SERVICE = "a fényképész". Elkészíti vagy kiválasztja a képet, és eltesz egy másolatot az app saját mappájába.
public class PhotoService
{
    // Fotó készítése a kamerával. Ha a felhasználó mégsem fotózik, null-t adunk vissza.
    public async Task<string?> TakePhotoAsync()
    {
        if (!MediaPicker.Default.IsCaptureSupported)
            throw new InvalidOperationException("Ezen a gépen nincs használható kamera.");

        // Engedélyt kérünk a kamerához (mint amikor a telefon rákérdez: "Engedélyezed?")
        var status = await Permissions.CheckStatusAsync<Permissions.Camera>();
        if (status != PermissionStatus.Granted)
            status = await Permissions.RequestAsync<Permissions.Camera>();
        if (status != PermissionStatus.Granted)
            throw new UnauthorizedAccessException("Engedély nélkül nem tudok fotózni.");

        var photo = await MediaPicker.Default.CapturePhotoAsync();
        return photo == null ? null : await SaveAsync(photo);
    }

    // Kép kiválasztása a galériából (ha nincs kamera, vagy kész kép kell)
    public async Task<string?> PickPhotoAsync()
    {
        var photo = await MediaPicker.Default.PickPhotoAsync();
        return photo == null ? null : await SaveAsync(photo);
    }

    // A képet átmásoljuk az app saját mappájába, új, egyedi névvel
    private static async Task<string> SaveAsync(FileResult photo)
    {
        var dir = Path.Combine(FileSystem.AppDataDirectory, "photos");
        Directory.CreateDirectory(dir);

        var ext = Path.GetExtension(photo.FileName);
        if (string.IsNullOrEmpty(ext)) ext = ".jpg";
        var path = Path.Combine(dir, $"{Guid.NewGuid()}{ext}");

        using var source = await photo.OpenReadAsync();
        using var target = File.OpenWrite(path);
        await source.CopyToAsync(target);
        return path;
    }

    public static void DeleteFile(string? path)
    {
        try { if (!string.IsNullOrEmpty(path) && File.Exists(path)) File.Delete(path); }
        catch { /* nem baj, ha a törlés nem sikerül */ }
    }
}
