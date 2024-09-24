using ItemChanger;
using ItemChanger.Locations;
using ItemChanger.Tags;
using ItemChanger.Util;
using MilliGolf.Rando.Manager;

namespace MilliGolf.Rando.IC
{
    public class GlobalGoalLocation : AutoLocation, ILocalHintLocation
    {
        public bool HintActive { get; set; } = GolfManager.SaveSettings.randoSettings.EnableGoalPreviews;
        public int threshold;
        public GlobalGoalLocation(string rank, int maxHits, float x, float y)
        {
            name = $"Milligolf_{rank}";
            threshold = maxHits;
            sceneName = SceneNames.GG_Workshop;
            flingType = FlingType.DirectDeposit;
            tags = [CompletionTag(x, y)];
        }
        
        private static Tag CompletionTag(float x, float y)
        {
            InteropTag tag = new();
            tag.Properties["ModSource"] = "Milligolf";
            tag.Properties["PoolGroup"] = "Golf";
            tag.Properties["PinSprite"] = new GolfSprite("flagSignE");
            tag.Properties["MapLocations"] = new (string, float, float)[] {(SceneNames.Town, x, y)};
            tag.Message = "RandoSupplementalMetadata";
            return tag;
        }

        protected override void OnUnload()
        {
            MilliGolf.OnBoardCheck -= GiveItem;
            if (threshold == golfMilestones.Grandmaster)
                MilliGolf.OnGrandmasterPreview -= GrandmasterHint;
            if (threshold == golfMilestones.Master)
                MilliGolf.OnMasterPreview -= MasterHint;
            if (threshold == golfMilestones.Radiant)
                MilliGolf.OnRadiantPreview -= RadiantHint;
            if (threshold == golfMilestones.Ascended)
                MilliGolf.OnAscendedPreview -= AscendedHint;
            if (threshold == 0)
                MilliGolf.OnAttunedPreview -= AttunedHint;
        }
        protected override void OnLoad()
        {
            MilliGolf.OnBoardCheck += GiveItem;
            if (threshold == golfMilestones.Grandmaster)
                MilliGolf.OnGrandmasterPreview += GrandmasterHint;
            if (threshold == golfMilestones.Master)
                MilliGolf.OnMasterPreview += MasterHint;
            if (threshold == golfMilestones.Radiant)
                MilliGolf.OnRadiantPreview += RadiantHint;
            if (threshold == golfMilestones.Ascended)
                MilliGolf.OnAscendedPreview += AscendedHint;
            if (threshold == 0)
                MilliGolf.OnAttunedPreview += AttunedHint;
        }
        private string GiveHint()
        {
            string hint = "";
            if (HintActive)
            {
                if (threshold > 0)
                {
                    hint = $"Completing all courses in less than {threshold} hits will grant you {Placement.GetUIName()}.";
                }
                else
                {
                    hint = $"Completing all courses will grant you {Placement.GetUIName()}.";
                }
                MilliGolf.Instance.Log(hint);
                Placement.OnPreview(hint);
            }
            return hint;
        }
        private string AttunedHint(string currentDialogue)
        {
            string hint = GiveHint();
            currentDialogue += $"<page>{hint}";
            return currentDialogue;
        }

        private string AscendedHint(string currentDialogue)
        {
            string hint = GiveHint();
            currentDialogue += $"<page>{hint}";
            return currentDialogue;
        }

        private string RadiantHint(string currentDialogue)
        {
            string hint = GiveHint();
            currentDialogue += $"<page>{hint}";
            return currentDialogue;
        }

        private string MasterHint(string currentDialogue)
        {
            string hint = GiveHint();
            currentDialogue += $"<page>{hint}";
            return currentDialogue;
        }

        private string GrandmasterHint(string currentDialogue)
        {
            string hint = GiveHint();
            currentDialogue += $"<page>{hint}";
            return currentDialogue;
        }

        private void GiveItem(int score)
        {
            if ((score <= threshold || threshold == 0) && !Placement.AllObtained()) // The Attuned mark has no hit count limit
            {
                ItemUtility.GiveSequentially(Placement.Items, Placement, new GiveInfo()
                {
                    FlingType = FlingType.DirectDeposit,
                    MessageType = MessageType.Corner,
                });
            };
        }
    }
}