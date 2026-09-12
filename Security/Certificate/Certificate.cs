
using System;
using System.IO;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace CreCraftStudios.HunterReborn.Launcher.Security.Certificate
{
    internal class Certificate
    {
        public static X509Certificate2 Generate(X509Certificate2 root) {
            using var key = RSA.Create(3072);

            var request = new CertificateRequest(
                "CN=*.thehunter.com",
                key,
                HashAlgorithmName.SHA256,
                RSASignaturePadding.Pkcs1);

            request.CertificateExtensions.Add(new X509BasicConstraintsExtension(
                certificateAuthority: false,
                hasPathLengthConstraint: false,
                pathLengthConstraint: 0,
                critical: true));

            request.CertificateExtensions.Add(new X509KeyUsageExtension(X509KeyUsageFlags.DigitalSignature, critical: true));

            request.CertificateExtensions.Add(new X509EnhancedKeyUsageExtension(new OidCollection { new Oid("1.3.6.1.5.5.7.3.1") }, critical: false));

            var san = new SubjectAlternativeNameBuilder();
            san.AddDnsName("*.thehunter.com");
            san.AddDnsName("thehunter.com");

            request.CertificateExtensions.Add(san.Build());

            var cert = request.Create(root, DateTimeOffset.UtcNow.AddMinutes(-5), DateTimeOffset.UtcNow.AddYears(2), RandomNumberGenerator.GetBytes(16));

            return cert.CopyWithPrivateKey(key);
        }

        public static void Write(X509Certificate2 cert) {
            File.WriteAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Nginx", "ssl", "thehunter.com.crt"), cert.ExportCertificatePem());

            using RSA key = cert.GetRSAPrivateKey() ?? throw new InvalidOperationException("Certificate has no RSA private key");

            File.WriteAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Nginx", "ssl", "thehunter.com.key"), key.ExportPkcs8PrivateKeyPem());
        }
    }
}