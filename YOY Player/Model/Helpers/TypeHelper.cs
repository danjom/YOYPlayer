using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YOYPlayer.Model.Helpers
{
    public static class TypeHelper
    {
        public static DateTime DateFromExpired(string expiredIn)
        {
            return DateTime.Now + TimeSpan.FromSeconds(UInt64.Parse(expiredIn));
        }
    }
}
