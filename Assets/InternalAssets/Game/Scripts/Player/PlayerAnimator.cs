using System.Collections.Generic;

using UnityEditor.Animations;

using UnityEngine;

[RequireComponent(typeof(PlayerCharacterController))]
public class PlayerAnimator : MonoBehaviour
{
    private PlayerCharacterController m_Player;
    private AnimatorController m_Controller;
    [SerializeField] private Motion motion;
    private void Start()
    {
        m_Player = GetComponent<PlayerCharacterController>();

        CreateAnimatorController();
    }

    private void CreateAnimatorController()
    {
        m_Controller = new AnimatorController();
        m_Controller.name = "Player";

        m_Controller.AddMotion(motion);
    }
}
