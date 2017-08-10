using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeTraveller.Common
{
    public static class Extend
    {
        /// <summary>
        /// 将整数四舍五入
        /// </summary>
        /// <param name="i">整数</param>
        /// <param name="position">进位的位数，如10，100</param>
        /// <returns>四舍五入后的结果</returns>
        public static int Round(this int i, int position)
        {
            StringBuilder sb = new StringBuilder("1");
            var positionLength = position.ToString().Length;
            if (position.ToString().FirstOrDefault() != '1')
            { throw new ArgumentException("position应为10的倍数"); }
            var radix = sb.ToString().PadRight(positionLength - 1, '0');
            int r = int.Parse(radix);
            if (position % r != 0)
            {
                throw new ArgumentException("position应为10的倍数");
            }
            if (i % position >= 5)
            {
                i = (i / position + 1) * position;
            }
            else
            {
                i = (i / position) * position;
            }
            return i;
        }
    }
}
