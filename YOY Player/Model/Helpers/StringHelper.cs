using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace YOYPlayer.Model.Helpers
{
    public static class StringHelper
    {
        public static bool IsEmail(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return false;

            return new EmailAddressAttribute().IsValid(value);
        }
    }
}
