using Juke.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataModel;
using System.Xml;
using System.Xml.Serialization;
using Juke.Control;

namespace Juke.External.Xml
{
    public class XmlSongWriter : SongWriter
    {
        private static readonly XmlSerializer serializerInstance = new XmlSerializer(typeof(PersistedSong[]));
        private string filename;

        public XmlSongWriter(string filename)
        {
            this.filename = filename;
        }

        public void Write(IList<Song> songs)
        {
            Messenger.Log("Writing: " + songs.Count + " to " + filename);
            using (var writer = XmlWriter.Create(filename))
            {
                var persistableSongs = new SongConverter().ConvertSongs(songs);
                serializerInstance.Serialize(writer, persistableSongs.ToArray());
                writer.Flush();
                writer.Close();
            }
        }
    }
}