using UnityEngine;
using System.Collections;
using DLMotion;
using UdpBasicClass;

public class CommandFunctions{

    /// <summary>
    /// 复位点位置
    /// </summary>
    public static Vector2 ResetPostion;

    /// <summary>
    /// 复位速度
    /// </summary>
    public static int ResetSpeed;
    
    /// <summary>
    /// 复位误差
    /// </summary>
    public static int ResetDeviation;

    /// <summary>
    /// 重连网络是否完成
    /// </summary>
    public static bool IsReconnectFinished;

    /// <summary>
    /// 游戏开始复位是否完成
    /// </summary>
    public static bool IsResetStartFinished;

    /// <summary>
    /// 游戏结束复位是否完成
    /// </summary>
    public static bool IsResetEndFinished;

    /// <summary>
    /// 痉挛复位点命令
    /// </summary>
    public static Vector2 SpasmPostion;

    /// <summary>
    /// 恢复痉挛过程是否完成
    /// </summary>
    public static bool IsSpasmFinished;

    /// <summary>
    /// 是否可以复位
    /// </summary>
    public static bool IsCanReset;

    /// <summary>
    /// 是否在复位中
    /// </summary>
    public static bool IsInReset;

    /// <summary>
    /// 是否二次痉挛
    /// </summary>
    public static bool IsSpasmAgain;

    /// <summary>
    /// 复位重发点位时间间隔 
    /// </summary>
    public static float ResendTime = 3;

    /// <summary>
    /// 清除痉挛前等待时间
    /// </summary>
    public static float SpasmWaitingTime = 5;

    /// <summary>
    /// 痉挛结束等待继续游戏时间
    /// </summary>
    public static float SpasmWaitingTraining = 60;
    /// <summary>
    /// 判断机器是否连接，true为连接状态，false为未连接状态
    /// </summary>
    public static bool IsConnected;

    /// <summary>
    /// 判断机器是否急停状态，true为机器处于急停状态，false为机器无急停
    /// </summary>
    public static bool IsEmrgencyStop;

    /// <summary>
    /// 判断机器是否就是0点，true为机器已经校准完毕，false为机器丢失0点
    /// </summary>
    public static bool IsCalibrationPass=true;

    /// <summary>
    /// 判断机器是否出错，true为机器报错，false为机器无报错
    /// </summary>
    public static bool IsFault;

    /// <summary>
    /// 判断机器是否有痉挛，true为机器出现痉挛，fasle为机器无痉挛
    /// </summary>
    public static bool IsCaution;

    /// <summary>
    /// 复位过程中机器处于哪一个阶段
    /// </summary>
    public static ResettingState resettingState=ResettingState.NoGaming;

    /// <summary>
    /// 痉挛过程中机器处于哪一个阶段
    /// </summary>
    public static SpasmState spasmState = SpasmState.NoSpasm;

    /// <summary>
    /// 当前游戏训练模式
    /// </summary>
    public static MachineMode machineMode = MachineMode.None;

    /// <summary>
    /// 跳过复位
    /// </summary>
    public static bool IsHalt;

    /// <summary>
    /// 暂停
    /// </summary>
    public static bool IsPause;

    /// <summary>
    /// 游戏训练模式
    /// </summary>
    public enum MachineMode
    {
        None,
        Passive,
        AssistLT
    }

    //在切换到UI主界面的时候要设置为NoGaming
    public enum ResettingState : int
    {
        NoGaming=0,//不在游戏中
        Resetting =1,//复位中       
        ResetWaiting = 2,//复位等待中
        EndResetting=3,//游戏结束复位中
        ResetFinshed = 4,//复位完成
    }

    //在痉挛结束之后要设置为NoSpasm
    public enum SpasmState : int
    {
        NoSpasm=0,//没有痉挛
        RecoveryWaiting =1,//恢复倒计时状态
        Recovering=2,//恢复中
        ResettingWaiting=3,//复位等待中
        SpasmAgain =4,//第二次痉挛状态
        SkipSpasm=5//跳过痉挛
    }

    /// <summary>
    /// 零点丢失
    /// </summary>
    public static void CmdHomeCal()
    {
        DynaLinkHS.CmdClearAlm();
        //判断MotFault
        //如果有错误是否要clearfault();,之前逻辑这边有通过MotFault去判断
        if (DiagnosticStatus.MotStatus.Fault)
        {
            DynaLinkHS.CmdClearFault();
        }
        DynaLinkHS.CmdServoOff();
        DynaLinkHS.CmdHomeCal();
    }
    

    /// <summary>
    /// 复位到点方法
    /// </summary>
    /// <param name="Pos">复位点位置</param>
    /// <param name="Speed">机器移动速度</param>
    /// <param name="Deviation">机器移动误差</param>
    /// <returns></returns>
    public static IEnumerator WaitForResetting(Vector2 Pos,int Speed,int Deviation)
    {
        Debug.Log("开始复位点");
        //发送复位命令
        DynaLinkHS.CmdServoOn();
        DynaLinkHS.CmdLinePassive((int)Pos.x, (int)Pos.y, Speed);
        //等待复位
        float waittime = 0;
        do
        {
            //复位中超过三秒不动重新发点
            if (waittime >= ResendTime)
            {
                DynaLinkHS.CmdServoOn();
                DynaLinkHS.CmdLinePassive((int)Pos.x, (int)Pos.y, Speed);
                waittime = 0;
            }
            yield return new WaitForSeconds(0.5f);
            //根据MotionInProcess来判断是否在运动
            if (!DiagnosticStatus.MotStatus.MotionInProcess)
            {
                waittime += 0.5f;
            }
            else
            {
                waittime = 0;
            }
        } while ((Mathf.Abs(DynaLinkHS.StatusMotRT.PosDataJ1 - Pos.x) > Deviation || Mathf.Abs(DynaLinkHS.StatusMotRT.PosDataJ2 - Pos.y) > Deviation || DiagnosticStatus.MotStatus.MotionInProcess) && !IsEmrgencyStop && IsConnected && !IsCaution&&!IsHalt&&!IsPause&&!IsFault);
        Debug.Log("Enter");
        yield return "ResettingFinished";
    }
        
    /// <summary>
    /// 对外接口，用于开始复位时初始化方法
    /// </summary>
    /// <param name="Pos">复位点位置</param>
    /// <param name="Speed">复位速度</param>
    /// <param name="Deviation">误差距离</param>
    public static void InitializeStartReset(Vector2 Pos,int Speed,int Deviation)
    {
        MainCore.Instance.IsGaming = true;        
        IsHalt = false;
        ResetPostion = Pos;
        ResetSpeed = Speed;
        ResetDeviation = Deviation;        
        IsInReset = true;
        IsResetStartFinished = false;
        resettingState = ResettingState.Resetting;
        IsCanReset = true;        
    }

    /// <summary>
    /// 对外接口，用于结束复位
    /// </summary>
    /// <param name="Pos">复位点位置</param>
    /// <param name="Speed">复位速度</param>
    /// <param name="Deviation">误差距离</param>
    public static void InitializeEndReset(Vector2 Pos, int Speed, int Deviation)
    {        
        IsHalt = false;
        ResetPostion = Pos;
        ResetSpeed = Speed;
        ResetDeviation = Deviation;
        IsInReset = true;
        IsResetEndFinished = false;
        resettingState = ResettingState.EndResetting;
        IsCanReset = true;
    }

    /// <summary>
    /// 对外接口。用于继续复位
    /// </summary>
    public static void RestartReset()
    {
        IsCanReset = true;
    }

    /// <summary>
    /// 对外接口，用于痉挛发生时调用方法
    /// </summary>
    /// <param name="Pos">上一个点位置</param>
    /// <param name="Speed">复位时速度</param>
    /// <param name="Deviation">距离误差</param>
    /// <param name="WaitingTime">清除痉挛前等待时间</param>
    /// <param name="WaitingTraining">复位完等待继续游戏时间</param>
    public static void SpasmRecover(Vector2 Pos,int Speed,int Deviation,float WaitingTime = 0,float WaitingTraining=0)
    {
        SpasmPostion = Pos;
        ResetSpeed = Speed;
        ResetDeviation = Deviation;
        SpasmWaitingTime = WaitingTime;
        SpasmWaitingTraining = WaitingTraining;
        IsCanReset = false;
        spasmState = SpasmState.RecoveryWaiting;
    }

    /// <summary>
    /// 对外接口，处理二次痉挛
    /// </summary>
    public static void SpasmAgainRecover()
    {        
        spasmState = SpasmState.SpasmAgain;
    }

    /// <summary>
    /// 对外接口，跳过痉挛
    /// </summary>
    public static void SkipSpasm()
    {        
        spasmState = SpasmState.SkipSpasm;
    }
}
