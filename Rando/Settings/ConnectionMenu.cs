using MenuChanger;
using MenuChanger.Extensions;
using MenuChanger.MenuElements;
using MenuChanger.MenuPanels;
using MilliGolf.Rando.Manager;
using RandomizerMod.Menu;
using UnityEngine;

namespace MilliGolf.Rando.Settings
{
    public class ConnectionMenu 
    {
        // Top-level definitions
        internal static ConnectionMenu Instance { get; private set; }
        private readonly SmallButton pageRootButton;

        // Menu page and elements
        private readonly MenuPage golfPage;
        private MenuElementFactory<GolfRandoSettings> topLevelElementFactory;

        public static void Hook()
        {
            RandomizerMenuAPI.AddMenuPage(ConstructMenu, HandleButton);
            MenuChangerMod.OnExitMainMenu += () => Instance = null;
        }

        private static bool HandleButton(MenuPage landingPage, out SmallButton button)
        {
            button = Instance.pageRootButton;
            button.Text.color = GolfManager.GlobalSettings.Enabled ? Colors.TRUE_COLOR : Colors.DEFAULT_COLOR;
            return true;
        }

        private static void ConstructMenu(MenuPage connectionPage)
        {
            Instance = new(connectionPage);
        }

        private ConnectionMenu(MenuPage connectionPage)
        {
            // Define connection page
            golfPage = new MenuPage("golfPage", connectionPage);
            topLevelElementFactory = new(golfPage, GolfManager.GlobalSettings);
            VerticalItemPanel topLevelPanel = new(golfPage, new Vector2(0, 400), 60, true, topLevelElementFactory.Elements); 
            topLevelElementFactory.ElementLookup[nameof(GolfRandoSettings.Enabled)].SelfChanged += EnableSwitch;
            topLevelPanel.ResetNavigation();
            topLevelPanel.SymSetNeighbor(Neighbor.Down, golfPage.backButton);
            topLevelPanel.SymSetNeighbor(Neighbor.Up, golfPage.backButton);
            pageRootButton = new SmallButton(connectionPage, "MilliGolf");
            pageRootButton.AddHideAndShowEvent(connectionPage, golfPage);
        }
        // Define parameter changes
        private void EnableSwitch(IValueElement obj)
        {
            pageRootButton.Text.color = GolfManager.GlobalSettings.Enabled ? Colors.TRUE_COLOR : Colors.DEFAULT_COLOR;
        }

        // Apply proxy settings
        public void Disable()
        {
            IValueElement elem = topLevelElementFactory.ElementLookup[nameof(GolfRandoSettings.Enabled)];
            elem.SetValue(false);
        }

        public void Apply(GolfRandoSettings settings)
        {
            topLevelElementFactory.SetMenuValues(settings);        
        }
    }
}