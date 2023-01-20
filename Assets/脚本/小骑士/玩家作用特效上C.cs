using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class 玩家作用特效上C : MonoBehaviour
{
    [HideInInspector] public Animator 玩家作用特效动画;

    [SerializeField]
    [Tooltip("0.默认\r\n1.超冲蓄力亮光\r\n2.超冲开始飞行\r\n3.")]
    private float[] 数组_特效缩放值;


    [Header("动画参数")]
    private int 动画参数_超冲蓄力亮光特效;
    private int 动画参数_超冲开始飞行特效;
    private int 动画参数_超冲空中停止;

    // Start is called before the first frame update
    private void Start()
    {
        玩家作用特效动画 = GetComponent<Animator>();

        动画参数_超冲蓄力亮光特效 = Animator.StringToHash("超冲蓄力特效");
        动画参数_超冲开始飞行特效 = Animator.StringToHash("超冲开始飞行");
        动画参数_超冲空中停止 = Animator.StringToHash("超冲空中停止");
    }

    // Update is called once per frame
    private void Update()
    {
        
    }

    /// <summary>
    /// 参数为1-3,依次是超冲蓄力亮光、超冲空中停止、超冲开始飞行、
    /// </summary>
    /// <param name="i"></param>
    public void 激活能力特效(int i)
    {
        switch (i)
        {
            // 玩家特效作用位置.localScale = new Vector3(数组_特效缩放值[0], 数组_特效缩放值[0], 数组_特效缩放值[0]);
            case 1:
                transform.localScale = new Vector3(数组_特效缩放值[1], 数组_特效缩放值[1], 数组_特效缩放值[1]);
                玩家作用特效动画.SetTrigger(动画参数_超冲蓄力亮光特效);
                break;
            case 2:
                transform.localScale = new Vector3(数组_特效缩放值[0], 数组_特效缩放值[0], 数组_特效缩放值[0]);
                玩家作用特效动画.SetTrigger(动画参数_超冲空中停止);
                break;
            case 3:
                transform.localScale = new Vector3(数组_特效缩放值[2], 数组_特效缩放值[2], 数组_特效缩放值[2]);
                玩家作用特效动画.SetTrigger(动画参数_超冲开始飞行特效);
                break;
            default: break;
        }
    }
}
