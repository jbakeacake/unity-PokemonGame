using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities.PokemonUtils;

public interface IPokemon
{
    bool isDead();
    string actionToString(bool isSuperEffective);
    void modifyHealth(int value, PokeType attackerType);

}
