using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using PokemonScripts.Pokemon;
using PlayerScripts.Player;
using Utilities.PlayerUtils;
using PlayerScripts.Actionable;
using PlayerScripts.Actionable.UI_Element;
using PlayerScripts.Actionable.Skill;
using ManagerScripts.BattleManager.BattleStates;
using UnityEngine.UI;


public class PlayerController : MonoBehaviour
{
    private Player_Action[,] UI_InitialState; // This is our current state of the UI, it will be called upon each frame. && //UI_InitialState returns our UI the 4 Header_Options we have
    public Player_Action[,] UI_State { get; set; }
    public GridPos currentPosition { get; set; }
    public bool isUIActive { get; set; }
    public Player StartingPlayer { get; set; }
    //For now, lets only deal with one AI pokemon
    public Pokemon enemy { get; set; }

    public GameObject player_src;
    public Clip_Switch Player_CS { get; set; }

    // Start is called before the first frame update
    void Awake()
    {
        this.StartingPlayer = ScriptableObject.CreateInstance<Player>();
        this.isUIActive = true;
        this.currentPosition = new GridPos(0,0);
        this.Player_CS = player_src.GetComponent<Clip_Switch>();

        //Setup UI State 
        this.Setup_UI_Elements();
    }

    public void doAction(Player_Action action, out BattleStates newState)
    {
        if (action.GetType() == typeof(Header_Option))
        {
            updateUI_State((Header_Option)action);
            newState = BattleStates.PLAYER;
        }
        else
        {
            action.execute(player: this.StartingPlayer.Current_Pokemon, enemy: this.enemy, this.StartingPlayer.PokeList);
            this.StartingPlayer.Current_Pokemon.chosenAction = action;
            newState = BattleStates.PLAYER_ANIMATION;
        }
    }

    private void updateUI_State(Header_Option header)
    {
        this.UI_State = getNewState(header.subList);
    }

    public void resetUI_State()
    {
        this.currentPosition = new GridPos(0, 0);
        this.UI_State = this.UI_InitialState;
    }

    private Player_Action getElementAtCurrentPosition()
    {
        return this.UI_State[currentPosition.x, currentPosition.y];
    }

    public Player_Action[,] getUI_State()
    {
        return this.UI_State;
    }
    /*
     * Setup_UI_Elements(void) : void
     * 
     * This sets up the initial UI elements.
     * 
     * Return : void
     */
    private void Setup_UI_Elements()
    {
        Player_Action[] inv = new Player_Action[StartingPlayer.Inventory.Count];
        StartingPlayer.Inventory.CopyTo(inv, 0); //Copy linked list into array

        Header_Option attacks = new Header_Option("Attacks", StartingPlayer.Current_Pokemon.moveSet);
        Header_Option bag = new Header_Option("Bag", inv);
        Header_Option pokemon = new Header_Option("Pokemon", UI_Util.PokeListToAction(this.StartingPlayer));
        Header_Option flee = new Header_Option("Flee");

        this.UI_State = new Player_Action[,] { { attacks, bag }, { pokemon, flee } }; // This will always be the initial state
        this.UI_InitialState = new Player_Action[,] { { attacks, bag }, { pokemon, flee } }; // This serves as a COPY of the initial state;
    }

    private Player_Action[,] getNewState(Player_Action[] subList)
    {
        //Lets make a N X 2 matrix, where is N is the 'sublist.Length / 2', and lets fill that matrix with every value of our sublist:
        Player_Action[,] rtnState = new Player_Action[subList.Length / 2, 2];

        int subList_idx = 0;
        while(subList_idx < subList.Length)
        {
            int row = subList_idx / 2; // For i to N: row = 0, 0, 1, 1, 2, 2, ...., n-1, n-1, n, n -- this value is floored each time allowing us to make this pattern
            int col = subList_idx % 2; // For i to N: col = 0, 1, 0, 1, 0 , 1,...., 0, 1, 0, 1 -- taking the mod allows us to alternate between column 1 and 2.

            rtnState[row, col] = subList[subList_idx];
            subList_idx++;
        }
        return rtnState;
    }

    public bool isInBounds(int x, int y)
    {
        if (x >= 0 && y >= 0 && x < 2 && y < 2)
        {
            return true;
        }

        return false;
    }
}
