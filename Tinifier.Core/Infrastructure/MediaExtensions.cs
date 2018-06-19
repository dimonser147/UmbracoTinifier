using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tinifier.Core.Models.Db;
using Umbraco.Core.IO;

namespace Tinifier.Core.Infrastructure
{
    /// <summary>
    /// Media extensions
    /// </summary>
    public static class MediaExtensions
    {
        /// <summary>
        /// Read stream to bytes
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static byte[] ToBytes(this Stream input)
        {
            byte[] buffer = new byte[16 * 1024];
            using (MemoryStream ms = new MemoryStream())
            {
                int read;
                while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }
                return ms.ToArray();
            }
        }

        /// <summary>
        /// Read bytes from TImage based on File System
        /// </summary>
        /// <param name="tImage">TImage</param>
        /// <param name="fs">Umbraco file system (default or Azure)</param>
        /// <returns></returns>
        public static byte[] ToBytes(this TImage tImage, IFileSystem fs)
        {
            byte[] data;
            string path = fs.GetRelativePath(tImage.AbsoluteUrl);
            using (Stream fileStream = fs.OpenFile(path))
            {
                data = fileStream.ToBytes();
            }
            return data;
        }
    }
}
