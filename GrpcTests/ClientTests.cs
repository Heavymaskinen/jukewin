using System.Collections.Generic;
using System.Linq;
using GrpcClient;
using NUnit.Framework;

namespace GrpcTests
{
    public class ClientTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void Interpreter_CommandsAreLoadedWithReflection()
        {
            var interpreter = new CommandInterpreter(new FakeGrpcClient());
            Assert.IsTrue(interpreter.GetCommandNames().Any(), "Commands not loaded.");
        }
    }

    class FakeGrpcClient : JukeClient
    {
        public bool Startup(string serverLocation, bool verbose)
        {
            return false;
        }

        public bool Shutdown()
        {
            throw new System.NotImplementedException();
        }

        public string[] Search(string query)
        {
            throw new System.NotImplementedException();
        }

        public bool LoadLibrary(string file)
        {
            throw new System.NotImplementedException();
        }

        public void Play(string name)
        {
            throw new System.NotImplementedException();
        }

        public string PlayRandom()
        {
            throw new System.NotImplementedException();
        }

        public bool AddSongs(string folder)
        {
            throw new System.NotImplementedException();
        }

        public Dictionary<string, string> GetInfo()
        {
            throw new System.NotImplementedException();
        }

        public void StreamOutput()
        {
            throw new System.NotImplementedException();
        }
    }
}