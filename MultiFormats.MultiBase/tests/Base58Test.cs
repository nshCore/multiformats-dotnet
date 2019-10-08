using System;
using System.Linq;
using System.Text;
using Xunit;

namespace TheDotNetLeague.MultiFormats.MultiBase.Tests
{
    public class Base58Test
    {
        [Fact]
        public void Decode()
        {
            Assert.Equal("this is a test", Encoding.UTF8.GetString(Base58.Decode("jo91waLQA1NNeBmZKUF")));
            Assert.Equal("this is a test", Encoding.UTF8.GetString("jo91waLQA1NNeBmZKUF".FromBase58()));
        }

        [Fact]
        public void Decode_Bad()
        {
            Assert.Throws<InvalidOperationException>(() => Base58.Decode("jo91waLQA1NNeBmZKUF=="));
        }

        [Fact]
        public void Encode()
        {
            Assert.Equal("jo91waLQA1NNeBmZKUF", Base58.Encode(Encoding.UTF8.GetBytes("this is a test")));
            Assert.Equal("jo91waLQA1NNeBmZKUF", Encoding.UTF8.GetBytes("this is a test").ToBase58());
        }

        /// <summary>
        ///     C# version of base58Test in
        ///     <see href="https://github.com/ipfs/java-ipfs-api/blob/master/test/org/ipfs/Test.java" />
        /// </summary>
        [Fact]
        public void Java()
        {
            var input = "QmPZ9gcCEpqKTo6aq61g2nXGUhM4iCL3ewB6LDXZCtioEB";
            var output = Base58.Decode(input);
            var encoded = Base58.Encode(output);
            Assert.Equal(input, encoded);
        }

        [Fact]
        public void Zero()
        {
            Assert.Equal("1111111", Base58.Encode(new byte[7]));
            Assert.Equal(7, Base58.Decode("1111111").Length);
            Assert.True(Base58.Decode("1111111").All(b => b == 0));
        }
    }
}