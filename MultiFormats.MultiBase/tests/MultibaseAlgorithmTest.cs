using System;
using System.Linq;
using FluentAssertions;
using Xunit;

namespace TheDotNetLeague.MultiFormats.MultiBase.Tests
{
    public class MultiBaseAlgorithmTest
    {
        [Fact]
        public void Algorithms_Are_Enumerable()
        {
            Assert.NotEqual(0, MultiBaseAlgorithm.All.Count());
        }

        [Fact]
        public void Bad_Name()
        {
            Assert.Throws<ArgumentNullException>(() => MultiBaseAlgorithm.Register(null, '?'));
            Assert.Throws<ArgumentNullException>(() => MultiBaseAlgorithm.Register("", '?'));
            Assert.Throws<ArgumentNullException>(() => MultiBaseAlgorithm.Register("   ", '?'));
        }

        [Fact]
        public void Code_Already_Exists()
        {
            Assert.Throws<ArgumentException>(() => MultiBaseAlgorithm.Register("base58btc-x", 'z'));
        }

        [Fact]
        public void Known_But_NYI()
        {
            var alg = MultiBaseAlgorithm.Register("nyi", 'n');
            try
            {
                Assert.Throws<NotImplementedException>(() => alg.Encode(null));
                Assert.Throws<NotImplementedException>(() => alg.Decode(null));
            }
            finally
            {
                MultiBaseAlgorithm.Deregister(alg);
            }
        }

        [Fact]
        public void Name_Already_Exists()
        {
            Assert.Throws<ArgumentException>(() => MultiBaseAlgorithm.Register("base58btc", 'z'));
        }

        [Fact]
        public void Name_Is_Also_ToString()
        {
            foreach (var alg in MultiBaseAlgorithm.All) Assert.Equal(alg.Name, alg.ToString());
        }

        [Fact]
        public void Roundtrip_All_Algorithms()
        {
            var bytes = new byte[]
            {
                1, 2, 3, 4, 5
            };

            foreach (var alg in MultiBaseAlgorithm.All)
            {
                var s = alg.Encode(bytes);
                bytes.Should().Contain(alg.Decode(s), alg.Name);
            }
        }
    }
}