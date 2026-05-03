using System;
using System.Collections.Generic;
using System.Linq;
using SubtitleQc.Core.Models;
using SubtitleQc.Core.Qc.Abstractions;

namespace SubtitleQc.Core.Qc.Rules
{
    public class CrossShotBoundaryCheckRule : IQcRule
    {
        private readonly IShotChangeProvider _shotChangeProvider;

        public CrossShotBoundaryCheckRule(IShotChangeProvider shotChangeProvider)
        {
            _shotChangeProvider = shotChangeProvider;
        }

        public IEnumerable<QcResult> Evaluate(IEnumerable<Cue> cues)
        {
            var results = new List<QcResult>();
            var cuts = _shotChangeProvider.GetShotChangeTimestamps();

            foreach (var cue in cues)
            {
                // A cue fails if there is any cut strictly between cue.Start and cue.End.
                bool spansCut = cuts.Any(cut => cut > cue.Start && cut < cue.End);
                var status = spansCut ? QcStatus.Failed : QcStatus.Passed;
                results.Add(new QcResult { CueId = cue.Id, Status = status });
            }
            return results;
        }
    }
}