using System.Collections.Generic;
using System.Linq;
using SubtitleQc.Core.Models;
using SubtitleQc.Core.Qc.Abstractions;

namespace SubtitleQc.Core.Qc.Rules
{
    public class OverlapCheckRule : IQcRule
    {
        public IEnumerable<QcResult> Evaluate(IEnumerable<Cue> cues)
        {
            var results = new List<QcResult>();
            var cueList = cues.ToList();
            
            for (int i = 0; i < cueList.Count; i++)
            {
                var current = cueList[i];
                bool isOverlapping = false;
                if (i > 0)
                {
                    var prev = cueList[i - 1];
                    if (current.Start < prev.End)
                    {
                        isOverlapping = true;
                    }
                }
                var status = isOverlapping ? QcStatus.Failed : QcStatus.Passed;
                results.Add(new QcResult { CueId = current.Id, Status = status });
            }
            return results;
        }
    }
}