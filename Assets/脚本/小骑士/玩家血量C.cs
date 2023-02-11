using QFramework;
using UnityEngine;
using UnityEngine.UI;

// 1. 定义一个 Model 对象
#region 数据层
public interface ICounterAppModel : IModel
{
    BindableProperty<int> 玩家血量 { get; set; }
}
public class CounterAppModel : AbstractModel, ICounterAppModel
{
    public BindableProperty<int> 玩家血量 { get; set; } = new BindableProperty<int>();

    protected override void OnInit()
    {
        // 设置初始值（不触发事件）
        玩家血量.SetValueWithoutEvent(10);

        // 当数据变更时,注册事件,如存储数据
        玩家血量.Register(newCount =>
        {
            Debug.Log(玩家血量);
        });
    }
}
#endregion

// 2.定义一个架构（提供 MVC、分层、模块管理等）
#region 架构层
public class CounterApp : Architecture<CounterApp>
{
    protected override void Init()
    {
        // 注册 Model
        this.RegisterModel<ICounterAppModel>(new CounterAppModel());

        // 注册 System 
        //this.RegisterSystem<IAchievementSystem>(new AchievementSystem());

        // 注册存储工具的对象
        //this.RegisterUtility<IStorage>(new Storage());
    }

    //protected override void ExecuteCommand(ICommand command)
    //{
    //    Debug.Log("Before " + command.GetType().Name + "Execute");
    //    base.ExecuteCommand(command);
    //    Debug.Log("After " + command.GetType().Name + "Execute");
    //}
}
#endregion

// 引入 Command
#region 命令层
public class 命令_血量增加 : AbstractCommand
{
    int 当前血量;
    int v;
    public 命令_血量增加(int v)
    {
        this.v = v;
    }

    protected override void OnExecute()
    {
        当前血量 = this.GetModel<ICounterAppModel>().玩家血量.Value;
        if (当前血量 < 10)
        {
            if (当前血量 + v >= 10)
            {
                this.GetModel<ICounterAppModel>().玩家血量.Value = 10;
            }
            else
            {
                this.GetModel<ICounterAppModel>().玩家血量.Value += v;
            }
        }
    }
}

public class 命令_血量减少 : AbstractCommand
{
    int 当前血量;
    int v;
    public 命令_血量减少(int v)
    {
        this.v = v;
    }

    protected override void OnExecute()
    {
        当前血量 = this.GetModel<ICounterAppModel>().玩家血量.Value;
        if (当前血量 > 0)
        {
            if (当前血量 - v <= 0)
            {
                this.GetModel<ICounterAppModel>().玩家血量.Value = 0;
            }
            else
            {
                this.GetModel<ICounterAppModel>().玩家血量.Value -= v;
            }
        }
    }
}
#endregion

// Controller
#region 表现层
public class 玩家血量C : MonoBehaviour, IController /* 3.实现 IController 接口 */
{
    // View
    [SerializeField] GameObject[] 血条;
    [SerializeField] private Button mBtnAdd;
    [SerializeField] private Button mBtnSub;
    [SerializeField] private Text mCountText;

    // 4. Model
    private ICounterAppModel mModel;

    void Start()
    {
        // 5. 获取模型
        mModel = this.GetModel<ICounterAppModel>();

        // 监听输入
        mBtnAdd.onClick.AddListener(() =>
        {
            // 交互逻辑
            //this.SendCommand<命令_血量增加>();
            this.SendCommand(new 命令_血量增加(1));
        });

        mBtnSub.onClick.AddListener(() =>
        {
            // 交互逻辑
            //this.SendCommand(new 命令_血量减少(/* 这里可以传参（如果有） */));
            this.SendCommand(new 命令_血量减少(1));
        });

        // 表现逻辑
        mModel.玩家血量.RegisterWithInitValue(newCount => // -+
        {
            UpdateView();

        }).UnRegisterWhenGameObjectDestroyed(gameObject);
    }

    void UpdateView()
    {
        mCountText.text = mModel.玩家血量.ToString();
        for (int i = 0; i < mModel.玩家血量; i++)
        {
            血条[i].transform.GetChild(0).gameObject.SetActive(true);
        }
        for (int i = 9; i > mModel.玩家血量 - 1; i--)
        {
            血条[i].transform.GetChild(0).gameObject.SetActive(false);
        }
    }

    // 3.
    public IArchitecture GetArchitecture()
    {
        return CounterApp.Interface;
    }

    private void OnDestroy()
    {
        // 8. 将 Model 设置为空
        mModel = null;
    }
}
#endregion