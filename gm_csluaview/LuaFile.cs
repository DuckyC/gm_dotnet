using GSharp;
using System;
using System.IO;
using PATH = System.IO.Path;

namespace gm_csluaview
{
    public class LuaFile
    {
        public string Path { get; set; }
        public int CRC { get; set; }
        private string _Content;
        public string Content
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_Content)) { _Content = GetContent(); }
                return _Content;
            }
        }

        private string GetContent()
        {
            try
            {
                var path = Extensions.AbsolutePath(PATH.Combine("garrysmod/cache/lua", CRC.ToString() + ".lua"));
                var fs = new FileStream(path, FileMode.Open);
                fs.Seek(4, SeekOrigin.Begin); // skip crc

                var decoder = new SevenZip.Compression.LZMA.Decoder();

                byte[] Props = new byte[5];
                if (fs.Read(Props, 0, Props.Length) != Props.Length)
                    throw new InvalidOperationException("Compressed memory too short");
                decoder.SetDecoderProperties(Props);

                byte[] fileLengthBytes = new byte[8];
                fs.Read(fileLengthBytes, 0, 8);
                long fileLength = BitConverter.ToInt64(fileLengthBytes, 0);

                var outStream = new MemoryStream();
                decoder.Code(fs, outStream, fs.Length, fileLength, null);

                outStream.Position = 0;
                var reader = new StreamReader(outStream);
                return reader.ReadToEnd();
            }
            catch (Exception e)
            {
                return "ERROR: " + e.ToString();
            }

        }
    }
}
