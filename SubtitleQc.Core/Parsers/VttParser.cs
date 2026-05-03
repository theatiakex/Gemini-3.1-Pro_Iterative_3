using System;
using System.Collections.Generic;
using System.Globalization;
using SubtitleQc.Core.Models;

namespace SubtitleQc.Core.Parsers
{
    public class VttParser : ISubtitleParser
    {
        public IEnumerable<Cue> Parse(string content)
        {
            var cues = new List<Cue>();
            var blocks = content.Split(new[] { "\r\n\r\n", "\n\n" }, StringSplitOptions.RemoveEmptyEntries);
            
            foreach (var block in blocks)
            {
                if (block.Trim().StartsWith("WEBVTT")) continue;
                var cue = ParseBlock(block);
                if (cue != null) cues.Add(cue);
            }
            return cues;
        }

        private Cue ParseBlock(string block)
        {
            var lines = block.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);
            int timecodeIndex = FindTimecodeIndex(lines);
            
            if (timecodeIndex == -1) return null;
            
            var timecodes = lines[timecodeIndex].Split(new[] { " --> " }, StringSplitOptions.None);
            if (timecodes.Length != 2) return null;

            return CreateCue(timecodes, lines, timecodeIndex + 1);
        }

        private int FindTimecodeIndex(string[] lines)
        {
            for (int i = 0; i < lines.Length; i++)
            {
                if (lines[i].Contains(" --> ")) return i;
            }
            return -1;
        }

        private Cue CreateCue(string[] timecodes, string[] lines, int textStartIndex)
        {
            var startTime = ParseTimecode(timecodes[0]);
            var endTime = ParseTimecode(timecodes[1]);
            
            var cueLines = new List<string>();
            for (int i = textStartIndex; i < lines.Length; i++)
            {
                cueLines.Add(lines[i]);
            }
            return new Cue(Guid.NewGuid().ToString("N"), startTime, endTime, cueLines);
        }

        private TimeSpan ParseTimecode(string timecode)
        {
            timecode = timecode.Trim();
            if (timecode.Split(':').Length == 2)
            {
                return TimeSpan.ParseExact(timecode, @"mm\:ss\.fff", CultureInfo.InvariantCulture);
            }
            return TimeSpan.ParseExact(timecode, @"hh\:mm\:ss\.fff", CultureInfo.InvariantCulture);
        }
    }
}