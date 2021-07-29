using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum MonType
{
    MT_Missile,
    MT_Zombi,
};

public class MonsterCtrl : MonoBehaviour
{
    public MonType m_MonType = MonType.MT_Missile;
    [HideInInspector] public int m_Level = 0;

    float m_MaxHP = 200.0f; //최대 최력
    float m_CurHP = 200.0f; //현재 체력
    public Image m_HPSdBar = null;

    HeroCtrl m_Hero = null;
    float m_Speed = 4.0f; //이동 속도
    Vector3 m_CurPos;
    Vector3 m_SpawnPos;

    Vector3 m_DirVec;   //이동 방향
    float m_CacPosY = 0.0f;
    float m_Rand_Y  = 0.0f;

    //-----총알 발사 관련 변수 선언
    public GameObject m_BulletObj = null;
    GameObject a_NewObj = null;
    BulletCtrl a_BulletSC = null;
    float shoot_Time = 0.0f;
    float Shoot_Delay = 1.5f;
    //-----

    float enemy_Att = 20.0f; // 공격력
    float BulletMvSpeed = 10.0f; //총알 이동 속도

    float DamageTick = 2.0f;

    bool m_AttackStop = false;
    // Start is called before the first frame update
    void Start()
    {
        m_SpawnPos = this.transform.position;
        m_Hero = GameObject.FindObjectOfType<HeroCtrl>();

        m_Rand_Y = Random.Range(0.2f, 2.6f);    //Sin함수의 램덤 진폭

        m_Speed += m_Speed * m_Level * 0.05f;               //총 2배까지 증가될 수 있음
        m_MaxHP += m_MaxHP * m_Level * 0.1f;                //총 3배까지 증가될 수 있음
        enemy_Att += enemy_Att * m_Level * 0.1f;            //총 3배까지 증가될 수 있음
        BulletMvSpeed += BulletMvSpeed * m_Level * 0.05f;   //총 2배까지 증가될 수 있음

        //Debug.Log(m_Speed + " : " + m_MaxHP);
    }//void Start()

    // Update is called once per frame
    void Update()
    {
        m_CurPos = this.transform.position;

        if(HeroCtrl.m_CycloneSkillOnOff == true && transform.position.x > 0.0f)
        {
            GameObject m_TestObj = HeroCtrl.a_CycloneSkillObj;
            Vector3 a_CacPos = m_TestObj.transform.position - this.transform.position;
            m_DirVec = a_CacPos;
            m_DirVec.Normalize();
            m_CurPos = m_CurPos + (m_DirVec * Time.deltaTime * m_Speed);
        }
        else if (m_MonType == MonType.MT_Zombi)
        {
            m_CurPos.x = m_CurPos.x + (-1.0f * Time.deltaTime * m_Speed);
            m_CacPosY += Time.deltaTime * (m_Speed / 2.2f);
            m_CurPos.y = m_SpawnPos.y + Mathf.Sin(m_CacPosY) * m_Rand_Y;
        }
        else if (m_MonType == MonType.MT_Missile)
        {
            Vector3 a_CacVec = m_Hero.transform.position - this.transform.position;

            //추적
            m_DirVec = a_CacVec; //몬스터의 방향벡터

            m_DirVec.Normalize();
            m_DirVec.x = -1.0f;
            m_DirVec.z = 0.0f;

            m_CurPos = m_CurPos + (m_DirVec * Time.deltaTime * m_Speed);
        }

        this.transform.position = m_CurPos;

        //-----총알 발사
        if (m_MonType == MonType.MT_Zombi && m_BulletObj != null && m_AttackStop == false)
        {
            shoot_Time += Time.deltaTime;
            if(Shoot_Delay <= shoot_Time)
            {
                a_NewObj = (GameObject)Instantiate(m_BulletObj);
                //오브젝트의 클론(복사체) 생성 함수
                a_BulletSC = a_NewObj.GetComponent<BulletCtrl>();
                a_BulletSC.BulletSpawn(this.transform, Vector3.left,
                                            BulletMvSpeed, enemy_Att);

                shoot_Time = 0.0f;
            }
        }//if (m_MonType == MonType.MT_Zombi && m_BulletObj != null)
        //-----

        if (this.transform.position.x < InGameMgr.m_ScreenWMin.x - 2.0f)
        {
            Destroy(gameObject);
        }

        if(HeroCtrl.m_CycloneSkillOnOff == true && 0.0 < DamageTick) 
        {
            DamageTick -= Time.deltaTime;
            if(DamageTick <= 0.0f)
            {
                TakeDamage(10.0f);
                DamageTick = 2.0f;
            }
        }

    }//void Update()

    public void TakeDamage(float a_Value)
    {
        if (m_CurHP <= 0.0f) //이렇게 하면 사망 처리는 한번만 될 것이다.
            return;

        InGameMgr.Inst.DamageTxt(a_Value, transform, Color.red);

        m_CurHP = m_CurHP - a_Value;
        if (m_CurHP < 0.0f)
            m_CurHP = 0.0f;

        if(m_HPSdBar != null)
            m_HPSdBar.fillAmount = m_CurHP / m_MaxHP;

        if(m_CurHP <= 0.0f) //몬스터 사망 처리
        {
            InGameMgr a_InGameMgr = GameObject.FindObjectOfType<InGameMgr>();
            if (a_InGameMgr != null)
                a_InGameMgr.AddScore();

            //----- 보상으로 아이템 드롭
            int dice = Random.Range(0, 10);
            if(dice < 3)    //30% 확률
                if(InGameMgr.m_CoinItem != null)
                {
                    GameObject a_CoinObj = (GameObject)Instantiate(InGameMgr.m_CoinItem);
                    a_CoinObj.transform.position = this.transform.position;
                    Destroy(a_CoinObj, 10.0f);  //10초 이내에 먹어야 한다.
                }
            //----- 
            MonsterGenerator.m_MonNum--;
            Destroy(gameObject);    //<--- 몬스터 GameObject 제거됨
        }//if(m_CurHP <= 0.0f) //몬스터 사망 처리

    }//public void TakeDamage(float a_Value)

    private void OnTriggerEnter2D(Collider2D col)   //몬스터에 뭔가 충돌 되었을 때 발생되는 함수
    {
        if (col.tag == "AllyBullet")
        {
            TakeDamage(80.0f);
            Destroy(col.gameObject);
        }
        else if(col.tag == "SkillBullet")
        {
            TakeDamage(40.0f);
            Destroy(col.gameObject);
        }
        else if(col.tag == "MagicCircle")
        {
            TakeDamage(10.0f);  //마법진안에 들어오면 일단 바로 데미지를 준다.
            DamageTick = 2.0f;  //그 이후 지속적으로 데미지를 주기 위한...
            m_AttackStop = true;
        }
        else if(col.tag == "SkillStar")
        {
            TakeDamage(9999.0f);
        }
    }//private void OnTriggerEnter2D(Collider2D col) 

    private void OnTriggerExit2D(Collider2D col)
    {
        if (col.tag == "MagicCircle")
        {
            m_AttackStop = false;
        }
    }
}