using PlayerScripts.Actionable;
using PokemonScripts.Pokemon;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPlayerEntity
{
    Pokemon Main_Slot { get; set; }
    Pokemon Current_Pokemon { get; set; }
    LinkedList<Pokemon> PokeList { get; set; }
    LinkedList<Player_Action> Inventory { get; set; }

    void AddPokemonToPokeList(Pokemon pokemon);
    void UpdatePokemonInPokeList(Pokemon pokemon);
    void AddItemToInventory(Player_Action item);
    void RemoveItemFromInventory(Player_Action item);
}
