using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Toggle))]
public class ToggleComponent : MonoBehaviour
{
    private Toggle toogle;
    private void Awake()
    {
        toogle = GetComponent<Toggle>();
    }
    private void Start()
    {
        toogle.onValueChanged.AddListener(PlayButtonSound);
    }

    private void PlayButtonSound(bool arg0)
    {
        AudioManager.Instance?.PlayButtonSound();
    }
}
