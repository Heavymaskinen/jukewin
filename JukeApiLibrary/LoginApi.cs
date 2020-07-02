using System;
using System.Collections.Generic;
using JukeApiModel;

namespace JukeApiLibrary
{
    public class LoginApi
    {
        private UserRepository repository;

        public LoginApi()
        {
            repository = ApiConfiguration.UserRepository;
        }

        public LoginToken Login(string userName, string password)
        {
            return repository.Login(userName, password);
        }

        public void Logout(int id)
        {
            repository.Logout(id);
        }

        public bool IsValid(int id)
        {
            return repository.GetFromId(id).IsValid();
        }

        public List<User> GetLoggedInUsers()
        {
            return repository.GetActiveUsers();
        }
    }
}
