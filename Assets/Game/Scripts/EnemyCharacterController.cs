using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class EnemyCharacterController : MonoBehaviour
{
    [SerializeField] private int m_Health;
    private Animator m_Animator;

    private void Start()
    {
        m_Animator = GetComponent<Animator>();
    }

    public void TakeDamage(int damage)
    {
        m_Health -= damage;
        if (m_Health <= 0)
        {
            m_Animator.SetBool("Death", true);
        }
        else
            m_Animator.SetTrigger("Damage");
    }

    private void Update()
    {

    }
}
