using System;
using System.Windows;
using System.Windows.Forms;

namespace Pingy.Extensions
{
    public static class WindowExt
    {
        public static void SetToMiddleOfScreen(this Window window)
        {
            if (window == null) { throw new ArgumentNullException(nameof(window)); }

            double screenHeight = SystemParameters.PrimaryScreenHeight;
            double windowHeight = window.Height;
            window.Top = (screenHeight / 2) - (windowHeight / 2);

            double screenWidth = SystemParameters.PrimaryScreenWidth;
            double windowWidth = window.Width;
            window.Left = (screenWidth / 2) - (windowWidth / 2);
        }

        public static void SetToMiddleOfScreen(this Window window, Screen currentMonitor)
        {
            if (window == null) { throw new ArgumentNullException(nameof(window)); }
            if (currentMonitor == null) { throw new ArgumentNullException(nameof(currentMonitor)); }

            double screenHeight = currentMonitor.WorkingArea.Height;
            double windowHeight = window.Height;
            window.Top = (screenHeight / 2) - (windowHeight / 2);
            
            double screenWidth = currentMonitor.WorkingArea.Width;
            double windowWidth = window.Width;
            window.Left = (screenWidth / 2) - (windowWidth / 2);
        }
    }
}
