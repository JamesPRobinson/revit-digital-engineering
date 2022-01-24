using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Utils.Revit
{
    public class ExternalCommandHelper
    {
        // DLL imports from user32.dll to set focus to
        // Revit to force it to forward the external event
        // Raise to actually call the external event 
        // Execute.

        /// <summary>
        /// The GetForegroundWindow function returns a 
        /// handle to the foreground window.
        /// </summary>
        [DllImport("user32.dll")]
        public static extern IntPtr GetForegroundWindow();

        /// <summary>
        /// Move the window associated with the passed 
        /// handle to the front.
        /// </summary>
        [DllImport("user32.dll")]
        public static extern bool SetForegroundWindow(
          IntPtr hWnd);
    }
}
