using Juke.Core;
using TagLib;

namespace Juke.External.Wmp
{
    public class TaglibTagReaderFactory : TagReaderFactory
    {
        public TagReader Create(string filename)
        {
            return new TaglibTagReader(filename);
        }
    }

    internal class TaglibTagReader : TagReader
    {
        private Tag tag;
        private string filename;
        internal TaglibTagReader(string filename)
        {
            tag = File.Create(filename).Tag;
            this.filename = filename;
        }
        public string Title
        {
            get
            {
                return tag.Title ?? FindFileName();
            }
        }

        private string FindFileName()
        {
            var parts = filename.Split('/','\\');
            var replacement = parts[parts.Length - 1];
            return replacement;
        }

        public string Album => tag.Album;

        public string Artist => tag.FirstPerformer;

        public string TrackNo => tag.Track.ToString();
    }
}
