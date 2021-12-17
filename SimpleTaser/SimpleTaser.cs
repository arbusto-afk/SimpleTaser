using Rocket.Core.Plugins;
using Rocket.Unturned.Player;
using SDG.Unturned;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Logger = Rocket.Core.Logging.Logger;
using Rocket.Unturned.Chat;
using Rocket.API;
using Rocket.Unturned;
using Steamworks;

namespace SimpleTaser
{

    public class SimpleTaser : RocketPlugin<Config>
    {
        public static List<CSteamID> TasedPlayers;
        protected override void Load()
        {
            Logger.Log("SimpleTaser loaded correctly! Have fun using it, for bugs and errors contact Arbusto#6794", ConsoleColor.Green);

            TasedPlayers = new List<CSteamID>();
            DamageTool.damagePlayerRequested += OnPlayerDamaged;
        }

        private void OnPlayerDamaged(ref DamagePlayerParameters parameters, ref bool shouldAllow)
        {
            UnturnedPlayer attacker = UnturnedPlayer.FromCSteamID(parameters.killer);
            if (parameters.cause == EDeathCause.GUN && attacker.Player.equipment.itemID == Configuration.Instance.TaserId && attacker.Player.equipment.isEquipped && !TasedPlayers.Contains(attacker.CSteamID))
            {

                parameters.player.movement.sendPluginSpeedMultiplier(0.1f);
                parameters.player.movement.sendPluginJumpMultiplier(0.1f);
                parameters.player.equipment.dequip();
                parameters.player.stance.stance = EPlayerStance.PRONE;
                parameters.player.stance.checkStance(EPlayerStance.PRONE);
                UnturnedPlayer victim = UnturnedPlayer.FromPlayer(parameters.player);
                parameters.player.animator.sendGesture(EPlayerGesture.SURRENDER_START, true);

                if (victim.IsInVehicle)
                {
                    VehicleManager.forceRemovePlayer(victim.CSteamID);
                }
                TasedPlayers.Add(victim.CSteamID);

                StartCoroutine(RemoveFromTased(victim));
                StartCoroutine(CheckTased(victim));

                shouldAllow = false;
                return;
            }

            IEnumerator CheckTased(UnturnedPlayer victim)
            {
                while (TasedPlayers.Contains(victim.CSteamID))
                { 
                    victim.Player.equipment.dequip();
                    victim.Player.animator.sendGesture(EPlayerGesture.SURRENDER_START, true);

                    if (victim.IsInVehicle)
                    {
                        VehicleManager.forceRemovePlayer(victim.CSteamID);
                    }

                    yield return new WaitForSeconds(0.4f);
                }
            }

            IEnumerator RemoveFromTased(UnturnedPlayer victim)
            {
                yield return new WaitForSeconds(Configuration.Instance.TaserTime);
                TasedPlayers.Remove(victim.CSteamID);
                victim.Player.movement.sendPluginSpeedMultiplier(1f);
                victim.Player.movement.sendPluginJumpMultiplier(1f);
            }
        }


    }
}
