using System.Collections.Generic;

using UnityEditor.Animations;

using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    [HideInInspector]public Animator Controller;
    private AnimatorController m_Controller;
    public PlayerMotion Motion;
    private void Start()
    {
        Controller = GetComponent<Animator>();
        CreateAnimatorController();
    }

    private void CreateAnimatorController()
    {
        m_Controller = new AnimatorController();
        m_Controller.name = "Player";

        m_Controller.AddLayer("Layer");

        m_Controller.AddMotion(Motion.Idle);
        m_Controller.AddMotion(Motion.Walk);
        m_Controller.AddMotion(Motion.Run);
        m_Controller.AddMotion(Motion.JumpUp);
        m_Controller.AddMotion(Motion.JumpDown);
        m_Controller.AddMotion(Motion.Landing);
        m_Controller.AddMotion(Motion.BreakMove);

        m_Controller.AddMotion(Motion.ForwardAttack);
        m_Controller.AddMotion(Motion.BackAttack);
        m_Controller.AddMotion(Motion.WalkAttack);

        m_Controller.AddMotion(Motion.SitIdle);
        m_Controller.AddMotion(Motion.SitDown);
        m_Controller.AddMotion(Motion.SitUp);
        m_Controller.AddMotion(Motion.Rest);
        m_Controller.AddMotion(Motion.Sleep);

        Controller.runtimeAnimatorController = m_Controller;

    }
}
