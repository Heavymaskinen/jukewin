using System;
namespace JukeAdminCli
{
    public struct Documentation
    {
        public string Name;
        public string Description;
        public string[] Parameters;

        public Documentation(string name, string description, string[] parameters)
        {
            Name = name;
            Description = description;
            Parameters = parameters;
        }

        public override string ToString()
        {
            var str = Name;
            foreach (var p in Parameters)
            {
                str += " <" + p + ">";
            }

            str += " - " + Description;
            return str;
        }
    }
}
