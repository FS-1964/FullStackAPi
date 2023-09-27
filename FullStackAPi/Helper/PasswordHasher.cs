using System;
using System.Drawing;
using System.Security.Cryptography;

namespace FullStackAPi.Helper
{
    public static class PasswordHasher
    {
      
        private static readonly int SaltSize = 16;
        private static readonly int HashSize = 20;
        private static readonly int Iterations = 10000;
        private static byte[]? salt;
        private const string usageText = "Usage: RFC2898 <password>\nYou must specify the password for encryption.\n";
        public static string HashPassword(string password)
        {
            
            if (password == null || password.Length < 1)
            {
               
                return usageText;
            }
            else
            {
                 salt = new byte[SaltSize];
                using (var rng = RandomNumberGenerator.Create())
                {
                    rng.GetBytes(salt);

                    try
                    {

                        var key = new Rfc2898DeriveBytes(password, salt!, Iterations, HashAlgorithmName.SHA256);
                        var hash = key.GetBytes(HashSize);
                        var hashBytes = new byte[SaltSize + HashSize];
                        Array.Copy(salt!, 0, hashBytes, 0, SaltSize);
                        Array.Copy(hash, 0, hashBytes, SaltSize, HashSize);
                        var base64Hash = Convert.ToBase64String(hashBytes);
                        return base64Hash;
                    }
                    catch (Exception)
                    {

                        throw;
                    }
                }
                
             
            }
        }

        public static bool VerifyPassword(string password, string base64Hash)
        {

            try
            {
                var hashBytes = Convert.FromBase64String(base64Hash);

                var salt = new byte[SaltSize];
                Array.Copy(hashBytes, 0, salt, 0, SaltSize);
                var key = new Rfc2898DeriveBytes(password, salt!, Iterations, HashAlgorithmName.SHA256);
                byte[] hash = key.GetBytes(HashSize);
                for (var i = 0; i < HashSize; i++)
                {
                    if (hashBytes[i + SaltSize] != hash[i]) { return false; }
                }
            }
            catch (Exception)
            {

                throw;
            }
            return true;
        }
    }
}
