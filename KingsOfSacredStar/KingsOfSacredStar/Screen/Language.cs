using System.Globalization;
using System.Threading;

namespace KingsOfSacredStar.Screen
{
    internal static class Language
    {
        public static void ChangeLanguage()
        {
            Settings.Dict["Language"] = Settings.Dict["Language"] == "en-US" ? "de-DE" : "en-US";
            Settings.WriteSettings();
            CurrentLanguage();
        }

        public static void CurrentLanguage()
        {
            Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo(Settings.Dict["Language"]);
        }
    }
}
