using System;
using System.IO;
using System.Linq;
using FluentAssertions;
using Xunit;

namespace TheDotNetLeague.MultiFormats.MultiBase.Tests
{
    public class HexStringTest
    {
        [Fact]
        public void Decode()
        {
            var buffer = Enumerable.Range(byte.MinValue, byte.MaxValue).Select(b => (byte) b).ToArray();
            var lowerHex = string.Concat(buffer.Select(b => b.ToString("x2")).ToArray());
            var upperHex = string.Concat(buffer.Select(b => b.ToString("X2")).ToArray());

            buffer.Should().Contain(lowerHex.ToHexBuffer(), "decode lower");
            buffer.Should().Contain(upperHex.ToHexBuffer(), "decode upper");
        }

        [Fact]
        public void Encode()
        {
            var buffer = Enumerable.Range(byte.MinValue, byte.MaxValue).Select(b => (byte) b).ToArray();
            var lowerHex = string.Concat(buffer.Select(b => b.ToString("x2")).ToArray());
            var upperHex = string.Concat(buffer.Select(b => b.ToString("X2")).ToArray());

            Assert.Equal(lowerHex, buffer.ToHexString());
            Assert.Equal(lowerHex, buffer.ToHexString());
            Assert.Equal(lowerHex, buffer.ToHexString("x"));
            Assert.Equal(upperHex, buffer.ToHexString("X"));
        }

        [Fact]
        public void InvalidFormatSpecifier()
        {
            Assert.Throws<FormatException>(() => HexString.Encode(new byte[0], "..."));
        }

        [Fact]
        public void InvalidHexStrings()
        {
            Assert.Throws<InvalidDataException>(() => HexString.Decode("0"));
            Assert.Throws<InvalidDataException>(() => HexString.Decode("0Z"));
        }
    }
}