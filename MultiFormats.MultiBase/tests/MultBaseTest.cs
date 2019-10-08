using System;
using System.Collections.Generic;
using System.Text;
using FluentAssertions;
using Xunit;

namespace TheDotNetLeague.MultiFormats.MultiBase.Tests
{
    public class MultiBaseTest
    {
        private readonly TestVector[] TestVectors =
        {
            new TestVector
            {
                Algorithm = "base16",
                Input = "yes mani !",
                Output = "f796573206d616e692021"
            },
            new TestVector
            {
                Algorithm = "base32",
                Input = "yes mani !",
                Output = "bpfsxgidnmfxgsibb"
            },
            new TestVector
            {
                Algorithm = "base32pad",
                Input = "yes mani !",
                Output = "cpfsxgidnmfxgsibb"
            },
            new TestVector
            {
                Algorithm = "base32",
                Input = "f",
                Output = "bmy"
            },
            new TestVector
            {
                Algorithm = "base32pad",
                Input = "f",
                Output = "cmy======"
            },
            new TestVector
            {
                Algorithm = "base32hex",
                Input = "f",
                Output = "vco"
            },
            new TestVector
            {
                Algorithm = "base32hexpad",
                Input = "f",
                Output = "tco======"
            },
            new TestVector
            {
                Algorithm = "base64pad",
                Input = "f",
                Output = "MZg=="
            },
            new TestVector
            {
                Algorithm = "base64",
                Input = "f",
                Output = "mZg"
            },
            new TestVector
            {
                Algorithm = "base64",
                Input = "\u00f7\u00ef\u00ff",
                Output = "mw7fDr8O/"
            },
            new TestVector
            {
                Algorithm = "base64url",
                Input = "\u00f7\u00ef\u00ff",
                Output = "uw7fDr8O_"
            },
            new TestVector
            {
                Algorithm = "base64url",
                Input = "f",
                Output = "uZg"
            },
            new TestVector
            {
                Algorithm = "base64url",
                Input = "fo",
                Output = "uZm8"
            },
            new TestVector
            {
                Algorithm = "base64url",
                Input = "foo",
                Output = "uZm9v"
            },
            new TestVector
            {
                Algorithm = "BASE16",
                Input = "yes mani !",
                Output = "F796573206D616E692021"
            },
            new TestVector
            {
                Algorithm = "BASE32",
                Input = "yes mani !",
                Output = "BPFSXGIDNMFXGSIBB"
            },
            new TestVector
            {
                Algorithm = "BASE32PAD",
                Input = "yes mani !",
                Output = "CPFSXGIDNMFXGSIBB"
            },
            new TestVector
            {
                Algorithm = "BASE32",
                Input = "f",
                Output = "BMY"
            },
            new TestVector
            {
                Algorithm = "BASE32PAD",
                Input = "f",
                Output = "CMY======"
            },
            new TestVector
            {
                Algorithm = "BASE32HEX",
                Input = "f",
                Output = "VCO"
            },
            new TestVector
            {
                Algorithm = "BASE32HEXPAD",
                Input = "f",
                Output = "TCO======"
            },
            new TestVector
            {
                Algorithm = "base32z",
                Input = "Decentralize everything!!",
                Output = "het1sg3mqqt3gn5djxj11y3msci3817depfzgqejb"
            },
            new TestVector
            {
                Algorithm = "base32z",
                Input = "yes mani !",
                Output = "hxf1zgedpcfzg1ebb"
            },
            new TestVector
            {
                Algorithm = "base32z",
                Input = "hello world",
                Output = "hpb1sa5dxrb5s6hucco"
            },
            new TestVector
            {
                Algorithm = "base32z",
                Input = "\x00\x00yes mani !",
                Output = "hyyy813murbssn5ujryoo"
            }
        };

        private class TestVector
        {
            public string Algorithm { get; set; }
            public string Input { get; set; }
            public string Output { get; set; }
        }

        /// <summary>
        ///     Test vectors from various sources.
        /// </summary>
        [Fact]
        public void CheckMultiBase()
        {
            foreach (var v in TestVectors)
            {
                var bytes = Encoding.UTF8.GetBytes(v.Input);
                var s = MultiBase.Encode(bytes, v.Algorithm);
                Assert.Equal(v.Output, s);
                bytes.Should().Contain(MultiBase.Decode(s));
            }
        }

        [Fact]
        public void Codec()
        {
            var bytes = new byte[] {1, 2, 3, 4, 5};
            var bytes1 = MultiBase.Decode(MultiBase.Encode(bytes));
            var bytes2 = MultiBase.Decode(MultiBase.Encode(bytes, "base16"));
            bytes.Should().Contain(bytes1);
            bytes.Should().Contain(bytes2);
        }

        [Fact]
        public void Decode_Bad_Formats()
        {
            Assert.Throws<ArgumentNullException>(() => MultiBase.Decode(null));
            Assert.Throws<ArgumentNullException>(() => MultiBase.Decode(""));
            Assert.Throws<ArgumentNullException>(() => MultiBase.Decode("   "));

            Assert.Throws<FormatException>(() => MultiBase.Decode("?"));
            Assert.Throws<FormatException>(() => MultiBase.Decode("??"));
            Assert.Throws<FormatException>(() => MultiBase.Decode("???"));
            Assert.Throws<FormatException>(() => MultiBase.Decode("fXX"));
        }

        [Fact]
        public void EmptyData()
        {
            var empty = new byte[0];
            foreach (var alg in MultiBaseAlgorithm.All)
            {
                var s = MultiBase.Encode(empty, alg.Name);
                empty.Should().Equal(MultiBase.Decode(s), alg.Name);
            }
        }

        [Fact]
        public void Encode_Null_Data_Not_Allowed()
        {
            Assert.Throws<ArgumentNullException>(() => MultiBase.Encode(null));
        }

        [Fact]
        public void Encode_Unknown_Algorithm()
        {
            var bytes = new byte[] {1, 2, 3, 4, 5};
            Assert.Throws<KeyNotFoundException>(() => MultiBase.Encode(bytes, "unknown"));
        }

        [Fact]
        public void Invalid_Encoded_String()
        {
            foreach (var alg in MultiBaseAlgorithm.All)
            {
                var bad = alg.Code + "?";
                Assert.Throws<FormatException>(() => MultiBase.Decode(bad));
            }
        }
    }
}