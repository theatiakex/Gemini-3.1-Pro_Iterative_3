using System;
using System.Collections.Generic;
using System.Globalization;
using SubtitleQc.Core.Models;

namespace SubtitleQc.Core.Parsers
{
    public class SrtParser : ISubtitleParser
    {
        public IEnumerable<Cue> Parse(string content)
        {
            var cues = new List<Cue>();
            var blocks = content.Split(new[] { "\r\n\r\n", "\n\n" }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var block in blocks)
            {
                var cue = ParseBlock(block);
                if (cue != null) cues.Add(cue);
            }
            return cues;
        }

        private Cue ParseBlock(string block)
        {
            var lines = block.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);
            if (lines.Length < 3) return null;
            
            var timecodes = lines[1].Split(new[] { " --> " }, StringSplitOptions.None);
            if (timecodes.Length != 2) return null;

            return CreateCue(timecodes, lines);
        }

        private Cue CreateCue(string[] timecodes, string[] lines)
        {
            var startTime = ParseTimecode(timecodes[0]);
            var endTime = ParseTimecode(timecodes[1]);
            
            var cueLines = new List<string>();
            for (int i = 2; i < lines.Length; i++)
            {
                cueLines.Add(lines[i]);
            }
            return new Cue(Guid.NewGuid().ToString("N"), startTime, endTime, cueLines);
        }

        private TimeSpan ParseTimecode(string timecode)
        {
            return TimeSpan.ParseExact(timecode.Trim(), @"hh\:mm\:ss\,fff", CultureInfo.InvariantCulture);
        }
    }
}