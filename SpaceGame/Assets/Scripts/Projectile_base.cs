using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.Mathematics;
using Unity.VisualScripting;

//using System.Numerics;
using UnityEngine;

public class Projectile_base : MonoBehaviour
{
    public Quaternion spawnRotation = Quaternion.identity;
    private Renderer _renderer;
    private Rigidbody2D _rb;
    private List<int> ignoreCollision;// = new List<int>() { 6, 7, 9 };
    [SerializeField] private float damage = 5f;
    [SerializeField] private GameObject explosion;
    [SerializeField] private Collider2D coll;

    void Awake()
    {
        coll = gameObject.GetComponent<Collider2D>();
        ignoreCollision = new List<int>() { gameObject.layer };
    }
    // Start is called before the first frame update
    void Start()
    {
        //Set velocity
        _renderer = GetComponent<Renderer>();
        _rb = GetComponent<Rigidbody2D>();
        _rb.velocity = gameObject.transform.up * 10f;



        //Listen for events
        StoryEvents.removeObs += RemoveObs;
    }

    // Update is called once per frame
    void Update()
    {
        if (!_renderer.isVisible)
        {
            Destroy(this.gameObject);
        }
    }

    private void RemoveObs(object sender, EventArgs args)
    {
        StoryEvents.removeObs -= RemoveObs;
        Destroy(gameObject);
    }

    public void IgnoreCollision(List<int> ignore)
    {
        ignoreCollision.AddRange(ignore);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        //Ignore colliding with other projectiles on layers that it should ignore
        if (ignoreCollision.IndexOf(collision.gameObject.layer) != -1)
        {
            return;
        }


        IDamageable check = collision.GetComponent<IDamageable>();
        if (check != null)
        {
            check.Damage(damage);
        }
        GameObject temp = GameObject.Instantiate(explosion, transform.position, Quaternion.identity);
        Destroy(temp, temp.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).length);
        Destroy(this.gameObject);
    }

    void OnDestroy()
    {
        StoryEvents.removeObs -= RemoveObs;
    }
}
