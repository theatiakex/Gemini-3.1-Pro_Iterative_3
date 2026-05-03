using System.Collections.Generic;
using SubtitleQc.Core.Models;
using SubtitleQc.Core.Qc.Abstractions;

namespace SubtitleQc.Core.Qc.Rules
{
    public class MaxCplRule : IQcRule
    {
        private readonly int _threshold;

        public MaxCplRule(int threshold)
        {
            _threshold = threshold;
        }

        public IEnumerable<QcResult> Evaluate(IEnumerable<Cue> cues)
        {
            var results = new List<QcResult>();
            foreach (var cue in cues)
            {
                bool failed = false;
                foreach (var line in cue.Lines)
                {
                    if (line.Length > _threshold) failed = true;
                }
                var status = failed ? QcStatus.Failed : QcStatus.Passed;
                results.Add(new QcResult { CueId = cue.Id, Status = status });
            }
            return results;
        }
    }
}