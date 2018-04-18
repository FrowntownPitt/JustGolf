using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RTSDirector : MonoBehaviour
{
    public GameObject pixel;

    public int gridSize;

    // Place all of the destructible tiles
    void Start()
    {
        Transform tf = gameObject.transform;
        for (int i = 0; i < gridSize; i++)
        {
            for (int j = 0; j < gridSize; j++)
            {
                Instantiate(pixel, new Vector3(i * 5f, 0, -j * 5f) + tf.position, new Quaternion(0, 0, 0, 0), tf);
            }
        }
    }
}
