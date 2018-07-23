using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommandTimer : MonoBehaviour {

    public delegate void OnCompleted();
    public delegate void OnUpdate(float t);

    /// <summary>
    /// 创建一个计时器
    /// </summary>
    /// <param name="timerName">计时器名字</param>
    /// <returns></returns>
    public static CommandTimer CreateTimer(string timerName="Timer")
    {
        CommandTimer timer = new GameObject(timerName).AddComponent<CommandTimer>();
        return timer;
    }

    OnCompleted onCompleted;//计时完成事件

    OnUpdate onUpdate;//计时中事件，可以获得剩余时间

    float startTime;//开始时间

    float curTime;//现在时间

    float leftTime;//剩余时间

    bool isTimer;//是否开始计时

    bool isDestory;//是否要摧毁	
	
	// Update is called once per frame
	void Update () {
        //出现断开连接或者急停或者痉挛，停止计时
        if (!MainCore.Instance.IsMachineDisabled)
        {
            if (CommandFunctions.IsEmrgencyStop || !CommandFunctions.IsConnected || CommandFunctions.spasmState != CommandFunctions.SpasmState.NoSpasm || CommandFunctions.IsPause)
                return;
        }

        if (isTimer)
        {
            if (curTime < startTime)
            {
                //剩余时间
                leftTime = startTime - curTime;
                //监听计时中
                if (onUpdate != null)
                    onUpdate(leftTime);
                //累加计时
                curTime += Time.deltaTime;
            }
            else
            {
                if (onCompleted != null)
                    onCompleted();
                destory();
            }            
        }
	}

    /// <summary>
    /// 记时结束，销毁计时器
    /// </summary>
    public void destory()
    {
        isTimer = false;
        if (isDestory)
            Destroy(gameObject);
    }
    /// <summary>
    /// 初始化计时器
    /// </summary>
    /// <param name="startTime">计时器时间</param>
    /// <param name="onCompleted">计时完成事件</param>
    /// <param name="onUpdate">计时中事件</param>
    /// <param name="isDestory">计时完是否要摧毁</param>
    public void StartTimer(float startTime,OnCompleted onCompleted=null,OnUpdate onUpdate=null,bool isDestory=true)
    {
        this.startTime = startTime;
        leftTime = startTime;
        curTime = 0;
        if (onCompleted != null)
            this.onCompleted = onCompleted;
        if (onUpdate != null)
            this.onUpdate = onUpdate;
        this.isDestory = isDestory;
        isTimer = true;
    }

    /// <summary>
    /// 暂停计时器
    /// </summary>
    public void PauseTimer()
    {
        if (isTimer)
            isTimer = false;
    }

    /// <summary>
    /// 继续开始计时器
    /// </summary>
    public void ContinueTimer()
    {
        if (!isTimer)
            isTimer = true;
    }
}
