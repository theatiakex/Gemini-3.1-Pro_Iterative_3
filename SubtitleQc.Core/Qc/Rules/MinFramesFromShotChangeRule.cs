using System;
using System.Collections.Generic;
using System.Linq;
using SubtitleQc.Core.Models;
using SubtitleQc.Core.Qc.Abstractions;

namespace SubtitleQc.Core.Qc.Rules
{
    public class MinFramesFromShotChangeRule : IQcRule
    {
        private readonly IShotChangeProvider _shotChangeProvider;
        private readonly int _thresholdFrames;

        public MinFramesFromShotChangeRule(IShotChangeProvider shotChangeProvider, int thresholdFrames)
        {
            _shotChangeProvider = shotChangeProvider;
            _thresholdFrames = thresholdFrames;
        }

        public IEnumerable<QcResult> Evaluate(IEnumerable<Cue> cues)
        {
            var results = new List<QcResult>();
            var cutFrames = _shotChangeProvider.GetShotChangeFrames();

            foreach (var cue in cues)
            {
                if (!cue.StartFrame.HasValue || cutFrames.Count == 0)
                {
                    results.Add(new QcResult { CueId = cue.Id, Status = QcStatus.Passed });
                    continue;
                }

                // Find the closest cut frame BEFORE or AT the cue start frame (or any closest cut)
                // The test says "a cut occurs at frame 1000 And an internal cue starts at frame 1001" -> fails if threshold is 2
                // Distance = |cue.StartFrame.Value - cutFrame|
                // But generally subtitle shouldn't start too close to ANY cut.
                int minDistance = cutFrames.Min(cut => Math.Abs(cue.StartFrame.Value - cut));

                var status = minDistance < _thresholdFrames ? QcStatus.Failed : QcStatus.Passed;
                results.Add(new QcResult { CueId = cue.Id, Status = status });
            }

            return results;
        }
    }
}