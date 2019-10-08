using System;
using System.IO;
using Google.Protobuf;
using Xunit;

namespace TheDotNetLeague.MultiFormats.MultiAddress.Tests
{
    public class NetworkProtocolTest
    {
        private class NameExists : NetworkProtocol
        {
            public override string Name => "tcp";
            public override uint Code => 0x7FFF;

            public override void ReadValue(CodedInputStream stream)
            {
            }

            public override void ReadValue(TextReader stream)
            {
            }

            public override void WriteValue(CodedOutputStream stream)
            {
            }
        }

        private class CodeExists : NetworkProtocol
        {
            public override string Name => "x-tcp";
            public override uint Code => 6;

            public override void ReadValue(CodedInputStream stream)
            {
            }

            public override void ReadValue(TextReader stream)
            {
            }

            public override void WriteValue(CodedOutputStream stream)
            {
            }
        }

        [Fact]
        public void Register_Code_Already_Exists()
        {
            Assert.Throws<ArgumentException>(() => NetworkProtocol.Register<CodeExists>());
        }

        [Fact]
        public void Register_Name_Already_Exists()
        {
            Assert.Throws<ArgumentException>(() => NetworkProtocol.Register<NameExists>());
        }

        [Fact]
        public void Stringing()
        {
            Assert.Equal("/tcp/8080", new MultiAddress("/tcp/8080").Protocols[0].ToString());
        }
    }
}