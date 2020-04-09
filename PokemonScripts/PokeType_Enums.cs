using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PokemonScripts.PokeType
{
    public enum PokeType
    {
        Normal  = 0, // Uses Strength Modifier / 2
        Psychic = 1, // Uses Intelligence Modifier
        Fighter = 2, // Uses Strength Modifier
        Fire    = 3, // Uses Wisdom Modifier
        Water   = 4, // Uses Int Mod / 2 + Wis Mod / 2
        Grass   = 5 // Uses Dexterity
    }
}
