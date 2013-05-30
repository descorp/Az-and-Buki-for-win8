using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using levelupspace;

namespace UnitTestLibrary
{
    [TestClass]
    public class ContentManagerClassTest
    {
        [TestMethod]
        public void ParseAlphabetIDWrongParams()
        {
            int expected = -1;
            var actual = ContentManager.ParseAlphabetID("2");
            Assert.AreEqual(expected, actual, 0);
        }

        [TestMethod]
        public void ParseAlphabetIDCorrectParam()
        {
            int expected = 2;
            var actual = ContentManager.ParseAlphabetID("Alphabet 2");
            Assert.AreEqual(expected, actual, 0);
        }

        [TestMethod]
        public void ParseLetterIDWrongParams()
        {
            int expected = -1;
            var actual = ContentManager.ParseLetterID("Alphabet 3");
            Assert.AreEqual(expected, actual, 0);
        }

        [TestMethod]
        public void ParseLetterIDCorrectParam()
        {
            int expected = 3;
            var actual = ContentManager.ParseLetterID("Alphabet 2 Letter 3");
            Assert.AreEqual(expected, actual, 0);
        }

        [TestMethod]
        public void ParseWordIDWrongParams()
        {
            int expected = -1;
            var actual = ContentManager.ParseWordID("2");
            Assert.AreEqual(expected, actual, 0);
        }

        [TestMethod]
        public void ParseWordIDCorrectParam()
        {
            int expected = 1;
            var actual = ContentManager.ParseWordID("Alphabet 2 Letter 3 Word 1");
            Assert.AreEqual(expected, actual, 0);
        }

    }
}
