using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NR.Infrastructure;

namespace UnitTest
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {

            string start = "Helloh ##URLLOKAL##  ##URLLOKAL (option)## ##URLLOKAL (option) ## ##URLLOKAL##";

            start.ReplaceTagContent();

        }
    }
}
