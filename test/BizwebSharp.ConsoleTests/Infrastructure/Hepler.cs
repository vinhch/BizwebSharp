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
                using (var userChoiceKey = Registry.CurrentUser.OpenSubKey(urlAssociation + @"\UserChoice", false))
                {
                    if (userChoiceKey == null)
                    {
                        //Read default browser path from Win XP registry key
                        using (var browserKey = Registry.ClassesRoot.OpenSubKey(@"HTTP\shell\open\command", false))
                        {
                            string path;
                            if (browserKey == null)
                            {
                                using (var browserKey2 = Registry.CurrentUser.OpenSubKey(urlAssociation, false))
                                {
                                    path = browserKey2.GetValue(null) as string;
                                }
                            }
                            else
                            {
                                path = browserKey.GetValue(null) as string;
                            }

                            return CleanifyBrowserPath(path);
                        }
                    }
                    else
                    {
                        // user defined browser choice was found
                        var progId = (userChoiceKey.GetValue("ProgId").ToString());
                        userChoiceKey.Dispose();

                        // now look up the path of the executable
                        var concreteBrowserKey = browserPathKey.Replace("$BROWSER$", progId);
                        using (var kp = Registry.ClassesRoot.OpenSubKey(concreteBrowserKey, false))
                        {
                            return CleanifyBrowserPath(kp.GetValue(null) as string);
                        }
                    }
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
