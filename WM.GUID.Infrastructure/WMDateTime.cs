using System;
using WM.Common;

namespace WM.GUID.Infrastructure
{
    public class WMDateTime : IDateTime
    {
        public DateTime Now => DateTime.Now;
    }
}