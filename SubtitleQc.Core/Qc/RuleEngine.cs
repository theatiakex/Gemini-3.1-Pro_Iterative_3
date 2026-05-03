using System.Collections.Generic;
using SubtitleQc.Core.Models;
using SubtitleQc.Core.Qc.Abstractions;

namespace SubtitleQc.Core.Qc
{
    public class RuleEngine
    {
        private readonly IEnumerable<IQcRule> _rules;

        public RuleEngine(IEnumerable<IQcRule> rules)
        {
            _rules = rules;
        }

        public QcReport Evaluate(IEnumerable<Cue> cues)
        {
            var results = new List<QcResult>();
            foreach (var rule in _rules)
            {
                results.AddRange(rule.Evaluate(cues));
            }
            return new QcReport { Results = results };
        }
    }
}