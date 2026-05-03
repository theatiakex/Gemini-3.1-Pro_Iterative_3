using System.Collections.Generic;
using System.Linq;
using SubtitleQc.Core.Models;
using SubtitleQc.Core.Qc.Abstractions;

namespace SubtitleQc.Core.Qc.Rules
{
    public class MaxCpsRule : IQcRule
    {
        private readonly int _threshold;

        public MaxCpsRule(int threshold)
        {
            _threshold = threshold;
        }

        public IEnumerable<QcResult> Evaluate(IEnumerable<Cue> cues)
        {
            var results = new List<QcResult>();
            foreach (var cue in cues)
            {
                if (cue.Duration.TotalSeconds <= 0)
                {
                    results.Add(new QcResult { CueId = cue.Id, Status = QcStatus.Failed });
                    continue;
                }
                var totalCharacters = cue.Lines.Sum(l => l.Length);
                var cps = totalCharacters / cue.Duration.TotalSeconds;
                var status = cps > _threshold ? QcStatus.Failed : QcStatus.Passed;
                results.Add(new QcResult { CueId = cue.Id, Status = status });
            }
            return results;
        }
    }
}