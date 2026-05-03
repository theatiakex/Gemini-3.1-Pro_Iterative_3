using System.Collections.Generic;

namespace SubtitleQc.Core.Qc
{
    public class QcReport
    {
        public IEnumerable<QcResult> Results { get; set; } = new List<QcResult>();
    }
}