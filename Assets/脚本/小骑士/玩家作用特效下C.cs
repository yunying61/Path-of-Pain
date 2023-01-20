using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class 玩家作用特效下C : MonoBehaviour
{
    [HideInInspector] public Animator 玩家作用特效动画;
    private Transform 玩家特效作用位置;
    [SerializeField] private Transform 玩家位置;

    [SerializeField]
    [Tooltip("0.默认\r\n1.")]
    private float[] 数组_特效缩放值;


    [Header("动画参数")]
    private int 动画参数_蹬墙跳特效;

    // Start is called before the first frame update
    private void Start()
    {
        玩家作用特效动画 = GetComponent<Animator>();
        玩家特效作用位置 = GetComponent<Transform>();

        动画参数_蹬墙跳特效 = Animator.StringToHash("蹬墙跳特效");
    }

    // Update is called once per frame
    private void Update()
    {

    }

    /// <summary>
    /// 参数为1-3,依次是蹬墙跳、
    /// </summary>
    /// <param name="i"></param>
    public void 激活能力特效(int i)
    {
        玩家特效作用位置.position = 玩家位置.position;
        玩家特效作用位置.rotation = 玩家位置.rotation;
        switch (i)
        {
            // 玩家特效作用位置.localScale = new Vector3(数组_特效缩放值[0], 数组_特效缩放值[0], 数组_特效缩放值[0]);
            case 1:
                玩家特效作用位置.localScale = new Vector3(数组_特效缩放值[0], 数组_特效缩放值[0], 数组_特效缩放值[0]);
                玩家作用特效动画.SetTrigger(动画参数_蹬墙跳特效);
                break;
            default: break;
        }
    }
}
