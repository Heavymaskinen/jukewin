using System;
using System.Collections.Generic;
using DataModel;
using Juke.Control;
using JukeAdminCli.Commands;
using Newtonsoft.Json;
using NUnit.Framework;

namespace JukeAdminTest
{
    public class AddSongsTest
    {
        private FakeSongLoaderFactory factory;
        private AddSongsCommand command;

        [SetUp]
        public void Setup()
        {
            factory = new FakeSongLoaderFactory();
            command = new AddSongsCommand(JukeController.Instance, factory);
        }

        [Test]
        public void MissingFolderParameter_Fail()
        {
            Assert.That(() => command.Execute(new string[] { }), Throws.Exception);
        }

        [Test]
        public void MissingDestinationFileParameter_Fail()
        {
            Assert.That(() => command.Execute(new string[] { "library_file" }), Throws.Exception);
        }

        [Test]
        public void LoaderCreatedWithSpecifiedPath()
        {
            command.Execute(new string[] { "targetfolder", "targetfile" });
            Assert.AreEqual("targetfolder", factory.CreatedLoader.UsedPath);
        }

        [Test]
        public void SongsLoadedToLibrary()
        {
            Song fakeSong = new Song("artist", "album", "song1");
            factory.SpecifiedLoader = new FakeSongLoader("mypath")
            {
                SongsToLoad = new List<Song>
                {
                    fakeSong
                }
            };

            command.Execute(new string[] { "targetfolder", "targetfile" });
            Assert.AreEqual(fakeSong, JukeController.Instance.Browser.Songs[0]);
        }

        [Test]
        public void LibrarySavedToFile()
        {
            command.Execute(new string[] { "targetfolder", "targetfile" });
            Assert.AreEqual("targetfile", factory.CreatedWriter.UsedFileName);
            Assert.IsTrue(factory.CreatedWriter.IsWritten);
        }

        [Test]
        public void DocumentationContainsNameAndParameters()
        {
            var doc = command.GetDocumentation();
            Assert.AreEqual("add", doc.Name);
            Assert.AreEqual(new string[] { "Target folder", "Library file" }, doc.Parameters);
        }

        [Test]
        public void SerializeSongListToJson()
        {
            var json = JsonConvert.SerializeObject(new List<Song>() { new Song("a","b","c")});
            Assert.IsNotNull(json);
        }
    }
}