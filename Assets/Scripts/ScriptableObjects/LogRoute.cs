using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
[CreateAssetMenu(fileName = "New Route", menuName = "LogRoute", order = 2)]
public class LogRoute : ScriptableObject
{
    public Template[] templates;
    [Serializable]
    public class Template
    {
        public float speed; // направление определяется скоростью
        public float durationInSeconds;
    }
}
