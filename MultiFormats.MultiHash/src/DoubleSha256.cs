using System;
using System.Security.Cryptography;

namespace TheDotNetLeague.MultiFormats.MultiHash
{
    internal class DoubleSha256 : HashAlgorithm
    {
        private readonly HashAlgorithm digest = SHA256.Create();
        private byte[] round1;

        public override int HashSize => digest.HashSize;

        public override void Initialize()
        {
            digest.Initialize();
            round1 = null;
        }

        protected override void HashCore(byte[] array, int ibStart, int cbSize)
        {
            if (round1 != null)
                throw new NotSupportedException("Already called.");

            round1 = digest.ComputeHash(array, ibStart, cbSize);
        }

        protected override byte[] HashFinal()
        {
            digest.Initialize();
            return digest.ComputeHash(round1);
        }
    }
}