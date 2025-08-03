using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelfDestruct : MonoBehaviour
{
    [SerializeField] private float timeBeforeSelfDestruct;

    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject, timeBeforeSelfDestruct);
    }
}
