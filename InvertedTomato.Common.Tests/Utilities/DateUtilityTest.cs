using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InvertedTomato.Common.Tests.Utilities {
    [TestClass]
    public class DateUtilityTest {

        [TestMethod]
        public void FromUnixTimestampTest() {// 10 years back date
            double unixTimestamp = (double)(DateTime.UtcNow.AddYears(-10).Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
            Assert.AreEqual(DateTime.UtcNow.AddYears(-10).Date, DateUtility.FromUnixTimestamp(unixTimestamp).Date);

        }
    }
}
