using System.Collections.Generic;
using SubtitleQc.Core.Models;
using SubtitleQc.Core.Qc.Abstractions;

namespace SubtitleQc.Core.Qc.Rules
{
    public class EmptyCueCheckRule : IQcRule
    {
        public IEnumerable<QcResult> Evaluate(IEnumerable<Cue> cues)
        {
            var results = new List<QcResult>();
            foreach (var cue in cues)
            {
                var text = string.Join("", cue.Lines).Trim();
                var status = string.IsNullOrEmpty(text) ? QcStatus.Failed : QcStatus.Passed;
                results.Add(new QcResult { CueId = cue.Id, Status = status });
            }
            return results;
        }
    }
}