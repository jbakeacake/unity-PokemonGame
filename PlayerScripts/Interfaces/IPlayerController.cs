using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPlayerController
{
    string[] getUIChoices();
    bool isUIActive { get; set; }

}
