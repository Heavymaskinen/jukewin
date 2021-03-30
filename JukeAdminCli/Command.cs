using System;
namespace JukeAdminCli
{
    public interface Command
    {
        public bool Execute(string[] args);
        public Documentation GetDocumentation();
    }
}
