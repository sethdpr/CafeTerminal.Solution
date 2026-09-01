namespace CafeTerminal.Maui.Services
{
    public class AuthService //Can be used for all sorts of authentication related functionality
    {
        private const string TokenKey = "auth_token";

        // Checks whether a JWT token is currently stored on the device.
        public async Task<bool> IsLoggedInAsync()
        {
            var token = await SecureStorage.Default.GetAsync(TokenKey);

            return !string.IsNullOrWhiteSpace(token);
        }

        // Removes the stored JWT token to sign the current user out.
        public async Task LogoutAsync()
        {
            SecureStorage.Default.Remove(TokenKey);
        }
    }
}