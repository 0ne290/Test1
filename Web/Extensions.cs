using System.Security.Cryptography;
using System.Text;

namespace Web;

public static class Extensions
{
    public static string GetHashString(this string inputString)// Этот метод используется в случае с ограничением доступа к админке способами 1 и 2
    {
        var sb = new StringBuilder();

        foreach (var b in SHA256.HashData(Encoding.UTF8.GetBytes(inputString)))
            sb.Append(b.ToString("X2"));

        return sb.ToString();
    }
}