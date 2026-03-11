using System.Runtime.Intrinsics.Arm;
using System.Security.Cryptography;
using System.Text;

namespace Backend.Src.Services
{
    public class CryptoService
    {

        public static string Hash(string password)
        {

            byte[] input = Encoding.UTF8.GetBytes(password);

            byte[] hash = SHA256.HashData(input);

            return Convert.ToHexString(hash);

        }

    }
}
