using System;
using System.Collections;

using UnityEngine;
using UnityEngine.InputSystem;


public class PlayerCharacterController : PlayerCharacter
{
    private Rigidbody2D m_Physics;

    private SpriteRenderer m_Sprite;

    private Coroutine m_MoveCoroutine;

    private float m_SpeedRun = 1;
    private float m_Axis = 0;


    private bool m_IsWalk = false;
    private bool m_IsRun = false;
    private bool m_IsSit = false;
    private bool m_IsRest = false;
    private bool m_IsSleep = false;
    private bool m_IsJump = false;
    private bool m_IsGround = false;

    public override void Start()
    {
        base.Start();
        m_Sprite = GetComponent<SpriteRenderer>();
        m_Physics = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        OnAttack();
        OnJumpDown();
    }

    private void OnAttack()
    {
        Vector2 down = (Vector2.down / 4f);
        Vector2 start = m_Physics.position + (Vector2.left / 2f) + down;
        Vector2 end = m_Physics.position + (Vector2.right / 2f) + down;

        Debug.DrawLine(start, end, Color.blue);

        RaycastHit2D hit = Physics2D.Linecast(start, end);
        if (hit.collider == null) return;

        if (Input.GetMouseButtonDown(0))
        {
            bool isEnemy = hit.collider.TryGetComponent(out EnemyCharacterController enemy);
            if (!isEnemy) return;

            bool forwardAttack = m_Physics.position.x < enemy.transform.position.x && !m_Sprite.flipX || m_Physics.position.x > enemy.transform.position.x && m_Sprite.flipX;
            bool backAttack = m_Physics.position.x < enemy.transform.position.x && m_Sprite.flipX || m_Physics.position.x > enemy.transform.position.x && !m_Sprite.flipX;
            int damage = 1;

            if (AnimatorController.GetBool("Run"))
            {
                AnimatorController.SetTrigger("RunAttack");
                damage = 2;
            }
            else if (forwardAttack)
                AnimatorController.SetTrigger("ForwardAttack");
            else if (backAttack)
                AnimatorController.SetTrigger("BackAttack");
            enemy.TakeDamage(damage);

        }
    }

    private void OnJumpDown()
    {
        if (m_Physics.velocity.y < -0.1f)
        {
            Vector2 start = m_Physics.position + Vector2.down / 4f;
            Vector2 end = m_Physics.position + Vector2.down / 1.96f;

            Debug.DrawLine(start, end);
            m_IsJump = false;
            AnimatorController.SetBool("JumpUp", m_IsJump);

            RaycastHit2D hit = Physics2D.Linecast(start, end);
            m_IsGround = (hit.collider != null);

            AnimatorController.SetBool("Ground", m_IsGround);
        }
    }

    public void OnJump(InputAction.CallbackContext input)
    {
        if (input.action.IsPressed())
        {
            Vector2 start = m_Physics.position + Vector2.down / 4f;
            Vector2 end = m_Physics.position + Vector2.down / 1.96f;

            Debug.DrawLine(start, end);

            RaycastHit2D hit = Physics2D.Linecast(start, end, 5 << 6);
            if (hit.collider == null) return;

            bool isGround = hit.collider.TryGetComponent(out Ground ground);
            if (!isGround) return;
            m_IsJump = true;
            AnimatorController.SetBool("JumpUp", m_IsJump);
            AnimatorController.SetTrigger("Jump");
            AnimatorController.SetBool("Ground", false);
            m_Physics.AddForce(new Vector2(0, 200f));

        }
    }

    public void OnMove(InputAction.CallbackContext input)
    {
        if (input.action.IsPressed())
            m_Axis = input.ReadValue<float>();

        if (!m_IsSit)
        {

            m_IsWalk = input.action.IsPressed();
            AnimatorController.SetBool("Walk", m_IsWalk);

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
        m_SpeedRun = 1f + Convert.ToInt32(m_IsRun);
        AnimatorController.SetBool("Run", m_IsRun);

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
                    AnimatorController.SetBool("Sleep", m_IsSleep);
                    return;
                }

                m_IsRest = true;
                AnimatorController.SetBool("Rest", m_IsRest);
                return;
            }

            m_IsSit = true;
            AnimatorController.SetBool("Sit", m_IsSit);
            return;
        }
    }

    public void OnSitUp(InputAction.CallbackContext input)
    {

        m_IsSit = false;
        m_IsRest = false;
        m_IsSleep = false;
        AnimatorController.SetBool("Sit", m_IsSit);
        AnimatorController.SetBool("Rest", m_IsRest);
        AnimatorController.SetBool("Sleep", m_IsSleep);
    }

    private void PlayMove(float axis)
    {
        if (m_MoveCoroutine != null)
            StopCoroutine(m_MoveCoroutine);

        m_MoveCoroutine = StartCoroutine(OnMove(axis * m_SpeedRun));
    }

    private IEnumerator OnMove(float x)
    {
    PLAY:
        m_Physics.velocity = new Vector2(x, m_Physics.velocity.y);
        yield return null;
        goto PLAY;

    }
}
