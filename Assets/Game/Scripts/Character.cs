using System.Collections;
using System.Collections.Generic;

using UnityEngine;
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Collider2D))]
public class Character : MonoBehaviour, IHealth
{
    protected Animator AnimatorController;
    protected Collider2D RootCollider;
    [SerializeField] private int m_Health = 10;
    public int Health { get => m_Health; set => m_Health = value; }


    public virtual void Start()
    {
        AnimatorController = GetComponent<Animator>();
        RootCollider = GetComponent<Collider2D>();
    }

    public virtual void TakeDamage(int damage)
    {
        Health -= damage;
        if (Health <= 0)
        {
            AnimatorController.SetBool("Death", true);
            RootCollider.enabled = false;
        }
        else
            AnimatorController.SetTrigger("Damage");
    }
}
