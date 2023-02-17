using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class 玩家C : MonoBehaviour
{
    #region 属性
    private PlayerInputActions 玩家输入控制器;
    private Rigidbody2D 玩家刚体;
    private CapsuleCollider2D 碰撞体_玩家脚部;

    private Animator 玩家动画;
    [SerializeField] private 能力特效C 能力特效控制器;
    [SerializeField] private 玩家作用特效下C 玩家作用特效下控制器;

    [Header("游戏参数")]
    private Vector2 玩家位移向量;
    [SerializeField] private float 玩家移动速度;
    [SerializeField] private float 最大垂直速度 = 16.0f;
    [SerializeField] private float 跳跃力度;
    [SerializeField] private float 蹬墙跳水平力度;
    [SerializeField] private float 蹬墙跳垂直力度;
    [SerializeField] private float 二段跳力度;
    [SerializeField] private float 地面重力比例 = 1.0f;
    [SerializeField] private float 跳跃重力比例 = 0.8f;
    [SerializeField] private float 下落重力比例 = 1.2f;
    [SerializeField] private float 冲刺时间;
    private float 冲刺_当前时间;
    [SerializeField] private float 冲刺冷却时间;
    private float 冲刺_当前冷却时间;
    [SerializeField] private float 冲刺速度;
    [SerializeField] private float 扒墙下滑速度;

    [Header("布尔参数")]
    public bool 判断_可以移动 = true;
    public bool 判断_可以跳跃 = true;
    public bool 判断_可以冲刺 = true;
    public bool 判断_可以扒墙下滑 = true;
    public bool 判断_玩家朝向左 = true;
    public bool 重力开关 = true;
    public bool 判断_地面检测;        // 是否接触地面
    public bool 判断_下落检测;
    public bool 判断_贴墙中 = false;
    [HideInInspector] public bool 判断_冲刺中 = false;
    [HideInInspector] public bool 判断_蹬墙跳中 = false;
    private bool 移动_发生过 = true;
    private bool 判断_跳跃中;
    private bool 判断_已经二段跳;
    private bool 冲刺输入开关 = true;
    private bool 冲刺限制开关 = false;
    private bool 判断_扒墙中 = false;

    [Header("动画参数")]
    private int 动画参数_在地面上;
    private int 动画参数_下落中;
    private int 动画参数_扒墙中;
    private int 动画参数_移动速度;
    private int 动画参数_朝向转换;
    private int 动画参数_跳跃;
    private int 动画参数_二段跳;
    private int 动画参数_冲刺;

    #endregion

    private void Awake()
    {
        玩家输入控制器 = new PlayerInputActions();
        玩家输入控制器.GamePlay.Move.performed += ctx => 玩家位移向量 = ctx.ReadValue<Vector2>();       // 读取 位移向量
        玩家输入控制器.GamePlay.Move.canceled += ctx => 玩家位移向量 = Vector2.zero;                    // 在不动左摇杆的时候，不发生位移

        玩家输入控制器.GamePlay.Jump.started += 跳跃_按下;
        //玩家输入控制器.GamePlay.Jump.performed += 跳跃_已执行;
        玩家输入控制器.GamePlay.Jump.canceled += 跳跃_松开;

        玩家输入控制器.GamePlay.Dash.started += 冲刺_按下;
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
        //Application.targetFrameRate = 60;
        玩家刚体 = GetComponent<Rigidbody2D>();
        碰撞体_玩家脚部 = GetComponent<CapsuleCollider2D>();
        玩家动画 = GetComponent<Animator>();

        动画参数_在地面上 = Animator.StringToHash("判断_在地面上");
        动画参数_扒墙中 = Animator.StringToHash("判断_扒墙中");
        动画参数_移动速度 = Animator.StringToHash("移动速度");
        动画参数_朝向转换 = Animator.StringToHash("朝向转换");
        动画参数_跳跃 = Animator.StringToHash("跳跃");
        动画参数_二段跳 = Animator.StringToHash("二段跳");
        动画参数_下落中 = Animator.StringToHash("判断_下落中");
        动画参数_冲刺 = Animator.StringToHash("冲刺");
    }

    // Update is called once per frame
    private void Update()
    {
        地面接触检测();
        下落检测();
        玩家转向();
        玩家冲刺限制();
    }

    private void FixedUpdate()
    {
        玩家移动();
        改变玩家重力();
        扒墙状态控制();
    }

    private void 改变玩家重力()
    {
        var gravityScale = 地面重力比例;

        if (!判断_地面检测)
        {
            gravityScale = 玩家刚体.velocity.y > 0.0f ? 跳跃重力比例 : 下落重力比例;
        }

        if (!重力开关)
        {
            gravityScale = 0;
        }

        玩家刚体.gravityScale = gravityScale;
    }

    #region  移动、跳跃方法
    private void 玩家移动()
    {
        Vector2 玩家速度 = 玩家刚体.velocity;
        if (玩家位移向量.x != 0)
        {
            玩家速度.y = Mathf.Clamp(玩家速度.y, -最大垂直速度 / 2, 最大垂直速度 / 2);
        }
        else
        {
            玩家速度.y = Mathf.Clamp(玩家速度.y, -最大垂直速度, 最大垂直速度);
        }

        if (!判断_冲刺中 && 判断_可以移动)
        {
            //玩家刚体.velocity = new Vector2(玩家位移向量.x * 玩家移动速度, 玩家速度.y);
            #region  移动赋值分离
            if (玩家位移向量.x != 0)
            {
                玩家刚体.velocity = new Vector2(玩家位移向量.x * 玩家移动速度, 玩家速度.y);
                移动_发生过 = true;
            }
            else if (玩家位移向量.x == 0 && 移动_发生过)
            {
                玩家刚体.velocity = new Vector2(0.0f, 玩家速度.y);
                移动_发生过 = false;
            }
            #endregion

            玩家动画.SetInteger(动画参数_移动速度, (int)玩家刚体.velocity.x);// 阶梯速度
        }
    }

    private void 玩家转向()
    {
        if (玩家刚体.velocity.x > Mathf.Epsilon && 判断_玩家朝向左)    // 向右移动
        {
            判断_玩家朝向左 = false;
            transform.localRotation = Quaternion.Euler(0, 180, 0);
            if (判断_地面检测) { 玩家动画.SetTrigger(动画参数_朝向转换); }
        }
        else if (玩家刚体.velocity.x < -Mathf.Epsilon && !判断_玩家朝向左)     // 向左移动
        {
            判断_玩家朝向左 = true;
            transform.localRotation = Quaternion.Euler(0, 0, 0);
            if (判断_地面检测) { 玩家动画.SetTrigger(动画参数_朝向转换); }
        }
    }

    private void 跳跃_按下(InputAction.CallbackContext context)
    {
        if (判断_可以跳跃)
        {
            if (判断_地面检测)
            {
                玩家刚体.velocity = new Vector2(玩家刚体.velocity.x, 跳跃力度);
                //玩家刚体.AddForce(new Vector2(0, 跳跃力度), ForceMode2D.Impulse);
                玩家动画.SetBool(动画参数_在地面上, 判断_地面检测);
                玩家动画.SetTrigger(动画参数_跳跃);
                判断_跳跃中 = true;
            }
            else if (判断_扒墙中)
            {
                判断_扒墙中 = false;
                判断_可以移动 = false;
                判断_蹬墙跳中 = true;
                玩家刚体.velocity = 判断_玩家朝向左 ? new Vector2(蹬墙跳水平力度, 蹬墙跳垂直力度) : new Vector2(-蹬墙跳水平力度, 蹬墙跳垂直力度);
                玩家动画.SetTrigger(动画参数_跳跃);
                StartCoroutine(协程_蹬墙跳向上力());
                玩家作用特效下控制器.激活能力特效(1);
            }
            else if ((判断_跳跃中 || !判断_地面检测) && !判断_已经二段跳)
            {
                玩家刚体.velocity = new Vector2(玩家刚体.velocity.x, 二段跳力度);
                玩家动画.SetTrigger(动画参数_二段跳);
                能力特效控制器.激活能力特效(1);
                判断_跳跃中 = true;
                判断_已经二段跳 = true;
            }
        }
    }

    private IEnumerator 协程_蹬墙跳向上力()
    {
        yield return new WaitForSeconds(0.2f);
        判断_可以移动 = true;
        yield return new WaitForSeconds(0.1f);
        玩家刚体.velocity = new Vector2(0f, 蹬墙跳垂直力度 - 1f);
    }

    //private void 跳跃_已执行(InputAction.CallbackContext context)
    //{
    //    // JumpCancel();
    //}

    private void 跳跃_松开(InputAction.CallbackContext context)
    {
        判断_可以扒墙下滑 = true;
        JumpCancel();
        if (玩家刚体.velocity.y > 0.0f && 判断_跳跃中)
        {
            玩家刚体.velocity = new Vector2(0.0f, 0.1f);
            判断_跳跃中 = false;
        }

        // 判断_可以跳跃 = false;
    }

    private void JumpCancel()
    {
        玩家动画.ResetTrigger(动画参数_跳跃);
        玩家动画.ResetTrigger(动画参数_二段跳);
    }
    #endregion

    #region 地面和下落检测
    private void 地面接触检测()        // 地面接触检测
    {
        判断_地面检测 = 碰撞体_玩家脚部.IsTouchingLayers(LayerMask.GetMask("可站立图层_地面")) ||              // 检查 “碰撞体_玩家脚部” 碰撞器是否接触到 “地面” 图层蒙版上的任何碰撞器
                   碰撞体_玩家脚部.IsTouchingLayers(LayerMask.GetMask("MovingPlatform")) ||
                   碰撞体_玩家脚部.IsTouchingLayers(LayerMask.GetMask("DestructibleLayer")) ||
                   碰撞体_玩家脚部.IsTouchingLayers(LayerMask.GetMask("OneWayPlatform"));

        //isOneWayPlatform = 碰撞体_玩家脚部.IsTouchingLayers(LayerMask.GetMask("OneWayPlatform"));// 检查 碰撞体_玩家脚部 碰撞器是否接触到 OneWayPlatform 图层蒙版上的任何碰撞器

        玩家动画.SetBool(动画参数_在地面上, 判断_地面检测);

        if (判断_地面检测)
        {
            判断_已经二段跳 = false;
            判断_蹬墙跳中 = false;
            判断_可以扒墙下滑 = false;
        }
    }

    private void 下落检测()
    {
        if (玩家刚体.velocity.y < 0)
        {
            判断_下落检测 = true;
            判断_可以扒墙下滑 = true;
        }
        else
        {
            判断_下落检测 = false;
        }
        玩家动画.SetBool(动画参数_下落中, 判断_下落检测);
    }

    private void 扒墙状态控制()
    {
        if (判断_扒墙中)
        {
            玩家刚体.velocity = 判断_可以扒墙下滑 ? new Vector2(玩家刚体.velocity.x, 扒墙下滑速度) : new Vector2(玩家刚体.velocity.x, 0f);
        }
    }
    #endregion

    #region 冲刺方法
    private void 玩家冲刺限制()
    {
        if (判断_地面检测)
        {
            冲刺限制开关 = false;
        }
    }

    private void 冲刺_按下(InputAction.CallbackContext context)
    {
        if (判断_可以冲刺 && 冲刺输入开关 && !冲刺限制开关)
        {
            冲刺限制开关 = true;
            StartCoroutine(冲刺协程(冲刺时间));
        }
    }

    private IEnumerator 冲刺协程(float 冲刺过程时间)
    {
        冲刺输入开关 = false;
        重力开关 = false;
        判断_冲刺中 = true;
        玩家刚体.velocity = 判断_玩家朝向左 ? new Vector2(-冲刺速度, 0.0f) : new Vector2(冲刺速度, 0.0f);
        玩家动画.SetTrigger(动画参数_冲刺);
        能力特效控制器.激活能力特效(2);
        yield return new WaitForSeconds(冲刺过程时间);
        玩家动画.ResetTrigger(动画参数_冲刺);
        判断_冲刺中 = false;
        重力开关 = true;
        玩家刚体.velocity = new Vector2(0.0f, 0.0f);
        yield return new WaitForSeconds(冲刺冷却时间);
        冲刺输入开关 = true;
    }
    #endregion

    public void 判断_开始爬墙()
    {
        判断_扒墙中 = true;
        判断_已经二段跳 = false;
        冲刺限制开关 = false;
        玩家动画.SetBool(动画参数_扒墙中, true);
    }

    public void 判断_退出爬墙()
    {
        判断_贴墙中 = false;
        玩家动画.SetBool(动画参数_扒墙中, false);
        判断_扒墙中 = false;
    }
}

