using System;
using System.Runtime.Serialization;

namespace JukeApiModel
{
    [DataContract]
    public class ApiSong
    {
        public ApiSong(string artist,string album, string title)
        {
            Artist = artist;
            Name = title;
            Album = album;
        }

        [DataMember(Name = "name")]
        public string Name { get; set; }
        [DataMember(Name = "album")]
        public string Album { get; set; }
        [DataMember(Name = "artist")]
        public string Artist { get; set; }
    }
}
