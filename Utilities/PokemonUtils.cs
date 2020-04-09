using System.Collections.Generic;

namespace Utilities.PokemonUtils
{
    public enum Condition
    {
        Normal, // Default State
        Stun, // Pokemon can NOT attack // Strength Saving Throw
        Freeze, // Pokemon can NOT attack // Dexterity Saving Throw
        Confused, // Pokemon has CHANCE to attack SELF // Intelligence Saving Throw
        Attack_Down, // Decreased attack // Wisdom Saving Throw
        Defense_Down // Decreased Dexterity // Wisdom Saving Throw
    }

    public enum PokeType
    {
        Normal = 0, // Uses Strength Modifier / 2
        Psychic = 1, // Uses Intelligence Modifier
        Fighter = 2, // Uses Strength Modifier
        Fire = 3, // Uses Wisdom Modifier
        Water = 4, // Uses Int Mod / 2 + Wis Mod / 2
        Grass = 5 // Uses Dexterity
    }

    public static class DnDCalculator
    {

        public static string determineStat(PokeType attackType)
        {
            /*
            Normal = 0, // Uses Strength Modifier
            Psychic = 1, // Uses Intelligence Modifier
            Fighter = 2, // Uses Strength Modifier
            Fire = 3, // Uses Wisdom Modifier
            Water = 4, // Uses Intelligence Modifier
            Grass = 5 // Uses Dexterity
            */

            switch (attackType)
            {
                case PokeType.Normal:
                    return "Strength";
                case PokeType.Psychic:
                    return "Intelligence";
                case PokeType.Fighter:
                    return "Strength";
                case PokeType.Fire:
                    return "Wisdom";
                case PokeType.Water:
                    return "Intelligence";
                case PokeType.Grass:
                    return "Dexterity";
                default:
                    return "Strength";
            }
        }
        public static string determineSavingThrowKey(Condition cdn)
        {
            switch (cdn)
            {
                case Condition.Defense_Down:
                    return "Wisdom";
                case Condition.Attack_Down:
                    return "Wisdom";
                case Condition.Confused:
                    return "Intelligence";
                case Condition.Freeze:
                    return "Dexterity";
                case Condition.Stun:
                    return "Strength";
                default:
                    return "Strength";
            }
        }

        public static bool savingThrow(int modifier)
        {
            //D20 + modifier, DC for Effect is 15:
            System.Random rand = new System.Random();
            int challengeRating = 15;
            int roll = rand.Next(1, 21) + modifier;

            if(roll > challengeRating)
            {
                return true;
            }

            return false;
        }

        public static int determineModifier(int stat)
        {
            if (stat >= 12 && stat < 14)
            {
                return 1;
            }
            else if (stat >= 14 && stat < 16)
            {
                return 2;
            }
            else if (stat >= 16 && stat < 18)
            {
                return 3;
            }
            else if (stat >= 18 && stat < 20)
            {
                return 4;
            }
            else if (stat >= 20)
            {
                return 5;
            }
            else
            {
                return 0;
            }
        }
    }
}
