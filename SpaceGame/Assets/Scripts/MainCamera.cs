using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCamera : MonoBehaviour
{
    [SerializeField] private Transform subject;
    private Transform defaultSubject;
    [SerializeField] private Vector3 offset = new Vector3(0, 0, -10f);
    private bool shouldTrack = true;
    // Start is called before the first frame update
    void Start()
    {
        defaultSubject = GameObject.FindWithTag("Player").transform;
    }

    // Update is called once per frame
    void Update()
    {
        if (shouldTrack)
        {
            transform.position = Vector3.MoveTowards(transform.position, subject.transform.position + offset, 10 * Time.deltaTime);
        }
    }

    public void setSubject(Transform s)
    {
        subject = s ?? defaultSubject;
    }


    public void ShouldTrack(bool track = true)
    {
        shouldTrack = track;
    }
}
