using System.Collections.Generic;
using System.Linq;

namespace Shpora.WordSearcher
{
    public class ViewHashRecorder
    {
        private readonly List<long> lastViewHashes = new List<long>();
        public List<long> LastViewHashes => new List<long>(lastViewHashes);
        public bool RecordHashes { get; set; }

        public void ClearPreviousHashes()
        {
            lastViewHashes.Clear();
        }

        public void StopRecording()
        {
            RecordHashes = false;
        }

        public void StartNewRecording()
        {
            RecordHashes = true;
            ClearPreviousHashes();
        }

        public void AddHash(bool[,] view)
        {
            if (RecordHashes)
                lastViewHashes.Add(view.CustomHashCode());
        }

        public bool IsHashesEndsWith(List<long> otherHashes)
        {
            if (otherHashes.Count > lastViewHashes.Count)
                return false;

            return lastViewHashes.Reversed().Take(otherHashes.Count).SequenceEqual(otherHashes.Reversed());
        }
    }
}
