using System;
using System.IO;
using System.Net;
using Newtonsoft.Json;
using TheDotNetLeague.MultiFormats.MultiBase;
using Xunit;

namespace TheDotNetLeague.MultiFormats.MultiAddress.Tests
{
    public class MultiAddressTest
    {
        private const string somewhere =
            "/ip4/10.1.10.10/tcp/29087/ipfs/QmVcSqVEsvm5RR9mBLjwpb2XjFVn5bPdPL69mL8PH45pPC";

        private const string nowhere = "/ip4/10.1.10.11/tcp/29087/ipfs/QmVcSqVEsvm5RR9mBLjwpb2XjFVn5bPdPL69mL8PH45pPC";

        [Fact]
        public void IsToString()
        {
            Assert.Equal(somewhere, new MultiAddress(somewhere).ToString());
        }

        [Fact]
        public void Missing_Protocol_Name()
        {
            Assert.Throws<FormatException>(() => new MultiAddress("/"));
        }

        [Fact]
        public void Parsing()
        {
            var a = new MultiAddress(somewhere);
            Assert.Equal(3, a.Protocols.Count);
            Assert.Equal("ip4", a.Protocols[0].Name);
            Assert.Equal("10.1.10.10", a.Protocols[0].Value);
            Assert.Equal("tcp", a.Protocols[1].Name);
            Assert.Equal("29087", a.Protocols[1].Value);
            Assert.Equal("ipfs", a.Protocols[2].Name);
            Assert.Equal("QmVcSqVEsvm5RR9mBLjwpb2XjFVn5bPdPL69mL8PH45pPC", a.Protocols[2].Value);

            Assert.Equal(0, new MultiAddress((string) null).Protocols.Count);
            Assert.Equal(0, new MultiAddress("").Protocols.Count);
            Assert.Equal(0, new MultiAddress("  ").Protocols.Count);
        }

        [Fact]
        public void Unknown_Protocol_Name()
        {
            Assert.Throws<FormatException>(() => new MultiAddress("/foobar/123"));
        }

        [Fact]
        public void Value_Equality()
        {
            var a0 = new MultiAddress(somewhere);
            var a1 = new MultiAddress(somewhere);
            var b = new MultiAddress(nowhere);
            MultiAddress c = null;
            MultiAddress d = null;

            Assert.True(c == d);
            Assert.False(c == b);
            Assert.False(b == c);

            Assert.False(c != d);
            Assert.True(c != b);
            Assert.True(b != c);

#pragma warning disable 1718
            Assert.True(a0 == a0);
            Assert.True(a0 == a1);
            Assert.False(a0 == b);

#pragma warning disable 1718
            Assert.False(a0 != a0);
            Assert.False(a0 != a1);
            Assert.True(a0 != b);

            Assert.True(a0.Equals(a0));
            Assert.True(a0.Equals(a1));
            Assert.False(a0.Equals(b));

            Assert.Equal(a0, a0);
            Assert.Equal(a0, a1);
            Assert.NotEqual(a0, b);

            Assert.Equal(a0, a0);
            Assert.Equal(a0, a1);
            Assert.NotEqual(a0, b);

            Assert.Equal(a0.GetHashCode(), a0.GetHashCode());
            Assert.Equal(a0.GetHashCode(), a1.GetHashCode());
            Assert.NotEqual(a0.GetHashCode(), b.GetHashCode());
        }

        [Fact]
        public void Bad_Port()
        {
            var tcp = new MultiAddress("/tcp/65535");
            Assert.Throws<FormatException>(() => new MultiAddress("/tcp/x"));
            Assert.Throws<FormatException>(() => new MultiAddress("/tcp/65536"));

            var udp = new MultiAddress("/udp/65535");
            Assert.Throws<FormatException>(() => new MultiAddress("/upd/x"));
            Assert.Throws<FormatException>(() => new MultiAddress("/udp/65536"));
        }

        [Fact]
        public void Bad_IPAddress()
        {
            var ipv4 = new MultiAddress("/ip4/127.0.0.1");
            Assert.Throws<FormatException>(() => new MultiAddress("/ip4/x"));
            Assert.Throws<FormatException>(() => new MultiAddress("/ip4/127."));
            Assert.Throws<FormatException>(() => new MultiAddress("/ip4/::1"));

            var ipv6 = new MultiAddress("/ip6/::1");
            Assert.Throws<FormatException>(() => new MultiAddress("/ip6/x"));
            Assert.Throws<FormatException>(() => new MultiAddress("/ip6/03:"));
            Assert.Throws<FormatException>(() => new MultiAddress("/ip6/127.0.0.1"));
        }

        [Fact(Skip = "not account for actual exceptions thrown correctly")]
        public void Bad_Onion_MultiAdress()
        {
            var badCases = new[]
            {
                "/onion/9imaq4ygg2iegci7:80",
                "/onion/aaimaq4ygg2iegci7:80",
                "/onion/timaq4ygg2iegci7:0",
                "/onion/timaq4ygg2iegci7:-1",
                "/onion/timaq4ygg2iegci7",
                "/onion/timaq4ygg2iegci@:666"
            };
            foreach (var badCase in badCases) Assert.Throws<Exception>(() => new MultiAddress(badCase));
        }

        [Fact]
        public void RoundTripping()
        {
            var addresses = new[]
            {
                somewhere,
                "/ip4/1.2.3.4/tcp/80/http",
                "/ip6/3ffe:1900:4545:3:200:f8ff:fe21:67cf/tcp/443/https",
                "/ip6/3ffe:1900:4545:3:200:f8ff:fe21:67cf/udp/8001",
                "/ip6/3ffe:1900:4545:3:200:f8ff:fe21:67cf/sctp/8001",
                "/ip6/3ffe:1900:4545:3:200:f8ff:fe21:67cf/dccp/8001",
                "/ip4/1.2.3.4/tcp/80/ws",
                "/libp2p-webrtc-star/ip4/127.0.0.1/tcp/9090/ws/ipfs/QmcgpsyWgH8Y8ajJz1Cu72KnS5uo2Aa2LpzU7kinSupNKC",
                "/ip4/127.0.0.1/tcp/1234/ipfs/QmcgpsyWgH8Y8ajJz1Cu72KnS5uo2Aa2LpzU7kinSupNKC",
                "/ip4/1.2.3.4/tcp/80/udt",
                "/ip4/1.2.3.4/tcp/80/utp",
                "/onion/aaimaq4ygg2iegci:80",
                "/onion/timaq4ygg2iegci7:80/http",
                "/p2p-circuit/ipfs/QmcgpsyWgH8Y8ajJz1Cu72KnS5uo2Aa2LpzU7kinSupNKC",
                "/dns/ipfs.io",
                "/dns4/ipfs.io",
                "/dns6/ipfs.io",
                "/dns4/wss0.bootstrap.libp2p.io/tcp/443/wss/ipfs/QmZMxNdpMkewiVZLMRxaNxUeZpDUb34pWjZ1kZvsd16Zic",
                "/ip4/127.0.0.0/ipcidr/16",
                "/p2p/QmNnooDu7bfjPFoTZYxMNLWUQJyrVwtbZg5gBMjTezGAJN",
                "/ip4/127.0.0.1/udp/4023/quic"
            };
            foreach (var a in addresses)
            {
                var ma0 = new MultiAddress(a);

                var ms = new MemoryStream();
                ma0.Write(ms);
                ms.Position = 0;
                var ma1 = new MultiAddress(ms);
                Assert.Equal(ma0, ma1);

                var ma2 = new MultiAddress(ma0.ToString());
                Assert.Equal(ma0, ma2);

                var ma3 = new MultiAddress(ma0.ToArray());
                Assert.Equal(ma0, ma3);
            }
        }

        [Fact]
        public void Reading_Invalid_Code()
        {
            Assert.Throws<InvalidDataException>(() => new MultiAddress(new byte[] {0x7F}));
        }

        [Fact]
        public void Reading_Invalid_Text()
        {
            Assert.Throws<FormatException>(() => new MultiAddress("tcp/80"));
        }

        [Fact]
        public void Implicit_Conversion_From_String()
        {
            MultiAddress a = somewhere;
            Assert.IsType<MultiAddress>(a);
        }

        [Fact]
        public void Wire_Formats()
        {
            Assert.Equal(
                new MultiAddress("/ip4/127.0.0.1/udp/1234").ToArray().ToHexString(),
                "047f000001910204d2");
            Assert.Equal(
                new MultiAddress("/ip4/127.0.0.1/udp/1234/ip4/127.0.0.1/tcp/4321").ToArray().ToHexString(),
                "047f000001910204d2047f0000010610e1");
            Assert.Equal(
                new MultiAddress("/ip6/2001:8a0:7ac5:4201:3ac9:86ff:fe31:7095").ToArray().ToHexString(),
                "29200108a07ac542013ac986fffe317095");
            Assert.Equal(
                new MultiAddress("/ipfs/QmcgpsyWgH8Y8ajJz1Cu72KnS5uo2Aa2LpzU7kinSupNKC").ToArray().ToHexString(),
                "a503221220d52ebb89d85b02a284948203a62ff28389c57c9f42beec4ec20db76a68911c0b");
            Assert.Equal(
                new MultiAddress("/ip4/127.0.0.1/udp/1234/utp").ToArray().ToHexString(),
                "047f000001910204d2ae02");
            Assert.Equal(
                new MultiAddress("/onion/aaimaq4ygg2iegci:80").ToArray().ToHexString(),
                "bc030010c0439831b48218480050");
        }

        [Fact]
        public void PeerID_With_ipfs()
        {
            var ma = new MultiAddress("/ip4/10.1.10.10/tcp/29087/ipfs/QmVcSqVEsvm5RR9mBLjwpb2XjFVn5bPdPL69mL8PH45pPC");
            Assert.Equal("QmVcSqVEsvm5RR9mBLjwpb2XjFVn5bPdPL69mL8PH45pPC", ma.PeerId.ToBase58());
        }

        [Fact]
        public void PeerID_With_p2p()
        {
            var ma = new MultiAddress("/ip4/10.1.10.10/tcp/29087/p2p/QmVcSqVEsvm5RR9mBLjwpb2XjFVn5bPdPL69mL8PH45pPC");
            Assert.Equal("QmVcSqVEsvm5RR9mBLjwpb2XjFVn5bPdPL69mL8PH45pPC", ma.PeerId.ToBase58());
        }

        [Fact]
        public void PeerID_ipfs_p2p_are_equal()
        {
            var ipfs = new MultiAddress(
                "/ip4/10.1.10.10/tcp/29087/ipfs/QmVcSqVEsvm5RR9mBLjwpb2XjFVn5bPdPL69mL8PH45pPC");
            var p2p = new MultiAddress("/ip4/10.1.10.10/tcp/29087/p2p/QmVcSqVEsvm5RR9mBLjwpb2XjFVn5bPdPL69mL8PH45pPC");
            Assert.Equal(ipfs, p2p);

            var p2p1 = new MultiAddress("/ip4/10.1.10.10/tcp/29087/p2p/QmVCSqVEsvm5RR9mBLjwpb2XjFVn5bPdPL69mL8PH45pPC");
            Assert.NotEqual(p2p, p2p1);

            var p2p2 = new MultiAddress("/p2p/QmVcSqVEsvm5RR9mBLjwpb2XjFVn5bPdPL69mL8PH45pPC");
            Assert.NotEqual(p2p, p2p2);
        }

        [Fact]
        public void PeerID_Missing()
        {
            var ma = new MultiAddress("/ip4/10.1.10.10/tcp/29087");
            Assert.Throws<Exception>(() =>
            {
                var _ = ma.PeerId;
            });
        }

        [Fact]
        public void PeerId_IsPresent()
        {
            Assert.True(
                new MultiAddress("/ip4/10.1.10.10/tcp/29087/ipfs/QmVcSqVEsvm5RR9mBLjwpb2XjFVn5bPdPL69mL8PH45pPC")
                    .HasPeerId);
            Assert.True(
                new MultiAddress("/ip4/10.1.10.10/tcp/29087/p2p/QmVcSqVEsvm5RR9mBLjwpb2XjFVn5bPdPL69mL8PH45pPC")
                    .HasPeerId);
            Assert.False(new MultiAddress("/ip4/10.1.10.10/tcp/29087").HasPeerId);
        }

        [Fact]
        public void Cloning()
        {
            var ma1 = new MultiAddress("/ip4/10.1.10.10/tcp/29087");
            var ma2 = ma1.Clone();
            Assert.Equal(ma1, ma2);
            Assert.NotSame(ma1, ma2);
            Assert.NotSame(ma1.Protocols, ma2.Protocols);
            for (var i = 0; i < ma1.Protocols.Count; ++i)
            {
                var p1 = ma1.Protocols[i];
                var p2 = ma2.Protocols[i];
                Assert.Equal(p1.Code, p2.Code);
                Assert.Equal(p1.Name, p2.Name);
                Assert.Equal(p1.Value, p2.Value);
                Assert.NotSame(p1, p2);
            }
        }

        [Fact]
        public void Ipv6ScopeId_Ignored()
        {
            var ma1 = new MultiAddress("/ip6/fe80::7573:b0a8:46b0:0bad%17/tcp/4009");
            var ma2 = new MultiAddress("/ip6/fe80::7573:b0a8:46b0:0bad/tcp/4009");
            Assert.Equal(ma2, ma1);
            Assert.Equal(ma2.ToString(), ma1.ToString());
        }

        [Fact]
        public void TryCreate_FromString()
        {
            Assert.NotNull(MultiAddress.TryCreate("/ip4/1.2.3.4/tcp/80"));
            Assert.Null(MultiAddress.TryCreate("/tcp/alpha")); // bad port
            Assert.Null(MultiAddress.TryCreate("/foobar")); // bad protocol
        }

        [Fact]
        public void TryCreate_FromBytes()
        {
            var good = MultiAddress.TryCreate("/ip4/1.2.3.4/tcp/80");
            var good1 = MultiAddress.TryCreate(good.ToArray());
            Assert.Equal(good, good1);

            Assert.Null(MultiAddress.TryCreate(new byte[] {0x7f}));
        }

        [Fact]
        public void JsonSerialization()
        {
            var a = new MultiAddress("/ip6/fe80::7573:b0a8:46b0:0bad/tcp/4009");
            var json = JsonConvert.SerializeObject(a);
            Assert.Equal($"\"{a}\"", json);
            var b = JsonConvert.DeserializeObject<MultiAddress>(json);
            Assert.Equal(a.ToString(), b.ToString());

            a = null;
            json = JsonConvert.SerializeObject(a);
            b = JsonConvert.DeserializeObject<MultiAddress>(json);
            Assert.Null(b);
        }

        [Fact]
        public void WithPeerId()
        {
            var id = "QmQusTXc1Z9C1mzxsqC9ZTFXCgSkpBRGgW4Jk2QYHxKE22";
            var id3 = "QmQusTXc1Z9C1mzxsqC9ZTFXCgSkpBRGgW4Jk2QYHxKE33";

            var ma1 = new MultiAddress("/ip4/127.0.0.1/tcp/4001");
            Assert.Equal($"{ma1}/p2p/{id}", ma1.WithPeerId(id));

            ma1 = new MultiAddress($"/ip4/127.0.0.1/tcp/4001/ipfs/{id}");
            Assert.Same(ma1, ma1.WithPeerId(id));

            ma1 = new MultiAddress($"/ip4/127.0.0.1/tcp/4001/p2p/{id}");
            Assert.Same(ma1, ma1.WithPeerId(id));

            Assert.Throws<Exception>(() =>
            {
                ma1 = new MultiAddress($"/ip4/127.0.0.1/tcp/4001/ipfs/{id3}");
                Assert.Same(ma1, ma1.WithPeerId(id));
            });
        }

        [Fact]
        public void WithoutPeerId()
        {
            var id = "QmQusTXc1Z9C1mzxsqC9ZTFXCgSkpBRGgW4Jk2QYHxKE22";

            var ma1 = new MultiAddress("/ip4/127.0.0.1/tcp/4001");
            Assert.Same(ma1, ma1.WithoutPeerId());

            ma1 = new MultiAddress($"/ip4/127.0.0.1/tcp/4001/ipfs/{id}");
            Assert.Equal("/ip4/127.0.0.1/tcp/4001", ma1.WithoutPeerId());

            ma1 = new MultiAddress($"/ip4/127.0.0.1/tcp/4001/p2p/{id}");
            Assert.Equal("/ip4/127.0.0.1/tcp/4001", ma1.WithoutPeerId());
        }

        [Fact]
        public void Alias_Equality()
        {
            var a = new MultiAddress("/ipfs/QmQusTXc1Z9C1mzxsqC9ZTFXCgSkpBRGgW4Jk2QYHxKE22");
            var b = new MultiAddress("/p2p/QmQusTXc1Z9C1mzxsqC9ZTFXCgSkpBRGgW4Jk2QYHxKE22");

            Assert.Equal(a, b);
            Assert.True(a == b);
            Assert.Equal(a.GetHashCode(), b.GetHashCode());
        }

        [Fact]
        public void GetHashCode_NullValue()
        {
            var a = new MultiAddress(
                "/ip4/139.178.69.3/udp/4001/quic/p2p/QmdGQoGuK3pao6bRDqGSDvux5SFHa4kC2XNFfHFcvcbydY/p2p-circuit/ipfs/QmPJkpfUedzahgVAj6tTUa3DHKVkfTSyvUmnn1USFpiCaF");
            var _ = a.GetHashCode();
        }

        [Fact]
        public void FromIPAddress()
        {
            var ma = new MultiAddress(IPAddress.Loopback);
            Assert.Equal("/ip4/127.0.0.1", ma.ToString());

            ma = new MultiAddress(IPAddress.IPv6Loopback);
            Assert.Equal("/ip6/::1", ma.ToString());
        }

        [Fact]
        public void FromIPEndpoint()
        {
            var ma = new MultiAddress(new IPEndPoint(IPAddress.Loopback, 4001));
            Assert.Equal("/ip4/127.0.0.1/tcp/4001", ma.ToString());

            ma = new MultiAddress(new IPEndPoint(IPAddress.IPv6Loopback, 4002));
            Assert.Equal("/ip6/::1/tcp/4002", ma.ToString());
        }
    }
}