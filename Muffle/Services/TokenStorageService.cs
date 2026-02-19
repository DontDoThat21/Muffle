using System.Text.Json;
using Muffle.Data.Models;
using Muffle.Data.Services;

namespace Muffle.Services
{
    /// <summary>
    /// Handles secure storage and retrieval of authentication tokens using MAUI SecureStorage
    /// Supports multiple accounts
    /// </summary>
    public static class TokenStorageService
    {
        private const string AuthTokenKey = "muffle_auth_token"; // Legacy single-token key
        private const string StoredAccountsKey = "muffle_stored_accounts"; // Multiple accounts key
        private const string LastUsedTokenKey = "muffle_last_used_token"; // Last used token for quick access

        /// <summary>
        /// Saves an authentication token to secure storage (legacy method for backward compatibility)
        /// </summary>
        /// <param name="token">The token to save</param>
        [Obsolete("Use SaveAccountAsync instead for multiple account support")]
        public static async Task SaveTokenAsync(string token)
        {
            try
            {
                await SecureStorage.SetAsync(AuthTokenKey, token);
                await SecureStorage.SetAsync(LastUsedTokenKey, token);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving auth token: {ex.Message}");
            }
        }

        /// <summary>
        /// Retrieves the authentication token from secure storage (legacy method)
        /// </summary>
        /// <returns>The stored token, or null if not found</returns>
        [Obsolete("Use GetLastUsedTokenAsync or GetAllStoredAccountsAsync instead")]
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
        /// Removes the authentication token from secure storage (legacy method)
        /// </summary>
        [Obsolete("Use RemoveAccountAsync instead")]
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
            var accounts = await GetAllStoredAccountsAsync();
            return accounts.Count > 0;
        }

        /// <summary>
        /// Saves an account with its authentication token to secure storage
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <param name="username">Username</param>
        /// <param name="email">Email</param>
        /// <param name="token">Authentication token</param>
        public static async Task SaveAccountAsync(int userId, string username, string email, string token)
        {
            try
            {
                var accounts = await GetAllStoredAccountsAsync();
                
                // Remove existing account with same user ID if it exists
                accounts.RemoveAll(a => a.UserId == userId);
                
                // Add new/updated account
                accounts.Add(new StoredAccount
                {
                    UserId = userId,
                    Username = username,
                    Email = email,
                    Token = token,
                    LastUsed = DateTime.Now
                });
                
                // Save to secure storage
                var json = JsonSerializer.Serialize(accounts);
                await SecureStorage.SetAsync(StoredAccountsKey, json);
                
                // Set as last used token
                await SecureStorage.SetAsync(LastUsedTokenKey, token);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving account: {ex.Message}");
            }
        }

        /// <summary>
        /// Retrieves all stored accounts from secure storage
        /// </summary>
        /// <returns>List of stored accounts</returns>
        public static async Task<List<StoredAccount>> GetAllStoredAccountsAsync()
        {
            try
            {
                var json = await SecureStorage.GetAsync(StoredAccountsKey);
                
                if (string.IsNullOrEmpty(json))
                {
                    return new List<StoredAccount>();
                }
                
                var accounts = JsonSerializer.Deserialize<List<StoredAccount>>(json);
                return accounts ?? new List<StoredAccount>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving stored accounts: {ex.Message}");
                return new List<StoredAccount>();
            }
        }

        /// <summary>
        /// Retrieves the last used authentication token
        /// </summary>
        /// <returns>The last used token, or null if not found</returns>
        public static async Task<string?> GetLastUsedTokenAsync()
        {
            try
            {
                return await SecureStorage.GetAsync(LastUsedTokenKey);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving last used token: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Sets a token as the last used (for quick switching)
        /// </summary>
        /// <param name="token">The token to mark as last used</param>
        public static async Task SetLastUsedTokenAsync(string token)
        {
            try
            {
                await SecureStorage.SetAsync(LastUsedTokenKey, token);
                
                // Update LastUsed timestamp in stored accounts
                var accounts = await GetAllStoredAccountsAsync();
                var account = accounts.FirstOrDefault(a => a.Token == token);
                if (account != null)
                {
                    account.LastUsed = DateTime.Now;
                    var json = JsonSerializer.Serialize(accounts);
                    await SecureStorage.SetAsync(StoredAccountsKey, json);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error setting last used token: {ex.Message}");
            }
        }

        /// <summary>
        /// Removes an account from secure storage
        /// </summary>
        /// <param name="userId">User ID of the account to remove</param>
        public static async Task RemoveAccountAsync(int userId)
        {
            try
            {
                var accounts = await GetAllStoredAccountsAsync();
                var account = accounts.FirstOrDefault(a => a.UserId == userId);
                
                if (account != null)
                {
                    accounts.Remove(account);
                    
                    // Save updated list
                    var json = JsonSerializer.Serialize(accounts);
                    await SecureStorage.SetAsync(StoredAccountsKey, json);
                    
                    // If this was the last used token, clear it
                    var lastUsedToken = await GetLastUsedTokenAsync();
                    if (lastUsedToken == account.Token)
                    {
                        SecureStorage.Remove(LastUsedTokenKey);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error removing account: {ex.Message}");
            }
        }

        /// <summary>
        /// Clears all stored accounts from secure storage
        /// </summary>
        public static void ClearAllAccounts()
        {
            try
            {
                SecureStorage.Remove(StoredAccountsKey);
                SecureStorage.Remove(LastUsedTokenKey);
                SecureStorage.Remove(AuthTokenKey); // Legacy key
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error clearing all accounts: {ex.Message}");
            }
        }
    }
}
