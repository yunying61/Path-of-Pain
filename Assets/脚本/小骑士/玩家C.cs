using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEditor.Experimental.AssetDatabaseExperimental.AssetDatabaseCounters;

namespace QFramework
{
    public class 玩家C : MonoBehaviour
    {
        #region 属性
        PlayerInputActions 玩家控制器;
        Rigidbody2D 玩家刚体;
        BoxCollider2D 碰撞体_玩家脚部;
        Animator 玩家动画;

        [Header("移动参数")]
        Vector2 玩家位移向量;
        [SerializeField]float 玩家移动速度;
        [SerializeField] float 最大垂直速度 = 10.0f;
        [SerializeField] float 跳跃力度;
        [SerializeField] float 二段跳力度;
        [SerializeField] float 地面重力比例=1.0f;
        [SerializeField] float 跳跃重力比例=0.0f;
        [SerializeField] float 下落重力比例=0.0f;

        bool 重力开关=true;
        private bool 判断_地面检测;        // 是否接触地面
        private bool 判断_玩家移动;
        private bool 判断_玩家朝向左;
        private bool 判断_可以跳跃;
        private bool 判断_跳跃中;
        private bool 判断_已经二段跳;
        private bool 判断_下落检测;
        private bool 冲刺输入开关;

        [Header("动画参数")]
        private int 动画参数_在地面上;
        private int 动画参数_移动中;
        private int 动画参数_移动量;
        private int 动画参数_朝向转换;
        private int 动画参数_跳跃;
        private int 动画参数_二段跳;
        private int 动画参数_下落中;

        #endregion

        void Awake()
        {
            玩家控制器 = new PlayerInputActions();
            玩家控制器.GamePlay.Move.performed += ctx => 玩家位移向量 = ctx.ReadValue<Vector2>();       // 读取 位移向量
            玩家控制器.GamePlay.Move.canceled += ctx => 玩家位移向量 = Vector2.zero;                    // 在不动左摇杆的时候，不发生位移

            // 玩家控制器.GamePlay.Jump.started += ctx => 玩家跳跃();
            玩家控制器.GamePlay.Jump.started += 跳跃_按下;
            玩家控制器.GamePlay.Jump.performed+= 跳跃_已执行;
            玩家控制器.GamePlay.Jump.canceled+= 跳跃_松开;
        }

        private void OnEnable()
        {
            玩家控制器.GamePlay.Enable();
        }
        private void OnDisable()
        {
            玩家控制器.GamePlay.Disable();
        }

        // Start is called before the first frame update
        void Start()
        {
            玩家刚体 = GetComponent<Rigidbody2D>();
            碰撞体_玩家脚部=GetComponent<BoxCollider2D>();
            玩家动画=GetComponent<Animator>();

            动画参数_在地面上 = Animator.StringToHash("判断_在地面上");
            动画参数_移动中 = Animator.StringToHash("判断_移动中");
            动画参数_移动量 = Animator.StringToHash("移动量");
            动画参数_朝向转换 = Animator.StringToHash("朝向转换");
            动画参数_跳跃 = Animator.StringToHash("跳跃");
            动画参数_二段跳 = Animator.StringToHash("二段跳");
            动画参数_下落中 = Animator.StringToHash("判断_下落中");
        }

        // Update is called once per frame
        void Update()
        {
            地面接触检测();
            下落检测();
            玩家移动();
            玩家转向();
            玩家跳跃();
            改变玩家重力();

        }

        void FixedUpdate()
        {
            
        }

        void 改变玩家重力()
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
        void 玩家移动()
        {
            判断_玩家移动 = Mathf.Abs(玩家刚体.velocity.x) > Mathf.Epsilon;

            Vector2 玩家速度=玩家刚体.velocity;
            if(玩家位移向量.x != 0){
                玩家速度.y = Mathf.Clamp(玩家速度.y,-最大垂直速度/2,最大垂直速度/2);
            }
            else{
                玩家速度.y = Mathf.Clamp(玩家速度.y,-最大垂直速度,最大垂直速度);
            }
            
            玩家刚体.velocity = new Vector2(玩家位移向量.x * 玩家移动速度, 玩家速度.y);
            玩家动画.SetInteger(动画参数_移动量, (int)玩家刚体.velocity.x);
        }

        void 玩家转向()
        {
            if (玩家位移向量.x > Mathf.Epsilon && 判断_玩家朝向左)    // 向右移动
            {
                判断_玩家朝向左 = false;
                transform.localRotation = Quaternion.Euler(0, 180, 0);
                if (判断_地面检测) { 玩家动画.SetTrigger(动画参数_朝向转换); }
            }
            else if (玩家位移向量.x < -Mathf.Epsilon && !判断_玩家朝向左)     // 向左移动
            {
                判断_玩家朝向左 = true;
                transform.localRotation = Quaternion.Euler(0, 0, 0);
                if (判断_地面检测) { 玩家动画.SetTrigger(动画参数_朝向转换); }
            }
        }

        void 玩家跳跃()
        {
            if (判断_地面检测) // && (判断_跳跃中 || 判断_已经二段跳), 如果已经落地了，则重置跳跃计数器
            {
                // 跳跃计数=0;
                判断_跳跃中 = false;
                判断_已经二段跳 = false;
            }
        }

        private void 跳跃_按下(InputAction.CallbackContext context)
        {
            if(判断_地面检测)
            {
                玩家刚体.velocity = new Vector2(0, 跳跃力度);
                // 玩家刚体.AddForce(new Vector2(0, 跳跃力度), ForceMode2D.Impulse);
                玩家动画.SetTrigger(动画参数_跳跃);
                判断_跳跃中 = true;
            }
            else if ((判断_跳跃中 || 玩家刚体.velocity.y != 0) && !判断_已经二段跳)
            {
                玩家刚体.velocity = new Vector2(0, 二段跳力度);
                // 玩家刚体.AddForce(new Vector2(0, 二段跳力度), ForceMode2D.Impulse);
                玩家动画.SetTrigger(动画参数_二段跳);
                判断_已经二段跳 = true;
            }
        }

        private void 跳跃_已执行(InputAction.CallbackContext context)
        {
            // JumpCancel();
        }

        private void 跳跃_松开(InputAction.CallbackContext context)
        {
            JumpCancel();
            if(玩家刚体.velocity.y > 0.0f)
            {
                玩家刚体.velocity= new Vector2(玩家刚体.velocity.x, 0.1f);
            }
            
            // 判断_可以跳跃 = false;
        }

        void JumpCancel()
        {
            // 判断_跳跃中 = false;
            // if (跳跃计数 ==1)
            {
                玩家动画.ResetTrigger(动画参数_跳跃);
            }
            // else if (跳跃计数 == 2)
            {
                玩家动画.ResetTrigger(动画参数_二段跳);
            }
        }
        #endregion

        void 地面接触检测()        // 地面接触检测
        {
            判断_地面检测 = 碰撞体_玩家脚部.IsTouchingLayers(LayerMask.GetMask("地面")) ||              // 检查 “碰撞体_玩家脚部” 碰撞器是否接触到 “地面” 图层蒙版上的任何碰撞器
                       碰撞体_玩家脚部.IsTouchingLayers(LayerMask.GetMask("MovingPlatform")) ||
                       碰撞体_玩家脚部.IsTouchingLayers(LayerMask.GetMask("DestructibleLayer")) ||
                       碰撞体_玩家脚部.IsTouchingLayers(LayerMask.GetMask("OneWayPlatform"));

            //isOneWayPlatform = 碰撞体_玩家脚部.IsTouchingLayers(LayerMask.GetMask("OneWayPlatform"));// 检查 碰撞体_玩家脚部 碰撞器是否接触到 OneWayPlatform 图层蒙版上的任何碰撞器

            玩家动画.SetBool(动画参数_在地面上,判断_地面检测);
        }

        void 下落检测()
        {
            if (玩家刚体.velocity.y < 0)
            {
                判断_下落检测 = true;
            }
            else
            {
                判断_下落检测 = false;
            }
            玩家动画.SetBool(动画参数_下落中, 判断_下落检测);
        }
    }
}
