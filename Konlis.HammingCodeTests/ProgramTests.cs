using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests
{
    [TestClass()]
    public class ProgramTests
    {
        [TestMethod]    
        public void TestCorrect()
        {
            HammingCodeCalculator.Main();
        }

        [TestMethod()]
        public void solveParityBitCountTest()
        {
            Assert.AreEqual(2, HammingCodeCalculator.SolveParityBitCount(1));
            Assert.AreEqual(3, HammingCodeCalculator.SolveParityBitCount(2));
            Assert.AreEqual(3, HammingCodeCalculator.SolveParityBitCount(3));
            Assert.AreEqual(3, HammingCodeCalculator.SolveParityBitCount(4));
            Assert.AreEqual(4, HammingCodeCalculator.SolveParityBitCount(5));
            Assert.AreEqual(4, HammingCodeCalculator.SolveParityBitCount(11));
            Assert.AreEqual(5, HammingCodeCalculator.SolveParityBitCount(12));
            Assert.AreEqual(5, HammingCodeCalculator.SolveParityBitCount(26));
            Assert.AreEqual(6, HammingCodeCalculator.SolveParityBitCount(27));
            Assert.AreEqual(9, HammingCodeCalculator.SolveParityBitCount(502));
            Assert.AreEqual(10, HammingCodeCalculator.SolveParityBitCount(503));
        }
    }
}