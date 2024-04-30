using System.Security.Cryptography;
using System.Text;

namespace Web;

public static class Extensions
{
    public static string GetHashString(this string inputString)
    {
        var sb = new StringBuilder();

        foreach (var b in SHA256.HashData(Encoding.UTF8.GetBytes(inputString)))
            sb.Append(b.ToString("X2"));

        return sb.ToString();
    }
}