using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorOpen : MonoBehaviour
{
    public Animation _animation;
    public BoxCollider boxCollider;

    void Start()
    {
        boxCollider = GetComponent<BoxCollider>();    
    }

    private void OnTriggerEnter(Collider other)
    {
        _animation.Play();
    }
}
