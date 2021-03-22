using System;
using Juke.Control;
using Juke.IO;

namespace Juke.UI
{
    public class WriterFactory
    {
        private static SongWriter writerInstance;

        public static void SetWriterInstance(SongWriter writer)
        {
            writerInstance = writer;
        }

        public SongWriter CreateWriter(string path)
        {
            if (writerInstance != null)
            {
                return writerInstance;
            }

            Messenger.Log("SongWriter not initialised!");
            throw new Exception("SongWriter not initialised!");
        }
        
        
    }
}