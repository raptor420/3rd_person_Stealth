using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseClassSpotter : MonoBehaviour
{
    public  static event Action OnSpotPlayer;




    public abstract void PlayerSpotted();


    public virtual void OnPlayerSpotted()
    {

        OnSpotPlayer?.Invoke();
    }
}
