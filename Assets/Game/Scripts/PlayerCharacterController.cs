using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

using UnityEditor.Timeline.Actions;

using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Composites;
using UnityEngine.InputSystem.Controls;

public class PlayerCharacterController : MonoBehaviour
{
    [SerializeField] private Rigidbody2D m_Physics;
    private SpriteRenderer m_Sprite;
    private Animator m_Animator;

    private Coroutine m_MoveCoroutine;

    private float m_SpeedRun = 1;
    private float m_Axis = 0;


    private bool m_IsWalk = false;
    private bool m_IsRun = false;
    private bool m_IsSit = false;
    private bool m_IsRest = false;
    private bool m_IsSleep = false;

    private void Start()
    {
        m_Sprite = GetComponent<SpriteRenderer>();
        m_Animator = GetComponent<Animator>();
    }

    private void Update()
    {
        OnAttack();
        OnJump();
    }

    private void OnAttack()
    {
        Vector2 start = m_Physics.position + Vector2.left / 2f;
        Vector2 end = m_Physics.position + Vector2.right / 2f;

        Debug.DrawLine(start + Vector2.down / 4f, end + Vector2.down / 4f);
        RaycastHit2D hit = Physics2D.Linecast(start, end);
        if (hit)
        {
            if (Input.GetMouseButtonDown(0))
            {
                bool isEnemy = hit.collider.TryGetComponent(out EnemyCharacterController enemy);
                if (isEnemy)
                {
                    bool forwardAttack = m_Physics.position.x < enemy.transform.position.x && !m_Sprite.flipX || m_Physics.position.x > enemy.transform.position.x && m_Sprite.flipX;
                    bool backAttack = m_Physics.position.x < enemy.transform.position.x && m_Sprite.flipX || m_Physics.position.x > enemy.transform.position.x && !m_Sprite.flipX;
                    int damage = 1;
                    if (m_Animator.GetBool("Run"))
                    {
                        m_Animator.SetTrigger("RunAttack");
                        damage = 2;
                    }
                    else
                    if (forwardAttack)
                        m_Animator.SetTrigger("ForwardAttack");
                    else if (backAttack)
                        m_Animator.SetTrigger("BackAttack");
                    enemy.TakeDamage(damage);
                }
            }
        }
    }

    private void OnJump()
    {
        Vector2 start = m_Physics.position + Vector2.down/4f;
        Vector2 end = m_Physics.position + Vector2.down / 2f;
        Debug.DrawLine(start, end);
    }

    public void OnMSove(InputAction.CallbackContext input)
    {
        if (input.action.IsPressed())
            m_Axis = input.ReadValue<float>();

        if (!m_IsSit)
        {

            m_IsWalk = input.action.IsPressed();
            m_Animator.SetBool("Walk", m_IsWalk);

            if (m_IsWalk)
                PlayMove(m_Axis);
            else
                StopAllCoroutines();

        }
        m_Sprite.flipX = m_Axis < 0;
    }

    public void OnRun(InputAction.CallbackContext input)
    {
        m_IsRun = input.action.IsPressed();
        m_SpeedRun = 1 + Convert.ToInt32(m_IsRun);
        m_Animator.SetBool("Run", m_IsRun);

        if (m_IsWalk)
            PlayMove(m_Axis);
    }

    public void OnSitDown(InputAction.CallbackContext input)
    {
        if (input.action.IsPressed() && !m_IsWalk)
        {
            if (m_IsSit)
            {
                if (m_IsRest)
                {
                    m_IsSleep = true;
                    m_Animator.SetBool("Sleep", m_IsSleep);
                    return;
                }

                m_IsRest = true;
                m_Animator.SetBool("Rest", m_IsRest);
                return;
            }

            m_IsSit = true;
            m_Animator.SetBool("Sit", m_IsSit);
            return;
        }
    }

    public void OnSitUp(InputAction.CallbackContext input)
    {

        m_IsSit = false;
        m_IsRest = false;
        m_IsSleep = false;
        m_Animator.SetBool("Sit", m_IsSit);
        m_Animator.SetBool("Rest", m_IsRest);
        m_Animator.SetBool("Sleep", m_IsSleep);
    }

    private void PlayMove(float axis)
    {
        if (m_MoveCoroutine != null)
            StopCoroutine(m_MoveCoroutine);

        m_MoveCoroutine = StartCoroutine(OnMove(new Vector2(axis * m_SpeedRun, 0)));
    }

    private IEnumerator OnMove(Vector2 velocity)
    {
    PLAY:
        m_Physics.velocity = velocity;
        yield return null;
        goto PLAY;

    }

    private void OnDrawGizmos()
    {
        //Debug.DrawLine(m_Physics.position, m_Physics.position + Vector2.right, Color.red);
    }
}
