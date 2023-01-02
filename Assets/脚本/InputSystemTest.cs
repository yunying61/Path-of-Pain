using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.Interactions;

public class InputSystemTest : MonoBehaviour
{
    PlayerInputActions InputActions ;
    void Awake()
    {
        InputActions  = new PlayerInputActions();
        InputActions.GamePlay.Test.started += ctx =>
        {
            Debug.Log("started");
        };
        InputActions.GamePlay.Test.performed += ctx =>
        {
            Debug.Log("performed");
            if (ctx.interaction is MultiTapInteraction)
            {
                Debug.Log("执行双击逻辑");
            }
            else if (ctx.interaction is HoldInteraction)
            {
                Debug.Log("执行长按逻辑");
            }
            else if (ctx.interaction is PressInteraction)
            {
                Debug.Log("执行按下逻辑");
            }
            else
            {
                //列表中只有MultiTapInteraction和HoldInteraction对应的两种Interaction。
                //故不会走到这个else里。
            }
        };
        InputActions.GamePlay.Test.canceled += ctx =>
        {
            Debug.Log("canceled");
            if (ctx.interaction is MultiTapInteraction)
            {
                Debug.Log("执行点击逻辑");
            }
        };
        // 玩家控制器.GamePlay.Move.performed += ctx => 玩家位移向量 = ctx.ReadValue<Vector2>();       // 读取 位移向量
        // 玩家控制器.GamePlay.Move.canceled += ctx => 玩家位移向量 = Vector2.zero;                    // 在不动左摇杆的时候，不发生位移

        // 玩家控制器.GamePlay.Jump.started += ctx => 玩家跳跃();
        // // 玩家控制器.GamePlay.Jump.started += 跳跃_交互;
        // 玩家控制器.GamePlay.Jump.performed+= 跳跃_交互完成;
        // 玩家控制器.GamePlay.Jump.canceled+= 跳跃_交互取消;
    }

    private void OnEnable()
    {
        InputActions .GamePlay.Enable();
    }
    private void OnDisable()
    {
        InputActions .GamePlay.Disable();
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
