using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace NewsHub.Domain.Enums
{
    public enum SourceType
    {
        [Description("API")]
        Api = 0,
        [Description("RSS")]
        Rss = 1,
        [Description("HTML (Coming Soon)")]
        Html = 2
    }
}
