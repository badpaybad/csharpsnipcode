using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

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
