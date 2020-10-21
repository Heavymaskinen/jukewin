using System;
using System.Runtime.Serialization;

namespace JukeApiModel
{
    [DataContract]
    public class User
    {
        [DataMember(Name ="userName")]
        public string UserName { get; set; }

        [DataMember(Name ="id")]
        public int Id { get; set; }

        public User(string userName, int id) {
            UserName = userName;
            Id = id;
        }
    }
}
