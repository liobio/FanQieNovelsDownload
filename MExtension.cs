using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FanQieNovelsDownload
{
    public static class MExtension
    {
        //静态方法
        public static string Text(this HttpResponseMessage r)  //this关键字
        {

            return r.Content.ReadAsStringAsync().Result;
        }
        public static IEnumerable<T> Add<T>(this IEnumerable<T> e, T value)
        {
            foreach (var cur in e)
            {
                yield return cur;
            }
            yield return value;
        }
    }
}
