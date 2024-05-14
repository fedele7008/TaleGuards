using System.Security.Cryptography;
using System.Text;
using AuthManager.Abstractions;

namespace AuthManager.Extensions;

public class RsaCryptoService : IRsaCryptoService
{
    private RSACryptoServiceProvider Rsa { get; } = new();
    private string PrivateKey { get; set; }
    private string PublicKey { get; set; }
    
    public string GetPublicKey() => PublicKey;
    public RsaCryptoService()
    {
        var privateKey = RSA.Create().ExportParameters(true);
        Rsa.ImportParameters(privateKey);
        PrivateKey = Rsa.ToXmlString(true);

        var publicKey = new RSAParameters
        {
            Modulus = privateKey.Modulus,
            Exponent = privateKey.Exponent
        };
        Rsa.ImportParameters(publicKey);
        PublicKey = Rsa.ToXmlString(false);
    }

    public string Encrypt(string data, string? publicKey = null)
    {
        if (string.IsNullOrEmpty(publicKey))
            publicKey = PublicKey;

        Rsa.FromXmlString(publicKey);

        var inBuffer = (new UTF8Encoding()).GetBytes(data);
        var outBuffer = Rsa.Encrypt(inBuffer, false);
        
        return Convert.ToBase64String(outBuffer);
    }

    public string Decrypt(string encryptedData, string? privateKey = null)
    {
        if (string.IsNullOrEmpty(privateKey))
            privateKey = PrivateKey;
        
        Rsa.FromXmlString(privateKey);
        
        var srcBuffer = Convert.FromBase64String(encryptedData);
        var outBuffer = Rsa.Decrypt(srcBuffer, false);
        
        var decryptedData = (new UTF8Encoding()).GetString(outBuffer);
        return decryptedData;
    }
}
