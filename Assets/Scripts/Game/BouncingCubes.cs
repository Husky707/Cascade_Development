using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BouncingCubes : MonoBehaviour
{

    [SerializeField] Rigidbody2D rb = null;
    [SerializeField] float force = 2f;
    [SerializeField] float chance = 5f;
    [SerializeField] float turn = 0.5f;


    private void LateUpdate()
    {
        if(UnityEngine.Random.value * 100f >= chance)
        {
            rb.AddForce(force * Vector2.up, ForceMode2D.Impulse);
            rb.AddTorque(turn);
        }
    }
}
