using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using PlayFab;
using PlayFab.ClientModels;

public enum InGameState
{
    GameReady,
    GameIng,
    GameEnd,
    GameExit
}

public enum PacketType
{ 
    BestScore,
    UserGold,
    NickUpdate
}


public class InGameMgr : MonoBehaviour
{
    public  Text   m_ScoreTxt    = null;
    public  Text   m_CurScoreTxt = null;
    public  Button m_AddScoreBtn = null;
    private Button m_ScoreBtnObj = null;

    public  Text    m_UserGoldTxt   = null;
    public  Button  m_AddGoldBtn    = null;
    public  Button  m_RemoveGoldBtn = null;
    private Button  m_GoldBtnObj    = null;

    public Button GoLobbyBtn    = null;
    public Text   m_UserInfoTxt = null;

    //스크린의 월드 좌표
    public static Vector3 m_ScreenWMin = new Vector3(-10.0f, -5.0f, 0.0f);
    public static Vector3 m_ScreenWMax = new Vector3(10.0f, 5.0f, 0.0f);

    int m_CurScore = 0;     //이번 스테이지에 얻은 게임 점수
    int m_CurGold  = 0;      //이번 스테이지에 얻은 골드 값

    public static GameObject m_CoinItem = null;

    bool isNetworkLock = false; //Network
    List<PacketType> m_PacketBuff = new List<PacketType>();
    //단순히 어떤 패킷을 보낼 필요가 있다 라는 버퍼 PacketBuffer
    [HideInInspector]public string m_TempStrBuff = "";  //버퍼 통해 보낼 문자열(지금은 닉네임밖에 없다.)

    public static InGameState m_InGState = InGameState.GameReady;

    [Header("------- GameOver -------")]
    public GameObject ResultPanel  = null;
    public Text       Result_Txt   = null;
    public Button     Replay_Btn   = null;
    public Button     RstLobby_Btn = null;

    //-----환경설정 Dlg 관련변수
    [Header("------- ConfigBox -------")]
    public Button     m_CfgBtn       = null;
    public GameObject Canvas_Dialog  = null;
    public GameObject m_ConfigBoxObj = null;
    //-----

    //-----머리위에 데미지 띄우기용 변수 선언
    GameObject a_DamClone;
    DamageTxt a_DamageTx;
    Vector3 a_StCacPos;
    [Header("------- DamageText -------")]
    public Transform m_HUD_Canvas = null;
    public GameObject m_DamageObj = null;
    //-----

    //-----ScrollView OnOff
    [Header("------- ScrollView OnOff -------")]
    public Button m_InVen_Btn = null;
    public Transform m_InVenScrollTr = null;
    private bool m_Inven_ScOnOff = false;
    private float m_ScSpeed = 9000.0f;
    private Vector3 m_ScOnPos = new Vector3(0.0f, 0.0f, 0.0f);
    private Vector3 m_ScOffPos = new Vector3(-1000.0f, 0.0f, 0.0f);
    private Vector3 m_BtnOnPos = new Vector3(410.0f, -247.8f, 0.0f);
    private Vector3 m_BtnOffPos = new Vector3(-569.6f, -247.8f, 0.0f);

    public Transform m_ScConTent;
    public GameObject m_CrSmallPrefab;
    CrSmallNode[] m_CrSmallList;
    //-----

    public static InGameMgr Inst = null;

    void Awake()
    {
        Inst = this;
        Application.targetFrameRate = 60;
    }

    // Start is called before the first frame update
    void Start()
    {
        Time.timeScale = 1.0f;
        m_InGState = InGameState.GameIng;

        GlobalValue.InitDate();
        
        RenewMyCharList(); //인벤토리 복원용 함수

        if (m_AddGoldBtn != null)
            m_AddGoldBtn.onClick.AddListener(() => 
            {
                m_GoldBtnObj = m_AddGoldBtn;

                //GlobalValue.g_UserGold += 15000;
                //m_UserGoldTxt.text = "보유골드(" + GlobalValue.g_UserGold + ")";
                UpdateGoldCo();
            });

        if (m_AddScoreBtn != null)
            m_ScoreBtnObj = m_AddScoreBtn;

        if (m_RemoveGoldBtn != null)
            m_RemoveGoldBtn.onClick.AddListener(() => 
            {
                m_GoldBtnObj = m_RemoveGoldBtn;
                UpdateGoldCo();
            });

        if (GoLobbyBtn != null)
            GoLobbyBtn.onClick.AddListener(()=>
            {
                //UnityEngine.SceneManagement.SceneManager.LoadScene("Lobby");
                Time.timeScale = 0.0f; //일시정지
                m_InGState = InGameState.GameExit;
            });

        if (m_AddScoreBtn != null)
            m_AddScoreBtn.onClick.AddListener(() => 
            {
                UpdateScoreCo();
                //GlobalValue.g_BestScore += Random.Range(5, 11);
            });

        if (Replay_Btn != null)
            Replay_Btn.onClick.AddListener(() => 
            {
                UnityEngine.SceneManagement.SceneManager.LoadScene("InGame");
            });

        if (RstLobby_Btn != null)
            RstLobby_Btn.onClick.AddListener(() => 
            {
                m_InGState = InGameState.GameExit;
            });

        if(m_InVen_Btn != null)
        {
            m_InVen_Btn.onClick.AddListener(() => 
            {
                m_Inven_ScOnOff = !m_Inven_ScOnOff;
            });
        }

        //-----환경설정 Dlg 관련 구현 부분
        if (m_CfgBtn != null)
            m_CfgBtn.onClick.AddListener(() =>
            {
                if (m_ConfigBoxObj != null)
                    m_ConfigBoxObj = Resources.Load("ConfigBox") as GameObject;

                GameObject a_CfgBoxObj = (GameObject)Instantiate(m_ConfigBoxObj);
                a_CfgBoxObj.transform.SetParent(Canvas_Dialog.transform, false);
                Time.timeScale = 0.0f;
            });
        //-----

        //----- Refrash Info
        if (m_UserInfoTxt != null)
            m_UserInfoTxt.text = "내정보 : 별명(" + GlobalValue.g_NickName + ")";

        if (m_ScoreTxt != null)
            m_ScoreTxt.text = "최고점수(" + GlobalValue.g_BestScore.ToString() + ")";

        if (m_UserGoldTxt != null)
            m_UserGoldTxt.text = "보유골드(" + GlobalValue.g_UserGold.ToString() + ")";
        //-----

        //-----스크린의 월드 좌표 구하기
        Vector3 a_ScMin = new Vector3(0.0f, 0.0f, 0.0f); //ScreenViewPort 좌측하든
        m_ScreenWMin = Camera.main.ViewportToWorldPoint(a_ScMin);
        //카메라 화면 좌측하단 코너의 월드 좌표

        Vector3 a_ScMax = new Vector3(1.0f, 1.0f, 1.0f); //ScreenViewPort 우측상단
        m_ScreenWMax = Camera.main.ViewportToWorldPoint(a_ScMax);
        //카메라 화면 우측상단 코너의 월드 좌표

        m_CoinItem = Resources.Load("CoinItemPrefab") as GameObject;
    }

    // Update is called once per frame
    void Update()
    {
        if(isNetworkLock == false) //지금 네트워크 통신 상태가 아니고
        {
            if(0 < m_PacketBuff.Count)  //대기 패킷이 존재한다면...
            {
                Req_NetWork();
            }
            else
            {
                Exe_GameEnd(); //매번 처리할 패킷이 하나도 없다면 종료 처리 해야 할지 확인한다.
            }

        }//if(isNetworkLock == false) //지금 네트워크 통신 상태가 아니고

        ScrollViewOnOff();
    }

    void UpdateGoldCo()
    {
        if (GlobalValue.g_Unique_ID == "")
            return;

        //< 플레이어 데이터(타이틀) >값 활용 코드
        var request = new UpdateUserDataRequest()
        {
            //Permission = UserDatePermission.Private, //디폴트값
            //Permission = UserDatePermission.Public,
            //Public 공개 설정 : 다른 유저들이 볼 수도 있게 하는 옵션
            //Private 비공개 설정(기본설정임) : 나만 접근할 수 있는 값의 속성으로 변경
            Data = new Dictionary<string, string>()
                //{ {"A", "AA"}, {"B", "BB"} };
                {
                    { "UserGold", GlobalValue.g_UserGold.ToString() }
                }
        };

        isNetworkLock = true;
        //PlayFabClientAPI.UpdateUserData(request, UpdateSuccess, UpdateFailure);
        PlayFabClientAPI.UpdateUserData(request, 
            (result) => 
            {
                isNetworkLock = false;
                //StartText.text = "데이터 저장 성공";
            },
            (error) => 
            {
                isNetworkLock = false;
                Debug.Log("Gold저장 실패" + error);
                //StartText.text = "데이터 저장 실패";
                //성공하든 실패하든 로비로 나가는 프로세스는 계속 진행된다.
            });
    }

    void UpdateScoreCo()
    {

        if (GlobalValue.g_Unique_ID == "")
            return;

        var request =  new UpdatePlayerStatisticsRequest
        {
            //BestScore, BestLevel
            Statistics = new List<StatisticUpdate>
            {
                new StatisticUpdate { StatisticName = "ShootingBestScroe",
                                       Value = GlobalValue.g_ShootingBestScroe },
                //new StatisticUpdate { StatisticName = "BestLevel",
                //                       Value = GlobalValue.g_BestScore }
            }
        };

        isNetworkLock = true;
        PlayFabClientAPI.UpdatePlayerStatistics(
                request,

                (result) =>
                {
                    //업데이트 성공 처리
                    isNetworkLock = false;
                },

                (error) =>
                {
                    //업데이트 실패시 응답 함수
                    isNetworkLock = false;
                    Debug.Log("Score저장 실패" + error);
                }
            );
    }//void UpdateScoreCo()

    public void NickChangeCo(string a_NickName)
    {
        if (GlobalValue.g_Unique_ID == "")
            return;

        if (a_NickName == "")
            return;

        PlayFabClientAPI.UpdateUserTitleDisplayName(
            new UpdateUserTitleDisplayNameRequest
            {
                DisplayName = a_NickName
            },
            (result) => 
                {
                    GlobalValue.g_NickName = result.DisplayName;
                    m_UserInfoTxt.text = "내정보 : 별명(" + GlobalValue.g_NickName + ")";
                },
            (error) =>
                {
                    Debug.Log(error.GenerateErrorReport());
                }
         );//PlayFabClientAPI.UpdateUserTitleDisplayName
    }

    public void AddScore(int Value = 10)
    {
        m_CurScore += Value;
        if (m_CurScore < 0)
            m_CurScore = 0;

        if (999999999 < m_CurScore)
            m_CurScore = 999999999;
        m_CurScoreTxt.text = "현재점수(" + m_CurScore + ")";

        if (GlobalValue.g_ShootingBestScroe < m_CurScore)
        {
            GlobalValue.g_ShootingBestScroe = m_CurScore;
            //PlayerPrefs.SetInt("ShootingBestScroe", GlobalValue.g_ShootingBestScroe);
            m_ScoreTxt.text = "최고점수(" + GlobalValue.g_ShootingBestScroe + ")";

            PushPacket(PacketType.BestScore);
        }
    }//public void AddScore(int Value = 10)

    public void AddGold(int Value = 10)
    {
        GlobalValue.g_UserGold += Value;
        if (GlobalValue.g_UserGold < 0)
            GlobalValue.g_UserGold = 0;
        m_CurGold += Value;
        if (m_CurGold < 0)
            m_CurGold = 0;
        //PlayerPrefs.SetInt("UserGold", GlobalValue.g_UserGold);
        m_UserGoldTxt.text = "보유골드(" + GlobalValue.g_UserGold + ")";
        
        PushPacket(PacketType.UserGold);
    }//public void AddGold(int Value = 10)

    public void PushPacket(PacketType a_PType)
    {
        bool a_isExist = false;
        for(int ii = 0; ii < m_PacketBuff.Count; ii++)
        {
            if (m_PacketBuff[ii] == a_PType)    //아직 처리 되지 않은 패킷이 존재하면 
                a_isExist = true;               //또 추가하지 않고 기존 버터의 패킷으로 업데이트한다.
        }

        if (a_isExist == false)
            m_PacketBuff.Add(a_PType);  //대기 중인 패킷이 없으면 새로 추가한다.
    }//void PushPacket()

    void Req_NetWork()  //RequestNetWork
    {
        if(m_PacketBuff[0] == PacketType.BestScore)
        {
            UpdateScoreCo();
        }
        else if(m_PacketBuff[0] == PacketType.UserGold)
        {
            UpdateGoldCo();
        }
        else if(m_PacketBuff[0] == PacketType.NickUpdate)
        {
            NickChangeCo(m_TempStrBuff);
        }

        m_PacketBuff.RemoveAt(0);
    }//void Req_NetWork()

    void Exe_GameEnd() //execute //실행한다.
    {
        //게임 종료 상태이고
        if(InGameMgr.m_InGState == InGameState.GameExit)
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("Lobby");
        }
    }//void Exe_GameEnd()

    public void GameOverFunc()
    {
        PushPacket(PacketType.BestScore);
        PushPacket(PacketType.UserGold);

        ResultPanel.SetActive(true);

        Result_Txt.text = "NickName\n" + GlobalValue.g_NickName + "\n\n" +
                    "획득 점수\n" + m_CurScore + "\n\n" + "획득 골드\n" + m_CurGold;
    }//public void GameOverFunc()

    public void DamageTxt(float a_Value, Transform txtTr, Color a_Color)
    {
        if (m_DamageObj == null || m_HUD_Canvas == null)
            return;

        a_DamClone = (GameObject)Instantiate(m_DamageObj) as GameObject;
        a_DamClone.transform.SetParent(m_HUD_Canvas);
        a_DamageTx = a_DamClone.GetComponent<DamageTxt>();
        if (a_DamageTx != null)
            a_DamageTx.InitDamage(a_Value, a_Color);
        a_StCacPos = new Vector3(txtTr.position.x, txtTr.position.y + 1.15f, 0.0f);
        a_DamClone.transform.position = a_StCacPos;
    }//public void DamageTxt(float a_Value, Transform txtTr, Color a_Color)

    void ScrollViewOnOff()
    {
        if (m_InVenScrollTr == null)
            return;

        if(Input.GetKeyDown(KeyCode.R))
        {
            m_Inven_ScOnOff = !m_Inven_ScOnOff;
        }

        if(m_Inven_ScOnOff == false)
        {
            if(m_InVenScrollTr.localPosition.x > m_ScOffPos.x)
            {
                m_InVenScrollTr.localPosition =
                            Vector3.MoveTowards(m_InVenScrollTr.localPosition,
                                                m_ScOffPos, m_ScSpeed * Time.deltaTime);
            }

            if(m_InVen_Btn.transform.localPosition.x > m_BtnOffPos.x)
            {
                m_InVen_Btn.transform.localPosition =
                            Vector3.MoveTowards(m_InVen_Btn.transform.localPosition,
                                                m_BtnOffPos, m_ScSpeed * Time.deltaTime);
            }
        }//if(m_Inven_ScOnOff == false)
        else
        {
            if (m_ScOnPos.x > m_InVenScrollTr.localPosition.x)
            {
                m_InVenScrollTr.localPosition =
                            Vector3.MoveTowards(m_InVenScrollTr.localPosition,
                                                m_ScOnPos, m_ScSpeed * Time.deltaTime);
            }

            if (m_BtnOnPos.x > m_InVen_Btn.transform.localPosition.x)
            {
                m_InVen_Btn.transform.localPosition =
                            Vector3.MoveTowards(m_InVen_Btn.transform.localPosition,
                                                m_BtnOnPos, m_ScSpeed * Time.deltaTime);
            }
        }//else
    }//void ScrollViewOnOff()

    void RenewMyCharList()
    {
        for(int ii = 0; ii < GlobalValue.m_CrDataList.Count; ii++)
        {
            GlobalValue.m_CrDataList[ii].m_CurSkillCount =
                            GlobalValue.m_CrDataList[ii].m_Level;   //시작할 때 초기화 해 주고 시작한다.

            if(GlobalValue.m_CrDataList[ii].m_Level <= 0)
            {
                break;
            }

            GameObject a_CharClone = Instantiate(m_CrSmallPrefab);
            a_CharClone.GetComponent<CrSmallNode>().InitState(GlobalValue.m_CrDataList[ii]);
            a_CharClone.transform.SetParent(m_ScConTent);
        }//for(int ii = 0; ii < GlobalValue.m_CrDataList.Count; ii++)
    }//void RenewMyCharList()
}
