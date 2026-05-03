using System.Collections.Generic;
using SubtitleQc.Core.Models;

namespace SubtitleQc.Core.Parsers
{
    public interface ISubtitleParser
    {
        IEnumerable<Cue> Parse(string content);
    }
}