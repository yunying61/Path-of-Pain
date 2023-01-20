using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering.LookDev;
using UnityEngine;

public class 相机跟随 : MonoBehaviour
{
    [SerializeField] private Transform 跟随对象;            // Transform 类型的对象，这里用于获取玩家对象
    [SerializeField]
    [Range(0, 1)]
    private float 相机位移线性插值;        // 相机平滑位移

    [SerializeField] private Vector2 相机范围_左下角;      // 相机可移动的最小值（左下角）
    [SerializeField] private Vector2 相机范围_右上角;      // 相机可移动的最大值（右上角）

    private void LateUpdate()
    {
        if (跟随对象 != null)         // 玩家对象仍存在时的行为
        {
            if (transform.position != 跟随对象.position)      // 如果相机位置不在玩家位置时
            {
                Vector3 targetPos = 跟随对象.position;
                // 相机位置线性且平滑的移动到玩家位置
                targetPos.x = Mathf.Clamp(targetPos.x, 相机范围_左下角.x, 相机范围_右上角.x);       // 将相机的x轴移动范围限制在一定范围内
                targetPos.y = Mathf.Clamp(targetPos.y, 相机范围_左下角.y, 相机范围_右上角.y);       // 将相机的y轴移动范围限制在一定范围内
                targetPos.z = -10f;
                transform.position = Vector3.Lerp(transform.position, targetPos, 相机位移线性插值);        // 线性插值，用于镜头跟随玩家时的平滑移动
            }
        }
    }
}
