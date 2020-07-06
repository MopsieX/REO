using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
namespace REO
{


    public static class Keyboard
    {
       
        const int VK_RETURN = 0x0D;
        const uint WM_KEYDOWN = 0x0100;
        const uint WM_KEYUP = 0x0101;
        const int WM_CHAR = 0x0102;
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool SetForegroundWindow(IntPtr hWnd);
        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, uint lParam);
        [DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        private static extern bool PostMessage(IntPtr hWnd, int msg, IntPtr wp, IntPtr lp);
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern uint MapVirtualKey(uint uCode, uint uMapType);

        public static void KeyDown(IntPtr hwnd, uint key)
        {
            var scanCode = MapVirtualKey(key, 0);
            var lParam = (0x00000001 | (scanCode << 16));
            SendMessage(hwnd, WM_KEYDOWN, (IntPtr)key, lParam);
        }
        public static void KeyUp(IntPtr hwnd, uint key)
        {

            var scanCode = MapVirtualKey(key, 0);
            var lParam = (0xC0000001 | (scanCode << 16));
            SendMessage(hwnd, WM_KEYUP, (IntPtr)key, lParam);
        }

        public static void SendString(IntPtr hWnd, string Message)
        {
            SetForegroundWindow(hWnd);
            for (int i = 0; i < Message.Length; i++)
            {
                PostMessage(hWnd, WM_CHAR, (IntPtr)Message[i], IntPtr.Zero);
            }
        }
        public static void SendEnterKey(IntPtr hWnd)
        {
            SetForegroundWindow(hWnd);
            KeyDown(hWnd, VK_RETURN);
            KeyUp(hWnd, VK_RETURN);
        }
    }
}
