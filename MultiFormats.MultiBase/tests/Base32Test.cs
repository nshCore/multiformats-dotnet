using System.Text;
using FluentAssertions;
using Xunit;

namespace TheDotNetLeague.MultiFormats.MultiBase.Tests
{
    public class Base32EncodeTests
    {
        private byte[] GetStringBytes(string x)
        {
            return Encoding.ASCII.GetBytes(x);
        }

        [Fact]
        public void Vector1()
        {
            Assert.Equal(string.Empty, Base32.Encode(GetStringBytes(string.Empty)));
        }

        [Fact]
        public void Vector2()
        {
            Assert.Equal("my", Base32.Encode(GetStringBytes("f")));
        }

        [Fact]
        public void Vector3()
        {
            Assert.Equal("mzxq", Base32.Encode(GetStringBytes("fo")));
        }

        [Fact]
        public void Vector4()
        {
            Assert.Equal("mzxw6", Base32.Encode(GetStringBytes("foo")));
        }

        [Fact]
        public void Vector5()
        {
            Assert.Equal("mzxw6yq", Base32.Encode(GetStringBytes("foob")));
        }

        [Fact]
        public void Vector6()
        {
            Assert.Equal("mzxw6ytb", Base32.Encode(GetStringBytes("fooba")));
        }

        [Fact]
        public void Vector7()
        {
            Assert.Equal("mzxw6ytboi", Base32.Encode(GetStringBytes("foobar")));
        }
    }

    public class Base32DecodeTests
    {
        private byte[] GetStringBytes(string x)
        {
            return Encoding.ASCII.GetBytes(x);
        }

        [Fact]
        public void Vector1()
        {
            GetStringBytes(string.Empty).Should().Equal(Base32.Decode(string.Empty));
        }

        [Fact]
        public void Vector2()
        {
            GetStringBytes("f").Should().Contain(Base32.Decode("MY======"));
        }

        [Fact]
        public void Vector3()
        {
            GetStringBytes("fo").Should().Contain(Base32.Decode("MZXQ===="));
        }

        [Fact]
        public void Vector4()
        {
            GetStringBytes("foo").Should().Contain(Base32.Decode("MZXW6==="));
        }

        [Fact]
        public void Vector5()
        {
            GetStringBytes("foob").Should().Contain(Base32.Decode("MZXW6YQ="));
        }

        [Fact]
        public void Vector6()
        {
            GetStringBytes("fooba").Should().Contain(Base32.Decode("MZXW6YTB"));
        }

        [Fact]
        public void Vector7()
        {
            GetStringBytes("foobar").Should().Contain(Base32.Decode("MZXW6YTBOI======"));
        }
    }
}