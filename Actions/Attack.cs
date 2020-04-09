using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayerScripts.Actionable;
using PokemonScripts.Pokemon;
using PlayerScripts.Action.Attack_Elements;
using Utilities.PlayerUtils;
using Utilities.PokemonUtils;

namespace PlayerScripts.Actionable.Skill
{
    public class Attack : Player_Action
    {
        /*
         * Attack : Action
         * 
         * An attack consists of several features:
         * > damage threshold
         * > condition/effects
         * > type of attack
         * > uses left of this type of attack
         * 
         * Upon execution of this Action, the enemy Pokemon will roll to see if the attack hits. The damage dealt, conditions, and # of uses left will then be calculated.
         */

        private PokeType attackType;
        private Condition condition;
        private DamageThreshold threshold;
        public bool hasDebuff { get; set; }
        public bool hasDamage { get; set;}
        public int PP { get; set; }

        public Attack(string name, PokeType attackType, Condition cdn, DamageThreshold threshold, int PP, bool hasDebuff = false, bool hasDamage = false)
        {
            this.name = name;
            this.attackType = attackType;
            this.condition = cdn;
            this.threshold = threshold;
            this.PP = PP;
            this.hasDebuff = hasDebuff;
            this.hasDamage = hasDamage;
        }

        public override void execute(Pokemon player = null, Pokemon enemy = null, LinkedList<Pokemon> pokeList = null)
        {
            if (player == null || enemy == null) throw new System.ArgumentException("Player or Enemy parameter can NOT be null");
            if (this.hasDebuff)
            {
                castDebuff(this.condition, this.attackType, enemy);
            }

            if (this.hasDamage)
            {
                int damage = rollForDamage(player);
                damage += (int) (damage * player.damageModifier);
                enemy.modifyHealth(-damage, this.attackType);
            }
        }

        private int rollForDamage(Pokemon player)
        {
            System.Random rand = new System.Random();
            string key = DnDCalculator.determineStat(this.attackType);
            int stat = player.stats[key];
            int modifier = DnDCalculator.determineModifier(stat);

            return rand.Next(this.threshold.lower + modifier, this.threshold.upper);
        }

        private void castDebuff(Condition cdn, PokeType attackType, Pokemon enemy)
        {
            enemy.addCondition(cdn, attackType);
        }
    }
}

