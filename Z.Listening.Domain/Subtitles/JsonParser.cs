using System.Text.Json;
using Z.Listening.Domain.ValueObjects;

namespace Z.Listening.Domain.Subtitles
{
    class JsonParser : ISubtitleParser
    {
        public bool Accept(string typeName)
        {
            return typeName.Equals("json", StringComparison.OrdinalIgnoreCase);
        }

        public IEnumerable<Sentence> Parse(string subtitle)
        {
            return JsonSerializer.Deserialize<IEnumerable<Sentence>>(subtitle);
        }
    }
}
