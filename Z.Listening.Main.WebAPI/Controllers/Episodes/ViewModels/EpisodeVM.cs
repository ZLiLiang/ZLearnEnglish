using Z.Listening.Domain.ValueObjects;

namespace Z.Listening.Main.WebAPI.Controllers.Episodes.ViewModels
{
    public record EpisodeVM(Guid Id, MultilingualString Name, Guid AlbumId, Uri AudioUrl, double DurationInSecond, IEnumerable<SentenceVM>? Sentences)
    {
        public static EpisodeVM? Create(Episode? episode, bool loadSubtitle)
        {
            if (episode == null)
            {
                return null;
            }
            List<SentenceVM> sentenceVMs = new();
            if (loadSubtitle)
            {
                var sentences = episode.ParseSubtitle();
                foreach (Sentence sentence in sentences)
                {
                    SentenceVM vm = new SentenceVM(sentence.StartTime.TotalSeconds, sentence.EndTime.TotalSeconds, sentence.Value);
                    sentenceVMs.Add(vm);
                }
            }
            return new EpisodeVM(episode.Id, episode.Name, episode.AlbumId, episode.AudioUrl, episode.DurationInSecond, sentenceVMs);
        }

        public static EpisodeVM[] Create(Episode[] items, bool loadSubtitle)
        {
            return items.Select(e => Create(e, loadSubtitle)!).ToArray();
        }
    }
}
