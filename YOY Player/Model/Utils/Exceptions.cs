using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YOYPlayer.Model.Utils.Exceptions
{
    public class WrongAuthDataException : Exception { }
    public class AuthTokenExpiredException : Exception { }
}
