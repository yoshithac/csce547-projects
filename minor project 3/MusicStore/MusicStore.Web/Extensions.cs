namespace MusicStore.Web;

public static class Extensions
{
    // "Admin mode" is a simple user setting stored in a cookie (no login required by the spec)
    public static bool IsAdmin(this HttpRequest request) => request.Cookies["admin"] == "1";
}
