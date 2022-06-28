using NUnit.Framework;
using PFormatTests.Properties;
using System;
using System.Collections.Generic;

namespace PFormat.Tests
{
    [TestFixture()]
    public class ReplaceEngineTests
    {
        #region Private Fields

        static readonly string[] splitter = new string[] { Environment.NewLine };

        #endregion

        #region Test Cases

        [TestCase("日時", "2022/1/1", "${日時}", "2022/1/1")]
        [TestCase("日時", "2022/1/1", "${{DateTime}日時}", "2022/01/01 0:00:00")]
        [TestCase("日時", "2022/1/1", "${{DateTime}{yyyyMMdd}日時}", "20220101")]
        public void ReplaceTest1(string fieldName, string fieldValue, string format, string expected)
        {
            var fields = new Dictionary<string, string>();
            fields[fieldName] = fieldValue;
            Assert.AreEqual(expected, ReplaceEngine.Replace(format, fields));
        }

        [TestCase]
        public void ReplaceTest2()
        {
            ReplaceTest2(Resources.Input1, Resources.Fields1, 3, Resources.Expected1);
        }

        [TestCase]
        public void ReplaceTest3()
        {
            ReplaceTest2(Resources.Input2, Resources.Fields2, 4, Resources.Expected2);
        }

        #endregion

        #region Private Methods

        private void ReplaceTest2(string input, string fields, int fieldsCount, string expected)
        {
            var fields_ = new Dictionary<string, string>();

            foreach (string field in fields.Split(splitter, StringSplitOptions.None))
            {
                string[] columns = field.Split('\t');
                Assert.AreEqual(2, columns.Length);
                fields_[columns[0]] = columns[1];
            }

            Assert.AreEqual(fieldsCount, fields_.Count);
            Assert.AreEqual(expected, ReplaceEngine.Replace(input, fields_));
        }

        #endregion
    }
}