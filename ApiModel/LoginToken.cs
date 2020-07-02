using System.Runtime.Serialization;

namespace JukeApiModel
{
    [DataContract]
    public class LoginToken
    {
        public static readonly LoginToken Empty = new LoginToken(-1, "Invalid");

        public LoginToken(int id, string userName)
        {
            Id = id;
            UserName = userName;
        }

        [DataMember(Name ="id")]
        public int Id { get; set; }
        [DataMember(Name = "userName")]
        public string UserName { get; set; }

        public bool IsValid() {
            return Id > 0;
        }
    }
}
