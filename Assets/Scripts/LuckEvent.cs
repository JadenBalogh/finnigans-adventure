using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class LuckEvent : ScriptableObject
{
    public abstract void Invoke(Player source, Enemy target);
}
