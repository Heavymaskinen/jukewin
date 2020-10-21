﻿using Juke.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataModel;

namespace Juke.Control.Tests
{
    public class FakeAsyncSongLoader : AsyncSongLoader
    {
        private IList<Song> list;
        public bool AutoComplete { get; set; }
        public FakeAsyncSongLoader():this(new List<Song>())
        {
        }

        public FakeAsyncSongLoader(IList<Song> list):base()
        {
            this.list = list;
        }

        public void SignalComplete()
        {
            Console.WriteLine("Notify complete " + list.Count);
            NotifyCompleted(list);
        }

        public void SignalProgress(string message)
        {
            NotifyProgress(message);
        }

        protected override void InvokeLoad()
        {
            Console.WriteLine("Invoke load!");
            if (AutoComplete)
            {
                SignalComplete();
            }
            
        }
    }
}