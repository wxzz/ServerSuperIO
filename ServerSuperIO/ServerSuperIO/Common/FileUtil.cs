using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace ServerSuperIO.Common
{
    public static class FileUtil
    {
        /// <summary>
        /// 追加内容到指定文件中
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="content"></param>
        public static void WriteAppend(string filePath, string content)
        {
            WriteAppend(filePath,new string[]{content});
        }

        public static void WriteAppend(string filePath, string[] contents)
        {
            //System.IO.StreamWriter sr = new System.IO.StreamWriter(filePath, true);
            //foreach (string c in contents)
            //{
            //    sr.WriteLine(c);
            //}
            //sr.Flush();
            //sr.Close();

            using (FileStream fs = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.Write, FileShare.ReadWrite))
            {
                fs.Seek(fs.Length, SeekOrigin.Current);

                string content = String.Join(Environment.NewLine, contents) + Environment.NewLine;

                byte[] data = System.Text.Encoding.UTF8.GetBytes(content);

                fs.Write(data, 0, data.Length);

                fs.Close();
            }
        }

        /// <summary>
        /// 10.7判断两个文件的哈希值是否一致
        /// </summary>
        /// <param name="fileName1"></param>
        /// <param name="fileName2"></param>
        /// <returns></returns>
        public static bool CompareFiles(string fileName1, string fileName2)
        {
            using (HashAlgorithm hashAlg = HashAlgorithm.Create())
            {
                using (FileStream fs1 = new FileStream(fileName1, FileMode.Open), fs2 = new FileStream(fileName2, FileMode.Open))
                {
                    byte[] hashBytes1 = hashAlg.ComputeHash(fs1);
                    byte[] hashBytes2 = hashAlg.ComputeHash(fs2);

                    return (BitConverter.ToString(hashBytes1) == BitConverter.ToString(hashBytes2));
                }
            }
        }

        /// <summary>
        /// 判断是否存在
        /// </summary>
        /// <param name="fullpath"></param>
        /// <returns></returns>
        public static bool IsExists(string fullpath)
        {
            return System.IO.File.Exists(fullpath);
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="fullpath"></param>
        public static void Delete(string fullpath)
        {
            if (File.Exists(fullpath))
            {
                File.Delete(fullpath);
            }
        }
    }
}
