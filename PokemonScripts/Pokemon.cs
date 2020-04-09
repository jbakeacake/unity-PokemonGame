namespace PokemonScripts.Pokemon
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using Utilities.PokemonUtils;
    using PlayerScripts.Actionable.Skill;
    using PlayerScripts.Actionable;

    public class Pokemon : IPokemon {
 
        public string name { get; set; }
        public Player_Action chosenAction { get; set; }
        private PokeType pokemonType, weaknessPokeType;
        public int slotNumber { get; set; }
        public LinkedList<Condition> currentConditions { get; set; }
        public float damageModifier { get; set; }
        private float defenseModifier { get; set; }
        public bool canAttack { get; set; }
        private bool isConfused { get; set; }
        public Dictionary<string, int> stats { get; set; }
        public Attack[] moveSet { get; set; }



        public Pokemon(int slotNumber, string name, Tuple<PokeType, PokeType> TypeAndWeakness, Attack[] moveSet, Player_Action chosenAction = null)
        {
            this.slotNumber = slotNumber;
            this.name = name;
            this.pokemonType = TypeAndWeakness.Item1;
            this.weaknessPokeType = TypeAndWeakness.Item2;
            
            this.currentConditions = new LinkedList<Condition>();
            this.canAttack = true; // If FROZEN or STUNNED, this pokemon cannot attack
            this.isConfused = false; // If this is set to true, theres a 50% chance that this Pokemon will attack itself
            this.damageModifier = 0.0f; // This is a value that ranges from -0.5 -- 0.5. It takes a proportion of , and adds that to the outgoing damage

            this.stats = getRandomStats(); // TODO: Make a base stats table in SQLite
            this.moveSet = moveSet;
            this.chosenAction = chosenAction;
        }

        /*
         * modifyHealth(value : int, isSuperEffective : bool) : void
         * 
         * modifyHealth(...) is responsible for modifying the current HP of THIS Pokemon. 
         * Additionally, if there is an attack/heal that is considered to be "super effective," 
         * it will adjust the value receieved according
         * to a constant percentage of this Pokemon's max HP.
         * 
         * value : int >> flat, constant value to adjust the current HP
         * isSuperEffective : bool >> true -- add additional damage/healing, false -- use only the flat damage/healing receieved
         * 
         * Return : void
         */
        public void modifyHealth(int value, PokeType attackerType)
        {
            double percentOfMaxHP = 0.1;
            bool isSuperEffective = determineIsSuperEffective(attackerType);
            if (isSuperEffective) //if something is supereffective, it implies that damage is being dealt -- so, we should subtract our proportion
            {
                value = (int) (value - (int)Math.Floor(percentOfMaxHP * stats["MaxHP"])); //If a move against THIS pokemon is 'super effective', tack on additional damage equal to 15% (adjust as needed) of the Pokemon's max health
            }

            // If the value is NEGATIVE, it's considered DAMAGE. If the value is POSITIVE, its considered HEALING
            // First determine if value being added is greater > maxHP:
            if (this.stats["HP"] + value > this.stats["MaxHP"])
            {
                this.stats["HP"] = this.stats["MaxHP"];
            }
            else if(this.stats["HP"] + value < 0)
            {
                this.stats["HP"] = 0;
            }
            else
            {
                Debug.Log(this.name + " -- def mod. : " + (value * this.defenseModifier));
                Debug.Log(this.name + " took " + value + " points of damage!");
                this.stats["HP"] += value; 
            }
        }

        public void addCondition(Condition cdn, PokeType attackType)
        {
            string key = DnDCalculator.determineSavingThrowKey(cdn);
            int modifier = DnDCalculator.determineModifier(this.stats[key]);
            bool isSuperEffective = determineIsSuperEffective(attackType);
            if(isSuperEffective)
            {
                modifier -= 2;
            }
            //Determine if saving throw succeeds:
            bool savingThrowSuccess = DnDCalculator.savingThrow(modifier);

            if(!savingThrowSuccess && !this.currentConditions.Contains(cdn))
            {
                this.currentConditions.AddLast(cdn);
            }
        }

        public void dumpConditions()
        {
            foreach(Condition cdn in this.currentConditions)
            {
                Debug.Log(this.name + " has " + cdn);
            }
        }

        public void tryToRecoverCondition()
        {
            if (this.currentConditions.Count == 0) return;

            List<Condition> cdnsToRemove = new List<Condition>();

            string key;
            int currentModifier;
            foreach(Condition cdn in this.currentConditions)
            {
                key = DnDCalculator.determineSavingThrowKey(cdn);
                currentModifier = DnDCalculator.determineModifier(this.stats[key]);
                bool savingThrowSuccess = DnDCalculator.savingThrow(currentModifier);
                if(savingThrowSuccess)
                {
                    cdnsToRemove.Add(cdn);
                }
            }

            //This is done to avoid in place modifiction of our collection:
            foreach(Condition cdn in cdnsToRemove)
            {
                Debug.Log(this.name + " REMOVED CONDITION : " + cdn);
                this.currentConditions.Remove(cdn);
            }
            Debug.Log("STOP ROUND");
        }

        /*
         * actionToString(isSuperEffective : bool) : string
         * 
         * Returns a string containing the text to be displayed after a GIVEN attack has been hit.
         * 
         * isSuperEffective : bool >> Changes the returned string to include that the attack was 'Super Effective'
         * 
         * Return : string >> Message to be displayed after being attacked.
         */
        public string actionToString(bool isSuperEffective)
        {
            string rtnString;
            if (chosenAction == null)
            {
                rtnString = $"{ this.name } is stunned!";
            }
            else
            {
                rtnString = $"{ this.name } used {this.chosenAction.name }";
            }

            if (isSuperEffective)
            {
                rtnString += " It was SUPER Effective!";
            }

            return rtnString;
        }

        public bool isDead()
        {
            int currHP;
            stats.TryGetValue("HP", out currHP);
            if (currHP <= 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /*
         * determineIsSuperEffective(attackerType : PokeType) : bool
         * 
         * Determines if the attacking pokemon type is this pokemon's weakness
         * 
         * attackerType : PokeType >> Type of the attacking Pokemon
         * 
         * Return : bool >> Returns true if the attacking type counters this Pokemon's type.
         */
        private bool determineIsSuperEffective(PokeType attackerType)
        {
            if(attackerType == this.weaknessPokeType)
            {
                return true;
            }

            return false;
        }

        public void applyConditions()
        {
            if(this.currentConditions.Count == 0)
            {
                this.damageModifier = 0.0f;
                this.defenseModifier = 0.0f;
                this.canAttack = true;
                this.isConfused = false;
            }
            foreach(Condition cdn in this.currentConditions)
            {
                switch (cdn)
                {
                    case Condition.Freeze:
                        this.canAttack = false;
                        break;
                    case Condition.Stun:
                        this.canAttack = false;
                        break;
                    case Condition.Attack_Down:
                        if(canBeDebuffed(this.damageModifier)) this.damageModifier -= 0.05f;
                        //Debug.Log(this.damageModifier);
                        break;
                    case Condition.Defense_Down:
                        if (canBeDebuffed(this.defenseModifier)) this.defenseModifier -= 0.05f;
                        //Debug.Log(this.defenseModifier);
                        break;
                    case Condition.Confused:
                        this.isConfused = true;
                        break;
                    case Condition.Normal:
                        //Do nothing
                        break;
                    default:
                        break;
                }
            }
        }

        private bool canBeDebuffed(float modifier)
        {
            if (Math.Abs(modifier) <= 0.2)
            {
                return true;
            }
            Debug.Log(this.name + " can't be debuffed anymore!");
            return false;
        }

        private Dictionary<string, int> getRandomStats()
        {
            Dictionary<string, int> rtnStats = new Dictionary<string, int>();
            System.Random rand = new System.Random();
            int maxHP, str, dex, intelli, wis;

            maxHP = rand.Next(20, 31);
            str = rand.Next(10, 15);
            dex = rand.Next(10, 15);
            intelli = rand.Next(10, 15);
            wis = rand.Next(10, 15);

            rtnStats.Add("MaxHP", maxHP);
            rtnStats.Add("HP", maxHP);
            rtnStats.Add("Strength", str);
            rtnStats.Add("Dexterity", dex);
            rtnStats.Add("Intelligence", intelli);
            rtnStats.Add("Wisdom", wis);

            return rtnStats;
        }
    }
}
