using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class 玩家超级冲刺 : MonoBehaviour
{
    private PlayerInputActions 玩家输入控制器;
    private Animator 玩家动画;
    private Rigidbody2D 玩家刚体;
    [SerializeField] private 玩家C 玩家控制器;
    [SerializeField] private 能力特效C 能力特效控制器;
    [SerializeField] private 玩家作用特效上C 玩家作用特效控制器;
    [SerializeField] private 玩家攻击 玩家攻击控制器;

    [SerializeField] private float 蓄力时间;
    [SerializeField] private float 冲刺速度;
    [SerializeField] private float 超冲判断延时 = 0.1f;
    [SerializeField] private float 超冲可中断延时 = 0.4f;
    [SerializeField] private float 超冲结束延时 = 0.1f;
    private float 当前蓄力时间 = 0f;

    private bool 判断_可以蓄力 = false;
    private bool 判断_可以超冲 = false;
    private bool 判断_超冲中 = false;

    [Header("动画参数")]
    private int 动画参数_超冲蓄力;
    private int 动画参数_超冲蓄力停止;
    private int 动画参数_超冲飞行;
    private int 动画参数_超冲撞墙;

    private void Awake()
    {
        玩家输入控制器 = new PlayerInputActions();

        玩家输入控制器.GamePlay.SDash.started += 超冲_按下;
        玩家输入控制器.GamePlay.SDash.canceled += 超冲_取消;

        玩家输入控制器.GamePlay.Move.started += ctx => 退出超级冲刺();
        玩家输入控制器.GamePlay.Jump.started += ctx => 退出超级冲刺();
    }

    private void 移动_交互中(InputAction.CallbackContext context)
    {
        if (context.ReadValue<Vector2>() != Vector2.zero && 玩家控制器.判断_可以移动)
        {
            退出超级冲刺();
        }
    }

    private void OnEnable()
    {
        玩家输入控制器.GamePlay.Enable();
    }
    private void OnDisable()
    {
        玩家输入控制器.GamePlay.Disable();
    }

    // Start is called before the first frame update
    private void Start()
    {
        玩家动画 = GetComponent<Animator>();
        玩家刚体 = GetComponent<Rigidbody2D>();

        动画参数_超冲蓄力 = Animator.StringToHash("超冲蓄力");
        动画参数_超冲蓄力停止 = Animator.StringToHash("超冲蓄力停止");
        动画参数_超冲飞行 = Animator.StringToHash("超冲飞行");
        动画参数_超冲撞墙 = Animator.StringToHash("超冲撞墙");
    }

    // Update is called once per frame
    private void Update()
    {
        超冲蓄力中();
        撞墙检测();
    }

    private void 超冲蓄力中()
    {
        if (判断_可以蓄力)
        {
            当前蓄力时间 += Time.deltaTime;
            if (当前蓄力时间 >= 蓄力时间)
            {
                判断_可以超冲 = true;
            }
            else
            {
                判断_可以超冲 = false;
            }
        }
        else
        {
            判断_可以超冲 = false;
            当前蓄力时间 = 0f;
        }
    }

    private void 超冲_按下(InputAction.CallbackContext context)
    {
        玩家动画.ResetTrigger(动画参数_超冲蓄力停止);
        能力特效控制器.玩家能力特效动画.ResetTrigger(动画参数_超冲蓄力停止);
        能力特效控制器.玩家能力特效动画.ResetTrigger("超冲蓄力特效");
        玩家作用特效控制器.玩家作用特效动画.ResetTrigger(动画参数_超冲蓄力停止);
        玩家作用特效控制器.玩家作用特效动画.ResetTrigger("超冲蓄力特效");

        if (判断_超冲中)
        {
            退出超级冲刺();
        }
        else if (玩家控制器.判断_地面检测 || 玩家控制器.判断_贴墙中)
        {
            玩家刚体.velocity = new Vector2(0.0f, 0.0f);
            玩家动画.SetTrigger(动画参数_超冲蓄力);
            能力特效控制器.激活能力特效(6);
            玩家作用特效控制器.激活能力特效(1);
            判断_可以蓄力 = true;
            玩家控制器.判断_可以移动 = false;
            玩家控制器.判断_可以跳跃 = false;
            玩家控制器.判断_可以冲刺 = false;
            玩家攻击控制器.判断_可以攻击 = false;
            玩家控制器.判断_可以扒墙下滑 = false;
            玩家控制器.重力开关 = false;
        }
    }

    private void 超冲_取消(InputAction.CallbackContext context)
    {
        玩家动画.ResetTrigger(动画参数_超冲蓄力);
        玩家动画.ResetTrigger(动画参数_超冲撞墙);
        判断_可以蓄力 = false;

        if (判断_可以超冲)
        {
            超级冲刺();
        }
        else
        {
            玩家控制器.重力开关 = true;
            玩家控制器.判断_可以移动 = true;
            玩家控制器.判断_可以跳跃 = true;
            玩家控制器.判断_可以冲刺 = true;
            玩家攻击控制器.判断_可以攻击 = true;
            玩家控制器.判断_可以扒墙下滑 = true;
            玩家动画.SetTrigger(动画参数_超冲蓄力停止);
            能力特效控制器.玩家能力特效动画.SetTrigger(动画参数_超冲蓄力停止);
            玩家作用特效控制器.玩家作用特效动画.SetTrigger(动画参数_超冲蓄力停止);   // 参数名一致，可共用
        }
    }

    private void 超级冲刺()
    {
        if (玩家控制器.判断_贴墙中 && !玩家控制器.判断_地面检测)
        {
            玩家刚体.velocity = 玩家控制器.判断_玩家朝向左 ? new Vector2(冲刺速度, 0.0f) : new Vector2(-冲刺速度, 0.0f);
        }
        else
        {
            玩家刚体.velocity = 玩家控制器.判断_玩家朝向左 ? new Vector2(-冲刺速度, 0.0f) : new Vector2(冲刺速度, 0.0f);
        }

        玩家动画.SetBool(动画参数_超冲飞行, true);
        玩家作用特效控制器.激活能力特效(3);
        能力特效控制器.激活能力特效(7);
        StartCoroutine("超冲判断延时协程");
    }

    private IEnumerator 超冲判断延时协程()
    {
        yield return new WaitForSeconds(超冲判断延时);
        判断_超冲中 = true;

        yield return new WaitForSeconds(超冲可中断延时);
        玩家控制器.判断_可以扒墙下滑 = true;
    }

    public void 退出超级冲刺()
    {
        if (判断_超冲中)
        {
            玩家动画.SetBool(动画参数_超冲飞行, false);
            能力特效控制器.激活能力特效(8);
            if (!玩家控制器.判断_贴墙中)
            {
                玩家作用特效控制器.激活能力特效(2);
            }
            判断_超冲中 = false;
            玩家刚体.velocity = new Vector2(0.0f, 0.0f);
            玩家控制器.判断_可以跳跃 = true;
            StartCoroutine("超冲结束延时协程");
        }
    }

    private IEnumerator 超冲结束延时协程()
    {
        yield return new WaitForSeconds(超冲结束延时);
        玩家控制器.重力开关 = true;
        玩家控制器.判断_可以移动 = true;
        玩家控制器.判断_可以冲刺 = true;
        玩家攻击控制器.判断_可以攻击 = true;
    }

    private void 撞墙检测()
    {
        if (判断_超冲中 && 玩家控制器.判断_贴墙中)
        {
            玩家动画.SetTrigger(动画参数_超冲撞墙);
            退出超级冲刺();
        }
    }
}
