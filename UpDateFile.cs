using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Helpers
{
    public static class FileList
    {
        public static void SaveFile(this Stream stream,string path)
        {
            FileStream fs = new FileStream(path, FileMode.Create);
            try
            {
                var file = new FileInfo(path);

                Directory.CreateDirectory(file.DirectoryName);
                BinaryReader reader = new BinaryReader(stream);

                byte[] buffer;

                BinaryWriter writer = new BinaryWriter(fs);
                long offset = fs.Length;
                writer.Seek((int)offset, SeekOrigin.Begin);

                do
                {
                    buffer = reader.ReadBytes(1024);
                    writer.Write(buffer);
                } while (buffer.Length > 0);
            }
            catch (Exception)
            {
            }
            finally
            {
                fs.Close();
                stream.Close();
            }
        }
        public static List<UpDateFile> GetAll(DirectoryInfo dir)//搜索文件夹中的文件
        {
            List<UpDateFile> FileList = new List<UpDateFile>();
            string startpath = System.Environment.CurrentDirectory;
            startpath = startpath + "\\UpdatePath\\UpdateFile\\";



            FileInfo[] allFile = dir.GetFiles();
            foreach (FileInfo fi in allFile)
            {

                UpDateFile f = new UpDateFile();
                f.Type = "1";
                f.Path = fi.FullName.Replace(startpath,"");
                f.MD5 = MD51.FileMD5(fi.FullName);
                FileList.Add(f);
            }

            DirectoryInfo[] allDir = dir.GetDirectories();
            foreach (DirectoryInfo d in allDir)
            {
                if (d.Name == "UpdateConfig" || d.Name == "UpdatePath") continue;
                UpDateFile f = new UpDateFile();
                f.Type = "0";
                f.Path = d.FullName.Replace(startpath, "");
                FileList.Add(f);
                FileList.AddRange(GetAll(d));
            }
            return FileList;
        }
       
    }
   
        /// <summary>
        /// 升级文件列表
        /// </summary>
    public class UpDateFile
    {
        /// <summary>
        /// md5
        /// </summary>
        public string MD5 { get; set; }
        /// <summary>
        /// 路径
        /// </summary>
        public string Path { get; set; }
        /// <summary>
        /// 文件还是文件夹 1为文件 0为文件夹
        /// </summary>
        public string Type { get; set; }
    }
}
