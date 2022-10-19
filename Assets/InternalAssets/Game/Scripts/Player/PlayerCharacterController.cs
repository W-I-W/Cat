using System;
using System.Threading.Tasks;
using System.Collections;

using UnityEngine;
using UnityEngine.InputSystem;
using System.Threading;
using System.Collections.Generic;

[RequireComponent(typeof(PlayerAnimator))]
public class PlayerCharacterController : PlayerCharacter
{
    private Rigidbody2D m_Physics;

    private SpriteRenderer m_Sprite;
    private PlayerMotion m_Motion;

    private Coroutine m_MoveCoroutine;

    private float m_SpeedRun = 1;
    private float m_Axis = 0;

    private Motion m_NextAnimation;


    private bool m_IsWalk = false;
    private bool m_IsRun = false;
    private bool m_IsSit = false;
    private bool m_IsRest = false;
    private bool m_IsSleep = false;
    //private bool m_IsJump = false;
    private bool m_IsGround = false;
    private bool m_IsLanding = false;

    public override void Start()
    {
        base.Start();

        m_Motion = GetComponent<PlayerAnimator>().Motion;
        m_Sprite = GetComponent<SpriteRenderer>();
        m_Physics = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        //OnAttack();
        m_IsGround = IsGround();
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

            if (AnimatorController.GetCurrentAnimatorStateInfo(0).IsName(m_Motion.Walk.name))
            {
                //AnimatorController.SetTrigger("RunAttack");
                AnimatorController.Play(m_Motion.WalkAttack.name);
                damage = 2;
            }
            else if (forwardAttack)
                AnimatorController.Play(m_Motion.ForwardAttack.name);
            //AnimatorController.SetTrigger("ForwardAttack");
            else if (backAttack)
                AnimatorController.Play(m_Motion.BackAttack.name);
            //AnimatorController.SetTrigger("BackAttack");
            enemy.TakeDamage(damage);

        }
    }

    private bool IsGround()
    {
        Vector2 start = m_Physics.position + Vector2.down / 4f;
        Vector2 end = m_Physics.position + Vector2.down / 1.96f;
        Debug.DrawLine(start, end);
        RaycastHit2D hit = Physics2D.Linecast(start, end);
        return (hit.collider != null);

    }

    private void OnJumpDown()
    {
        if (m_Physics.velocity.y < -0.1f)
        {
            AnimatorStateInfo info = AnimatorController.GetCurrentAnimatorStateInfo(0);
            if (info.IsName(m_Motion.JumpUp.name))
            {
                AnimatorController.Play(m_Motion.JumpDown.name);
            }
            else if (m_IsGround && info.IsName(m_Motion.JumpDown.name))
                StartCoroutine(SleepAnimation(m_Motion.Landing, m_NextAnimation));
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
            // m_IsJump = true;
            AnimatorController.Play(m_Motion.JumpUp.name);
            m_Physics.AddForce(new Vector2(0, 200f));

        }
    }

    public void OnMove(InputAction.CallbackContext input)
    {
        m_Axis = input.ReadValue<float>();


        if (!m_IsSit)
        {
            m_IsWalk = input.action.IsPressed();
            m_NextAnimation = m_Motion.Walk;
            if (m_IsWalk)
                PlayMove(m_Axis);
            else
                StopAllCoroutines();

        }
        if (input.action.IsPressed())
            m_Sprite.flipX = m_Axis < 0;
        else
        {
            m_NextAnimation = m_Motion.Idle;
        }
        AnimatorController.Play(m_NextAnimation.name);
        AnimatorController.CrossFade(m_NextAnimation.name);

    }

    public void OnRun(InputAction.CallbackContext input)
    {
        if (input.control.IsPressed())
        {
            m_IsRun = input.action.IsPressed();
            m_SpeedRun = 1f + Convert.ToInt32(m_IsRun);
            //AnimatorController.SetBool("Run", m_IsRun);
            //m_NextAnimation = m_Motion.Run;
            AnimatorController.Play(m_Motion.Run.name);
            AnimatorController.CrossFade();

            if (m_IsWalk)
                PlayMove(m_Axis);
        }
        else
        {
            AnimatorController.Play(m_NextAnimation.name);
            m_SpeedRun = 0;
        }

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
                    AnimatorController.Play(m_Motion.Sleep.name);
                    //AnimatorController.SetBool("Sleep", m_IsSleep);
                    return;
                }

                m_IsRest = true;
                AnimatorController.Play(m_Motion.Rest.name);
                //AnimatorController.SetBool("Rest", m_IsRest);
                return;
            }

            m_IsSit = true;
            StartCoroutine(SleepAnimation(m_Motion.SitDown, m_Motion.SitIdle));
            //AnimatorController.SetBool("Sit", m_IsSit);
            return;
        }
    }


    public void OnSitUp(InputAction.CallbackContext input)
    {

        m_IsSit = false;
        m_IsRest = false;
        m_IsSleep = false;
        StartCoroutine(SleepAnimation(m_Motion.SitUp, m_Motion.Idle));
        //AnimatorController.SetBool("Sit", m_IsSit);
        //AnimatorController.SetBool("Rest", m_IsRest);
        //AnimatorController.SetBool("Sleep", m_IsSleep);
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

    private IEnumerator SleepAnimation(Motion begin, Motion end)
    {
        AnimatorController.Play(begin.name);
        yield return null;
        AnimatorStateInfo info = AnimatorController.GetCurrentAnimatorStateInfo(0);
        yield return new WaitForSeconds(info.length);
        AnimatorController.Play(end.name);
    }
}
