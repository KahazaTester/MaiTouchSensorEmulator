﻿using System.Runtime.InteropServices;
using System.Windows;

namespace WpfMaiTouchEmulator.Managers;

class TouchPanelPositionManager
{
    [DllImport("user32.dll", SetLastError = true)]
    static extern nint FindWindow(string? lpClassName, string lpWindowName);

    [DllImport("user32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    static extern bool GetWindowRect(nint hWnd, out RECT lpRect);

    [StructLayout(LayoutKind.Sequential)]
    public struct RECT
    {
        public int Left;
        public int Top;
        public int Right;
        public int Bottom;

        public readonly int Width => Right - Left;
        public readonly int Height => Bottom - Top;
    }

    public Rect? GetSinMaiWindowPosition()
    {
        try
        {
            var hWnd = FindWindow(null, "Sinmai");
            if (hWnd != nint.Zero)
            {
                RECT rect;
                if (GetWindowRect(hWnd, out rect))
                {
                    // Calculate the desired size and position based on the other application's window
                    var renderRect = GetLargest916Rect(rect);
                    var height = renderRect.Height;
                    var left = rect.Left + (rect.Right - rect.Left - renderRect.Width) / 2; // Center horizontally
                    var top = renderRect.Top;
                    return new Rect(left, top, renderRect.Width, height);
                }
            }
        }
        catch (Exception ex)
        {
            Logger.Error("Failed top get sinmai window position", ex);
        }

        return null;
    }

    private static RECT GetLargest916Rect(RECT original)
    {
        var originalWidth = original.Width;
        var originalHeight = original.Height;

        var widthBasedHeight = originalWidth * 10 / 16;
        var heightBasedWidth = originalHeight * 16 / 10;

        if (widthBasedHeight <= originalHeight)
        {
            // Width-based rectangle fits
            return new RECT
            {
                Left = original.Left,
                Top = original.Top + (originalHeight - widthBasedHeight) / 2,
                Right = original.Right,
                Bottom = original.Top + (originalHeight + widthBasedHeight) / 2
            };
        }
        else
        {
            // Height-based rectangle fits
            return new RECT
            {
                Left = original.Left + (originalWidth - heightBasedWidth) / 2,
                Top = original.Top,
                Right = original.Left + (originalWidth + heightBasedWidth) / 2,
                Bottom = original.Bottom
            };
        }
    }
}
