using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SquareScript : MonoBehaviour
{
    public Collider2D myCollider;

    private void Awake()
    {
        CountColliders();
    }

    public void CountColliders()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 100f);

        Debug.Log("Colliders inside my circle:" + colliders.Length);
    }
}
