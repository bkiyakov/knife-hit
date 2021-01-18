using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LogController : MonoBehaviour
{
    private GameObject applePrefab;
    private GameObject knifePrefab;
    private Transform centerRotator;
    private GameObject flasher;

    private LogRoute stageRoute;
    private float rotationSpeed = -1f;


    private int appleSpawnChance;
    private int knifeStartSpawnQuantityMin;
    private int knifeStartSpawnQuantityMax;
    private bool rotate;
    private ParticleSystem particle;
    private SpriteRenderer sr;
    private Collider2D _collider;

    private List<float> startItemAngles;

    private void Awake()
    {
        particle = GetComponent<ParticleSystem>();
        sr = GetComponent<SpriteRenderer>();
        _collider = GetComponent<Collider2D>();

        centerRotator = transform.GetChild(0);
        flasher = transform.GetChild(1).gameObject;

        startItemAngles = new List<float>();
        stageRoute = GameManager.Instance.currentStage.route;
    }
    private void Start()
    {
        GameManager.Instance.onWinTrigger += OnWin;
        GameManager.Instance.OnStartingNewGame += OnNewGameStart;

        applePrefab = GameManager.Instance.applePrefab;
        knifePrefab = GameManager.Instance.knifePrefab;

        SetStageData();

        rotate = true;
        StartCoroutine(RotateByRoute());
    }

    void FixedUpdate()
    {
        if(GameManager.Instance.onPlay && rotate)
        {
            transform.Rotate(0, 0, rotationSpeed * Time.fixedDeltaTime);
            sr.enabled = true;
        }
    }

    private IEnumerator RotateByRoute()
    {
        var templateIndex = 0;

        while (true)
        {
            var currentTemplate = stageRoute.templates[templateIndex];

            rotationSpeed = currentTemplate.speed;

            yield return new WaitForSeconds(currentTemplate.durationInSeconds);

            templateIndex++;

            if (templateIndex >= stageRoute.templates.Length)
                templateIndex = 0;
        }
    }
    private void SetStageData()
    {
        rotate = false;

        stageRoute = GameManager.Instance.currentStage.route;
        rotationSpeed = stageRoute.templates[0].speed;
        appleSpawnChance = GameManager.Instance.appleSpawnChance;
        knifeStartSpawnQuantityMin = GameManager.Instance.knifeStartSpawnQuantityMin;
        knifeStartSpawnQuantityMax = GameManager.Instance.knifeStartSpawnQuantityMax;
        particle.textureSheetAnimation.SetSprite(0, GameManager.Instance.currentStage.LogPartSprite);
        sr.sprite = GameManager.Instance.currentStage.LogSprite;
        flasher.GetComponent<SpriteMask>().sprite = GameManager.Instance.currentStage.LogSprite;

        // Настройки партикля для босса
        if (GameManager.Instance.currentStage.IsBoss)
        {
            var main = particle.main;
            var emission = particle.emission;

            main.gravityModifier = 0;
            main.startSize = new ParticleSystem.MinMaxCurve(0.2f, 0.7f);
            emission.SetBurst(0, new ParticleSystem.Burst(0, 15, 20, 1, 0.01f));
        }
            

        SpawnAppleWithChance(appleSpawnChance);
        SpawnStartKnifes();        
    }

    private void OnNewGameStart()
    {
        GameManager.Instance.onWinTrigger -= OnWin;
        GameManager.Instance.OnStartingNewGame -= OnNewGameStart;

        Destroy(gameObject);
    }
    private void OnWin()
    {
        rotate = false;

        sr.enabled = false;
        particle.Play();

        GameManager.Instance.onWinTrigger -= OnWin;
        GameManager.Instance.OnStartingNewGame -= OnNewGameStart;

        Destroy(gameObject, particle.main.startLifetime.constantMax);
    }

    public void Flash()
    {
        if(GameManager.Instance.knifeQuantity != 0)
            flasher.GetComponent<Animator>().SetTrigger("Flash");
    }

    private void SpawnAppleWithChance(int chanceInPercent)
    {
        bool needToSpawn = Random.Range(0, 101) <= chanceInPercent;

        if (needToSpawn)
        {
            var apple = Instantiate(applePrefab);

            FindPlaceAndSpawnGameobjectOnLog(apple);
        }
    }

    private void SpawnStartKnifes()
    {
        int knifeQuantity = Random.Range(knifeStartSpawnQuantityMin, knifeStartSpawnQuantityMax + 1);

        if(knifeQuantity > 0)
        {
            for(int i = 0; i < knifeQuantity; i++)
            {
                var knife = Instantiate(knifePrefab);
                knife.GetComponent<KnifeController>().enabled = false;
                knife.transform.Rotate(new Vector3(0, 0, 180));
                knife.transform.GetChild(0).gameObject.SetActive(false);
                FindPlaceAndSpawnGameobjectOnLog(knife);
            }
        }
    }

    private void FindPlaceAndSpawnGameobjectOnLog(GameObject gameObject)
    {
        transform.rotation = Quaternion.identity;

        centerRotator.transform.rotation = Quaternion.identity;

        gameObject.transform.parent = null;

        float maxAngleDifference = 15f;

        bool isFree = false;
        float spawnPositionDegree = 0;

        while (!isFree)
        {
            isFree = true;

            spawnPositionDegree = Random.Range(0, 360);

            if(startItemAngles.Count == 0)
            {
                break;
            }

            for(int i = 0; i < startItemAngles.Count; i++)
            {
                if(Mathf.Abs(startItemAngles[i] - spawnPositionDegree) <= maxAngleDifference
                    || Mathf.Abs(Mathf.Abs(startItemAngles[i] - spawnPositionDegree) - 360) <= maxAngleDifference)
                {
                    isFree = false;
                    break;
                }
            }
        }

        Debug.Log("Setting angle =" + spawnPositionDegree);


        centerRotator.Rotate(new Vector3(0, 0, spawnPositionDegree));
        gameObject.transform.parent = centerRotator;

        float offsetY = 0;
        var gameObjectCollider = gameObject.GetComponent<Collider2D>();

        if (gameObject.CompareTag("Apple"))
        {
            offsetY = _collider.bounds.size.y + gameObjectCollider.bounds.size.y * 0.75f;
        } else if (gameObject.CompareTag("Knife"))
        {
            offsetY = _collider.bounds.size.y + gameObjectCollider.bounds.size.y / 3.5f;
        }

        gameObject.transform.position = new Vector3(0, offsetY, 0);

        startItemAngles.Add(spawnPositionDegree);

        Debug.Log("Actual rotation: " + gameObject.transform.rotation);
    }
}
