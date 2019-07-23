using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using danielCherrin_MajorAppIntClocks;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Threading;
using System.Collections.Generic;

namespace ClassUnitTests
{
    [TestClass]
    public class TimezoneTests
    {
        [TestMethod]
        public async void TestClockGet()
        {
            try
            {
                StoredTimeClock tempClock = await StoredTimeClock.GetTimezoneClock("WET");
                Assert.IsTrue(true);
            }
            catch
            {
                Assert.IsTrue(false);
            }
        }

        [TestMethod]
        public async void TestClockSetDatetime()
        {
            try
            {
                StoredTimeClock tempClock = await StoredTimeClock.GetTimezoneClock("WET");
                tempClock.SetCurrentDateTimeClock();
                Assert.IsTrue(true);
            }
            catch
            {
                Assert.IsTrue(false);
            }
        }

        [TestMethod]
        public async void TestTimezones()
        {
            try
            {
                List<string> timezones = await IntTimezones.GetTimezones();

                if(timezones.Count>0)
                {
                    Assert.IsTrue(true);
                }
                else
                {
                    Assert.IsTrue(false);
                }
            }
            catch
            {
                Assert.IsTrue(false);
            }
        }
    }
}
