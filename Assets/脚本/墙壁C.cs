using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class 墙壁C : MonoBehaviour
{
    // Start is called before the first frame update
    private void Start()
    {

    }

    // Update is called once per frame
    private void Update()
    {

    }

    // 如果另一个碰撞器 2D 进入了触发器，则调用 OnTriggerEnter2D (仅限 2D 物理)
    //private void OnTriggerEnter2D(Collider2D collision)
    //{

    //}

    // 如果其他每个碰撞器 2D 接触触发器，OnTriggerStay2D 将在每一帧被调用一次(仅限 2D 物理)
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.TryGetComponent(out 玩家C 玩家控制器))
        {
            玩家控制器.判断_贴墙中 = true;

            if (玩家控制器.判断_下落检测 || 玩家控制器.判断_冲刺中 || 玩家控制器.判断_蹬墙跳中)
            {
                玩家控制器.判断_开始爬墙();
            }
        }
    }

    // 如果另一个碰撞器 2D 停止接触触发器，则调用 OnTriggerExit2D (仅限 2D 物理)
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.TryGetComponent(out 玩家C 玩家控制器))
        {
            玩家控制器.判断_退出爬墙();
        }
    }

}
