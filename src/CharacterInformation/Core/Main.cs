using ImGuiNET;
using PoeHUD.Models.Enums;
using PoeHUD.Plugins;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using PoeHUD.Framework.Helpers;
using PoeHUD.Hud.UI;
using Color = SharpDX.Color;
using ImVector2 = System.Numerics.Vector2;
using ImVector4 = System.Numerics.Vector4;
using Vector4 = SharpDX.Vector4;

namespace CharacterInformation.Core
{
    public class Main : BaseSettingsPlugin<Settings>
    {
        //https://stackoverflow.com/questions/826777/how-to-have-an-auto-incrementing-version-number-visual-studio
        public Version version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
        public string PluginVersion;
        public DateTime buildDate;

        public Main() { PluginName = "Character Information"; }

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

            if (ImGui.CollapsingHeader("Module Colors", "ModuleColorHeaders", false, false))
            {
                Settings.OverrideColors = ImGuiExtension.Checkbox("Override Module Colors", Settings.OverrideColors);
                Settings.DegenTitle = ImGuiExtension.ColorPicker("Degen Title", Settings.DegenTitle);
                Settings.DegenPositive = ImGuiExtension.ColorPicker("Degen Positive Number", Settings.DegenPositive);
                Settings.DegenNegitive = ImGuiExtension.ColorPicker("Degen Negitive Number", Settings.DegenNegitive);
                Settings.DegenBackground = ImGuiExtension.ColorPicker("Degen Background", Settings.DegenBackground);
                ImGui.Separator();
            }
        }

        public override void Render()
        {
            base.Render();
            DegenPanel();
        }

        public void DegenPanel()
        {
            if (!Settings.RenderDegen)
                return;

            var refBool = true;
            float menuOpacity = ImGui.GetStyle().GetColor(ColorTarget.WindowBg).W;
            if (Settings.OverrideColors)
            {
                ImGui.PushStyleColor(ColorTarget.WindowBg, ToImVector4(Settings.DegenBackground.ToVector4()));
                menuOpacity = ImGui.GetStyle().GetColor(ColorTarget.WindowBg).W;
            }

            ImGui.BeginWindow("Degen Calculator", ref refBool, new ImVector2(200, 150), menuOpacity, Settings.RenderDegenLocked ? WindowFlags.NoCollapse | WindowFlags.NoScrollbar | WindowFlags.NoMove | WindowFlags.NoResize | WindowFlags.NoInputs | WindowFlags.NoBringToFrontOnFocus | WindowFlags.NoTitleBar | WindowFlags.NoFocusOnAppearing : WindowFlags.Default | WindowFlags.NoTitleBar | WindowFlags.ResizeFromAnySide);

            if (Settings.OverrideColors)
            {
                ImGui.PopStyleColor();
            }

            double FinalDegenCalculation = -TryGetStat(GameStat.TotalNonlethalDamageTakenPerMinuteToEnergyShield) / 60;
            double FinalOtherSourceDegen = -TryGetStat(GameStat.TotalDamageTakenPerMinuteToEnergyShield) / 60;
            double FinalLifeRegen = TryGetStat(GameStat.LifeRegenerationRatePerMinute) / 60;
            var FinalCombinedDegen = FinalDegenCalculation + FinalOtherSourceDegen;
            var FinalTotalRegen = FinalLifeRegen + FinalDegenCalculation + FinalOtherSourceDegen;

            if (!Settings.OverrideColors)
            {
                var DegenString = $"Degen: {ToNiceString(FinalCombinedDegen)}";
                if (Settings.RenderDegenCalculations)
                    DegenString += " (" + ToNiceString(FinalDegenCalculation) + "," + ToNiceString(FinalOtherSourceDegen) + ")";

                var RegenString = $"Regen: {ToNiceString(FinalLifeRegen)}";

                var FinalString = $"Final: {ToNiceString(FinalTotalRegen)}";
                if (Settings.RenderDegenCalculations)
                    FinalString += " (" + ToNiceString(FinalLifeRegen) + "," + ToNiceString(FinalCombinedDegen) + ")";

                ImGui.BulletText("Degen Calcuator");
                if (Settings.RenderDegenDegen)
                    ImGui.Text(DegenString);
                if (Settings.RenderDegenRegen)
                    ImGui.Text(RegenString);
                if (Settings.RenderDegenFinal)
                    ImGui.Text(FinalString);
            }
            else
            {
                var DegenString = $"Degen: {ToNiceStringColor(FinalCombinedDegen)}";
                if (Settings.RenderDegenCalculations)
                    DegenString += " (" + ToNiceStringColor(FinalDegenCalculation) + "," + ToNiceStringColor(FinalOtherSourceDegen) + ")";

                var RegenString = $"Regen: {ToNiceStringColor(FinalLifeRegen)}";

                var FinalString = $"Final: {ToNiceStringColor(FinalTotalRegen)}";
                if (Settings.RenderDegenCalculations)
                    FinalString += " (" + ToNiceStringColor(FinalLifeRegen) + "," + ToNiceStringColor(FinalCombinedDegen) + ")";

                ImGui.BulletText("Degen Calcuator");
                if (Settings.RenderDegenDegen)
                    Coloredtext(DegenString);
                if (Settings.RenderDegenRegen)
                    Coloredtext(RegenString);
                if (Settings.RenderDegenFinal)
                    Coloredtext(FinalString);
            }
            ImGui.EndWindow();
        }

        private string ToNiceString(double number)
        {
            if (number > 0)
                return $"+{number.ToString($"F{Settings.SignificantDigits.Value}")}";

            return $"{number.ToString($"F{Settings.SignificantDigits.Value}")}";
        }
        private string ToNiceStringColor(double number)
        {
            if (number > 0)
                return string.Format("{{{0}}}+{1}{2}", HexConverter(Settings.DegenPositive), number.ToString($"F{Settings.SignificantDigits.Value}"), "{}");

            if (number < 0)
                return string.Format("{{{0}}}{1}{2}", HexConverter(Settings.DegenNegitive), number.ToString($"F{Settings.SignificantDigits.Value}"), "{}");

            return $"{number.ToString($"F{Settings.SignificantDigits.Value}")}";
        }


        private ImVector4 ToImVector4(Vector4 vector) => new ImVector4(vector.X, vector.Y, vector.Z, vector.W);

        private int TryGetStat(GameStat stat) => GameController.EntityListWrapper.PlayerStats.TryGetValue(stat, out var statInt) ? statInt : 0;


        /*
         * format is as follows
         * To change color of the string surround hex codes with {} Example: "Uncolored {#AARRGGBB}Colored"
         * having a blank {} will make it go back to default imgui text color, Example: "Uncolored {#AARRGGBB}Colored {}Back to orig color"
         */
        public void Coloredtext(string TextIn)
        {
            try
            {
                var accumulatedText = "";
                var StartColor = ImGui.GetStyle().GetColor(ColorTarget.Text);
                var foundBracketStart = "";
                var hexCode = "";
                var sameLine = false;
                var nextColor = StartColor;
                for (var i = 0; i < TextIn.Length; i++)
                {
                    if (TextIn[i] == '{')
                    {
                        foundBracketStart = TextIn.Substring(i + 1);
                        for (var j = 0; j < foundBracketStart.Length; j++)
                        {
                            i++;
                            if (foundBracketStart[j] == '}')
                                break;
                            hexCode += foundBracketStart[j];
                        }

                        if (sameLine)
                            ImGui.SameLine(0f, 0f);
                        ImGui.Text(accumulatedText, nextColor);
                        if (TextIn[i - 1] == '{')
                            nextColor = StartColor;
                        accumulatedText = "";
                        sameLine = true;
                        if (hexCode != "")
                        {
                            var tempColor = ColorTranslator.FromHtml(hexCode);
                            var tempColor2 = new SharpDX.Color(tempColor.R, tempColor.G, tempColor.B, tempColor.A).ToVector4();
                            nextColor = new System.Numerics.Vector4(tempColor2.X, tempColor2.Y, tempColor2.Z, tempColor2.W);
                        }

                        i++;
                        hexCode = "";
                    }

                    accumulatedText += TextIn[i];
                }

                if (sameLine)
                    ImGui.SameLine(0f, 0f);
                ImGui.Text(accumulatedText, nextColor);
            }
            catch (Exception e)
            {
                // This spams all the time even tho nothing seems broken so it can fuck riiiiiiiight off
                //LogError("ColorText: Incorrect hex format \n" + e, 15);
            }
        }

        // Used for converting SharpDX.Color into string #AARRGGBB
        private static string HexConverter(Color c)
        {
            var rtn = string.Empty;
            try
            {
                rtn = "#" + c.A.ToString("X2") + c.R.ToString("X2") + c.G.ToString("X2") + c.B.ToString("X2");
                return rtn;
            }
            catch (Exception)
            {
                //doing nothing
            }

            return rtn;
        }
    }
}