namespace CafeTerminal.Maui.Services
{
    public class AuthService //Can be used for all sorts of authentication related functionality
    {
        private const string TokenKey = "auth_token";

        public async Task<bool> IsLoggedInAsync()
        {
            var token = await SecureStorage.Default.GetAsync(TokenKey);

            return !string.IsNullOrWhiteSpace(token);
        }

        public async Task LogoutAsync()
        {
            SecureStorage.Default.Remove(TokenKey);
        }
    }
}