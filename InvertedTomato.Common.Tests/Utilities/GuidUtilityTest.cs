using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InvertedTomato.Common.Tests.Utilities {
    [TestClass()]
    public class GuidUtilityTest {

        [TestMethod]
        public void CreateFromTest_Int() {
            int value = 1223434567;
            Assert.AreEqual("48ec2147-0000-0000-0000-000000000000", GuidUtility.CreateFrom(value).ToString());
        }

        [TestMethod]
        public void CreateFrom_Long() {
            long value = 1223434567;
            Assert.AreEqual("48ec2147-0000-0000-0000-000000000000", GuidUtility.CreateFrom(value).ToString());
        }

        [TestMethod]
        public void CreateFrom_String() {
            string value ="ABCD123" ; 
            Assert.AreEqual("0c49edf7-9580-86e2-8a88-77ef5bb8c441", GuidUtility.CreateFrom(value).ToString());
        }

        [TestMethod]
        public void FromShort() {
            string value = "ABCD1+-/`~";
            var sdd = GuidUtility.CreateFrom(value).ToString();
            Assert.AreEqual("622b6107-c746-3d8c-c8bd-732da025090b", GuidUtility.CreateFrom(value).ToString());
        }

    }
}
