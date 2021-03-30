using Juke.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Juke.Core
{
    public class PlayerEngineFactory
    {
        private static PlayerEngine engine;

        public static PlayerEngine Engine
        {
            get { return engine; }
            set { engine = value; }
        }
    }
}