using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DLMotion;
using System;

public class GameCore : MonoBehaviour {

    // Update is called once per frame
    /// <summary>
    /// 用于标记是否产生二次痉挛，在产生第一次痉挛并Caution置下去后开始监听，直到痉挛过程结束
    /// 如果再次发生痉挛会变成true
    /// </summary>
    bool ControlSpasmAgain;

    /// <summary>
    /// 在二次痉挛或者跳过痉挛的时候，等待Caution标志位置下去后改变痉挛过程结束标志位
    /// </summary>
    bool WaitforCaution;

    IEnumerator Ienumerator;
    void Start()
    {       
        //监听痉挛标志位，主要处理二次痉挛和Caution标志位清除后改变标志位
        DetectionManage.Instance.ActionCaution += OnSpasmProtection;
        DetectionManage.Instance.ActionFault += OnFault;
    }

    

    void Update () {
        //出现断开连接或者急停或者痉挛，不执行痉挛/复位的任何操作
        if (CommandFunctions.IsEmrgencyStop || !CommandFunctions.IsConnected)
            return;
        //获得Caution标志位
        DetectionManage.Instance.Caution = DiagnosticStatus.MotStatus.Caution;
        DetectionManage.Instance.Fault = DiagnosticStatus.MotStatus.Fault;
        //痉挛流程处理
        switch (CommandFunctions.spasmState)
        {
            //痉挛等待阶段，等待X秒后开始清除痉挛
            case CommandFunctions.SpasmState.RecoveryWaiting:
                //等待时间到
                if (CommandFunctions.SpasmWaitingTime <= 0)
                {
                    Debug.Log("等待完毕");
                    //等待时间到，切换痉挛状态为清除痉挛
                    CommandFunctions.spasmState = CommandFunctions.SpasmState.Recovering;
                }
                else
                {
                    CommandFunctions.SpasmWaitingTime -= Time.deltaTime;
                }
                break;
                //清除痉挛阶段
            case CommandFunctions.SpasmState.Recovering:
                //首先判断是在复位中还是游戏中
                //如果在复位中继续复位
                if (CommandFunctions.IsInReset)
                {
                    Debug.Log("IsInReset");                    
                    ControlSpasmAgain = false;
                    //清除痉挛
                    DynaLinkHS.CmdClearAlm();
                    DynaLinkHS.CmdServoOff();
                    DynaLinkHS.CmdServoOn();
                    //改变痉挛完成标志位                  
                    //CommandFunctions.IsSpasmFinished = true;
                    CommandFunctions.spasmState = CommandFunctions.SpasmState.NoSpasm;
                }
                else
                {
                    //在游戏中，改变痉挛状态为等待复位中
                    Debug.Log("IsInGame");
                    CommandFunctions.spasmState = CommandFunctions.SpasmState.ResettingWaiting;
                    //开始复位到上一个点                    
                    StartCoroutine(WaitForSpasmReset(CommandFunctions.SpasmPostion, CommandFunctions.ResetSpeed, CommandFunctions.ResetDeviation));
                }
                break;
                //等待不做任何事
            case CommandFunctions.SpasmState.ResettingWaiting:

                break;
                //二次痉挛
            case CommandFunctions.SpasmState.SpasmAgain:
                Debug.Log("二次痉挛");
                //停止所有协程
                StopAllCoroutines();
                //等待清除痉挛标志位
                WaitforCaution = true;
                //延迟0.5秒清除痉挛
                Invoke("DelayClearAlm", 0.5f);
                CommandFunctions.spasmState = CommandFunctions.SpasmState.ResettingWaiting;
                break;
                //跳过痉挛
            case CommandFunctions.SpasmState.SkipSpasm:
                Debug.Log("跳过痉挛");
                CommandFunctions.spasmState = CommandFunctions.SpasmState.ResettingWaiting;
                //停止现在所有协程
                StopAllCoroutines();
                if (Ienumerator!=null)
                {                    
                    StopCoroutine(Ienumerator);
                }
                DynaLinkHS.CmdServoOn();
                CommandFunctions.SpasmWaitingTraining = 0;
                //如果还有痉挛清除痉挛
                if (CommandFunctions.IsCaution)
                {
                    WaitforCaution = true;
                    DynaLinkHS.CmdClearAlm();
                    DynaLinkHS.CmdServoOn();                    
                }
                else
                {
                    //痉挛已清除，直接改变痉挛过程完成标志位
                    ControlSpasmAgain = false;
                    //CommandFunctions.IsSpasmFinished = true;
                    //设置痉挛状态为无痉挛
                    CommandFunctions.spasmState = CommandFunctions.SpasmState.NoSpasm;
                }
                break;
        }
        //复位流程处理
        if (CommandFunctions.resettingState!= CommandFunctions.ResettingState.ResetFinshed&&CommandFunctions.IsCanReset&&!CommandFunctions.IsCaution&&!CommandFunctions.IsPause&&DiagnosticStatus.MotStatus.SysCalPass)
        {
            switch (CommandFunctions.resettingState)
            {
                //游戏开始前复位
                case CommandFunctions.ResettingState.Resetting:
                    CommandFunctions.resettingState = CommandFunctions.ResettingState.ResetWaiting;
                    //开始复位到游戏开始点
                    StartCoroutine(WaitForReset(CommandFunctions.ResetPostion, CommandFunctions.ResetSpeed, CommandFunctions.ResetDeviation));
                    break;
                    //等待阶段什么都不做
                case CommandFunctions.ResettingState.ResetWaiting:
                    break;
                    //游戏结束后复位
                case CommandFunctions.ResettingState.EndResetting:
                    CommandFunctions.resettingState = CommandFunctions.ResettingState.ResetWaiting;
                    //开始复位到游戏结束点
                    StartCoroutine(WaitForEndReset(CommandFunctions.ResetPostion, CommandFunctions.ResetSpeed, CommandFunctions.ResetDeviation));
                    break;
            }
        }
    }

    /// <summary>
    /// 游戏开始前复位
    /// </summary>
    /// <param name="Pos">复位点位置</param>
    /// <param name="Speed">复位速度</param>
    /// <param name="Deviation">误差</param>
    /// <returns></returns>
    IEnumerator WaitForReset(Vector2 Pos, int Speed, int Deviation)
    {
        //开始等待复位完成
        yield return StartCoroutine(CommandFunctions.WaitForResetting(Pos, Speed, Deviation));
        //如果是出现断开连接或者急停或者痉挛，停止复位，等故障清除后重新复位
        if (CommandFunctions.IsEmrgencyStop || !CommandFunctions.IsConnected || CommandFunctions.IsCaution||CommandFunctions.IsPause)
        {
            Debug.Log("出现急停或者断开连接,ResettingFailed");
            //重新设置复位状态为复位中
            CommandFunctions.resettingState = CommandFunctions.ResettingState.Resetting;
            yield break;
        }
        //复位结束，改变游戏开始复位标志位
        Debug.Log("复位完成");
        CommandFunctions.resettingState = CommandFunctions.ResettingState.ResetFinshed;
        CommandFunctions.IsInReset = false;
        CommandFunctions.IsResetStartFinished = true;
    }

    /// <summary>
    /// 游戏结束后复位
    /// </summary>
    /// <param name="Pos">复位点位置</param>
    /// <param name="Speed">复位速度</param>
    /// <param name="Deviation">误差</param>
    /// <returns></returns>
    IEnumerator WaitForEndReset(Vector2 Pos, int Speed, int Deviation)
    {
        //开始等待复位完成
        yield return StartCoroutine(CommandFunctions.WaitForResetting(Pos, Speed, Deviation));
        //如果是出现断开连接或者急停或者痉挛，停止复位，等故障清除后重新复位
        if (CommandFunctions.IsEmrgencyStop || !CommandFunctions.IsConnected || CommandFunctions.IsCaution||CommandFunctions.IsPause)
        {
            Debug.Log("出现急停或者断开连接,ResettingFailed");
            CommandFunctions.resettingState = CommandFunctions.ResettingState.EndResetting;
            yield break;
        }
        //复位结束，改变游戏结束复位标志位。
        Debug.Log("ResettingFinished");
        CommandFunctions.resettingState = CommandFunctions.ResettingState.ResetFinshed;
        CommandFunctions.IsInReset = false;        
        CommandFunctions.IsResetEndFinished = true;
        MainCore.Instance.IsGaming = false;


    }

    /// <summary>
    /// 痉挛复位
    /// </summary>
    /// <param name="Pos">复位点位置</param>
    /// <param name="Speed">复位速度</param>
    /// <param name="Deviation">误差</param>
    /// <returns></returns>
    IEnumerator WaitForSpasmReset(Vector2 Pos,int Speed,int Deviation)
    {
        //如果有痉挛，清除痉挛
        if (CommandFunctions.IsCaution)
        {
            DynaLinkHS.CmdClearAlm();
        }
        float waittime = 0;
        do
        {
            yield return new WaitForSeconds(0.5f);
            waittime += 0.5f;
            DynaLinkHS.CmdClearAlm();
        }
        while (CommandFunctions.IsCaution && waittime <= 2f);
        if (waittime > 2)
        {
            Debug.Log("清除痉挛失败");
            CommandFunctions.spasmState = CommandFunctions.SpasmState.Recovering;
            yield break;
        }
        DynaLinkHS.CmdServoOff();
        Ienumerator = CommandFunctions.WaitForResetting(Pos, Speed, Deviation);
        //等待复位到上一个点
        yield return StartCoroutine(Ienumerator);
        //如果痉挛复位中出现断开连接或者急停，停止复位，并把状态重新切换为在痉挛复位阶段
        if (CommandFunctions.IsEmrgencyStop || !CommandFunctions.IsConnected||CommandFunctions.IsCaution)
        {
            Debug.Log("出现急停或者断开连接,ResettingFailed");
            //把状态重新切换为在痉挛复位阶段
            CommandFunctions.spasmState = CommandFunctions.SpasmState.Recovering;
            yield break;
        }
        if (CommandFunctions.IsFault)
        {
            if(CommandFunctions.IsCaution)
            {
                DynaLinkHS.CmdClearAlm();
            }            
        }
        Debug.Log("痉挛复位完成");
        //复位完成，痉挛处理过程结束，切换标志位
        ControlSpasmAgain = false;
        //CommandFunctions.IsSpasmFinished = true;
        CommandFunctions.spasmState = CommandFunctions.SpasmState.NoSpasm;
    }

    /// <summary>
    /// 二次痉挛后清除痉挛，由于立马清除会有问题，延迟0.5秒清除
    /// </summary>
    void DelayClearAlm()
    {
        //二次痉挛发生后让电机断电
        Debug.Log("二次痉挛清除");
        DynaLinkHS.CmdClearAlm();
        DynaLinkHS.CmdServoOff();
    }    

    /// <summary>
    /// 监听Caution标志位，处理是否二次痉挛以及二次痉挛/跳过痉挛后的标志位切换
    /// </summary>
    /// <param name="IsCaution"></param>
    void OnSpasmProtection(bool IsCaution)
    {
        
        CommandFunctions.IsCaution = IsCaution;
        if (IsCaution)
        {
            //已经发生痉挛，在未恢复的时再一次发生痉挛
            if (CommandFunctions.spasmState != CommandFunctions.SpasmState.NoSpasm&& ControlSpasmAgain)
            {
                //二次痉挛标志位
                CommandFunctions.IsSpasmAgain = true;
            }
        }
        else
        {
            //第一次发生痉挛并清除，但仍在痉挛复位中
            if (CommandFunctions.spasmState != CommandFunctions.SpasmState.NoSpasm&&!WaitforCaution)
            {
                ControlSpasmAgain = true;
            }
            //二次痉挛或者跳过痉挛后Caution置下去，改变痉挛完成标志位
            if (WaitforCaution)
            {
                WaitforCaution = false;
                ControlSpasmAgain = false;
                //CommandFunctions.IsSpasmFinished = true;                
                CommandFunctions.spasmState = CommandFunctions.SpasmState.NoSpasm;
            }
            
        }
    }

     void OnFault(bool IsFault)
    {
        if (IsFault)
            CommandFunctions.IsFault = true;
        else
            CommandFunctions.IsFault = false;

    }
    void OnDestroy()
    {
        DetectionManage.Instance.ActionCaution -= OnSpasmProtection;
        DetectionManage.Instance.ActionFault -= OnFault;
    }

    public void Reset()
    {
        CommandFunctions.resettingState = CommandFunctions.ResettingState.NoGaming;
        CommandFunctions.IsInReset = false;
        CommandFunctions.IsResetStartFinished = false;
        CommandFunctions.IsResetEndFinished = false;
        WaitforCaution = false;
        ControlSpasmAgain = false;
        CommandFunctions.IsSpasmFinished = false;
        CommandFunctions.IsPause = false;
        CommandFunctions.spasmState = CommandFunctions.SpasmState.NoSpasm;
    }
}
