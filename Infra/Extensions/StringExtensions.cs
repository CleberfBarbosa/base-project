using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Infra.Extensions
{
    public static class StringExtensions
    {
        public static bool IsValidEmail(this string email)
        {
            try
            {
                if(string.IsNullOrEmpty(email))
                    return false;
                new MailAddress(email);
                return true;

            }catch { return false; }
        }

        public static string HashSha256(this string password)
        {
            try
            {
                using SHA256 sha256 = SHA256.Create();
                byte[] hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
            }
            catch (Exception ex)
            {
                throw new ArgumentException("invalid password! ", ex.Message);
            }
        }
    }
}
