using PoeHUD.Framework.Helpers;
using PoeHUD.Models;
using PoeHUD.Plugins;
using PoeHUD.Poe.Components;

namespace CharacterInformation.Core
{
    public class LocalPlayer
    {
        public static EntityWrapper Entity => BasePlugin.API.GameController.Player;
        public static Stats Stat => Entity.GetComponent<Stats>();
        public static Life Health => Entity.GetComponent<Life>();
    }
}