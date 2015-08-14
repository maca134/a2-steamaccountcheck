using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;

namespace ConsoleApp
{
    class Program
    {
        private static readonly ARMAExt _ext = new ARMAExt();

        static void Main(string[] args)
        {
            _ext.Load(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "SteamAccountCheck.dll"));
            while (true)
            {
                Console.Write("Enter Command: ");
                var cmd = Console.ReadLine();
                if (cmd == "")
                {
                    _ext.Unload();
                    break;
                }

                var watch = Stopwatch.StartNew();
                cmd = cmd.Replace("\\n", "\n") + "\n";
                var result = _ext.Invoke(cmd);
                watch.Stop();
                var elapsedMs = watch.ElapsedMilliseconds;
                Console.WriteLine("Command took {0} ms to run", elapsedMs);
                Console.WriteLine(result);
                Console.WriteLine("--------------");
            }

        }
    }
}
