using System;
using System.Collections;
using System.Collections.Generic;
using DataModel;

namespace ConsoleFrontend.Overlays
{
    public class SongBrowserOverlay : Overlay
    {
        public event EventHandler<string> SongSelected;

        private IList<Song> songs;
        private ConsoleMenu menu;

        public SongBrowserOverlay(Screen parent, IList<Song> songs) : base(parent)
        {
            this.songs = songs;

            var background = new Background
            {
                BackgroundColor = ConsoleColor.Black,
                ForegroundColor = ConsoleColor.Green,
                Top             = 0,
                Left            = 0,
                MinHeight       = Console.WindowHeight,
                Width           = Console.WindowWidth
            };

            var items = new List<ConsoleMenuItem>();
            foreach (var song in songs)
            {
                items.Add(new ConsoleMenuItem {ID = song.Name, Label = song.Name});
            }

            menu                      =  new ConsoleMenu("Songs", items.ToArray());
            menu.InstanceItemSelected += MenuOnInstanceItemSelected;

            AddComponent(background);
            AddComponent(menu);
        }

        private void MenuOnInstanceItemSelected(object sender, string e)
        {
            SongSelected?.Invoke(this, e);
            Close();
        }

        protected override void CustomInputHandle(ConsoleKey key)
        {
            if (menu.UpdateInput(key))
            {
                Invalidate(false);
            }
        }
    }
}