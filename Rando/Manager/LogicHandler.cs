using RandomizerCore.Logic;
using RandomizerCore.LogicItems.Templates;
using RandomizerCore.StringItems;
using RandomizerMod.RC;
using RandomizerMod.Settings;
using System.Collections.Generic;
using System.Linq;

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

            List<string> tentExitConditions = new()
            {
                $"GG_Workshop[left1{MilliGolf.golfTransitionSuffix}]"
            };

            foreach ((string name, string scene) in GolfManager.courseList)
            {
                lmb.GetOrAddTerm($"Milligolf_Access-{name}");
                lmb.AddItem(new StringItemTemplate($"Course_Access-{name}", $"Milligolf_Access-{name}++"));
                lmb.GetOrAddTerm($"Milligolf_Complete-{name}");
                lmb.AddItem(new StringItemTemplate($"Course_Completion-{name}", $"Milligolf_Complete-{name}++"));

                int doorIndex = golfScene.courseList.IndexOf(scene) + 1;
                string returnTransition = $"{scene}[{golfScene.courseDict[scene].startTransition}{MilliGolf.golfTransitionSuffix}]";

                lmb.AddTransition(new RawLogicDef($"GG_Workshop[door{doorIndex}{MilliGolf.golfTransitionSuffix}]", $"GG_Workshop[left1{MilliGolf.golfTransitionSuffix}] + Milligolf_Access-{name}"));
                lmb.AddTransition(new RawLogicDef(returnTransition, returnTransition));
                lmb.AddLogicDef(new RawLogicDef($"Milligolf_Course-{name}", returnTransition));
                // this requires that the player be able to go back through
                // a course door that isn't open yet. there is a solution for this
                tentExitConditions.Add($"GG_Workshop[door{doorIndex}{MilliGolf.golfTransitionSuffix}]");
            }

            string tentEntryConditions = string.Join(" | ", GolfManager.courseList.Select(c => $"Milligolf_Access-{c.Item1}"));

            lmb.AddTransition(new RawLogicDef($"Town[{MilliGolf.golfTentTransition}]", $"Town + ({tentEntryConditions})"));
            lmb.AddTransition(new RawLogicDef($"GG_Workshop[left1{MilliGolf.golfTransitionSuffix}]", string.Join(" | ", tentExitConditions)));
            lmb.DoLogicEdit(new RawLogicDef("Town", $"ORIG | Town[{MilliGolf.golfTentTransition}]"));

            if (GolfManager.GlobalSettings.GlobalGoals > Settings.MaxTier.None)
            {
                lmb.AddItem(new EmptyItemTemplate("Milligolf-Goal_Tier"));

                string req = $"GG_Workshop[left1{MilliGolf.golfTransitionSuffix}]";
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