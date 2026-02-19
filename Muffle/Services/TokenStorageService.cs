namespace Muffle.Services
{
    /// <summary>
    /// Handles secure storage and retrieval of authentication tokens using MAUI SecureStorage
    /// </summary>
    public static class TokenStorageService
    {
        private const string AuthTokenKey = "muffle_auth_token";

        /// <summary>
        /// Saves an authentication token to secure storage
        /// </summary>
        /// <param name="token">The token to save</param>
        public static async Task SaveTokenAsync(string token)
        {
            try
            {
                await SecureStorage.SetAsync(AuthTokenKey, token);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving auth token: {ex.Message}");
            }
        }

        /// <summary>
        /// Retrieves the authentication token from secure storage
        /// </summary>
        /// <returns>The stored token, or null if not found</returns>
        public static async Task<string?> GetTokenAsync()
        {
            try
            {
                return await SecureStorage.GetAsync(AuthTokenKey);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving auth token: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Removes the authentication token from secure storage
        /// </summary>
        public static void RemoveToken()
        {
            try
            {
                SecureStorage.Remove(AuthTokenKey);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error removing auth token: {ex.Message}");
            }
        }

        /// <summary>
        /// Checks if an authentication token exists in secure storage
        /// </summary>
        /// <returns>True if a token exists, false otherwise</returns>
        public static async Task<bool> HasTokenAsync()
        {
            var token = await GetTokenAsync();
            return !string.IsNullOrEmpty(token);
        }
    }
}
