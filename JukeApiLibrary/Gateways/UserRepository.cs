using System;
using System.Collections.Generic;
using JukeApiModel;

namespace JukeApiLibrary
{
    public interface UserRepository
    {
        LoginToken Login(string userName, string password);
        LoginToken GetFromId(int id);
        bool Logout(int id);
        List<User> GetActiveUsers();
    }
}
