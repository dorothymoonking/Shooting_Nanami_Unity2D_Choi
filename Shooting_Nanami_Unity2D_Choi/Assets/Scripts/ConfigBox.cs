using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using PlayFab;
using PlayFab.ClientModels;

public class ConfigBox : MonoBehaviour
{
    public Button m_OK_Btn = null;
    public Button m_Close_Btn = null;
    public InputField IDInputField = null;

    float ShowMsTimer = 0.0f;
    public Text m_Message = null;

    // Start is called before the first frame update
    void Start()
    {
        if (m_OK_Btn != null)
            m_OK_Btn.onClick.AddListener(OKBtnFunction);

        if (m_Close_Btn != null)
            m_Close_Btn.onClick.AddListener(CloseBtnFunction);

        Text a_Placeholder = null;
        if (IDInputField != null)
        {
            Transform a_PlhTr = IDInputField.transform.Find("Placeholder");
            a_Placeholder = a_PlhTr.GetComponent<Text>();
        }

        if (a_Placeholder != null)
            a_Placeholder.text = GlobalValue.g_NickName;
    }

    //Update is called once per frame
    void Update()
    {
        if(0.0f < ShowMsTimer)
        {
            ShowMsTimer -= Time.unscaledDeltaTime; //Time.timeScale = 0.0; 이라도 사용가능한 DeltaTime이다.
            if (ShowMsTimer <= 0.0f)
            {
                ShowMsTimer = 0.0f;
                MessageOnOff(false);    //메세지 끄기
            }
        }//if(0.0f < ShowMsTimer)
    }

    void OKBtnFunction()
    {
        //닉네임 변경 ToDo...
        string a_NickStr = IDInputField.text.Trim();
        if (a_NickStr == "")
        {
            MessageOnOff(true, "별명 빈칸없이 입력해 주셔야 합니다.");
            return;
        }

        if (!(2 <= a_NickStr.Length && a_NickStr.Length < 20))
        {
            MessageOnOff(true, "별명은 2글자이상 20글자 이하로 작성해 주세요.");
            return;
        }
        InGameMgr a_InGameMgr = GameObject.FindObjectOfType<InGameMgr>();
        if (a_InGameMgr != null)
        {
            a_InGameMgr.m_TempStrBuff = a_NickStr;
            a_InGameMgr.PushPacket(PacketType.NickUpdate);
        }

        Time.timeScale = 1.0f;
        Destroy(this.gameObject);
    }

    void CloseBtnFunction()
    {
        Time.timeScale = 1.0f;
        Destroy(this.gameObject);
    }

    void MessageOnOff(bool isOn = true, string Mess = "")
    {
        if(isOn == true)
        {
            m_Message.text = Mess;
            m_Message.gameObject.SetActive(true);
            ShowMsTimer = 5.0f;
        }
        else
        {
            m_Message.text = "";
            m_Message.gameObject.SetActive(false);
        }

    }//void MessageOnOff(bool isOn = true, string Mess = "")

}
