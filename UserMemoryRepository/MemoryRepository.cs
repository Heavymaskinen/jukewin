using System;
using System.Collections.Generic;
using JukeApiLibrary;
using JukeApiModel;

namespace UserMemoryRepository
{
    public class MemoryRepository : UserRepository
    {
        private Dictionary<string, string> users;
        private Dictionary<string, User> loggedInUsers;

        public MemoryRepository()
        {
            users = new Dictionary<string, string>();
            loggedInUsers = new Dictionary<string, User>();
        }

        public LoginToken Login(string userName, string password)
        {
            if (userName == null || password == null)
            {
                return LoginToken.Empty;
            }

            if (loggedInUsers.ContainsKey(userName) && users[userName].Equals(password))
            {
                var user = loggedInUsers[userName];
                return new LoginToken(user.Id, user.UserName);
            }

            if (users.ContainsKey(userName) && users[userName].Equals(password))
            {
                var random = new Random();
                var id = random.Next();

                var token = new LoginToken(id, userName);
                loggedInUsers.Add(userName, new User(userName, token.Id));
                return token;
            }

            return LoginToken.Empty;
        }

        public bool Logout(int id)
        {
            var token = GetFromId(id);
            if (!token.IsValid())
            {
                return false;
            }

            loggedInUsers.Remove(token.UserName);
            return true;
        }

        public void AddUser(string userName, string password)
        {
            users.Add(userName, password);
        }

        public LoginToken GetFromId(int id)
        {
            foreach (var user in loggedInUsers.Values)
            {
                if (user.Id == id)
                {
                    return new LoginToken(user.Id, user.UserName);
                }
            }

            return LoginToken.Empty;
        }

        public List<User> GetActiveUsers()
        {
            var list = new List<User>();
            list.AddRange(loggedInUsers.Values);
            return list;
        }
    }
}
