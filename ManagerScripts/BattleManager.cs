using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ManagerScripts.BattleManager.BattleStates;
using PokemonScripts.Pokemon;
using PlayerScripts.Player;
using PlayerScripts.Actionable;
using Utilities.PlayerUtils;

public class BattleManager : MonoBehaviour
{
    private BattleStates CURRENT_STATE;
    private GFX_Controller gfx_controller;
    private PlayerController playerController;
    private EnemyController opponentController;

    public GameObject playerObject, opponentObject, gfx_object;
    public GameObject DisplayBox_Container, PlayerInfo_Container, EnemyInfo_Container;

    private bool isDisplaying = true;

    // Start is called before the first frame update
    void Start()
    {
        this.CURRENT_STATE = BattleStates.PLAYER; //Begin the Battle with player taking the first turn
        this.playerController = playerObject.GetComponent<PlayerController>();
        this.opponentController = opponentObject.GetComponent<EnemyController>();
        

        this.gfx_controller = gfx_object.GetComponent<GFX_Controller>();
    }

    // Update is called once per frame
    void Update()
    {
        BattleStateMachine(this.CURRENT_STATE);
    }
    /*
     * BattleManager : Class
     * 
     * Gives a barebones structure of what's needed for our BattleManager Class.
     * The BattleManager class is responsible for changing the states throughout the battle to:
     *  > Enable player actions
     *  > Display Effects / Text Boxes
     *  > Enable enemy actions
     *  > Determine when a battle is finished
     * 
     * The BattleManager class should be as simple as possible, and is, otherwise, just a controller for the flow of combat.
     */
    private void setState(BattleStates newState)
    {
        this.CURRENT_STATE = newState;
    }
    /*
     * transitionState(currentState : BattleStates) : void
     * 
     * This is called for every fixed frame of the battle.
     * 
     * This method is responsible for calling the appropriate method given the current state. Once the method is finished, the next state is set and the state machine continues.
     * 
     * currentState : BattleStates >> Describes the current state of the battle, and transitions to the next appropriate state.
     * 
     * Return : void
     */
     
    private void BattleStateMachine(BattleStates currentState)
    {
        switch(currentState)
        {
            case BattleStates.PLAYER:
                START_playerTurn();
                break;
            case BattleStates.ENEMY:
                START_enemyTurn();
                break;
            case BattleStates.PLAYER_TEXT:
                if (isDisplaying)
                {
                    this.playerController.Player_CS.Set_Audio_Clip(this.playerController.StartingPlayer.Current_Pokemon.chosenAction);
                    SHOW_displayText(this.playerController.StartingPlayer, this.playerController.player_src.GetComponent<AudioSource>());
                    isDisplaying = false;
                }

                if (Input.GetKeyDown(KeyCode.Return))
                {
                    isDisplaying = true;
                    this.gfx_controller.Hide_Text_Container();
                    this.playerController.Player_CS.src.Stop();
                    this.setState(BattleStates.ENEMY);
                }

                break;
            case BattleStates.ENEMY_TEXT:
                if(isDisplaying)
                {
                    this.opponentController.Enemy_CS.Set_Audio_Clip(this.opponentController.StartingEnemy.Current_Pokemon.chosenAction);
                    START_enemyAnim();
                    SHOW_displayText(this.opponentController.StartingEnemy, this.opponentController.enemy_src.GetComponent<AudioSource>());
                    isDisplaying = false;
                }
                if (Input.GetKeyDown(KeyCode.Return))
                {
                    isDisplaying = true;
                    this.gfx_controller.Hide_Text_Container();
                    this.opponentController.Enemy_CS.src.Stop();
                    this.setState(BattleStates.PLAYER);
                }
                break;
            case BattleStates.PLAYER_ANIMATION:
                START_playerAnim();
                this.setState(BattleStates.PLAYER_TEXT);
                break;
            case BattleStates.ENEMY_ANIMATION:
                START_enemyAnim();
                BattleStates roundConclusion = examineHP(this.playerController.StartingPlayer.Current_Pokemon, this.opponentController.StartingEnemy.Current_Pokemon);
                this.setState(roundConclusion); // Round Conclusion can be 3 different states: WIN (enemy.HP <= 0), LOSE (player.HP <= 0), PLAYER -- For now, let's always let the player go first
                break;
            case BattleStates.WIN:
                SHOW_winScreen();
                break;
            case BattleStates.LOSE:
                SHOW_loseScreen();
                break;
            default:
                throw new System.ArgumentException("Current State is non-existent", "currentState");
        }
    }

    private void START_playerTurn()
    {
        this.playerController.StartingPlayer.Current_Pokemon.chosenAction = null;
        playerController.StartingPlayer.Current_Pokemon.tryToRecoverCondition();
        playerController.StartingPlayer.Current_Pokemon.applyConditions();
        playerController.StartingPlayer.Current_Pokemon.dumpConditions();
        this.playerController.enemy = this.opponentController.StartingEnemy.Current_Pokemon;
        if (!playerController.StartingPlayer.Current_Pokemon.canAttack)
        {
            this.playerController.resetUI_State();
            this.CURRENT_STATE = BattleStates.PLAYER_ANIMATION;
            return;
        }

        this.gfx_controller.SetUI_ElementNames();
        this.UI_control();

        if (Input.GetKeyDown(KeyCode.Return))
        {
            float P_prevHP = this.playerController.StartingPlayer.Current_Pokemon.stats["HP"];
            float E_prevHP = this.opponentController.StartingEnemy.Current_Pokemon.stats["HP"];

            playerController.doAction(playerController.UI_State[playerController.currentPosition.x, playerController.currentPosition.y], out this.CURRENT_STATE);

            float P_newHP = this.playerController.StartingPlayer.Current_Pokemon.stats["HP"];
            float E_newHP = this.opponentController.StartingEnemy.Current_Pokemon.stats["HP"];

            this.gfx_controller.updateOnDamage(P_prevHP, P_prevHP, E_prevHP, E_newHP);
        }
        else if (Input.GetKeyDown(KeyCode.RightShift))
        {
            playerController.resetUI_State();
        }

        this.playerController.isUIActive = true;
    }
    private void START_playerAnim()
    {
        gfx_controller.Play_Pokemon_Anim(this.playerObject.GetComponent<Animator>(), this.playerController.StartingPlayer.Current_Pokemon.chosenAction);
        StartCoroutine(WaitForAnim(1f));
    }

    private void STOP_enemyAnim()
    {
        gfx_controller.Play_Pokemon_Anim(this.opponentObject.GetComponent<Animator>());
    }
    private void START_enemyTurn()
    {
        this.opponentController.StartingEnemy.Current_Pokemon.chosenAction = null;
        opponentController.StartingEnemy.Current_Pokemon.tryToRecoverCondition();
        opponentController.StartingEnemy.Current_Pokemon.applyConditions();
        opponentController.StartingEnemy.Current_Pokemon.dumpConditions();
        this.opponentController.player = this.playerController.StartingPlayer.Current_Pokemon;
        if (!opponentController.StartingEnemy.Current_Pokemon.canAttack)
        {
            this.CURRENT_STATE = BattleStates.ENEMY_ANIMATION;
            return;
        }

        this.playerController.isUIActive = false;
        this.opponentController.updatePercentages();
        
        float P_prevHP = this.playerController.StartingPlayer.Current_Pokemon.stats["HP"];
        float E_prevHP = this.opponentController.StartingEnemy.Current_Pokemon.stats["HP"];

        this.opponentController.doAction(out this.CURRENT_STATE);

        float P_newHP = this.playerController.StartingPlayer.Current_Pokemon.stats["HP"];
        float E_newHP = this.playerController.StartingPlayer.Current_Pokemon.stats["HP"];
        this.gfx_controller.updateOnDamage(P_prevHP, P_newHP, E_prevHP, E_prevHP);
    }
    private void START_enemyAnim()
    {
        gfx_controller.Play_Pokemon_Anim(this.opponentObject.GetComponent<Animator>(), this.opponentController.StartingEnemy.Current_Pokemon.chosenAction);
    }
    private void SHOW_displayText(Player player, AudioSource src)
    {
        gfx_controller.Display_Text(player.Current_Pokemon.actionToString(false), src);
    }
    private void SHOW_winScreen()
    {
        //TODO:
        Debug.Log("WIN");
    }
    private void SHOW_loseScreen()
    {
        //TODO:
        Debug.Log("LOSE");
    }

    /*
     * examineHP(player : Player, opponent : Player) : BattleStates
     * 
     * This method is responsible for determining whether a battle sequence has finished or not.
     * 
     * ## For now, let's only deal with one 1v1 battles -- no extra pokemon ##
     */
    private BattleStates examineHP(Pokemon player, Pokemon opponent)
    {
        if(player.isDead())
        {
            return BattleStates.LOSE;
        }
        else if(opponent.isDead())
        {
            return BattleStates.WIN;
        }

        return BattleStates.ENEMY_TEXT;
    }

    private void UI_control()
    {
        int x = playerController.currentPosition.x;
        int y = playerController.currentPosition.y;
        //Update Currently Selected UI Element:
        this.gfx_controller.UpdateSelectedUI_Element(x, y);

        if (Input.GetKeyDown(KeyCode.UpArrow) && playerController.isInBounds(x, y - 1))
        {
            playerController.currentPosition = new GridPos(x, y - 1);
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow) && playerController.isInBounds(x, y + 1))
        {
            playerController.currentPosition = new GridPos(x, y + 1);
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow) && playerController.isInBounds(x - 1, y))
        {
            playerController.currentPosition = new GridPos(x - 1, y);
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow) && playerController.isInBounds(x + 1, y))
        {
            playerController.currentPosition = new GridPos(x + 1, y);
        }
    }

    private IEnumerator WaitForAnim(float secs)
    {
        yield return new WaitForSeconds(secs);
        gfx_controller.Play_Pokemon_Anim(this.playerObject.GetComponent<Animator>());
    }

}
