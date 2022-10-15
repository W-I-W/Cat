using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleport : MonoBehaviour
{
    [SerializeField] private Transform m_Spawn;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        collision.transform.position = m_Spawn.position;
    }
}
