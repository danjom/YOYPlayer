using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using YOYPlayer.Resources.Strings;

namespace YOYPlayer.Model.Helpers
{
    public static class ResourcesHelper
    {
        public static string GetString(string name)
        {
            return new ResourceManager(typeof(Strings)).GetString(name);
        }
    }
}
