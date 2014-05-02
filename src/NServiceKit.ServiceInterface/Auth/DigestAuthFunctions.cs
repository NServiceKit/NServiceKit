using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using System.Globalization;

namespace NServiceKit.ServiceInterface.Auth
{
    /// <summary>A digest authentication functions.</summary>
    public class DigestAuthFunctions
    {
        /// <summary>Private hash encode.</summary>
        ///
        /// <param name="TimeStamp"> The time stamp.</param>
        /// <param name="IPAddress"> The IP address.</param>
        /// <param name="PrivateKey">The private key.</param>
        ///
        /// <returns>A string.</returns>
        public string PrivateHashEncode(string TimeStamp, string IPAddress, string PrivateKey)
        {
            var hashing = MD5.Create();
            return(ConvertToHexString(hashing.ComputeHash(Encoding.UTF8.GetBytes(string.Format("{0}:{1}:{2}", TimeStamp, IPAddress, PrivateKey)))));

        }

        /// <summary>Base 64 encode.</summary>
        ///
        /// <param name="StringToEncode">The string to encode.</param>
        ///
        /// <returns>A string.</returns>
        public string Base64Encode(string StringToEncode)
        {
            if (StringToEncode != null)
            {
                return Convert.ToBase64String(Encoding.UTF8.GetBytes(StringToEncode));
            }
            return null;
        }

        /// <summary>Base 64 decode.</summary>
        ///
        /// <param name="StringToDecode">The string to decode.</param>
        ///
        /// <returns>A string.</returns>
        public string Base64Decode(string StringToDecode)
        {
            if (StringToDecode != null)
            {
                return Encoding.UTF8.GetString(Convert.FromBase64String(StringToDecode));
            }
            return null;
        }

        /// <summary>Gets nonce parts.</summary>
        ///
        /// <param name="nonce">The nonce.</param>
        ///
        /// <returns>An array of string.</returns>
        public string[] GetNonceParts(string nonce)
        {
            return Base64Decode(nonce).Split(':');
        }

        /// <summary>Gets a nonce.</summary>
        ///
        /// <param name="IPAddress"> The IP address.</param>
        /// <param name="PrivateKey">The private key.</param>
        ///
        /// <returns>The nonce.</returns>
        public string GetNonce(string IPAddress, string PrivateKey)
        {
            double dateTimeInMilliSeconds = (DateTime.UtcNow - DateTime.MinValue).TotalMilliseconds;
            string dateTimeInMilliSecondsString = dateTimeInMilliSeconds.ToString(CultureInfo.InvariantCulture);
            string privateHash = PrivateHashEncode(dateTimeInMilliSecondsString, IPAddress, PrivateKey);
            return Base64Encode(string.Format("{0}:{1}", dateTimeInMilliSecondsString, privateHash));
        }

        /// <summary>Validates the nonce.</summary>
        ///
        /// <param name="nonce">     The nonce.</param>
        /// <param name="IPAddress"> The IP address.</param>
        /// <param name="PrivateKey">The private key.</param>
        ///
        /// <returns>true if it succeeds, false if it fails.</returns>
        public bool ValidateNonce(string nonce, string IPAddress, string PrivateKey)
        { 
            var nonceparts = GetNonceParts(nonce);
            string privateHash = PrivateHashEncode(nonceparts[0], IPAddress, PrivateKey);
            return string.CompareOrdinal(privateHash, nonceparts[1]) == 0;
        }

        /// <summary>Stale nonce.</summary>
        ///
        /// <param name="nonce">  The nonce.</param>
        /// <param name="Timeout">The timeout.</param>
        ///
        /// <returns>true if it succeeds, false if it fails.</returns>
        public bool StaleNonce(string nonce, int Timeout)
        {
            var nonceparts = GetNonceParts(nonce);
            return TimeStampAsDateTime(nonceparts[0]).AddSeconds(Timeout) < DateTime.UtcNow;
        }
        private DateTime TimeStampAsDateTime(string TimeStamp)
        {
            double nonceTimeStampDouble;
            if (Double.TryParse(TimeStamp, NumberStyles.Float, CultureInfo.InvariantCulture, out nonceTimeStampDouble))
            {
               return DateTime.MinValue.AddMilliseconds(nonceTimeStampDouble);
            }
            throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "The given nonce time stamp {0} was not valid", TimeStamp));
        }

        /// <summary>Converts a hash to a hexadecimal string.</summary>
        ///
        /// <param name="hash">The hash.</param>
        ///
        /// <returns>The given data converted to a hexadecimal string.</returns>
        public string ConvertToHexString(IEnumerable<byte> hash)
        {
            var hexString = new StringBuilder();
            foreach (byte byteFromHash in hash)
            {
                hexString.AppendFormat("{0:x2}", byteFromHash);
            }
            return hexString.ToString();
        }

        /// <summary>Creates authentication response.</summary>
        ///
        /// <param name="digestHeaders">The digest headers.</param>
        /// <param name="Ha1">          The first ha.</param>
        ///
        /// <returns>The new authentication response.</returns>
        public string CreateAuthResponse(Dictionary<string, string> digestHeaders, string Ha1)
        {
            string Ha2 = CreateHa2(digestHeaders);
            return CreateAuthResponse(digestHeaders, Ha1, Ha2);
        }

        /// <summary>Creates authentication response.</summary>
        ///
        /// <param name="digestHeaders">The digest headers.</param>
        /// <param name="Ha1">          The first ha.</param>
        /// <param name="Ha2">          The second ha.</param>
        ///
        /// <returns>The new authentication response.</returns>
        public string CreateAuthResponse(Dictionary<string, string> digestHeaders, string Ha1, string Ha2)
        {
            string response = string.Format(CultureInfo.InvariantCulture, "{0}:{1}:{2}:{3}:{4}:{5}", Ha1, digestHeaders["nonce"], digestHeaders["nc"], digestHeaders["cnonce"], digestHeaders["qop"].ToLower(), Ha2);
            return ConvertToHexString(MD5.Create().ComputeHash(Encoding.UTF8.GetBytes(response)));
        }

        /// <summary>Creates ha 1.</summary>
        ///
        /// <param name="digestHeaders">The digest headers.</param>
        /// <param name="password">     The password.</param>
        ///
        /// <returns>The new ha 1.</returns>
        public string CreateHa1(Dictionary<string,string> digestHeaders, string password)
        {
            return CreateHa1(digestHeaders["username"],digestHeaders["realm"],password);
        }

        /// <summary>Creates ha 1.</summary>
        ///
        /// <param name="Username">The username.</param>
        /// <param name="Realm">   The realm.</param>
        /// <param name="Password">The password.</param>
        ///
        /// <returns>The new ha 1.</returns>
        public string CreateHa1(string Username, string Realm, string Password)
        {
            return ConvertToHexString(MD5.Create().ComputeHash(Encoding.UTF8.GetBytes(string.Format("{0}:{1}:{2}", Username,Realm,Password))));
        }

        /// <summary>Creates ha 2.</summary>
        ///
        /// <param name="digestHeaders">The digest headers.</param>
        ///
        /// <returns>The new ha 2.</returns>
        public string CreateHa2(Dictionary<string, string> digestHeaders)
        {
            return ConvertToHexString(MD5.Create().ComputeHash(Encoding.UTF8.GetBytes(string.Format("{0}:{1}",digestHeaders["method"],digestHeaders["uri"]))));
        }

        /// <summary>Validates the response.</summary>
        ///
        /// <param name="digestInfo">  Information describing the digest.</param>
        /// <param name="PrivateKey">  The private key.</param>
        /// <param name="NonceTimeOut">The nonce time out.</param>
        /// <param name="DigestHA1">   The first digest ha.</param>
        /// <param name="sequence">    The sequence.</param>
        ///
        /// <returns>true if it succeeds, false if it fails.</returns>
        public bool ValidateResponse(Dictionary<string, string> digestInfo, string PrivateKey, int NonceTimeOut, string DigestHA1, string sequence)
        {
            var noncevalid = ValidateNonce(digestInfo["nonce"], digestInfo["userhostaddress"], PrivateKey);
            var noncestale = StaleNonce(digestInfo["nonce"], NonceTimeOut);
            var uservalid = CreateAuthResponse(digestInfo, DigestHA1) == digestInfo["response"];
            var sequencevalid = sequence != digestInfo["nc"];
            return noncevalid && !noncestale && uservalid && sequencevalid;
        }
    }
}
