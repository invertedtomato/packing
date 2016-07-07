using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InvertedTomato.Common.Tests.Utilities {
    [TestClass]
  public  class EmailAddressUtilityTest {

        [TestMethod]
        public void IsValidEmailAddressTest_Valid() {
            var email = "vlife@gmail.com"; 
            Assert.IsTrue(EmailAddressUtility.IsValidEmailAddress(email));
        }

        [TestMethod] 
        public void IsValidEmailAddressTest_Invalid() {
            var email = "vlifegmail.com";
            Assert.IsFalse(EmailAddressUtility.IsValidEmailAddress(email));

        }
    }
}
