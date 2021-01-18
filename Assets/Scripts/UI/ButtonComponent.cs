using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ButtonComponent : MonoBehaviour
{
    private Button button;
    private void Awake()
    {
        button = GetComponent<Button>();
    }

    private void Start()
    {
        button.onClick.AddListener(PlayButtonSound);
    }

    private void PlayButtonSound()
    {
        AudioManager.Instance?.PlayButtonSound();
    }
}
