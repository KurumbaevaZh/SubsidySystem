namespace SubsidySystem.Services
{
    /// <summary>
    /// Класс для хранения информации о текущем пользователе
    /// </summary>
    public static class UserSession
    {
        public static CurrentUser? CurrentUser { get; set; }

        public static bool IsAuthenticated => CurrentUser != null;

        public static bool IsAdmin => CurrentUser?.Role == "администратор";

        public static bool IsManager => CurrentUser?.Role == "руководитель";

        public static bool IsSpecialist => CurrentUser?.Role == "специалист";

        public static bool HasRole(string role)
        {
            return CurrentUser?.Role == role;
        }

        public static void Logout()
        {
            CurrentUser = null;
        }
    }

    /// <summary>
    /// Класс с данными текущего пользователя
    /// </summary>
    public class CurrentUser
    {
        public int UserId { get; set; }
        public string Login { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
    }
}