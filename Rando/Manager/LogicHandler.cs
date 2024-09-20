using RandomizerCore.Logic;
using RandomizerCore.LogicItems.Templates;
using RandomizerCore.StringItems;
using RandomizerMod.RC;
using RandomizerMod.Settings;


namespace MilliGolf.Rando.Manager
{
    public class LogicHandler
    {
        public static void Hook()
        {
            RCData.RuntimeLogicOverride.Subscribe(0f, ApplyLogic);
        }

        private static void ApplyLogic(GenerationSettings gs, LogicManagerBuilder lmb)
        {
            if (!GolfManager.GlobalSettings.Enabled)
                return;

            foreach ((string name, string scene) in GolfManager.courseList)
            {
                lmb.GetOrAddTerm($"Milligolf_Access-{name}");
                lmb.AddItem(new StringItemTemplate($"Course_Access-{name}", $"Milligolf_Access-{name}++"));
                lmb.GetOrAddTerm($"Milligolf_Complete-{name}");
                lmb.AddItem(new StringItemTemplate($"Course_Completion-{name}", $"Milligolf_Complete-{name}++"));
                lmb.AddLogicDef(new RawLogicDef($"Milligolf_Course-{name}", $"Town + Milligolf_Access-{name}"));
            }

            if (GolfManager.GlobalSettings.GlobalGoals > Settings.MaxTier.None)
            {
                lmb.AddItem(new EmptyItemTemplate("Milligolf-Goal_Tier"));

                string req = "Town";
                foreach ((string name, string scene) in GolfManager.courseList)
                {
                    req += $" + Milligolf_Access-{name} + Milligolf_Complete-{name}";
                }
                lmb.AddLogicDef(new RawLogicDef("Milligolf_Attuned", req));
                lmb.AddLogicDef(new RawLogicDef("Milligolf_Ascended", req));
                lmb.AddLogicDef(new RawLogicDef("Milligolf_Radiant", req));
                lmb.AddLogicDef(new RawLogicDef("Milligolf_Master", req));
                lmb.AddLogicDef(new RawLogicDef("Milligolf_Grandmaster", req));
            }
        }
    }
}