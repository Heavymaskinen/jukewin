using System;
using ConsoleFrontend;
using ConsoleFrontend.Screens;
using NUnit.Framework;

namespace JukeCliTests
{
    [TestFixture]
    public class RenderTest
    {
        [Test]
        public void RenderOnResize()
        {
            var list   = new Listener();
            var screen = new MainMenuScreen(200, 200);
            screen.Draw();
            screen.Resized(300, 300);
            Assert.IsTrue(list.Invalidated);
            Assert.IsTrue(list.FullyRendered);
        }
    }

    class Listener
    {
        public bool Invalidated;
        public bool FullyRendered;

        public Listener()
        {
            Screen.Invalidated += ScreenOnInvalidated;
        }

        private void ScreenOnInvalidated(object sender, bool e)
        {
            Invalidated = true;
            var screen = (Screen) sender;
            screen.Redraw();
            FullyRendered = true;
        }
    }

    class FakeScreen : Screen
    {
        public FakeScreen()
        {
        }

        protected override void CustomRedraw()
        {
            Console.WriteLine("Redraw!");
        }

        protected override void CustomDraw()
        {
            Console.WriteLine("Small draw!");
        }

        public override void Resized(int newWidth, int newHeight)
        {
            base.Resized(newWidth, newHeight);
            Invalidate(true);
        }

        protected override void CustomUpdateInput(ConsoleKey key)
        {
        }
    }
}