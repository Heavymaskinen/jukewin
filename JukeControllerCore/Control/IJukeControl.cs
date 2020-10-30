using System;
using Juke.Core;
using Juke.IO;
using JukeControllerCore.IO;

namespace Juke.Control
{
    public interface IJukeControl : IDisposable
    {
        LibraryBrowser Browser { get; }
        LoadHandler LoadHandler { get; }

        SaveHandler SaveHandler { get; }
        Player Player { get; set; }
    }
}