using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace Vulcanova.Features.Auth;

public static class ClientIdentityStore
{
    private const string PemPrivateKeyItemKey = "PemPrivateKey";
    private const string FirebaseTokenItemKey = "FirebaseToken";

    public static async Task SaveIdentityAsync(ClientIdentity identity)
    {
        var (x509Certificate2, pemPrivateKey, firebaseToken) = identity;

        using var x509Store = new X509Store(StoreName.My, StoreLocation.CurrentUser);
        x509Store.Open(OpenFlags.ReadWrite);
            
        x509Store.Add(x509Certificate2);

        await SecureStorage.SetAsync($"{PemPrivateKeyItemKey}_{x509Certificate2.Thumbprint}", pemPrivateKey);
        await SecureStorage.SetAsync($"{FirebaseTokenItemKey}_{x509Certificate2.Thumbprint}", firebaseToken);
    }

    public static async Task<ClientIdentity> GetIdentityAsync(string thumbprint)
    {
        using var x509Store = new X509Store(StoreName.My, StoreLocation.CurrentUser);
        x509Store.Open(OpenFlags.ReadOnly);

        var certs = x509Store.Certificates.Find(X509FindType.FindByThumbprint, thumbprint, false);

        if (certs.Count == 0)
        {
            return null;
        }

        var pemPrivateKey = await SecureStorage.GetAsync($"{PemPrivateKeyItemKey}_{thumbprint}");
        var firebaseToken = await SecureStorage.GetAsync($"{FirebaseTokenItemKey}_{thumbprint}");

        if (pemPrivateKey == null || firebaseToken == null)
        {
            return null;
        }

        return new ClientIdentity(certs[0], pemPrivateKey, firebaseToken);
    }
}

public record ClientIdentity(X509Certificate2 Certificate, string PemPrivateKey, string FirebaseToken);