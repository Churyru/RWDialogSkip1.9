using BepInEx;
using BepInEx.Logging;
using On.HUD;
using System.Linq;
using System.Security.Permissions;
using UnityEngine;
using ImprovedInput;

// Allows access to private members
#pragma warning disable CS0618
[assembly: SecurityPermission(SecurityAction.RequestMinimum, SkipVerification = true)]
#pragma warning restore CS0618

namespace MonologSkip
{
    [BepInPlugin("NV.DialogSkip", "Dialog skip", "2.0.0")]
    sealed class Plugin : BaseUnityPlugin
    {
        const int framespersecond = 40;
        const int cooldown = (int)(framespersecond * 0.2f);
        int counter = 0;

        public static readonly PlayerKeybind SkipMonolog = PlayerKeybind.Register("DialogSkip:skip", "Dialog Skip", "Skip line", KeyCode.None, KeyCode.None, KeyCode.None);

        public new static ManualLogSource Logger;
        static bool IsInit;

        public void OnEnable()
        {
            Logger = base.Logger;
            On.RainWorld.OnModsInit += OnModsInit;
        }

        void OnModsInit(On.RainWorld.orig_OnModsInit orig, RainWorld self)
        {
            orig(self);

            if (IsInit) return;
            IsInit = true;

            DialogBox.Update += DialogBoxOnUpdate;
        }

        void DialogBoxOnUpdate(DialogBox.orig_Update orig, HUD.DialogBox self)
        {
            
            orig(self);
            if (self.hud.owner is Player player && player.abstractCreature.world.game.Players.Any(abstractPlayer => abstractPlayer.realizedCreature is Player p2 && SkipMonolog.CheckRawPressed(p2.playerState.playerNumber) && counter == 0))
            {
                if (self.CurrentMessage != null)
                {
                    self.showCharacter = self.CurrentMessage.text.Length;
                }
                self.lingerCounter = 10000;
                counter = cooldown;
            }

            if (counter > 0) counter--;
        }
    }
}
