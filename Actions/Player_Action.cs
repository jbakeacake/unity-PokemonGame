using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PokemonScripts.Pokemon;

namespace PlayerScripts.Actionable
{
    
    public abstract class Player_Action
    {
        public string name { get; set; }
        public abstract void execute(Pokemon player = null, Pokemon enemy = null, LinkedList<Pokemon> pokeList = null);
    }
}