using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnifeController : MonoBehaviour
{
    public float speed = 20f;

    private bool winner;
    private SpriteRenderer sr;
    private Collider2D _collider;
    private Rigidbody2D rb;
    private ParticleSystem particle;
    private GameObject trail;
    public bool isMoving { get; private set; }


    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        _collider = GetComponent<Collider2D>();
        rb = GetComponent<Rigidbody2D>();
        particle = GetComponent<ParticleSystem>();
        trail = transform.GetChild(0).gameObject;
        rb.bodyType = RigidbodyType2D.Kinematic;
        winner = false;
        isMoving = false;

        GameManager.Instance.onWinTrigger += OnWin;
        GameManager.Instance.onLoseTrigger += OnLose;
        GameManager.Instance.OnStartingNewGame += OnNewGameStart;
    }
    private void FixedUpdate()
    {
        if (isMoving)
            Move();
    }

    public event System.Action<GameObject, GameObject> OnHittedSomething;
    public void Hitted(Collider2D collider)
    {
        if(OnHittedSomething != null)
        {
            OnHittedSomething(gameObject, collider.gameObject);
        }
    }
    public void Throw()
    {
        winner = true; // может стать победным, отменяется когда втыкается в бревно
        isMoving = true;
    }

    public void Move()
    {
        Vector2 newPosition = rb.position + new Vector2(0, speed) * Time.fixedDeltaTime;

        rb.MovePosition(newPosition);
    }
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isMoving)
        {
            Debug.Log("Hitted " + collision.tag);

            switch (collision.tag)
            {
                case "Apple":
                    Hitted(collision);
                    break;
                case "Log":
                    particle.Play();
                    Hitted(collision);
                    break;
                case "Knife":
                    FallDown();
                    Hitted(collision);
                    break;
                default:
                    break;
            }
        }
    }

    private void FallDown()
    {
        winner = false;
        isMoving = false;

        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.AddTorque(Random.Range(-360f, 360f));
    }

    public void AttachToTheLog(Transform log)
    {
        winner = false;
        isMoving = false;
        transform.position += new Vector3(0, _collider.bounds.size.y / 5f, 0);
        transform.parent = log;
        StartCoroutine(StopTrailing());
    }

    private IEnumerator StopTrailing()
    {
        yield return new WaitForSeconds(trail.GetComponent<TrailRenderer>().time);
        trail.SetActive(false);
    }

    private void OnWin()
    {
        if (!winner)
        {
            transform.parent = null;
            rb.bodyType = RigidbodyType2D.Dynamic;
            rb.AddForce(rb.position - new Vector2(1, 0), ForceMode2D.Impulse);
            rb.AddTorque(Random.Range(-180f, 180f));
        }

        _collider.enabled = false;

        GameManager.Instance.onWinTrigger -= OnWin;
        GameManager.Instance.onLoseTrigger -= OnLose;
        Destroy(gameObject, 3f);
    }

    private void OnLose()
    {
        GameManager.Instance.onWinTrigger -= OnWin;
        GameManager.Instance.onLoseTrigger -= OnLose;
        GameManager.Instance.OnStartingNewGame -= OnNewGameStart;
    }

    private void OnNewGameStart()
    {
        GameManager.Instance.onWinTrigger -= OnWin;
        GameManager.Instance.OnStartingNewGame -= OnLose;
        GameManager.Instance.OnStartingNewGame -= OnNewGameStart;

        if(this != null)
            Destroy(gameObject);
    }
}
