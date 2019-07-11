
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

[TestClass]
    public class TestElementArraySumUp
    {
        [TestMethod]
        public void Do1Arr()
        {
            var abc = new ElementArraySumUp(new int[][] { new[] { 1, 2, 3 } });
            Console.WriteLine(string.Join(",", abc.DoSumElement()));
        }
        [TestMethod]
        public void Do2Arr()
        {
            var abc = new ElementArraySumUp(new int[][] { new[] { 1, 2, 3 }, new[] { 5, 0 } });
            Console.WriteLine(string.Join(",", abc.DoSumElement()));
        }

        [TestMethod]
        public void Do5Arr()
        {
            var abc = new ElementArraySumUp(new int[][]
            {
                new[] {5,4,3,2,1 }
                , new[] { 4,1 }
                , new[] { 5,0,0 }
                , new[] { 6,4,2 }
                , new[] { 1 }
            });
            Console.WriteLine(string.Join(",", abc.DoSumElement()));
        }
    }
