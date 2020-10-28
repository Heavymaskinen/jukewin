﻿using Juke.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataModel;
using System.Xml;
using System.Xml.Serialization;
using System.IO;

namespace Juke.External.Xml
{
    public class XmlSongReader : SongLoader
    {
        private string filename;
        private static readonly XmlSerializer serializerInstance = new XmlSerializer(typeof(PersistedSong[]));

        public XmlSongReader(string filename)
        {
            this.filename = filename;
        }

        public override IList<Song> LoadSongs()
        {
            return LoadFromXml();
        }

        private IList<Song> LoadFromXml()
        {
            if (!File.Exists(filename))
            {
                throw new Exception(filename + " not found!");
            }

            List<PersistedSong> songs = null;

            using (XmlReader reader = XmlReader.Create(filename))
            {
                songs = new List<PersistedSong>((PersistedSong[])serializerInstance.Deserialize(reader));
            }

            return new SongConverter().ConvertPersistedSongs(songs);
        }
    }
}