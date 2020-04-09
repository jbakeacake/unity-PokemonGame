using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayerScripts.Actionable;
using PokemonScripts.Pokemon;
using PlayerScripts.Player;

public class PokeSwitch : Player_Action
{
    private Player playerData;
    public PokeSwitch(string name, Player playerData)
    {
        this.name = name;
        this.playerData = playerData;
    }

    public override void execute(Pokemon player = null, Pokemon enemy = null, LinkedList<Pokemon> pokeList = null)
    {
        pokemonDoesNotExistException(player);
        this.playerData.UpdatePokemonInPokeList(this.playerData.Current_Pokemon);
        this.playerData.Current_Pokemon = player; // player : Pokemon in this instance refers to the pokemon being switched to.
    }

    private void pokemonDoesNotExistException(Pokemon switchTo)
    {
        foreach(Pokemon p in this.playerData.PokeList)
        {
            if(switchTo == p && !switchTo.isDead())
            {
                return;
            }

            throw new System.ArgumentException("The Pokemon being switched to does not exist, or is KO'd", "switchTo");
        }
    }
}
