using System.Collections;
using System.Collections.Generic;

using Sirenix.OdinInspector;

using UnityEngine;


[CreateAssetMenu(fileName = "Motion", menuName = "GameData/Motion", order = 10)]
public class PlayerMotion : ScriptableObject
{
    [BoxGroup("Move")]
    public Motion Idle;
    [BoxGroup("Move")]
    public Motion Walk;
    [BoxGroup("Move")]
    public Motion Run;
    [BoxGroup("Move")]
    public Motion JumpUp;
    [BoxGroup("Move")]
    public Motion JumpDown;
    [BoxGroup("Move")]
    public Motion Landing;
    [BoxGroup("Move")]
    public Motion BreakMove;


    [BoxGroup("Attack")]
    public Motion ForwardAttack;
    [BoxGroup("Attack")]
    public Motion BackAttack;
    [BoxGroup("Attack")]
    public Motion WalkAttack;

    [BoxGroup("Sit")]
    public Motion SitIdle;
    [BoxGroup("Sit")]
    public Motion SitUp;
    [BoxGroup("Sit")]
    public Motion SitDown;
    [BoxGroup("Sit")]
    public Motion Rest;
    [BoxGroup("Sit")]
    public Motion Sleep;

    //public Motion
    //public Motion
    //public Motion

}
