
    public class ElementArraySumUp
    {
        int[][] _main = null;

        public ElementArraySumUp(int[][] main)
        {
            _main = main;
        }

        public int[] DoSumElement()
        {
            if (_main == null | _main.Length == 0) throw new NoNullAllowedException("no element to sum");
            foreach (var iarr in _main)
            {
                if (iarr == null || iarr.Length == 0) throw new NoNullAllowedException("array with no element");
            }
            if (_main.Length == 1) return _main[0];// early return no need do , only one array no sum

            var temp = _main.ToList();

            do
            {
                var newArr = SumElement2Array(temp[0], temp[1]).ToArray();
                temp = temp.Skip(2).ToList();

                temp.Add(newArr);

            } while ((temp.Count > 1));

            return temp[0];
        }

        List<int> SumElement2Array(int[] arr1, int[] arr2)
        {
            List<int> res = new List<int>();
            foreach (var a1 in arr1)
            {
                foreach (var a2 in arr2)
                {
                    res.Add(a1 + a2);
                }
            }
            return res;
        }

    }
    
[TestClass]
    public class Test
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
