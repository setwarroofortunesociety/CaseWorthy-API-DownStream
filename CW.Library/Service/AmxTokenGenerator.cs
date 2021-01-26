using System;
using System.Net;
using System.Security;
using System.Security.Cryptography;
using System.Text;

namespace CW.Library.Service
{
    public static class AmxTokenGenerator
    {
        private const string errorMessage = "Invalid authorization header";
        private static readonly DateTime epochStart = new DateTime(1970, 01, 01, 0, 0, 0, 0, DateTimeKind.Utc);

        public static string GenerateAuthorizationHeader(string secretKey, string accessKey, string method, string url, byte[] body)
        {
            var timestamp = Convert.ToUInt64((DateTime.UtcNow - epochStart).TotalSeconds).ToString();
            var nonce = Guid.NewGuid().ToString("N");

            var signature = GenerateSignature(secretKey, accessKey, method, url, timestamp, nonce, body);
            var header = new AmxAuthorizationToken(accessKey, signature, nonce, timestamp);
            var headerString = header.ToString();
            return headerString;
        }

        private static string GenerateSignature(string secretKey, string accessKey, string method, string url, string timestamp, string nonce, byte[] body)
        {
            string bodyHash = null;
            if (body != null && body.Length > 0)
            {
                using (var md5 = MD5.Create())
                    bodyHash = Convert.ToBase64String(md5.ComputeHash(body, 0, body.Length));
            }

            var urlEncoded = WebUtility.UrlEncode(url.ToLower());

            var signatureString = $"{accessKey}{method.ToUpper()}{urlEncoded}{timestamp}{nonce}{bodyHash}";
            var secretKeyBytes = Convert.FromBase64String(secretKey);
            var signatureBytes = Encoding.UTF8.GetBytes(signatureString);
            using (var hmac = new HMACSHA256(secretKeyBytes))
            {
                var signatureHash = hmac.ComputeHash(signatureBytes);
                var signature = Convert.ToBase64String(signatureHash);
                return signature;
            }
        }

        public static AmxAuthorizationToken ParseHeader(string authorizationHeader)
        {
            var split1 = authorizationHeader.Split(" ");
            if (split1.Length != 2)
                throw new SecurityException(errorMessage);
            if (split1[0] != "amx")
                throw new SecurityException(errorMessage);

            var split = split1[1].Split(':');
            if (split.Length != 4)
                throw new SecurityException(errorMessage);

            var token = new AmxAuthorizationToken(split[0], split[1], split[2], split[3]);

            return token;
        }

        public static bool Validate(AmxAuthorizationToken token, string secretKey, string method, string url, byte[] body)
        {
            var signature = GenerateSignature(secretKey, token.AccessKey, method, url, token.Timestamp, token.Nonce, body);

            var valid =  token.Signature.Equals(signature, StringComparison.Ordinal);
            return valid;
        }

        public class AmxAuthorizationToken
        {
            public string AccessKey { get; private set; }
            public string Signature { get; private set; }
            public string Nonce { get; private set; }
            public string Timestamp { get; private set; }

            public AmxAuthorizationToken(string accessKey, string signature, string nonce, string timestamp)
            {
                if (string.IsNullOrWhiteSpace(accessKey)) throw new ArgumentException("accessKey");
                if (string.IsNullOrWhiteSpace(signature)) throw new ArgumentException("signature");
                if (string.IsNullOrWhiteSpace(nonce)) throw new ArgumentException("nonce");
                if (string.IsNullOrWhiteSpace(timestamp)) throw new ArgumentException("timestamp");

                this.AccessKey = accessKey;
                this.Signature = signature;
                this.Nonce = nonce;
                this.Timestamp = timestamp;
            }

            public override string ToString()
            {
                return $"amx {AccessKey}:{Signature}:{Nonce}:{Timestamp}";
            }
        }
    }
}
