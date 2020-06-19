using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YOYPlayer.Model.Utils.Enums
{
    public enum AfterLoginAction
    {
        Start,
        Exit,
    }
    public enum AfterFileChangeAction
    {
        Back,
        Open,
    }
    public enum LogState
    {
        UnSync,
        InSync,
        Synced,
    }
}
