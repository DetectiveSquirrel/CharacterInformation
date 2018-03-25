using PoeHUD.Hud.Settings;
using PoeHUD.Plugins;
using SharpDX;

namespace CharacterInformation.Core
{
    public class Settings : SettingsBase
    {
        [Menu("Display Degen Module", 100)]
        public ToggleNode RenderDegen { get; set; } = true;
        [Menu("Show Calculation Chunks", 105, 100)]
        public ToggleNode RenderDegenCalculations {get; set; } = true;
        [Menu("Locked In Place", 104, 100)]
        public ToggleNode RenderDegenLocked { get; set; } = true;
        [Menu("Display Degen", 101, 100)]
        public ToggleNode RenderDegenDegen { get; set; } = true;
        [Menu("Display Regen", 102, 100)]
        public ToggleNode RenderDegenRegen { get; set; } = true;
        [Menu("Display Final", 103, 100)]
        public ToggleNode RenderDegenFinal { get; set; } = true;

        [Menu("Significant Digits", 200)]
        public RangeNode<int> SignificantDigits { get; set; } = new RangeNode<int>(2, 0, 10);

        public ToggleNode OverrideColors { get; set; } = false;
        public Color DegenBackground { get; set; }
        public Color DegenTitle { get; set; } = Color.White;
        public Color DegenPositive { get; set; } = Color.White;
        public Color DegenNegitive { get; set; } = Color.White;
    }
}