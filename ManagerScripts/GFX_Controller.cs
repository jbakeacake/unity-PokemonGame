using ManagerScripts.BattleManager.BattleStates;
using PlayerScripts.Actionable;
using PokemonScripts.Pokemon;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Animations;
using UnityEngine.UI;
using Utilities.PlayerUtils;

public class GFX_Controller : MonoBehaviour
{
    public GameObject playerObject, enemyObject;
    public GameObject DisplayBox_Container, DisplayText_Container, PlayerInfo_Container, EnemyInfo_Container;

    private PlayerController playerController;
    private EnemyController enemyController;

    private Text PlayerName_Text, EnemyName_Text, PlayerHP_Text, EnemyHP_Text;
    private Text[] Options;
    private Slider PlayerHP_Bar, EnemyHP_Bar;

    private void Start()
    {
        this.playerController = playerObject.GetComponent<PlayerController>();
        this.enemyController = enemyObject.GetComponent<EnemyController>();

        this.PlayerHP_Bar = this.PlayerInfo_Container.transform.GetChild(0).GetComponent<Slider>();
        this.EnemyHP_Bar = this.EnemyInfo_Container.transform.GetChild(0).GetComponent<Slider>();

        this.PlayerName_Text = this.PlayerInfo_Container.transform.GetChild(1).GetComponent<Text>();
        this.EnemyName_Text = this.EnemyInfo_Container.transform.GetChild(1).GetComponent<Text>();

        this.PlayerHP_Text = this.PlayerInfo_Container.transform.GetChild(2).GetComponent<Text>();
        this.EnemyHP_Text = this.EnemyInfo_Container.transform.GetChild(2).GetComponent<Text>();

        //TODO: make Options a 2D list:
        this.Options = Create_Options(4); // For the time being, lets limit the amount of options to 4 -- later we may want this to be a dynamic list

        //Update the UI given the current pokemon:
        this.UpdatePokemonInfo();
    }

    public void UpdatePokemonInfo()
    {
        float playerCurrHealth = this.playerController.StartingPlayer.Current_Pokemon.stats["HP"];
        float playerMaxHealth = this.playerController.StartingPlayer.Current_Pokemon.stats["MaxHP"];
        float enemyCurrHealth = this.enemyController.StartingEnemy.Current_Pokemon.stats["HP"];
        float enemyMaxHealth = this.enemyController.StartingEnemy.Current_Pokemon.stats["MaxHP"];

        this.PlayerHP_Bar.value = playerCurrHealth / playerMaxHealth;
        this.EnemyHP_Bar.value = enemyCurrHealth / enemyMaxHealth;

        this.PlayerHP_Text.text = ((int)playerCurrHealth) + "/" + ((int)playerMaxHealth);
        this.EnemyHP_Text.text = ((int)enemyCurrHealth) + "/" + ((int)enemyMaxHealth);

        this.PlayerName_Text.text = this.playerController.StartingPlayer.Current_Pokemon.name;
        this.EnemyName_Text.text = this.enemyController.StartingEnemy.Current_Pokemon.name;
    }

    public void updateOnDamage(float P_prevHP, float P_newHP, float E_prevHP, float E_newHP)
    {
        Pokemon Player_Pokemon = this.playerController.StartingPlayer.Current_Pokemon;
        Pokemon Enemy_Pokemon = this.playerController.StartingPlayer.Current_Pokemon;
        float P_prevPercent = P_prevHP / (float) Player_Pokemon.stats["MaxHP"];
        float P_newPercent = P_newHP / (float) Player_Pokemon.stats["MaxHP"];

        float E_prevPercent = E_prevHP / (float) Enemy_Pokemon.stats["MaxHP"];
        float E_newPercent = E_newHP / (float) Enemy_Pokemon.stats["MaxHP"];

        StartCoroutine(DecreaseHealth(this.PlayerHP_Bar, this.PlayerHP_Text, P_prevPercent, P_newPercent, Player_Pokemon.stats["MaxHP"]));
        StartCoroutine(DecreaseHealth(this.EnemyHP_Bar, this.EnemyHP_Text, E_prevPercent, E_newPercent, Enemy_Pokemon.stats["MaxHP"]));

        this.PlayerName_Text.text = this.playerController.StartingPlayer.Current_Pokemon.name;
        this.EnemyName_Text.text = this.enemyController.StartingEnemy.Current_Pokemon.name;
    }
    public void UpdateSelectedUI_Element(int x, int y)
    {
        if (x == 0 && y == 0)
        {
            this.Options[0].text = "> " + this.Options[0].text;
        }
        else if (x == 0 && y == 1)
        {
            this.Options[2].text = "> " + this.Options[2].text;
        }
        else if (x == 1 && y == 0)
        {
            this.Options[1].text = "> " + this.Options[1].text;
        }
        else if (x == 1 && y == 1)
        {
            this.Options[3].text = "> " + this.Options[3].text;
        }
    }

    public void SetUI_ElementNames()
    {
        string[,] elemNames = GetUI_ElementNames(this.playerController.UI_State);
        this.Options[0].text = elemNames[0, 0];
        this.Options[1].text = elemNames[1, 0];
        this.Options[2].text = elemNames[0, 1];
        this.Options[3].text = elemNames[1, 1];
    }

    private string[,] GetUI_ElementNames(Player_Action[,] options)
    {
        string[,] rtnList = new string[2, 2];
        for (int i = 0; i < rtnList.GetLength(0); i++)
        {
            for (int n = 0; n < rtnList.GetLength(1); n++)
            {
                if (options[i, n] == null)
                {
                    rtnList[i, n] = "[Empty]";
                }
                else
                {
                    rtnList[i, n] = options[i, n].name;
                }

            }
        }
        return rtnList;
    }

    private Text[] Create_Options(int optionLength)
    {
        Text[] rtnList = new Text[optionLength];
        for(int i = 0; i < rtnList.Length; i++)
        {
            rtnList[i] = this.DisplayBox_Container.transform.GetChild(i).GetComponent<Text>();
        }
        return rtnList;
    }

    public void Display_Text(string text, AudioSource src)
    {
        this.DisplayText_Container.SetActive(true);
        Text txt = this.DisplayText_Container.transform.GetChild(0).GetComponent<Text>();
        txt.text = text;
        StartCoroutine(TypeWrite(txt, src));
    }

    public void Hide_Text_Container()
    {
        this.DisplayText_Container.SetActive(false);
    }

    public void Play_Pokemon_Anim(Animator anim, Player_Action atk=null)
    {
        if(atk == null)
        {
            anim.Play(anim.runtimeAnimatorController.animationClips[0].name);
        }
        else
        {
            AnimationClip toPlay = null;
            foreach(AnimationClip clip in anim.runtimeAnimatorController.animationClips)
            {
                if(clip.name.Contains(atk.name))
                {
                    toPlay = clip;
                    break;
                }
            }
            if (toPlay == null) return;
            
            anim.Play(toPlay.name);
        }
    }

    public void Play_Trainer_Sound(AudioSource src)
    {
        if (src == null) return;
        src.Play();
    }


    //Coroutines:
    private IEnumerator TypeWrite(Text txt, AudioSource src)
    {
        string str = txt.text;
        string toDisplay = "";
        Play_Trainer_Sound(src);
        foreach (char c in str)
        {
            toDisplay += c;
            txt.text = toDisplay;
            yield return new WaitForSeconds(0.03f);
        }
    }

    private IEnumerator DecreaseHealth(Slider HP_Bar, Text HP_text, float initial, float end, int maxHP)
    {

        float HP_percent = initial;
        float HP_val = (int) ((HP_percent * 100) * maxHP) / 100;
        Image barFill = HP_Bar.transform.GetChild(1).transform.GetChild(0).GetComponent<Image>();
        while(HP_percent > ((float) end))
        {
            HP_text.text = ((int) HP_val) + "/" + ((int) maxHP);
            HP_Bar.value = (HP_percent);
            HP_val = (int)((HP_percent * 100) * maxHP) / 100;
            HP_percent -= 0.02f;
            barFill.color = new Color(barFill.color.r + 0.1f, barFill.color.g - 0.02f, barFill.color.b);
            yield return new WaitForSeconds(0.03f);
        }
    }
}
