using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnifeQuantity : MonoBehaviour
{
    public GameObject knifeIconPrefab;
    public GameObject knifeEmptyIconPrefab;

    private int currentknifeQuantity;

    private void Start()
    {
        InitKnifeIcons();

        GameManager.Instance.OnStartingNewGame += OnNewStageStarted;
        GameManager.Instance.onKnifeWasThrownTrigger += OnKnifeWasThrown;
        GameManager.Instance.onStartingNextStageTrigger += OnNewStageStarted;
    }

    private void InitKnifeIcons()
    {
        currentknifeQuantity = GameManager.Instance.knifeQuantity;

        var iconSizeY = knifeIconPrefab.GetComponentInChildren<SpriteMask>().bounds.size.y;

        for (int i = 0; i < currentknifeQuantity; i++)
        {
            var icon = Instantiate(knifeIconPrefab, transform);
            icon.transform.SetParent(transform);
            icon.transform.localPosition = new Vector3(0, i * iconSizeY, 0);
        }
    }

    private void OnKnifeWasThrown()
    {
        currentknifeQuantity = GameManager.Instance.knifeQuantity;
        Destroy(transform.GetChild(currentknifeQuantity).gameObject);

        var iconSizeY = knifeIconPrefab.GetComponentInChildren<SpriteMask>().bounds.size.y;

        var icon = Instantiate(knifeEmptyIconPrefab, transform);
        icon.transform.SetParent(transform);
        icon.transform.localPosition = new Vector3(0, currentknifeQuantity * iconSizeY, 0);
    }

    private void OnNewStageStarted()
    {
        for(int i = 0; i < transform.childCount; i++)
        {
            Destroy(transform.GetChild(i).gameObject);
        }

        InitKnifeIcons();
    }
}
