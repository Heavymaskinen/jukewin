using System;
using ConsoleFrontend;
using NUnit.Framework;

namespace JukeCliTests
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void PartsHaveSameWidth()
        {
            var menu = new ConsoleMenu("title", new ConsoleMenuItem()) {Width = 100};
            Assert.AreEqual(100, menu.TitlePart.Width);
            Assert.AreEqual(100, menu.MenuPart.Width);
        }

        [Test]
        public void PartsHaveSameColours()

        {
            var menu = new ConsoleMenu("title", new ConsoleMenuItem())
            {
                Width           = 100,
                BackgroundColor = ConsoleColor.Black,
                ForegroundColor = ConsoleColor.White, SelectedColor = ConsoleColor.Green
            };

            Assert.AreEqual(menu.TitlePart.BackgroundColor, menu.MenuPart.BackgroundColor);
            Assert.AreEqual(menu.TitlePart.ForegroundColor, ConsoleColor.White);
        }
    }
}