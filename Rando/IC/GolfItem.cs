using ItemChanger;
using ItemChanger.Tags;
using ItemChanger.UIDefs;
using MilliGolf.Rando.Manager;

namespace MilliGolf.Rando.IC
{
    public class GolfItem : AbstractItem
    {
        public string itemScene;
        public string itemType;
        public override void GiveImmediate(GiveInfo info)
        {
            if (itemType == "Course_Access")
            {
                GolfManager.SaveSettings.randoSaveState.courseAccess.SetVariable(itemScene, true);
            }
            if (itemType == "Course_Completion")
            {
                GolfManager.SaveSettings.randoSaveState.courseCompletion.SetVariable(itemScene, true);
            }
            if (itemType == "Milligolf")
            {
                GolfManager.SaveSettings.randoSaveState.globalGoals += 1;
            }
        }

        public GolfItem(string itemName, string scene, string type)
        {
            name = $"{type}-{itemName}";
            itemScene = scene;
            itemType = type;
            UIDef = new MsgUIDef()
            {
                name = new BoxedString(name.Replace('_', ' ').Replace("-", " - ")),
                shopDesc = new BoxedString("It is time for vengeance."),
                sprite = new GolfSprite("flagSignE")
            };
            tags = new() {GolfItemTag()};
        }
        private static Tag GolfItemTag()
        {
            InteropTag tag = new();
            tag.Properties["ModSource"] = "Milligolf";
            tag.Properties["PoolGroup"] = "Golf";
            tag.Properties["PinSprite"] = new GolfSprite("flagSignE");
            tag.Message = "RandoSupplementalMetadata";
            return tag;
        }
    }
}