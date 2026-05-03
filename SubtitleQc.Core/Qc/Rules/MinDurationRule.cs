using System;
using System.Collections.Generic;
using SubtitleQc.Core.Models;
using SubtitleQc.Core.Qc.Abstractions;

namespace SubtitleQc.Core.Qc.Rules
{
    public class MinDurationRule : IQcRule
    {
        private readonly TimeSpan _threshold;

        public MinDurationRule(TimeSpan threshold)
        {
            _threshold = threshold;
        }

        public IEnumerable<QcResult> Evaluate(IEnumerable<Cue> cues)
        {
            var results = new List<QcResult>();
            foreach (var cue in cues)
            {
                var status = cue.Duration < _threshold ? QcStatus.Failed : QcStatus.Passed;
                results.Add(new QcResult { CueId = cue.Id, Status = status });
            }
            return results;
        }
    }
}