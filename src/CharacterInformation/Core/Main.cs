using ImGuiNET;
using PoeHUD.Models.Enums;
using PoeHUD.Plugins;
using System;
using ImVector2 = System.Numerics.Vector2;

namespace CharacterInformation.Core
{
    public class Main : BaseSettingsPlugin<Settings>
    {
        //https://stackoverflow.com/questions/826777/how-to-have-an-auto-incrementing-version-number-visual-studio
        public Version version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
        public string PluginVersion;
        public DateTime buildDate;

        public Main()
        {
            PluginName = "Character Information";
        }

        public override void Initialise()
        {
            buildDate = new DateTime(2000, 1, 1).AddDays(version.Build).AddSeconds(version.Revision * 2);
            PluginVersion = $"{version}";
        }

        public override void DrawSettingsMenu()
        {
            ImGui.BulletText($"v{PluginVersion}");
            ImGui.BulletText($"Last Updated: {buildDate}");

            base.DrawSettingsMenu();
        }

        public override void Render()
        {
            base.Render();
            RighteousFirePanel();
        }

        public void RighteousFirePanel()
        {
            if (!Settings.RenderRighteousFire)
                return;

            var refBool = true;
            ImGui.BeginWindow("RF Calculator", ref refBool, new ImVector2(200, 150), Settings.RenderRighteousFireLocked ? WindowFlags.NoCollapse | WindowFlags.NoScrollbar | WindowFlags.NoMove | WindowFlags.NoResize | WindowFlags.NoInputs | WindowFlags.NoBringToFrontOnFocus | WindowFlags.NoTitleBar | WindowFlags.NoFocusOnAppearing : WindowFlags.Default | WindowFlags.NoTitleBar | WindowFlags.ResizeFromAnySide);

            double FinalRFCalculation = -TryGetStat(GameStat.TotalNonlethalDamageTakenPerMinuteToEnergyShield) / 60;
            double FinalOtherSourceDegen = -TryGetStat(GameStat.TotalDamageTakenPerMinuteToEnergyShield) / 60;
            double FinalLifeRegen = TryGetStat(GameStat.LifeRegenerationRatePerMinute) / 60;
            var FinalCombinedDegen = FinalRFCalculation + FinalOtherSourceDegen;
            var FinalTotalRegen = FinalLifeRegen + FinalRFCalculation + FinalOtherSourceDegen;

            var DegenString = $"Degen: {ToNiceString(FinalCombinedDegen)}";
            if (Settings.RenderRighteousFireCalculations)
                DegenString += " (" + ToNiceString(FinalRFCalculation) + "," + ToNiceString(FinalOtherSourceDegen) + ")";

            var RegenString = $"Regen: {ToNiceString(FinalLifeRegen)}";

            var FinalString = $"Final: {ToNiceString(FinalTotalRegen)}";
            if (Settings.RenderRighteousFireCalculations)
                FinalString += " (" + ToNiceString(FinalLifeRegen) + "," + ToNiceString(FinalCombinedDegen) + ")";

            ImGui.BulletText("RF Calcuator");
            if (Settings.RenderRighteousFireDegen)
                ImGui.Text(DegenString);
            if (Settings.RenderRighteousFireRegen)
                ImGui.Text(RegenString);
            if (Settings.RenderRighteousFireFinal)
                ImGui.Text(FinalString);
            ImGui.EndWindow();
        }

        private string ToNiceString(double number)
        {
            if (number > 0)
                return $"+{number.ToString($"F{Settings.SignificantDigits.Value}")}";

            return $"{number.ToString($"F{Settings.SignificantDigits.Value}")}";
        }

        private int TryGetStat(GameStat stat) => GameController.EntityListWrapper.PlayerStats.TryGetValue(stat, out var statInt) ? statInt : 0;
    }
}