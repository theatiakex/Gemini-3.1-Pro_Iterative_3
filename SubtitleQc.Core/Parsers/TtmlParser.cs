using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml.Linq;
using SubtitleQc.Core.Models;

namespace SubtitleQc.Core.Parsers
{
    public class TtmlParser : ISubtitleParser
    {
        public IEnumerable<Cue> Parse(string content)
        {
            var doc = XDocument.Parse(content);
            var cues = new List<Cue>();

            foreach (var p in doc.Descendants().Where(e => e.Name.LocalName == "p"))
            {
                var cue = ProcessParagraph(p);
                if (cue != null) cues.Add(cue);
            }

            return cues;
        }

        private Cue ProcessParagraph(XElement p)
        {
            var beginAttr = p.Attribute("begin")?.Value;
            var endAttr = p.Attribute("end")?.Value;
            if (beginAttr == null || endAttr == null) return null;

            var startTime = ParseTimecode(beginAttr);
            var endTime = ParseTimecode(endAttr);
            var lines = ExtractLines(p);

            return new Cue(Guid.NewGuid().ToString("N"), startTime, endTime, lines);
        }

        private IReadOnlyList<string> ExtractLines(XElement p)
        {
            var lines = new List<string>();
            var currentLine = string.Empty;
            foreach (var node in p.Nodes())
            {
                ProcessNode(node, lines, ref currentLine);
            }
            if (!string.IsNullOrEmpty(currentLine) || lines.Count == 0) lines.Add(currentLine);
            return lines.Select(l => l.Trim().Replace("\n", "").Replace("\r", "")).ToList();
        }

        private void ProcessNode(XNode node, List<string> lines, ref string currentLine)
        {
            if (node is XText textNode)
            {
                currentLine += textNode.Value;
            }
            else if (node is XElement element && element.Name.LocalName == "br")
            {
                lines.Add(currentLine);
                currentLine = string.Empty;
            }
        }

        private TimeSpan ParseTimecode(string timecode)
        {
            timecode = timecode.Trim();
            if (TimeSpan.TryParse(timecode, out var result)) return result;
            
            var formats = new[] { @"hh\:mm\:ss\.fff", @"hh\:mm\:ss\:fff" };
            if (TimeSpan.TryParseExact(timecode, formats, CultureInfo.InvariantCulture, out var exact))
            {
                return exact;
            }
            throw new FormatException($"Invalid timecode: {timecode}");
        }
    }
}