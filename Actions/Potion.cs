/*
 * Usable : Action
 * 
 * Most usables are player-centric, adding health or increasing PP for a skill.
 */
namespace PlayerScripts.Actionable.Consumable
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using PlayerScripts.Actionable;
    using PokemonScripts.Pokemon;
    using Utilities.ItemUtils.Consumables;
    using Utilities.PokemonUtils;
    public class Potion : Player_Action
    {
        private int healAmount { get; set; }
        public Potion(string name, int healAmount)
        {
            this.name = name;
            this.healAmount = healAmount;
        }

        public override void execute(Pokemon player = null, Pokemon enemy = null, LinkedList<Pokemon> pokeList = null)
        {
            if (player == null) throw new System.ArgumentException("Player cannot be null for item: Potion", "player");
            modifyHealth(player, this.healAmount);
        }

        private void modifyHealth(Pokemon player, int HP_Modifier)
        {
            player.modifyHealth(HP_Modifier, PokeType.Normal);
        }
    }
}