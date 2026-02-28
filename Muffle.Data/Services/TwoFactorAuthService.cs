using System.Security.Cryptography;
using System.Text;
using Dapper;
using Muffle.Data.Models;

namespace Muffle.Data.Services
{
    /// <summary>
    /// TOTP-based two-factor authentication service (RFC 6238 / RFC 4226).
    /// Fully self-contained — no external TOTP library required.
    /// </summary>
    public static class TwoFactorAuthService
    {
        private const string Issuer = "Muffle";
        private const int TotpDigits = 6;
        private const int TotpStep = 30;         // seconds per window
        private const int AllowedSkew = 1;       // ±1 window for clock drift
        private const int BackupCodeCount = 8;
        private const int SecretByteLength = 20; // 160-bit secret (standard)

        // ── Base32 helpers ──────────────────────────────────────────────────

        private static readonly char[] Base32Alphabet =
            "ABCDEFGHIJKLMNOPQRSTUVWXYZ234567".ToCharArray();

        private static string ToBase32(byte[] data)
        {
            var sb = new StringBuilder();
            int buffer = data[0], next = 1, bitsLeft = 8;

            while (bitsLeft > 0 || next < data.Length)
            {
                if (bitsLeft < 5)
                {
                    if (next < data.Length)
                    {
                        buffer = (buffer << 8) | (data[next++] & 0xFF);
                        bitsLeft += 8;
                    }
                    else
                    {
                        buffer <<= 5 - bitsLeft;
                        bitsLeft = 5;
                    }
                }
                int index = 0x1F & (buffer >> (bitsLeft - 5));
                bitsLeft -= 5;
                sb.Append(Base32Alphabet[index]);
            }

            return sb.ToString();
        }

        private static byte[] FromBase32(string base32)
        {
            base32 = base32.ToUpperInvariant().TrimEnd('=');
            var output = new List<byte>();
            int buffer = 0, bitsLeft = 0;

            foreach (char c in base32)
            {
                int value = Array.IndexOf(Base32Alphabet, c);
                if (value < 0) continue;
                buffer = (buffer << 5) | value;
                bitsLeft += 5;
                if (bitsLeft >= 8)
                {
                    output.Add((byte)(buffer >> (bitsLeft - 8)));
                    bitsLeft -= 8;
                }
            }

            return output.ToArray();
        }

        // ── TOTP core (RFC 6238) ────────────────────────────────────────────

        private static int ComputeTotp(byte[] secret, long counter)
        {
            byte[] counterBytes = BitConverter.GetBytes(counter);
            if (BitConverter.IsLittleEndian)
                Array.Reverse(counterBytes); // TOTP uses big-endian

            using var hmac = new HMACSHA1(secret);
            byte[] hash = hmac.ComputeHash(counterBytes);

            int offset = hash[^1] & 0x0F;
            int truncated = ((hash[offset] & 0x7F) << 24)
                          | ((hash[offset + 1] & 0xFF) << 16)
                          | ((hash[offset + 2] & 0xFF) << 8)
                          |  (hash[offset + 3] & 0xFF);

            return truncated % (int)Math.Pow(10, TotpDigits);
        }

        private static long GetCurrentCounter()
            => DateTimeOffset.UtcNow.ToUnixTimeSeconds() / TotpStep;

        // ── Public API ──────────────────────────────────────────────────────

        /// <summary>
        /// Generates a cryptographically random Base32-encoded TOTP secret.
        /// </summary>
        public static string GenerateSecret()
        {
            byte[] secretBytes = RandomNumberGenerator.GetBytes(SecretByteLength);
            return ToBase32(secretBytes);
        }

        /// <summary>
        /// Builds the otpauth:// URI used by Google Authenticator, Authy, etc.
        /// </summary>
        public static string GenerateSetupUri(string secret, string userEmail)
        {
            string label = Uri.EscapeDataString($"{Issuer}:{userEmail}");
            return $"otpauth://totp/{label}?secret={secret}&issuer={Uri.EscapeDataString(Issuer)}&digits={TotpDigits}&period={TotpStep}";
        }

        /// <summary>
        /// Verifies a 6-digit TOTP code, allowing ±1 window of clock skew.
        /// </summary>
        public static bool VerifyCode(string base32Secret, string code)
        {
            if (string.IsNullOrWhiteSpace(base32Secret) || string.IsNullOrWhiteSpace(code))
                return false;

            if (!int.TryParse(code.Trim(), out int providedCode))
                return false;

            byte[] secret = FromBase32(base32Secret);
            long counter = GetCurrentCounter();

            for (int skew = -AllowedSkew; skew <= AllowedSkew; skew++)
            {
                if (ComputeTotp(secret, counter + skew) == providedCode)
                    return true;
            }

            return false;
        }

        // ── Database operations ─────────────────────────────────────────────

        /// <summary>
        /// Returns the TwoFactorAuth record for the user, or null if none exists.
        /// </summary>
        public static TwoFactorAuth? GetTwoFactorAuth(int userId)
        {
            try
            {
                using var connection = SQLiteDbService.CreateConnection();
                connection.Open();
                return connection.QueryFirstOrDefault<TwoFactorAuth>(
                    "SELECT * FROM TwoFactorAuth WHERE UserId = @UserId;",
                    new { UserId = userId });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting 2FA record: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Returns true if 2FA is currently enabled for the given user.
        /// </summary>
        public static bool IsTwoFactorEnabled(int userId)
        {
            try
            {
                using var connection = SQLiteDbService.CreateConnection();
                connection.Open();
                return connection.ExecuteScalar<bool>(
                    "SELECT IsEnabled FROM TwoFactorAuth WHERE UserId = @UserId;",
                    new { UserId = userId });
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Generates backup codes, stores their BCrypt hashes, and returns the
        /// plaintext codes for one-time display to the user.
        /// </summary>
        public static List<string> GenerateBackupCodes(int userId)
        {
            var plainCodes = new List<string>();
            var hashedCodes = new List<string>();

            for (int i = 0; i < BackupCodeCount; i++)
            {
                string code = GenerateBackupCode();
                plainCodes.Add(code);
                hashedCodes.Add(BCrypt.Net.BCrypt.HashPassword(code));
            }

            try
            {
                using var connection = SQLiteDbService.CreateConnection();
                connection.Open();
                connection.Execute(
                    @"UPDATE TwoFactorAuth SET BackupCodes = @BackupCodes WHERE UserId = @UserId;",
                    new { BackupCodes = string.Join(";", hashedCodes), UserId = userId });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving backup codes: {ex.Message}");
            }

            return plainCodes;
        }

        /// <summary>
        /// Enables 2FA for a user after they have verified the first code.
        /// </summary>
        public static bool EnableTwoFactor(int userId, string secret)
        {
            try
            {
                using var connection = SQLiteDbService.CreateConnection();
                connection.Open();

                connection.Execute(@"
                    INSERT INTO TwoFactorAuth (UserId, IsEnabled, Secret, EnabledAt)
                    VALUES (@UserId, 1, @Secret, @EnabledAt)
                    ON CONFLICT(UserId) DO UPDATE SET
                        IsEnabled = 1,
                        Secret    = @Secret,
                        EnabledAt = @EnabledAt;",
                    new { UserId = userId, Secret = secret, EnabledAt = DateTime.UtcNow });

                connection.Execute(
                    "UPDATE Users SET IsTwoFactorEnabled = 1 WHERE UserId = @UserId;",
                    new { UserId = userId });

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error enabling 2FA: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Disables 2FA for a user (keeps the record but clears secret and codes).
        /// </summary>
        public static bool DisableTwoFactor(int userId)
        {
            try
            {
                using var connection = SQLiteDbService.CreateConnection();
                connection.Open();

                connection.Execute(@"
                    UPDATE TwoFactorAuth
                    SET IsEnabled = 0, Secret = NULL, BackupCodes = NULL, EnabledAt = NULL
                    WHERE UserId = @UserId;",
                    new { UserId = userId });

                connection.Execute(
                    "UPDATE Users SET IsTwoFactorEnabled = 0 WHERE UserId = @UserId;",
                    new { UserId = userId });

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error disabling 2FA: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Validates and consumes a backup code (each code is single-use).
        /// </summary>
        public static bool ValidateBackupCode(int userId, string code)
        {
            if (string.IsNullOrWhiteSpace(code)) return false;

            try
            {
                using var connection = SQLiteDbService.CreateConnection();
                connection.Open();

                var record = connection.QueryFirstOrDefault<TwoFactorAuth>(
                    "SELECT * FROM TwoFactorAuth WHERE UserId = @UserId AND IsEnabled = 1;",
                    new { UserId = userId });

                if (record?.BackupCodes == null) return false;

                var hashes = record.BackupCodes
                    .Split(';', StringSplitOptions.RemoveEmptyEntries)
                    .ToList();

                string? matchedHash = hashes.FirstOrDefault(h =>
                    BCrypt.Net.BCrypt.Verify(code.Trim(), h));

                if (matchedHash == null) return false;

                // Consume the used code
                hashes.Remove(matchedHash);
                connection.Execute(
                    "UPDATE TwoFactorAuth SET BackupCodes = @BackupCodes WHERE UserId = @UserId;",
                    new { BackupCodes = string.Join(";", hashes), UserId = userId });

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error validating backup code: {ex.Message}");
                return false;
            }
        }

        // ── Helpers ─────────────────────────────────────────────────────────

        private static string GenerateBackupCode()
        {
            const string chars = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789";
            var bytes = RandomNumberGenerator.GetBytes(8);
            var sb = new StringBuilder(9);
            for (int i = 0; i < 8; i++)
            {
                if (i == 4) sb.Append('-');
                sb.Append(chars[bytes[i] % chars.Length]);
            }
            return sb.ToString();
        }
    }
}
