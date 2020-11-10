using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace PayInc_Customer_web.Models
{
    public class PasswordHash
    {
        public string HashShA1(string password)
        {
            string response = string.Empty;
            try
            {
                using (SHA1Managed sha1 = new SHA1Managed())
                {
                    var hashSh1 = sha1.ComputeHash(Encoding.UTF8.GetBytes(password));

                    return System.Convert.ToBase64String(hashSh1);
                    // declare stringbuilder
                    //var sb = new StringBuilder(hashSh1.Length * 2);
                    //// computing hashSh1
                    //foreach (byte b in hashSh1) { sb.Append(b.ToString("X2").ToLower()); }
                    //return sb.ToString();
                }
            }
            catch
            {

            }
            return response;
        }
    }
}
