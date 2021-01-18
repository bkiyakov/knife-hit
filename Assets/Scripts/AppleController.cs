using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AppleController : MonoBehaviour
{
    public GameObject appleOneHalfPrefab;

    private SpriteRenderer sr;
    private Rigidbody2D rb;
    private Collider2D _collider;
    private ParticleSystem particle;
    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        _collider = GetComponent<Collider2D>();
        particle = GetComponent<ParticleSystem>();
        rb.bodyType = RigidbodyType2D.Kinematic;
    }

    private void Start()
    {
        GameManager.Instance.onWinTrigger += OnWin;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Knife"))
        {
            WasHitByKnife();
        }
    }

    private void WasHitByKnife()
    {
        _collider.enabled = false;
        sr.enabled = false;
        transform.parent = null;
        particle.Play();

        GameManager.Instance.onWinTrigger -= OnWin;
        Destroy(gameObject, 3f);
    }

    private void OnWin()
    {
        if(this != null)
        {
            _collider.enabled = false;
            particle.Stop();

            if (transform.parent != null)
            {
                transform.parent = null;
                rb.bodyType = RigidbodyType2D.Dynamic;
                rb.AddForce(transform.up * Random.Range(2f, 5f), ForceMode2D.Impulse);
                rb.AddTorque(Random.Range(-45f, 45f));
            }

            GameManager.Instance.onWinTrigger -= OnWin;
            Destroy(gameObject, 3f);
        }
    }
}
