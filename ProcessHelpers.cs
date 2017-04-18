using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace Helpers
{
    public class ProcessHelpers
    {
        public static System.Diagnostics.Process startProcess(string path,string ags="")
        {
            //启动主程序
            System.Diagnostics.ProcessStartInfo Info = new System.Diagnostics.ProcessStartInfo();

            var file = new FileInfo(path);
            //设置外部程序名  
            Info.FileName = file.Name;
            Info.Arguments = ags;

            //设置外部程序工作目录为   C:\  
            Info.WorkingDirectory = file.DirectoryName;

            //最小化方式启动
            Info.WindowStyle = System.Diagnostics.ProcessWindowStyle.Normal;

            //声明一个程序类  
            System.Diagnostics.Process Proc;

            try
            {
                Proc = System.Diagnostics.Process.Start(Info);
                System.Threading.Thread.Sleep(500);
            }
            catch (System.ComponentModel.Win32Exception)
            {
                throw;
            }
            return Proc;
        }
        public static bool IsProcess(string processName)
        {
            var file = new FileInfo(processName);
            
            Process[] ps = Process.GetProcesses();//获取计算机上所有进程
            foreach (Process p in ps)
            {
                if (p.ProcessName == file.Name.Remove(file.Name.Length-4))//判断进程名称
                {
                    return true;
                }
            }
            return false;
        }
        public static void KillProcess(string processName)
        {
            var file = new FileInfo(processName);
            Process[] ps = Process.GetProcesses();//获取计算机上所有进程
            foreach (Process p in ps)
            {
                if (p.ProcessName == file.Name.Remove(file.Name.Length - 4))//判断进程名称
                {
                    p.Kill();//停止进程
                }
            }
        }
    }
}
