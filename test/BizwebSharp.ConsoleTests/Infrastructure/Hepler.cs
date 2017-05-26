using System;
using Microsoft.Win32;

namespace BizwebSharp.ConsoleTests
{
    public static class Hepler
    {
        public static string GetDefaultBrowserPath()
        {
            const string urlAssociation = @"Software\Microsoft\Windows\Shell\Associations\UrlAssociations\http";
            const string browserPathKey = @"$BROWSER$\shell\open\command";

            try
            {
                //Read default browser path from userChoiceLKey
                var userChoiceKey = Registry.CurrentUser.OpenSubKey(urlAssociation + @"\UserChoice", false);

                //If user choice was not found, try machine default
                if (userChoiceKey == null)
                {
                    //Read default browser path from Win XP registry key
                    var browserKey = Registry.ClassesRoot.OpenSubKey(@"HTTP\shell\open\command", false);

                    //If browser path wasn’t found, try Win Vista (and newer) registry key
                    if (browserKey == null)
                    {
                        browserKey =
                        Registry.CurrentUser.OpenSubKey(
                        urlAssociation, false);
                    }
                    var path = CleanifyBrowserPath(browserKey.GetValue(null) as string);
                    browserKey.Dispose();
                    return path;
                }
                else
                {
                    // user defined browser choice was found
                    var progId = (userChoiceKey.GetValue("ProgId").ToString());
                    userChoiceKey.Dispose();

                    // now look up the path of the executable
                    var concreteBrowserKey = browserPathKey.Replace("$BROWSER$", progId);
                    var kp = Registry.ClassesRoot.OpenSubKey(concreteBrowserKey, false);
                    var browserPath = CleanifyBrowserPath(kp.GetValue(null) as string);
                    kp.Dispose();
                    return browserPath;
                }
            }
            catch (Exception)
            {
                return "";
            }
        }

        private static string CleanifyBrowserPath(string p)
        {
            var url = p.Split('"');
            var clean = url[1];
            return clean;
        }
    }
}
