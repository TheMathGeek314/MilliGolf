using MilliGolf.Rando.Manager;
using MilliGolf.Rando.Settings;
using RandoSettingsManager;
using RandoSettingsManager.SettingsManagement;
using RandoSettingsManager.SettingsManagement.Versioning;

namespace MilliGolf.Rando.Interop
{
    internal static class RSM_Interop
    {
        public static void Hook()
        {
            RandoSettingsManagerMod.Instance.RegisterConnection(new GolfSettingsProxy());
        }
    }

    internal class GolfSettingsProxy : RandoSettingsProxy<GolfRandoSettings, string>
    {
        public override string ModKey => MilliGolf.Instance.GetName();

        public override VersioningPolicy<string> VersioningPolicy { get; }
            = new EqualityVersioningPolicy<string>(MilliGolf.Instance.GetVersion());

        public override void ReceiveSettings(GolfRandoSettings settings)
        {
            if (settings != null)
            {
                ConnectionMenu.Instance!.Apply(settings);
            }
            else
            {
                ConnectionMenu.Instance!.Disable();
            }
        }

        public override bool TryProvideSettings(out GolfRandoSettings settings)
        {
            settings = GolfManager.GlobalSettings;
            return settings.Enabled;
        }
    }
}