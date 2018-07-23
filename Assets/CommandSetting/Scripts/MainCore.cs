using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DLMotion;
using System.Security.Cryptography;
using System.Text;
using System;


public class MainCore : MonoBehaviour {

    public bool IsMachineDisabled;    
    private static MainCore _instance;    
    private bool _isGaming;
    GameCore gameCore;
    public static int FirstWarningNum = 350000;
    public static int SecondWarningNum = 450000;
    internal int MovementCount;
    public static MainCore Instance
    {
        get {
            if (_instance == null)
            {
                _instance = FindObjectOfType(typeof(MainCore)) as MainCore;
                //if (_instance == null)
                //{
                //    GameObject obj =Instantiate(Resources.Load("MainCore") as GameObject);

                //    //obj.hideFlags = HideFlags.HideAndDontSave;
                //    _instance=obj.AddComponent<MainCore>();                   
                //}
            }
            return _instance;
        }
    }

    public bool IsFinished;
    public bool IsGaming
    {
        get { return _isGaming; }
        set
        {
            if (_isGaming != value)
            {
                _isGaming = value;
                if (_isGaming)
                {
                    gameCore.Reset();
                    gameCore.enabled = true;
                }
                else
                {                    
                    gameCore.enabled = false;
                }
            }
        }
    }

    void Awake()
    {
        IsFinished = true;
        if (!IsMachineDisabled)
        {
            IsFinished = false;
            StartCoroutine(InitializeConnection());
        }
        else
        {
            DetectionManage.Instance.ActionReConnect += OnReConnect;
            DetectionManage.Instance.ActionEmrgencyStop += OnEmrgencyStop;
        }

        gameCore = this.GetComponent<GameCore>();
        if (gameCore != null)
        {
            gameCore.enabled = false;
        }
        else
        {
            gameCore = this.gameObject.AddComponent<GameCore>();
            gameCore.enabled = false;
        }        
    }    
	
	// Update is called once per frame
	void Update () {        
        if (!IsMachineDisabled)
        {
            if (!DynaLinkHS.ServerLinkActBit)
            {
                //Debug.Log("DynaLinkHS.ServerLinkActBit=========" + DynaLinkHS.ServerLinkActBit);
            }
            //获取机器连接的标志位
            DetectionManage.Instance.Connected = DynaLinkHS.ServerLinkActBit;
            
            //获取机器急停的标志位（只有在机器连接状态下）
            if (CommandFunctions.IsConnected)//DetectionManage.Instance.Connected
            {
                DetectionManage.Instance.EMstopBit = DiagnosticStatus.MotStatus.EMstopBit;
            }
        }        
    }

    /// <summary>
    /// 机器连接状态监听
    /// </summary>
    /// <param name="IsConnected"></param>
    void OnReConnect(bool IsConnected)
    {        
        if (!IsConnected)
        {            
            CommandFunctions.IsConnected = false;           
        }
        else
        {
            //CommandFunctions.IsConnected = true;          
        }
    }

    /// <summary>
    /// 机器急停监听
    /// </summary>
    /// <param name="IsEmrgencyStop"></param>
    void OnEmrgencyStop(bool IsEmrgencyStop)
    {
        if (IsEmrgencyStop)
        {            
            CommandFunctions.IsEmrgencyStop = true;            
        }
        else
        {            
            CommandFunctions.IsEmrgencyStop = false;            
        }
    }

    public void AddMovementCount()
    {
        MovementCount++;
    }

    void OnDestroy()
    {
        DetectionManage.Instance.ActionReConnect -= OnReConnect;
        DetectionManage.Instance.ActionEmrgencyStop -= OnEmrgencyStop;        
    }
     void OnApplicationQuit()
    {
        DynaLinkCore.StopSocket();
        UdpBasicClass.UdpSocketClient.SocketQuit();
    }

    /// <summary>
    /// 开机连接到机器
    /// </summary>
    /// <returns></returns>
    IEnumerator InitializeConnection()
    {
        //如果机器还未连接，发送连接命名
        if (!DetectionManage.Instance.Connected)
        {
            Debug.Log("Begin Connect");
            DynaLinkCore.ConnectClick();
        }

        //三秒等待连接
        float initialTimeOut = 0;
        while (!DetectionManage.Instance.Connected)
        {
            if (initialTimeOut >= 3f)
            {
                //超过三秒，连接超时，断开连接准备重连
                DynaLinkCore.StopSocket();
                Debug.Log("Initialize connection Time-out!");
                Debug.Log("Disconnected!");                
                //断开连接有一定的延迟
                yield return new WaitForSeconds(1f);
                //开始监听连接和急停的事件
                DetectionManage.Instance.ActionReConnect += OnReConnect;
                DetectionManage.Instance.ActionEmrgencyStop += OnEmrgencyStop;
                //由于未连接上执行断开连接的事件一次
                OnReConnect(false);
                IsFinished = true;                
                yield break;
            }
            yield return new WaitForSeconds(1f);
            initialTimeOut += 1f;
        }
        
            CommandFunctions.IsConnected = true;
            //如果连接上了，开始监听连接事件
            DetectionManage.Instance.ActionReConnect += OnReConnect;
            //判断机器是否有急停情况
            if (DetectionManage.Instance.EMstopBit)
            {
                OnEmrgencyStop(true);
            }
            //开始监听急停事件
            DetectionManage.Instance.ActionEmrgencyStop += OnEmrgencyStop;
            IsFinished = true;

    }


    


}
