using System.Collections.Generic;
using MilliGolf.Rando.Settings;
using Newtonsoft.Json;
using RandomizerMod.Logging;

namespace MilliGolf.Rando.Manager
{
    internal static class GolfManager
    {
        public static List<(string, string)> courseList = [
                    ("Dirtmouth", "Town"),
                    ("Crossroads", "Crossroads_07"),
                    ("Grounds", "RestingGrounds_05"),
                    ("Hive", "Hive_03"),
                    ("Greenpath", "Fungus1_31"),
                    ("Canyon", "Fungus3_02"),
                    ("Edge", "Deepnest_East_11"),
                    ("Waterways", "Waterways_02"),
                    ("Cliffs", "Cliffs_01"),
                    ("Abyss", "Abyss_06_Core"),
                    ("Fungal", "Fungus2_12"),
                    ("Sanctum", "Ruins1_30"),
                    ("Basin", "Abyss_04"),
                    ("Gardens", "Fungus3_04"),
                    ("City", "Ruins1_03"),
                    ("Deepnest", "Deepnest_35"),
                    ("Peak", "Mines_23"),
                    ("Palace", "White_Palace_19")
                ];        
        public static GolfRandoSettings GlobalSettings => MilliGolf.GS;
        public static LocalGolfSettings SaveSettings => MilliGolf.golfData;
        public static void Hook()
        {
            LogicHandler.Hook();
            ItemHandler.Hook();
            ConnectionMenu.Hook();
            SettingsLog.AfterLogSettings += AddFileSettings;
        }

        private static void AddFileSettings(LogArguments args, System.IO.TextWriter tw)
        {
            // Log settings into the settings file
            tw.WriteLine("Milligolf Settings:");
            using JsonTextWriter jtw = new(tw) { CloseOutput = false };
            RandomizerMod.RandomizerData.JsonUtil._js.Serialize(jtw, GlobalSettings);
            tw.WriteLine();
        }

    }
}