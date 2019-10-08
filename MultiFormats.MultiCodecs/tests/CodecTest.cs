using System;
using System.Linq;
using Xunit;

namespace TheDotNetLeague.MultiFormats.MultiCodecs.Tests
{
    public class CodecTest
    {
        [Fact]
        public void Algorithms_Are_Enumerable()
        {
            Assert.NotEqual(0, Codec.All.Count());
        }

        [Fact]
        public void Bad_Name()
        {
            Assert.Throws<ArgumentNullException>(() => Codec.Register(null, 1));
            Assert.Throws<ArgumentNullException>(() => Codec.Register("", 1));
            Assert.Throws<ArgumentNullException>(() => Codec.Register("   ", 1));
        }

        [Fact]
        public void Code_Already_Exists()
        {
            Assert.Throws<ArgumentException>(() => Codec.Register("raw-x", 0x55));
        }

        [Fact]
        public void Name_Already_Exists()
        {
            Assert.Throws<ArgumentException>(() => Codec.Register("raw", 1));
        }

        [Fact]
        public void Register()
        {
            var codec = Codec.Register("something-new", 0x0bad);
            try
            {
                Assert.Equal("something-new", codec.Name);
                Assert.Equal("something-new", codec.ToString());
                Assert.Equal(0x0bad, codec.Code);
            }
            finally
            {
                Codec.Deregister(codec);
            }
        }
    }
}