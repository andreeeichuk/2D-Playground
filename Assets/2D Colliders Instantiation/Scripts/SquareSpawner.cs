using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SquareSpawner : MonoBehaviour
{
    public GameObject squarePrefab;

    private Vector2 maxPoints = new Vector2(20,20);
    private Vector2 minPoints = new Vector2(-20,-20);

    private int checkedSquare = 9;

    private void Start()
    {
        for (int i = 0; i < 10; i++)
        {
            SquareScript square = SpawnSquare();
            //if (i == checkedSquare)
            //    square.CountColliders();
        }
    }

    private SquareScript SpawnSquare()
    {
        float randomX = Random.Range(minPoints.x, maxPoints.x);
        float randomY = Random.Range(minPoints.y, maxPoints.y);

        GameObject square = Instantiate(squarePrefab, new Vector3(randomX, randomY), Quaternion.identity);

        return square.GetComponent<SquareScript>();
    }
}
