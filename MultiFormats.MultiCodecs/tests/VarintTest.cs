using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using TheDotNetLeague.MultiFormats.MultiBase;
using Xunit;

namespace TheDotNetLeague.MultiFormats.MultiCodecs.Tests
{
    public class VarintTest
    {
        [Fact]
        public void Decode_From_Offset()
        {
            var x = new byte[]
            {
                0x00, 0xAC, 0x02
            };
            Assert.Equal(300, Varint.DecodeInt32(x, 1));
        }

        [Fact]
        public void Empty()
        {
            var bytes = new byte[0];
            Assert.Throws<EndOfStreamException>(() => Varint.DecodeInt64(bytes));
        }

        [Fact]
        public void Encode_Negative()
        {
            Assert.Throws<NotSupportedException>(() => Varint.Encode(-1));
        }

        [Fact]
        public void Example()
        {
            for (long v = 1; v <= 0xFFFFFFFL; v = v << 4)
            {
                Console.Write($"| {v} (0x{v.ToString("x")}) ");
                Console.WriteLine($"| {Varint.Encode(v).ToHexString()} |");
            }
        }

        [Fact]
        public void MaxLong()
        {
            var x = "ffffffffffffffff7f".ToHexBuffer();
            Assert.Equal(9, Varint.RequiredBytes(long.MaxValue));
            x.Should().Contain(Varint.Encode(long.MaxValue));
            Assert.Equal(long.MaxValue, Varint.DecodeInt64(x));
        }

        [Fact]
        public async Task ReadAsync()
        {
            using (var ms = new MemoryStream("ffffffffffffffff7f".ToHexBuffer()))
            {
                var v = await ms.ReadVarint64Async();
                Assert.Equal(long.MaxValue, v);
            }
        }

        [Fact]
        public void ReadAsync_Cancel()
        {
            var ms = new MemoryStream(new byte[]
            {
                0
            });
            var cs = new CancellationTokenSource();
            cs.Cancel();
            Assert.ThrowsAsync<TaskCanceledException>(() => ms.ReadVarint32Async(cs.Token));
        }

        [Fact]
        public void ThreeHundred()
        {
            var x = new byte[]
            {
                0xAC, 0x02
            };
            Assert.Equal(2, Varint.RequiredBytes(300));
            x.Should().Contain(Varint.Encode(300));
            Assert.Equal(300, Varint.DecodeInt32(x));
        }

        [Fact]
        public void TooBig_Int32()
        {
            var bytes = Varint.Encode((long)
                                      int.MaxValue + 1);
            Assert.Throws<InvalidDataException>(() => Varint.DecodeInt32(bytes));
        }

        [Fact]
        public void TooBig_Int64()
        {
            var bytes = "ffffffffffffffffff7f".ToHexBuffer();
            Assert.Throws<InvalidDataException>(() => Varint.DecodeInt64(bytes));
        }

        [Fact]
        public void Unterminated()
        {
            var bytes = "ff".ToHexBuffer();
            Assert.Throws<InvalidDataException>(() => Varint.DecodeInt64(bytes));
        }

        [Fact]
        public async Task WriteAsync()
        {
            using (var ms = new MemoryStream())
            {
                await ms.WriteVarintAsync(long.MaxValue);
                ms.Position = 0;
                Assert.Equal(long.MaxValue, ms.ReadVarint64());
            }
        }

        [Fact]
        public void WriteAsync_Cancel()
        {
            var ms = new MemoryStream();
            var cs = new CancellationTokenSource(100);
            cs.Token.ThrowIfCancellationRequested();
            Assert.ThrowsAsync<TaskCanceledException>(() => ms.WriteVarintAsync(0, cs.Token));
        }

        [Fact]
        public void WriteAsync_Negative()
        {
            var ms = new MemoryStream();
            Assert.ThrowsAsync<Exception>(() => ms.WriteVarintAsync(-1));
        }

        [Fact]
        public void Zero()
        {
            var x = new byte[]
            {
                0
            };
            Assert.Equal(1, Varint.RequiredBytes(0));
            x.Should().Contain(Varint.Encode(0));
            Assert.Equal(0, Varint.DecodeInt32(x));
        }
    }
}