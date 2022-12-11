using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class destroyOnTime : MonoBehaviour
{
    public float timeToDestroy = .1f;

    private void Awake()
    {
        Destroy(gameObject, timeToDestroy);
    }
}
