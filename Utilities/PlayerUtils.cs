namespace Utilities.PlayerUtils
{
    using PlayerScripts.Player;
    using PokemonScripts.Pokemon;
    using System.Collections.Generic;

    public struct GridPos
    {
        public int x { get; set; }
        public int y { get; set; }

        public GridPos(int newX, int newY)
        {
            x = newX;
            y = newY;
        }
    }

    public static class UI_Util
    {
        public static PokeSwitch[] PokeListToAction(Player playerData)
        {
            PokeSwitch[] rtnList = new PokeSwitch[4]; // Will always be 4 until new UI is added
            int idx = 0;
            foreach(Pokemon p in playerData.PokeList)
            {
                rtnList[idx] = new PokeSwitch(p.name, playerData);
                idx++;
            }

            return rtnList;
        }
    }
}