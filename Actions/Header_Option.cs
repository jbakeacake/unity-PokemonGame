using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayerScripts.Actionable;
using PokemonScripts.Pokemon;

namespace PlayerScripts.Actionable.UI_Element
{
    public class Header_Option : Player_Action
    {
        public Player_Action[] subList { get; }
        public Header_Option(string name, Player_Action[] subList = null)
        {
            this.name = name;
            this.subList = subList;
        }

        public override void execute(Pokemon player = null, Pokemon enemy = null, LinkedList<Pokemon> pokeList = null)
        {
            if(subList == null)
            {
                //Flee
            }
        }
    }
}
