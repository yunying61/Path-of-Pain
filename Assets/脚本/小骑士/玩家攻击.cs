using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class 玩家攻击 : MonoBehaviour
{
    private PlayerInputActions 玩家输入控制器;
    private Animator 玩家动画;
    [SerializeField] private 能力特效C 能力特效控制器;
    [SerializeField] private 玩家C 玩家控制器;
    [SerializeField] private PolygonCollider2D 平砍攻击碰撞框;
    [SerializeField] private PolygonCollider2D 上劈攻击碰撞框;
    [SerializeField] private PolygonCollider2D 下劈攻击碰撞框;

    [SerializeField] private int 攻击力;
    [SerializeField] private float 攻击框延时;
    [SerializeField] private float 攻击框延时关闭;
    [SerializeField] private float 攻击冷却时间;
    private Vector2 玩家位移向量;

    public bool 判断_可以攻击 = true;
    private bool 攻击输入开关 = true;
    private bool 判断_下劈 = false;
    private bool 判断_上劈 = false;

    [Header("动画参数")]
    private int 动画参数_平砍;
    private int 动画参数_上劈;
    private int 动画参数_下劈;

    private void Awake()
    {
        玩家输入控制器 = new PlayerInputActions();

        玩家输入控制器.GamePlay.Attack.started += 攻击_按下;

        玩家输入控制器.GamePlay.Move.performed += ctx => 玩家位移向量 = ctx.ReadValue<Vector2>();
        玩家输入控制器.GamePlay.Move.canceled += ctx => 玩家位移向量 = Vector2.zero;
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
    void Start()
    {
        玩家动画 = GetComponent<Animator>();

        动画参数_平砍 = Animator.StringToHash("平砍");
        动画参数_上劈 = Animator.StringToHash("上劈");
        动画参数_下劈 = Animator.StringToHash("下劈");
    }

    // Update is called once per frame
    void Update()
    {
        上下劈检测();
    }

    void 上下劈检测()
    {
        if (玩家位移向量.y < -Mathf.Epsilon && !玩家控制器.判断_地面检测)     // 下劈
        {
            判断_下劈 = true;
        }
        else if (玩家位移向量.y > Mathf.Epsilon)    // 上劈
        {
            判断_上劈 = true;
        }
        else
        {
            判断_下劈= false;
            判断_上劈= false;
        }
        
    }

    private void 攻击_按下(InputAction.CallbackContext context)
    {
        if (判断_可以攻击 && 攻击输入开关)
        {
            if (判断_下劈)     // 下劈
            {
                攻击输入开关 = false;
                玩家动画.SetTrigger(动画参数_下劈);
                能力特效控制器.激活能力特效(5);
                StartCoroutine(攻击框开始协程(3));
            }
            else if (判断_上劈)    // 上劈
            {
                攻击输入开关 = false;
                玩家动画.SetTrigger(动画参数_上劈);
                能力特效控制器.激活能力特效(4);
                StartCoroutine(攻击框开始协程(2));
            }
            else
            {
                攻击输入开关 = false;
                玩家动画.SetTrigger(动画参数_平砍);
                能力特效控制器.激活能力特效(3);
                StartCoroutine(攻击框开始协程(1));
            }
                
        }
            
    }

    /// <summary>
    /// 参数为1-3,依次是平砍、上劈、下劈
    /// </summary>
    /// <param name="i"></param>
    /// <returns></returns>
    private IEnumerator 攻击框开始协程(int i)
    {
        yield return new WaitForSeconds(攻击框延时);
        switch (i)
        {
            case 1:// 平砍
                平砍攻击碰撞框.enabled = true;
                break;
            case 2:// 上劈
                上劈攻击碰撞框.enabled = true;
                break;
            case 3:// 下劈
                下劈攻击碰撞框.enabled = true;
                break;
            default: break;
        }
        StartCoroutine(攻击框结束协程());
    }
    
    private IEnumerator 攻击框结束协程()
    {
        yield return new WaitForSeconds(攻击框延时关闭);
        平砍攻击碰撞框.enabled = false;
        上劈攻击碰撞框.enabled = false;
        下劈攻击碰撞框.enabled = false;

        yield return new WaitForSeconds(攻击冷却时间);
        攻击输入开关 = true;
    }

    //private void OnTriggerEnter2D(Collider2D collision)
    //{
    // 应该在3个攻击动作的物体上，绑另外一个统一检测攻击框的脚本
    //}

}
