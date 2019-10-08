using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using TheDotNetLeague.MultiFormats.MultiBase;
using Xunit;

namespace TheDotNetLeague.MultiFormats.MultiHash.Tests
{
    public class HashingAlgorithmTest
    {
        [Fact]
        public void GetHasher()
        {
            using (var hasher = HashingAlgorithm.GetAlgorithm("sha3-256"))
            {
                Assert.NotNull(hasher);
                var input = new byte[]
                {
                    0xe9
                };
                var expected = "f0d04dd1e6cfc29a4460d521796852f25d9ef8d28b44ee91ff5b759d72c1e6d6".ToHexBuffer();

                var actual = hasher.ComputeHash(input);
                expected.Should().Contain(actual);
            }
        }

        [Fact]
        public void GetHasher_Unknown()
        {
            Assert.Throws<KeyNotFoundException>(() => HashingAlgorithm.GetAlgorithm("unknown"));
        }

        [Fact]
        public void GetMetadata()
        {
            var info = HashingAlgorithm.GetAlgorithmMetadata("sha3-256");
            Assert.NotNull(info);
            Assert.Equal("sha3-256", info.Name);
            Assert.Equal(0x16, info.Code);
            Assert.Equal(256 / 8, info.DigestSize);
            Assert.NotNull(info.Hasher);
        }

        [Fact]
        public void GetMetadata_Alias()
        {
            var info = HashingAlgorithm.GetAlgorithmMetadata("id");
            Assert.NotNull(info);
            Assert.Equal("identity", info.Name);
            Assert.Equal(0, info.Code);
            Assert.Equal(0, info.DigestSize);
            Assert.NotNull(info.Hasher);
        }

        [Fact]
        public void GetMetadata_Unknown()
        {
            Assert.Throws<KeyNotFoundException>(() => HashingAlgorithm.GetAlgorithmMetadata("unknown"));
        }

        [Fact]
        public void HashingAlgorithm_Alias_Already_Exists()
        {
            Assert.Throws<ArgumentException>(() => HashingAlgorithm.RegisterAlias("id", "identity"));
        }

        [Fact]
        public void HashingAlgorithm_Alias_Target_Does_Not_Exist()
        {
            Assert.Throws<ArgumentException>(() => HashingAlgorithm.RegisterAlias("foo", "sha1-x"));
        }

        [Fact]
        public void HashingAlgorithm_Alias_Target_Is_Bad()
        {
            Assert.Throws<ArgumentNullException>(() => HashingAlgorithm.RegisterAlias("foo", "  "));
        }

        [Fact]
        public void HashingAlgorithm_Bad_Alias()
        {
            Assert.Throws<ArgumentNullException>(() => HashingAlgorithm.RegisterAlias(null, "sha1"));
            Assert.Throws<ArgumentNullException>(() => HashingAlgorithm.RegisterAlias("", "sha1"));
            Assert.Throws<ArgumentNullException>(() => HashingAlgorithm.RegisterAlias("   ", "sha1"));
        }

        [Fact]
        public void HashingAlgorithm_Bad_Name()
        {
            Assert.Throws<ArgumentNullException>(() => HashingAlgorithm.Register(null, 1, 1));
            Assert.Throws<ArgumentNullException>(() => HashingAlgorithm.Register("", 1, 1));
            Assert.Throws<ArgumentNullException>(() => HashingAlgorithm.Register("   ", 1, 1));
        }

        [Fact]
        public void HashingAlgorithm_Name_Already_Exists()
        {
            Assert.Throws<ArgumentException>(() => HashingAlgorithm.Register("sha1", 0x11, 1));
        }

        [Fact]
        public void HashingAlgorithm_Number_Already_Exists()
        {
            Assert.Throws<ArgumentException>(() => HashingAlgorithm.Register("sha1-x", 0x11, 1));
        }

        [Fact]
        public void HashingAlgorithms_Are_Enumerable()
        {
            Assert.True(5 <= HashingAlgorithm.All.Count());
        }
    }
}