using System;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.IO;

namespace CreCraftStudios.HunterReborn.Launcher.Security.Certificate
{
    internal class RootCA
    {
        private static readonly string subject = "Hunter Reborn CA for theHunter";
        private static readonly string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Nginx", "ssl", "root.pfx");

        public static X509Certificate2 GetOrCreate() {
            X509Certificate2? cert = Find();
            if (cert != null) {
                bool valid = IsValid(cert);
                bool hasPrivateKey = cert.HasPrivateKey;
                bool installed = IsInstalled(cert);

                if (NeedToRenew(cert))
                {
                    Remove(cert);
                    cert.Dispose();

                    var newCert= Generate();
                    Install(newCert);
                    Save(newCert);
                    return newCert;
                }

                if (valid && hasPrivateKey) {
                    if (!installed)
                        Install(cert);

                    return cert;
                }

                Remove(cert);
                cert.Dispose();
            }

            var root = Generate();
            Install(root);
            Save(root);
            return root;
        }

        private static void Save(X509Certificate2 root)
        {
            byte[] pfxBytes = root.Export(X509ContentType.Pfx);
            File.WriteAllBytes(path, pfxBytes);
        }

        private static void Install(X509Certificate2 root) {
            using var store = new X509Store(StoreName.Root, StoreLocation.CurrentUser);
            store.Open(OpenFlags.ReadWrite);

            var existing = store.Certificates.Find(X509FindType.FindByThumbprint, root.Thumbprint, validOnly: false);

            if (existing.Count == 0)
            {
                var publicCert = X509CertificateLoader.LoadCertificate(root.Export(X509ContentType.Cert));

                store.Add(publicCert);
            }
        }

        private static X509Certificate2? Find() {
            if (!File.Exists(path))
                return null;

            var cert = X509CertificateLoader.LoadPkcs12FromFile(path, string.Empty);//new X509Certificate2(path, string.Empty, X509KeyStorageFlags.Exportable);
            return cert;
        }

        public static X509Certificate2 Generate() {
            using var key = RSA.Create(4096);

            var request = new CertificateRequest(
                    $"CN={subject}",
                    key,
                    HashAlgorithmName.SHA256,
                    RSASignaturePadding.Pkcs1
                );

            request.CertificateExtensions.Add(new X509BasicConstraintsExtension(
                 certificateAuthority: true,
                 hasPathLengthConstraint: true,
                 pathLengthConstraint: 1,
                 critical: true
                ));

            request.CertificateExtensions.Add(new X509KeyUsageExtension(
                X509KeyUsageFlags.KeyCertSign | X509KeyUsageFlags.CrlSign,
                critical: true
                ));

            request.CertificateExtensions.Add(new X509SubjectKeyIdentifierExtension(request.PublicKey, critical: false));

            return request.CreateSelfSigned(DateTimeOffset.UtcNow.AddMinutes(-5), DateTimeOffset.UtcNow.AddYears(10));
        }

        private static void Remove(X509Certificate2 root) {
            using (var store = new X509Store(StoreName.Root, StoreLocation.CurrentUser)) {
                store.Open(OpenFlags.ReadWrite);

                var matches = store.Certificates.Find(
                    X509FindType.FindByThumbprint,
                    root.Thumbprint,
                    validOnly: false);

                foreach(var cert in matches)
                    store.Remove(cert);
            }
        }

        public static bool IsInstalled(X509Certificate2 root) {
            using var store = new X509Store(StoreName.Root, StoreLocation.CurrentUser);

            store.Open(OpenFlags.ReadOnly);

            return store.Certificates.Find(X509FindType.FindByThumbprint, root.Thumbprint, validOnly: false).Count > 0;
        }

        public static bool IsValid(X509Certificate2 cert) { 
            DateTime now = DateTime.UtcNow;

            return now >= cert.NotBefore.ToUniversalTime() && now <= cert.NotAfter.ToUniversalTime();
        }

        public static bool NeedToRenew(X509Certificate2 cert) {
            DateTime now = DateTime.UtcNow;
            DateTime renewDate = cert.NotAfter.ToUniversalTime().AddDays(-30);
            return now >= renewDate;
        }
    }
}