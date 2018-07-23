/*
 * 监听标志位，单例模式，不用挂物体上
 */
using System;

public class DetectionManage {

    private static DetectionManage _instance;
    

    public static DetectionManage Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new DetectionManage();
            }
            return _instance;
        }
    }
    //连接标志位
    private bool _Connected;
    //急停标志位
    private bool _EMstopBit;
    //0点丢失标志位
    private bool _SysCalPass;
    //痉挛标志位
    private bool _Caution;
    //机器故障标志位
    private bool _Fault; 
    
    //机器断开连接事件
    public Action<bool> ActionReConnect;
    //急停事件
    public Action<bool> ActionEmrgencyStop;
    //0点丢失事件 
    public Action<bool> ActionHomeCalibrate;
    //痉挛事件
    public Action<bool> ActionCaution;
    //机器故障事件
    public Action<bool> ActionFault;   

    //当标志位改变值的时候会触发断开或者重新连接的事件
    public bool Connected
    {
        get { return _Connected; }
        set
        {
            if (_Connected != value)
            {
                _Connected = value;
                if (ActionReConnect != null)
                {
                    ActionReConnect(value);
                }
                
            }
        }
    }

    //当标志位改变值的时候会触发急停或者恢复正常的事件
    public bool EMstopBit
    {
        get { return _EMstopBit; }
        set
        {
            if (_EMstopBit != value)
            {
                _EMstopBit = value;
                if (ActionEmrgencyStop != null)
                    ActionEmrgencyStop(value);
            }
        }
    }

    //当标志位改变值的时候会0点丢失或者校准完毕的事件
    public bool SysCalPass
    {
        get { return _SysCalPass; }
        set
        {
            if (_SysCalPass != value)//CommandFunctions.resettingState == CommandFunctions.ResettingState.NoGaming&&
            {
                _SysCalPass = value;
                if (ActionHomeCalibrate != null)
                {
                    ActionHomeCalibrate(value);
                }
                
            }
        }
    }

    //当标志位改变值的时候会发生痉挛或者恢复正常的事件
    public bool Caution
    {
        get { return _Caution; }
        set
        {
            if (_Caution != value)
            {
                _Caution = value;
                if (ActionCaution != null)
                {
                    ActionCaution(value);
                }
                
            }
        }
    }

    ////当标志位改变值的时候会触发机器故障事件
    public bool Fault
    {
        get { return _Fault; }
        set
        {
            if (_Fault != value)
            {
                _Fault = value;
                if (ActionFault != null)
                {
                    ActionFault(value);
                }
                
            }
        }
    }
}
