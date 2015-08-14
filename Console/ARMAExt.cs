
using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace ConsoleApp
{
    public class ARMAExt
    {
        [UnmanagedFunctionPointer(CallingConvention.Winapi)]
        private delegate int RVExtension_t(StringBuilder output, int outputSize, char[] function);

        private IntPtr _dllPtr;
        private RVExtension_t _rvextension;
        public bool Loaded;

        public void Load(String dllPath)
        {
            if (!File.Exists(dllPath))
            {
                throw new ARMAExtException();
            }
            _dllPtr = Win32.LoadLibrary(dllPath);
            _rvextension = loadFunction<RVExtension_t>("_RVExtension@12");
            Loaded = true;
        }

        public String Invoke(String input)
        {
            if (!Loaded)
            {
                throw new ARMAExtException();
            }
            const int outputSize = 8000;
            var output = new StringBuilder(outputSize, outputSize);
            var function = input.ToCharArray();
            _rvextension(output, outputSize, function);
            return output.ToString();
        }

        public void Unload()
        {
            if (!Loaded)
            {
                Win32.FreeLibrary(_dllPtr);
            }
            _rvextension = null;
            Loaded = false;
        }

        private T loadFunction<T>(string name) where T : class
        {
            var address = Win32.GetProcAddress(_dllPtr, name);
            var fn_ptr = Marshal.GetDelegateForFunctionPointer(address, typeof(T));
            return fn_ptr as T;
        }
    }
}
