using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlayerScripts.Action.Attack_Elements
{
    public struct DamageThreshold
    {
        public int lower, upper;

        public DamageThreshold(int lower, int upper)
        {
            this.lower = lower;
            this.upper = upper;
        }
    }

}