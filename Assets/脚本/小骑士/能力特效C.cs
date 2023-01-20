using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class 能力特效C : MonoBehaviour
{
    [HideInInspector] public Animator 玩家能力特效动画;

    [Header("动画参数")]
    private int 动画参数_二段跳特效;
    private int 动画参数_冲刺特效;
    private int 动画参数_平砍特效;
    private int 动画参数_上劈特效;
    private int 动画参数_下劈特效;
    private int 动画参数_超冲蓄力特效;
    private int 动画参数_超冲开始飞行特效;
    private int 动画参数_超冲结束飞行特效;

    private void Start()
    {
        玩家能力特效动画 = GetComponent<Animator>();

        动画参数_二段跳特效 = Animator.StringToHash("二段跳特效");
        动画参数_冲刺特效 = Animator.StringToHash("冲刺特效");
        动画参数_平砍特效 = Animator.StringToHash("平砍特效");
        动画参数_上劈特效 = Animator.StringToHash("上劈特效");
        动画参数_下劈特效 = Animator.StringToHash("下劈特效");
        动画参数_超冲蓄力特效 = Animator.StringToHash("超冲蓄力特效");
        动画参数_超冲开始飞行特效 = Animator.StringToHash("超冲开始飞行");
        动画参数_超冲结束飞行特效 = Animator.StringToHash("超冲结束飞行");
    }

    /// <summary>
    /// 触发特效动画，参数范围为1-9
    /// <para>
    /// 1-5，二段跳、冲刺、平砍、上劈、下劈、
    /// </para>
    /// <para>
    /// 6-9，超冲蓄力气旋、超冲开始飞行、超冲结束飞行
    /// </para>
    /// </summary>
    /// <param name="i">范围:1-9</param>
    public void 激活能力特效(int i)
    {
        switch (i)
        {
            case 1:
                玩家能力特效动画.SetTrigger(动画参数_二段跳特效);
                break;
            case 2:
                玩家能力特效动画.SetTrigger(动画参数_冲刺特效);
                break;
            case 3:
                玩家能力特效动画.SetTrigger(动画参数_平砍特效);
                break;
            case 4:
                玩家能力特效动画.SetTrigger(动画参数_上劈特效);
                break;
            case 5:
                玩家能力特效动画.SetTrigger(动画参数_下劈特效);
                break;
            case 6:
                玩家能力特效动画.SetTrigger(动画参数_超冲蓄力特效);
                break;
            case 7:
                玩家能力特效动画.SetTrigger(动画参数_超冲开始飞行特效);
                break;
            case 8:
                玩家能力特效动画.SetTrigger(动画参数_超冲结束飞行特效);
                break;
            default:break;
        }
    }
}
