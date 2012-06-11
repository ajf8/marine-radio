using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace MarineRadioRTP.NUnitFixtures
{
    [TestFixture]
    public class ConfigurationSingletonTests
    {
        private ConfSingleton conf;

        [SetUp]
        public void SetUpFixture()
        {
            this.conf = ConfSingleton.Instance;
        }

        [Test]
        [ExpectedException(typeof(FormatException))]
        public void InvalidIp()
        {
            conf.MulticastNetworkStart = "224.1.1.259";
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void Ipv6Ip()
        {
            conf.MulticastNetworkStart = "2001:6b0:1:ea:202:a5ff:fecd:13a6";
        }

        [Test]
        public void SaveMulticastPrefix()
        {
            try
            {
                this.conf.MulticastPrefix = 25;
                this.conf.Save();
                this.conf.ReloadConfMap();
            }
            catch { }
            Assert.AreEqual(25, this.conf.MulticastPrefix);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void MmsiTooLong()
        {
            this.conf.MMSI = 12345678901;
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void MmsiTooShort()
        {
            this.conf.MMSI = 1234;
        }

        [Test]
        public void SaveCompressionChoice()
        {
            try
            {
                this.conf.Compression = true;
                this.conf.Save();
                this.conf.ReloadConfMap();
            }
            catch { }
            Assert.AreEqual(true, this.conf.Compression);
        }
    }
}
