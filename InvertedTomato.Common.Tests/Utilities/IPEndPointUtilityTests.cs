using NUnit.Framework;

namespace InvertedTomato.Tests {
    [TestFixture]
    public class IPEndPointUtilityTests {
        [Test]
        public void ParseTest_1() {
            var ipEndPoint = IPEndPointUtility.Parse("0.0.0.0:100");
            Assert.AreEqual(100, ipEndPoint.Port);
            Assert.AreEqual("0.0.0.0", ipEndPoint.Address.ToString());
        }
        [Test]
        public void ParseTest_2() {
            var ipEndPoint = IPEndPointUtility.Parse("0.0.0.0");
            Assert.AreEqual(0, ipEndPoint.Port);
            Assert.AreEqual("0.0.0.0", ipEndPoint.Address.ToString());
        }
        [Test]
        public void ParseTest_3() {
            var ipEndPoint = IPEndPointUtility.Parse("[::1]:100");
            Assert.AreEqual(100, ipEndPoint.Port);
            Assert.AreEqual("::1", ipEndPoint.Address.ToString());
        }
        [Test]
        public void ParseTest_4() {
            var ipEndPoint = IPEndPointUtility.Parse("[::1]");
            Assert.AreEqual(0, ipEndPoint.Port);
            Assert.AreEqual("::1", ipEndPoint.Address.ToString());
        }
        [Test]
        public void ParseTest_5() {
            var ipEndPoint = IPEndPointUtility.Parse("::1");
            Assert.AreEqual(0, ipEndPoint.Port);
            Assert.AreEqual("::1", ipEndPoint.Address.ToString());
        }
        // Not supported
        //[Test]
        //public void ParseTest_6() {
        //    var ipEndPoint = IPEndPointUtility.Parse("[a:b:c:d]");
        //    Assert.AreEqual(0, ipEndPoint.Port);
        //    Assert.AreEqual("a:b:c:d", ipEndPoint.Address.ToString());
        //}
        //[Test]
        //public void ParseTest_7() {
        //    var ipEndPoint = IPEndPointUtility.Parse("[a:b:c:d]:241");
        //    Assert.AreEqual(241, ipEndPoint.Port);
        //    Assert.AreEqual("a:b:c:d", ipEndPoint.Address.ToString());
        //}

        [Test]
        public void ParseTest_8() {
            var ipEndPoint = IPEndPointUtility.Parse("http://example.org");
            Assert.AreEqual(80, ipEndPoint.Port);
            Assert.AreEqual("93.184.216.34", ipEndPoint.Address.ToString());
        }
        [Test]
        public void ParseTest_9() {
            var ipEndPoint = IPEndPointUtility.Parse("http://example.org:1");
            Assert.AreEqual(1, ipEndPoint.Port);
            Assert.AreEqual("93.184.216.34", ipEndPoint.Address.ToString());

        }
        [Test]
        public void ParseTest_10() {
            var ipEndPoint = IPEndPointUtility.Parse("example.org");
            Assert.AreEqual(0, ipEndPoint.Port);
            Assert.AreEqual("93.184.216.34", ipEndPoint.Address.ToString());
        }
        [Test]
        public void ParseTest_11() {
            var ipEndPoint = IPEndPointUtility.Parse("example.org:1");
            Assert.AreEqual(1, ipEndPoint.Port);
            Assert.AreEqual("93.184.216.34", ipEndPoint.Address.ToString());
        }
        [Test]
        public void ParseTest_12() {
            var ipEndPoint = IPEndPointUtility.Parse("[2001:db8:85a3:8d3:1319:8a2e:370:7348]");
            Assert.AreEqual(0, ipEndPoint.Port);
            Assert.AreEqual("2001:db8:85a3:8d3:1319:8a2e:370:7348", ipEndPoint.Address.ToString());
        }
        [Test]
        public void ParseTest_13() {
            var ipEndPoint = IPEndPointUtility.Parse("[2001:db8:85a3:8d3:1319:8a2e:370:7348]:100");
            Assert.AreEqual(100, ipEndPoint.Port);
            Assert.AreEqual("2001:db8:85a3:8d3:1319:8a2e:370:7348", ipEndPoint.Address.ToString());
        }

        [Test]
        public void ParseTest_14() {
            var ipEndPoint = IPEndPointUtility.Parse("http://0.0.0.0");
            Assert.AreEqual(80, ipEndPoint.Port);
            Assert.AreEqual("0.0.0.0", ipEndPoint.Address.ToString());
        }
        [Test]
        public void ParseTest_15() {
            var ipEndPoint = IPEndPointUtility.Parse("http://0.0.0.0:100");
            Assert.AreEqual(100, ipEndPoint.Port);
            Assert.AreEqual("0.0.0.0", ipEndPoint.Address.ToString());
        }

        [Test]
        public void ParseTest_16() {
            var ipEndPoint = IPEndPointUtility.Parse("http://[::1]");
            Assert.AreEqual(80, ipEndPoint.Port);
            Assert.AreEqual("::1", ipEndPoint.Address.ToString());
        }
        [Test]
        public void ParseTest_17() {
            var ipEndPoint = IPEndPointUtility.Parse("http://[::1]:100");
            Assert.AreEqual(100, ipEndPoint.Port);
            Assert.AreEqual("::1", ipEndPoint.Address.ToString());
        }
    }
}