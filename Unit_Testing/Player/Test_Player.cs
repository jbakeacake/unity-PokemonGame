using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using PlayerScripts.Player;
using PokemonScripts.Pokemon;
using Utilities.PokemonUtils;
using PlayerScripts.Actionable;

namespace Tests
{
    public class Test_Player
    {
        // A Test behaves as an ordinary method
        [Test]
        public void Test_PlayerSimplePasses()
        {


        }

        // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
        // `yield return null;` to skip a frame.
        [UnityTest]
        public IEnumerator Test_PlayerWithEnumeratorPasses()
        {
            // Use the Assert class to test conditions.
            // Use yield to skip a frame.
            yield return null;
        }
    }
}
