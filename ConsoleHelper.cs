using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Helpers
{
    public class ConsoleHelper
    {
        private string path = "";
        public ConsoleHelper(string _path="Log\\")
        {
            
            if (!Directory.Exists(_path))
            {
                Directory.CreateDirectory(_path);
            }
            path = _path;
        }
        public void WriteInfo(string msg)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            msg = AppendMsg("Info", msg);
            Console.WriteLine(msg);
            WriteLog(msg);
        }
        public void WriteError(string msg)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            msg = AppendMsg("Error", msg);
            Console.WriteLine(msg);
            Console.ForegroundColor = ConsoleColor.Green;
            WriteLog(msg);
        }
        public void WriteWarning(string msg)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            msg = AppendMsg("Warning", msg);
            Console.WriteLine(msg);
            Console.ForegroundColor = ConsoleColor.Green;
            WriteLog(msg);
        }
        private  string AppendMsg(string type,string msg)
        {
            return System.DateTime.Now.ToString() + "\t" + type + "\t" + msg;
        }
        private void WriteLog(string msg)
        {
            try
            {
                string filename = path + System.DateTime.Now.ToShortDateString().Replace("/", "") + ".txt";
                FileInfo LogWrite = new FileInfo(filename);
                
                StreamWriter w = LogWrite.AppendText();
                w.WriteLine(msg);
                w.Flush();
                w.Close();
            }
            catch
            {

            }
            
        }
    }
}
