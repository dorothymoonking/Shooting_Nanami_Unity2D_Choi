using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HeroCtrl : MonoBehaviour
{
    float m_MaxHP = 200.0f;
    float m_CurHP = 200.0f;
    public Image m_HPSdBar = null;

    //----- 키보드 입력값 변수 선언
    private float h = 0.0f;
    private float v = 0.0f;

    private float moveSpeed = 7.0f;
    Vector3 moveDir = Vector3.zero;
    //-----

    //----- 주인공이 지형 밖으로 나갈 수 없도록 막기 위한 변수 void LimitMove
    Vector3 HalfSize = Vector3.zero;
    Vector3 m_CacCurPos = Vector3.zero;
    
    float a_LmtBdLeft = 0;
    float a_LmtBdTop = 0;
    float a_LmtBdRight;
    float a_LmtBdBottom = 0;
    //-----

    //----- 총알 발사 관련 변수 선언
    [Header("----- 총알 -----")]
    public GameObject m_BulletObj = null;
    float m_AttSpeed = 0.15f;   //주인공 공속
    float m_CacAtTick = 0.0f;   //기관총 발사 틱 만들기...
    GameObject a_NewObj = null;
    BulletCtrl a_BulletSC = null;
    //----- 

    //----- 스킬 사용 관련 변수
    [Header("----- 추가 공격 스킬 -----")]
    public GameObject AddAttackSkillPrefab;
    GameObject a_AddSkillObj;
    float m_AddSkillTimer = 0.0f;
    bool m_AddAttackSkillOnOff = false;

    [Header("----- 방어 시스템 스킬 -----")]
    public GameObject[] StarAttackSkillPrefab;
    List<GameObject> m_StarAttList = new List<GameObject>();
    float m_StarSkillTimer = 0.0f;
    bool m_StarAttckSkillOnOff;

    [Header("----- 싸이클론 스킬 -----")]
    public GameObject CycloneSkillPrefab;
    public static GameObject a_CycloneSkillObj;
    public Text m_Skill_Text = null;
    public Image m_MaginCircleImg = null;
    public static bool m_CycloneSkillOnOff = false;
    float m_CycloneSkillTimer = 0.0f;

    [Header("----- 스킬 관련 UI -----")]
    public Text m_SkillTypeText = null;
    public Text m_SkillTimerText = null;
    public Image m_SkillTimerImg = null;
    float MaxFill = 0.0f;
    public Text m_HPUpSkill_Text = null;
    float TextTime = 0.0f;
    //----- 

    // Start is called before the first frame update
    void Start()
    {
        //----- LimitMove(주인공이 지형 밖으로 나갈 수 없도록 막아주는 코드)
        //GameObject GroundObj = GameObject.Find("BackGround_1");
        //float GrdHalfSizeY = GroundObj.transform.localScale.y / 2.0f;
        //float m_GroundYMin = GroundObj.transform.position.y - GrdHalfSizeY;
        //float m_GroundYMax = GroundObj.transform.position.y + GrdHalfSizeY;
        //----- 참고용

        //----- 캐릭터의 가로 반사이즈, 세로 반사이즈 구하기
        //월드에 그려진 스프라이트 사이즈 얻어오기
        SpriteRenderer sprRend = gameObject.GetComponentInChildren<SpriteRenderer>();
        //sprRend.transform.localScale <-- 스프라이트는 이걸로 사이즈를 구하면 안된다.
        HalfSize.x = sprRend.bounds.size.x / 2.0f - 0.23f;
        //나중에 주인공 캐릭터 외형을 바꾸면 다시 계산해 준다.
        HalfSize.y = sprRend.bounds.size.y / 2.0f - 0.05f;
        //여백이 커서 조금 줄여 주자
        HalfSize.z = 1.0f;
        //-----

        m_SkillTimerImg.fillAmount = MaxFill;
    }

    // Update is called once per frame
    void Update()
    {
        h = Input.GetAxis("Horizontal");
        v = Input.GetAxis("Vertical");

        if(h != 0.0f || v != 0.0f)
        {
            moveDir = new Vector3(h, v, 0);
            if (1.0f < moveDir.magnitude)
                moveDir.Normalize();
            transform.position += moveDir.normalized * moveSpeed * Time.deltaTime;
        }

        LimitMove();

        //-----총알 발사 코드
        if (0.0f < m_CacAtTick)
            m_CacAtTick = m_CacAtTick - Time.deltaTime;

        if(m_CacAtTick <= 0.0f)
        {
            a_NewObj = (GameObject)Instantiate(m_BulletObj);
            //오브젝트의 클론(복사체) 생성 함수
            a_BulletSC = a_NewObj.GetComponent<BulletCtrl>();
            a_BulletSC.BulletSpawn(this.transform, Vector3.right);

            m_CacAtTick = m_AttSpeed;
        }
        //-----

        if(0.0f < m_AddSkillTimer)
        {
            m_AddSkillTimer -= Time.deltaTime;
            int m_Time = (int)m_AddSkillTimer;
            m_SkillTimerText.text = m_Time.ToString();
            m_SkillTimerImg.fillAmount = m_AddSkillTimer / MaxFill;
            if (m_AddSkillTimer <= 0.0f)
            {
                Destroy(a_AddSkillObj);
                m_AddSkillTimer = 0.0f;
                m_AddAttackSkillOnOff = false;
                SkillUIOff();
            }
        }

        if (0.0f < m_StarSkillTimer)
        {
            m_StarSkillTimer -= Time.deltaTime;
            int m_Time = (int)m_StarSkillTimer;
            m_SkillTimerText.text = m_Time.ToString();
            m_SkillTimerImg.fillAmount = m_StarSkillTimer / MaxFill;
            if (m_StarSkillTimer <= 0.0f)
            {
                for(int ii = 0; ii < m_StarAttList.Count; ii++)
                {
                    Destroy(m_StarAttList[ii]);
                }
                m_StarAttList.Clear();
                m_StarSkillTimer = 0.0f;
                m_StarAttckSkillOnOff = false;
                SkillUIOff();
            }
        }

        if(0.0f < m_CycloneSkillTimer)
        {
            m_CycloneSkillTimer -= Time.deltaTime;
            m_MaginCircleImg.fillAmount = m_CycloneSkillTimer / 10.0f;
            if(m_CycloneSkillTimer <= 0.0f)
            {
                m_CycloneSkillTimer = 0.0f;
                m_CycloneSkillOnOff = false;
                Destroy(a_CycloneSkillObj);
                m_Skill_Text.gameObject.SetActive(false);
                m_MaginCircleImg.gameObject.SetActive(false);
            }
        }

        if(0.0f < TextTime)
        {
            TextTime -= Time.deltaTime;
            if(TextTime <= 0.0f)
            {
                TextTime = 0.0f;
                m_HPUpSkill_Text.gameObject.SetActive(false);
            }
        }
    }

    void LimitMove()
    {
        m_CacCurPos = transform.position;

        a_LmtBdLeft = InGameMgr.m_ScreenWMin.x + HalfSize.x;
        a_LmtBdTop = InGameMgr.m_ScreenWMin.y + HalfSize.y;
        a_LmtBdRight = InGameMgr.m_ScreenWMax.x - HalfSize.x;
        a_LmtBdBottom = InGameMgr.m_ScreenWMax.y - HalfSize.y;

        if (m_CacCurPos.x < a_LmtBdLeft)
            m_CacCurPos.x = a_LmtBdLeft;

        if (a_LmtBdRight < m_CacCurPos.x)
            m_CacCurPos.x = a_LmtBdRight;

        if (m_CacCurPos.y < a_LmtBdTop)
            m_CacCurPos.y = a_LmtBdTop;

        if (a_LmtBdBottom < m_CacCurPos.y)
            m_CacCurPos.y = a_LmtBdBottom;

        transform.position = m_CacCurPos;
    }//void LimitMove()

    public void TakeDamage(float a_Value)
    {
        if (m_CurHP <= 0.0f)
            return;

        InGameMgr.Inst.DamageTxt(a_Value, transform, Color.blue);

        m_CurHP = m_CurHP - a_Value;
        if (m_CurHP < 0.0f)
            m_CurHP = 0.0f;

        if (m_HPSdBar != null)
            m_HPSdBar.fillAmount = m_CurHP / m_MaxHP;

        if(m_CurHP <= 0.0f)
        {
            Time.timeScale = 0.0f; //일시정지
            InGameMgr a_InGameMgr = GameObject.FindObjectOfType<InGameMgr>();
            if (a_InGameMgr != null)
                a_InGameMgr.GameOverFunc();
        }
    }//public void TakeDamage(float a_Value)

    private void OnTriggerEnter2D(Collider2D col)
    {
        if(col.tag == "Monster")
        {
            TakeDamage(50);
            Destroy(col.gameObject);
        }
        else if(col.tag == "EnemyBullet")
        {
            float a_Damage = 20.0f;
            //BulletCtrl = a_BullCtrl = col.GetComponent<BulletCtrl>();
            //if(a_BullCtrl != null)
            //      a_Damage = a_BullCtrl.Enemy_Att;

            TakeDamage(a_Damage);
            Destroy(col.gameObject);
        }
        else if(col.gameObject.name.Contains("CoinItem") == true)
        {
            InGameMgr a_InGameMgr = GameObject.FindObjectOfType<InGameMgr>();
            if (a_InGameMgr != null)
                a_InGameMgr.AddGold();

            Destroy(col.gameObject);
        }
    }// private void OnTriggerEnter2D(Collider2D col)

    public void UseItem(CharType a_CrType)
    {
        if (a_CrType < 0 || CharType.CrCount <= a_CrType)   //선택이 없는 경우
            return;

        bool isHeal = false;
        if(a_CrType == CharType.Char_0)
        {
            m_CurHP += m_MaxHP * 0.3f;
            HealUIOn(m_MaxHP * 0.3f);
            isHeal = true;
        }
        else if(a_CrType == CharType.Char_1)
        {
            m_CurHP += m_MaxHP * 0.5f;
            HealUIOn(m_MaxHP * 0.5f);
            isHeal = true;
        }
        else if (a_CrType == CharType.Char_2)
        {
            m_CurHP = m_MaxHP;
            HealUIOn(m_MaxHP);
            isHeal = true;
        }
        else if(a_CrType == CharType.Char_3)
        {
            if (m_AddAttackSkillOnOff == true || m_StarAttckSkillOnOff == true)
                return;
            
            m_AddAttackSkillOnOff = true;
            AddAttackSkillOn();
        }
        else if (a_CrType == CharType.Char_4)
        {
            if (m_AddAttackSkillOnOff == true || m_StarAttckSkillOnOff == true)
                return;

            m_StarAttckSkillOnOff = true;
            StarAttckSkillOn();
        }
        else if (a_CrType == CharType.Char_5)
        {
            if (m_CycloneSkillOnOff == true)
                return;

            m_CycloneSkillOnOff = true;
            CycloneSkillOn();
        }

        if (isHeal == true)
        {
            if (m_MaxHP < m_CurHP)
                m_CurHP = m_MaxHP;
            if (m_HPSdBar != null)
                m_HPSdBar.fillAmount = m_CurHP / m_MaxHP;
        }

        GlobalValue.m_CrDataList[(int)a_CrType].m_CurSkillCount--;
    }

    void AddAttackSkillOn()
    {
        SkillUIOn();
        m_SkillTypeText.gameObject.SetActive(true);
        m_SkillTypeText.text = "스킬 : 추가 공격!";
        a_AddSkillObj = Instantiate(AddAttackSkillPrefab) as GameObject;
        m_AddSkillTimer = 10.0f;
        MaxFill = m_AddSkillTimer;
        int m_Time = (int)m_AddSkillTimer;
        m_SkillTimerText.text = m_Time.ToString();
    }

    void StarAttckSkillOn()
    {
        SkillUIOn();
        m_SkillTypeText.text = "스킬 : 방어 시스템!";
        for (int ii = 0; ii < 2; ii++)
        {
            GameObject a_Obj = Instantiate(StarAttackSkillPrefab[ii]) as GameObject;
            m_StarAttList.Add(a_Obj);
            if (ii == 0)
                m_StarAttList[ii].GetComponent<StarAttack>().Spwan(-1.0f);
            else
                m_StarAttList[ii].GetComponent<StarAttack>().Spwan(1.0f);
        }
        m_StarSkillTimer = 5.0f;
        MaxFill = m_StarSkillTimer;
        int m_Time = (int)m_StarSkillTimer;
        m_SkillTimerText.text = m_Time.ToString();
    }
    
    void CycloneSkillOn()
    {
        a_CycloneSkillObj = Instantiate(CycloneSkillPrefab) as GameObject;
        m_CycloneSkillTimer = 10.0f;
        m_Skill_Text.gameObject.SetActive(true);
        m_MaginCircleImg.gameObject.SetActive(true);
    }

    void SkillUIOn()
    {
        m_SkillTypeText.gameObject.SetActive(true);
        m_SkillTimerText.gameObject.SetActive(true);
        m_SkillTimerImg.gameObject.SetActive(true);
    }

    void SkillUIOff()
    {
        m_SkillTypeText.gameObject.SetActive(false);
        m_SkillTimerText.gameObject.SetActive(false);
        m_SkillTimerImg.gameObject.SetActive(false);
    }

    void HealUIOn(float HP)
    {
        TextTime = 2.0f;
        m_HPUpSkill_Text.gameObject.SetActive(true);
        int m_HP = (int)HP;
        m_HPUpSkill_Text.text = "체력회복" + m_HP.ToString();
    }
}
