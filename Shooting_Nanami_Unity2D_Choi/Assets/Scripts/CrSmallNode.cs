using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CrSmallNode : MonoBehaviour
{
    [HideInInspector] public CharType m_CrType;

    [HideInInspector] public int m_CurSkCount = 0;
    public Text  m_LvText;
    public Image m_CrIconImg;
    public Text  m_SkCountText;  //스킬카운트
    public Image m_BackBtnImg;
    public Text  m_SkillKeyText;
    int Skill_Key = 0;
    // Start is called before the first frame update
    void Start()
    {
        Button m_BtnCom = this.GetComponent<Button>();
        if(m_BtnCom != null)
        {
            m_BtnCom.onClick.AddListener(() =>
            {
                if (GlobalValue.m_CrDataList[(int)m_CrType].m_CurSkillCount <= 0)
                    return;
                HeroCtrl a_Hero = GameObject.FindObjectOfType<HeroCtrl>();
                if (a_Hero != null)
                    a_Hero.UseItem(m_CrType);
                Refresh_UI(m_CrType);
            });
        }
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
            Skill_Key = 1;
        else if (Input.GetKeyDown(KeyCode.Alpha2))
            Skill_Key = 2;
        else if (Input.GetKeyDown(KeyCode.Alpha3))
            Skill_Key = 3;
        else if (Input.GetKeyDown(KeyCode.Alpha4))
            Skill_Key = 4;
        else if (Input.GetKeyDown(KeyCode.Alpha5))
            Skill_Key = 5;
        else if (Input.GetKeyDown(KeyCode.Alpha6))
            Skill_Key = 6;


        if (Skill_Key == ((int)m_CrType + 1))
        {
            if (GlobalValue.m_CrDataList[(int)m_CrType].m_CurSkillCount <= 0)
                return;

            HeroCtrl a_Hero = GameObject.FindObjectOfType<HeroCtrl>();

            if (a_Hero != null)
                a_Hero.UseItem(m_CrType);

            Refresh_UI(m_CrType);
            Skill_Key = 0;
        }
    }

    public void InitState(CharInfo a_CrInfo)
    {
        m_CrType = a_CrInfo.m_CrType;
        m_CrIconImg.sprite = a_CrInfo.m_IconImg;
        m_CrIconImg.GetComponent<RectTransform>().sizeDelta
             = new Vector2(a_CrInfo.m_IconSize.x * 103.0f, 103.0f);
        m_CurSkCount = a_CrInfo.m_Level;
        //스프라이트 사이즈 조정 필요
        m_LvText.text = "Lv " + a_CrInfo.m_Level.ToString();
        m_SkCountText.text = m_CurSkCount.ToString() + " / " + a_CrInfo.m_Level.ToString();
        m_SkillKeyText.text = ((int)m_CrType + 1).ToString();
    }

    void Refresh_UI(CharType a_CrType)
    {
        m_CurSkCount = GlobalValue.m_CrDataList[(int)a_CrType].m_CurSkillCount;
        m_SkCountText.text = m_CurSkCount.ToString() + " / " +
                            GlobalValue.m_CrDataList[(int)a_CrType].m_Level.ToString();
        if(m_CurSkCount <= 0)
        {
            m_CrIconImg.color = new Color32(255, 255, 255, 100);
        }
    }
}
