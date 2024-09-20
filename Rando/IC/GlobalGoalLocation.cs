using ItemChanger;
using ItemChanger.Locations;
using ItemChanger.Tags;
using ItemChanger.Util;

namespace MilliGolf.Rando.IC
{
    public class GlobalGoalLocation : AutoLocation
    {
        public string locationScene;
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
        }
        protected override void OnLoad()
        {
            MilliGolf.OnBoardCheck += GiveItem;
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