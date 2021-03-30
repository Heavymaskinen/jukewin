using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Juke.Control;
using Juke.Control.Tests;
using Juke.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace JukeControllerTests
{
    [TestClass]
    public class QueueTests
    {
        [TestMethod]
        public void LargeLibrary_SongNotRecurringAtLeastTenTimes()
        {
            var cat = new FakeSongCatalogue(FakeSongCatalogue.CreateSongs(1, 1, 100));
            JukeController.Instance.LoadHandler.LoadSongs(cat);
            var queue = (JukeController.Instance.Player as Player).Queue;
            var first = queue.Random;
            for (var i = 0; i < 10; i++)
            {
                var other = queue.Random;
                Assert.AreNotEqual(first, other, "Failed at " + i);
                Assert.IsNotNull(other, "Was null at " + i);
            }
        }

        [TestMethod]
        public void FiveSongs_SongNotRecurringAtLeastFourTimes()
        {
            var cat = new FakeSongCatalogue(FakeSongCatalogue.CreateSongs(1, 1, 5));
            JukeController.Instance.LoadHandler.LoadSongs(cat);
            var queue = (JukeController.Instance.Player as Player).Queue;
            var first = queue.Random;
            for (var i = 0; i < 4; i++)
            {
                var other = queue.Random;
                Assert.AreNotEqual(first, other, "Failed at " + i);
                Assert.IsNotNull(other, "Was null at " + i);
            }
        }
    }
}