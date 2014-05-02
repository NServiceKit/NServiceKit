using System;
using System.Security.Cryptography;
using System.Text;

namespace NServiceKit.ServiceInterface.Auth
{
    /// <summary>Interface for hash provider.</summary>
    public interface IHashProvider
    {
        /// <summary>Gets hash and salt.</summary>
        ///
        /// <param name="Data">The data.</param>
        /// <param name="Hash">The hash.</param>
        /// <param name="Salt">The salt.</param>
        void GetHashAndSalt(byte[] Data, out byte[] Hash, out byte[] Salt);

        /// <summary>Gets hash and salt string.</summary>
        ///
        /// <param name="Data">The data.</param>
        /// <param name="Hash">The hash.</param>
        /// <param name="Salt">The salt.</param>
        void GetHashAndSaltString(string Data, out string Hash, out string Salt);

        /// <summary>Verify hash.</summary>
        ///
        /// <param name="Data">The data.</param>
        /// <param name="Hash">The hash.</param>
        /// <param name="Salt">The salt.</param>
        ///
        /// <returns>true if it succeeds, false if it fails.</returns>
        bool VerifyHash(byte[] Data, byte[] Hash, byte[] Salt);

        /// <summary>Verify hash string.</summary>
        ///
        /// <param name="Data">The data.</param>
        /// <param name="Hash">The hash.</param>
        /// <param name="Salt">The salt.</param>
        ///
        /// <returns>true if it succeeds, false if it fails.</returns>
        bool VerifyHashString(string Data, string Hash, string Salt);
    }

    /// <summary>
    /// Thank you Martijn
    /// http://www.dijksterhuis.org/creating-salted-hash-values-in-c/
    /// </summary>
    public class SaltedHash : IHashProvider
    {
        readonly HashAlgorithm HashProvider;
        readonly int SalthLength;

        /// <summary>Initializes a new instance of the NServiceKit.ServiceInterface.Auth.SaltedHash class.</summary>
        ///
        /// <param name="HashAlgorithm">The hash algorithm.</param>
        /// <param name="theSaltLength">Length of the salt.</param>
        public SaltedHash(HashAlgorithm HashAlgorithm, int theSaltLength)
        {
            HashProvider = HashAlgorithm;
            SalthLength = theSaltLength;
        }

        /// <summary>Initializes a new instance of the NServiceKit.ServiceInterface.Auth.SaltedHash class.</summary>
        public SaltedHash() : this(new SHA256Managed(), 4) {}

        private byte[] ComputeHash(byte[] Data, byte[] Salt)
        {
            var DataAndSalt = new byte[Data.Length + SalthLength];
            Array.Copy(Data, DataAndSalt, Data.Length);
            Array.Copy(Salt, 0, DataAndSalt, Data.Length, SalthLength);

            return HashProvider.ComputeHash(DataAndSalt);
        }

        /// <summary>Gets hash and salt.</summary>
        ///
        /// <param name="Data">The data.</param>
        /// <param name="Hash">The hash.</param>
        /// <param name="Salt">The salt.</param>
        public void GetHashAndSalt(byte[] Data, out byte[] Hash, out byte[] Salt)
        {
            Salt = new byte[SalthLength];

            var random = new RNGCryptoServiceProvider();
            random.GetNonZeroBytes(Salt);

            Hash = ComputeHash(Data, Salt);
        }

        /// <summary>Gets hash and salt string.</summary>
        ///
        /// <param name="Data">The data.</param>
        /// <param name="Hash">The hash.</param>
        /// <param name="Salt">The salt.</param>
        public void GetHashAndSaltString(string Data, out string Hash, out string Salt)
        {
            byte[] HashOut;
            byte[] SaltOut;

            GetHashAndSalt(Encoding.UTF8.GetBytes(Data), out HashOut, out SaltOut);

            Hash = Convert.ToBase64String(HashOut);
            Salt = Convert.ToBase64String(SaltOut);
        }

        /// <summary>Verify hash.</summary>
        ///
        /// <param name="Data">The data.</param>
        /// <param name="Hash">The hash.</param>
        /// <param name="Salt">The salt.</param>
        ///
        /// <returns>true if it succeeds, false if it fails.</returns>
        public bool VerifyHash(byte[] Data, byte[] Hash, byte[] Salt)
        {
            var NewHash = ComputeHash(Data, Salt);

            if (NewHash.Length != Hash.Length) return false;

            for (int Lp = 0; Lp < Hash.Length; Lp++)
                if (!Hash[Lp].Equals(NewHash[Lp]))
                    return false;

            return true;
        }

        /// <summary>Verify hash string.</summary>
        ///
        /// <param name="Data">The data.</param>
        /// <param name="Hash">The hash.</param>
        /// <param name="Salt">The salt.</param>
        ///
        /// <returns>true if it succeeds, false if it fails.</returns>
        public bool VerifyHashString(string Data, string Hash, string Salt)
        {
            byte[] HashToVerify = Convert.FromBase64String(Hash);
            byte[] SaltToVerify = Convert.FromBase64String(Salt);
            byte[] DataToVerify = Encoding.UTF8.GetBytes(Data);
            return VerifyHash(DataToVerify, HashToVerify, SaltToVerify);
        }
    }

    /*
    /// <summary>
    /// This little demo code shows how to encode a users password.
    /// </summary>
    class SaltedHashDemo
    {
        public static void Main(string[] args)
        {
            // We use the default SHA-256 & 4 byte length
            SaltedHash demo = new SaltedHash();

            // We have a password, which will generate a Hash and Salt
            string Password = "MyGlook234";
            string Hash;
            string Salt;

            demo.GetHashAndSaltString(Password, out Hash, out Salt);
            Console.WriteLine("Password = {0} , Hash = {1} , Salt = {2}", Password, Hash, Salt);

            // Password validation
            //
            // We need to pass both the earlier calculated Hash and Salt (we need to store this somewhere safe between sessions)

            // First check if a wrong password passes
            string WrongPassword = "OopsOops";
            Console.WriteLine("Verifying {0} = {1}", WrongPassword, demo.VerifyHashString(WrongPassword, Hash, Salt));

            // Check if the correct password passes
            Console.WriteLine("Verifying {0} = {1}", Password, demo.VerifyHashString(Password, Hash, Salt));
        }	 
    }
 */

}