using System;
using System.Runtime.InteropServices;

using Microsoft.Windows.ApplicationModel.Resources;

namespace CMC_WinUI.Helpers
{
    internal static class ResourceExtensions
    {
        private static ResourceLoader _resLoader = new ResourceLoader();

        public static string GetLocalized(this string resourceKey)
        {
            return _resLoader.GetString(resourceKey);
        }
    }
}
