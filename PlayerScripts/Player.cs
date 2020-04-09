using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PokemonScripts.Pokemon;
using Utilities.PokemonUtils;



namespace PlayerScripts.Player
{
    using PlayerScripts.Actionable;
    using PlayerScripts.Actionable.Consumable;
    using PlayerScripts.Actionable.Skill;
    using PlayerScripts.Action.Attack_Elements;
    using Utilities.PlayerUtils;

    public class Player : ScriptableObject, IPlayerEntity
    {
        public Pokemon Main_Slot { get; set; }
        public LinkedList<Pokemon> PokeList { get; set; }
        public LinkedList<Player_Action> Inventory { get; set; }
        public Pokemon Current_Pokemon { get; set; }

        public Player()
        {
            this.Main_Slot = this.Create_Pokemon();
            this.Current_Pokemon = this.Main_Slot;
            this.PokeList = new LinkedList<Pokemon>(); // A player will have 4 slots for caught pokemon
            this.Inventory = new LinkedList<Player_Action>();
            AddPokemonToPokeList(this.Main_Slot);
            AddItemToInventory(new Potion("Reg. Potion", 10));
            AddItemToInventory(null);
            AddItemToInventory(null);
            AddItemToInventory(null);
        }

        public void AddItemToInventory(Player_Action item)
        {
            if(this.PokeList.Count >= 4)
            {
                //TODO: Show message saying inventory is full
                Debug.Log("INVENTORY IS FULL!");
            }
            else
            {
                this.Inventory.AddLast(item);
            }
        }

        public void AddPokemonToPokeList(Pokemon pokemon)
        {
            if (this.PokeList.Count >= 4)
            {
                //TODO : Show message saying pokelist is full
                Debug.Log("POKELIST IS FULL!");
            }
            else
            {
                this.PokeList.AddLast(pokemon);
            }
        }

        public void RemoveItemFromInventory(Player_Action item)
        {
            this.Inventory.Remove(item);
        }

        public void UpdatePokemonInPokeList(Pokemon pokemon)
        {
            foreach (Pokemon p in this.PokeList)
            {
                if(pokemon.slotNumber == p.slotNumber)
                {
                    this.PokeList.Find(p).Value = pokemon;
                    break;
                }
            }
        }

        public Pokemon Create_Pokemon()
        {
            int slot = 0;
            string name = "Zug Zug";
            Tuple<PokeType, PokeType> typeAndWeakness = Tuple.Create(PokeType.Normal, PokeType.Fighter);

            Attack tackle = new Attack(name: "Tackle", PokeType.Normal, Condition.Normal, new DamageThreshold(5, 9), 20, hasDamage: true);
            Attack headButt = new Attack(name: "Ketchup", PokeType.Fighter, Condition.Stun, new DamageThreshold(1, 5), 5, hasDebuff: true, hasDamage: true);
            Attack evade = new Attack(name: "Evade", PokeType.Normal, Condition.Attack_Down, new DamageThreshold(0, 0), 10, hasDebuff: true);
            Attack yell = new Attack(name: "Yell", PokeType.Normal, Condition.Defense_Down, new DamageThreshold(0, 0), 10, hasDebuff: true);

            Attack[] moveSet = new Attack[] { tackle, headButt, evade, yell };

            Pokemon rtnPokemon = new Pokemon(slot, name, typeAndWeakness, moveSet);
            return rtnPokemon;
        }
    }
}
