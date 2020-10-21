using System;
using System.Runtime.Serialization;

namespace JukeApiModel
{
    [DataContract(Name = "LoginToken")]
    public class TokenContract
    {

        public string UserName;
        public int Id;
    }
}
