using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Data<T> where T : Data<T>, new()
{
    public static T GetInitialData()
    {
        T data = new T();

        return data.InitialData();
    }

    public abstract T InitialData();
}
