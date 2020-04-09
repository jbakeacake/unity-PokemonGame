using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayerScripts.Player;
using UnityEngine.UI;
using ManagerScripts.BattleManager.BattleStates;
using PlayerScripts.Actionable;
using PlayerScripts.Actionable.Skill;
using PokemonScripts.Pokemon;


public class EnemyController : MonoBehaviour
{

    public Player StartingEnemy { get; set; }
    private float atkPercent, debuffPercent;
    public Pokemon player { get; set; }

    public GameObject enemy_src;
    public Clip_Switch Enemy_CS { get; set; }

    // Start is called before the first frame update
    void Awake()
    {
        this.StartingEnemy = ScriptableObject.CreateInstance<Player>();
        this.StartingEnemy.Current_Pokemon.name = "Ketchup Boi";
        this.Enemy_CS = enemy_src.GetComponent<Clip_Switch>();
        this.atkPercent = 0.0f;
        this.debuffPercent = 0.0f;
    }
    
    public void doAction(out BattleStates currentState)
    {
        float HP_Percent = StartingEnemy.Current_Pokemon.stats["HP"] / StartingEnemy.Current_Pokemon.stats["MaxHP"];

        System.Random rand = new System.Random();
        float randPercent = (float) rand.NextDouble();

        if(randPercent < this.atkPercent)
        {
            getRandomAttack(hasDamage: true).execute(player: this.StartingEnemy.Current_Pokemon, enemy: this.player, this.StartingEnemy.PokeList);
        }
        else if (randPercent < this.debuffPercent)
        {
            getRandomAttack(hasDebuff: true).execute(player: this.StartingEnemy.Current_Pokemon, enemy: this.player, this.StartingEnemy.PokeList);
        }

        currentState = BattleStates.ENEMY_TEXT;
    }
    private Attack getRandomAttack(bool hasDamage=false, bool hasDebuff=false)
    {
        List<Attack> possibleAttacks = new List<Attack>();
        for (int i = 0; i < this.StartingEnemy.Current_Pokemon.moveSet.Length; i++)
        {
            bool currentMoveHasDebuff = this.StartingEnemy.Current_Pokemon.moveSet[i].hasDebuff;
            bool currentMoveHasDamage = this.StartingEnemy.Current_Pokemon.moveSet[i].hasDamage;
            if ((currentMoveHasDamage && hasDamage) || (currentMoveHasDebuff && hasDebuff)) // if ((the current move has damage && we're looking for damage) OR (the current move has debuff && we're looking for debuffs)) then add to possible attacks
            {
                possibleAttacks.Add(this.StartingEnemy.Current_Pokemon.moveSet[i]);
            }
        }
        
        double possibleChances = 1.0 / ((double) possibleAttacks.Count);
        return ChooseRandomAttackFromPossible(possibleAttacks, possibleChances);
    }

    private Attack ChooseRandomAttackFromPossible(List<Attack> possibleAttacks, double possibleChances)
    {
        System.Random rand = new System.Random();
        double randomPercent = rand.NextDouble();
        for(int i = 0; i < possibleAttacks.Count; i++)
        {
            if(randomPercent <= possibleChances * ((double) i + 1.0))
            {
                Debug.Log("ENEMY USED " + possibleAttacks[i].name);
                this.StartingEnemy.Current_Pokemon.chosenAction = (Attack)possibleAttacks[i];
                return possibleAttacks[i];
            }
        }
        Debug.Log("ENEMY USED " + possibleAttacks[0].name);
        this.StartingEnemy.Current_Pokemon.chosenAction = (Attack)possibleAttacks[0];
        return possibleAttacks[0];
    }

    public void updatePercentages()
    {
        float HP_Percent = StartingEnemy.Current_Pokemon.stats["HP"] / StartingEnemy.Current_Pokemon.stats["MaxHP"];
        //Highest Priority will be attack:
        if (HP_Percent >= 0.9)
        {
            this.atkPercent = 0.75f;
            this.debuffPercent = 1.0f;
        }
        else if (HP_Percent >= 0.5 && HP_Percent < 0.9)
        {
            this.atkPercent = 0.50f;
            this.debuffPercent = 1.0f;
        }
        else
        {
            this.atkPercent = 0.0f;
            this.debuffPercent = 1.0f;
        }
    }
}
