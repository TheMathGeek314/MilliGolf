using ItemChanger;
using ItemChanger.Locations;
using ItemChanger.Tags;
using ItemChanger.Util;

namespace MilliGolf.Rando.IC
{
    public class CourseCompletionLocation : AutoLocation
    {
        public CourseCompletionLocation(string mapName, string scene, float x, float y)
        {
            name = $"MilliGolf_Course-{mapName}";
            sceneName = scene;
            flingType = FlingType.DirectDeposit;
            tags = new() {CompletionTag(x, y)};
        }
        
        private static Tag CompletionTag(float x, float y)
        {
            InteropTag tag = new();
            tag.Properties["ModSource"] = "MilliGolf";
            tag.Properties["PoolGroup"] = "Golf";
            tag.Properties["PinSprite"] = new GolfSprite("flagSignE");
            tag.Properties["MapLocations"] = new (string, float, float)[] {(SceneNames.Town, x, y)};
            tag.Message = "RandoSupplementalMetadata";
            return tag;
        }

        protected override void OnUnload()
        {
            pbTracker.OnScoreUpdated -= GiveItem;
        }
        protected override void OnLoad()
        {
            pbTracker.OnScoreUpdated += GiveItem;
        }

        private void GiveItem()
        {
            if (sceneName == GameManager.instance.sceneName && !Placement.AllObtained())
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