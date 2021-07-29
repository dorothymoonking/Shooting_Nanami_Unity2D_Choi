using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

enum ColorNum
{
    Red = 0,
    Yellow,
    Black,
    Green,
    Blue
}

class ColorText
{
    public ColorNum m_ColorName = ColorNum.Red;
    public Color m_Color = Color.white;
    public string m_Text = "";
}

public class ColorTextMgr : MonoBehaviour
{
    public Text ColorTxt = null;
    public Text m_UserScore;
    public Text m_TimerTxt;
    public Button[] ColorBtn;
    ColorText a_Text = new ColorText();

    Color[] m_ColorList = { Color.red, Color.yellow, Color.black,
                            Color.green, Color.white, Color.blue};
    string[] m_ColorText = { "빨강", "노랑" ,"검정" ,"초록" ,"흰색", "파랑" };

    float m_Time = 0;

    // Start is called before the first frame update
    void Start()
    {
        for(int ii = 0; ii < ColorBtn.Length; ii++)
        {
            int Index = ii;
            if(ColorBtn[ii] != null)
                ColorBtn[ii].onClick.AddListener(() => 
                {
                    Judgment((ColorNum)Index);
                });
        }

        m_UserScore.text = "User점수 : " + GlobalValue.g_BestScore.ToString();
        ChangeColorText();
    }

    // Update is called once per frame
    void Update()
    {
        if(0.0f < m_Time)
        {
            m_Time -= Time.deltaTime;
            int TimeNum = (int)m_Time;
            m_TimerTxt.text = TimeNum.ToString();
            
            if (m_Time <= 0.0f)
            {
                m_Time = 0.0f;
                GlobalValue.g_BestScore -= 50;
                if (GlobalValue.g_BestScore < 0)
                    GlobalValue.g_BestScore = 0;
                m_UserScore.text = "User점수 : " + GlobalValue.g_BestScore.ToString();
                ChangeColorText();
            }
        }
    }

    void ChangeColorText()
    {
        m_Time = 3.0f;
        int RanText = Random.Range(0, 6);
        int RanColor = Random.Range(0, 6);
        a_Text.m_Text = m_ColorText[RanText];
        a_Text.m_Color = m_ColorList[RanColor];
        a_Text.m_ColorName = (ColorNum)RanColor;
        SetText();
    }

    void SetText()
    {
        ColorTxt.text = a_Text.m_Text;
        ColorTxt.color = a_Text.m_Color;
    }

    private void Judgment(ColorNum colorNum)
    {
        if (colorNum == a_Text.m_ColorName)
        {
            GlobalValue.g_BestScore += 100;
        }
        else
        {
            GlobalValue.g_BestScore -= 50;
            if (GlobalValue.g_BestScore < 0)
                GlobalValue.g_BestScore = 0;
        }
        ChangeColorText();
        m_UserScore.text = "User점수 : " + GlobalValue.g_BestScore.ToString();
    }
}
