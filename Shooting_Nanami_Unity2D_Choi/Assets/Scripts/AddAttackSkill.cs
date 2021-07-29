using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AddAttackSkill : MonoBehaviour
{
    HeroCtrl a_Hero;
    Vector3 m_CurPos = Vector3.zero;

    //----- 총알 발사 관련 변수 선언
    public GameObject m_BulletObj = null;
    float m_AttSpeed = 0.5f;   //주인공 공속
    float m_CacAtTick = 0.0f;   //기관총 발사 틱 만들기...
    GameObject a_NewObj = null;
    BulletCtrl a_BulletSC = null;
    //----- 
    // Start is called before the first frame update
    void Start()
    {
        a_Hero = GameObject.FindObjectOfType<HeroCtrl>();

    }

    // Update is called once per frame
    void Update()
    {
        MoveObj();
        //-----총알 발사 코드
        if (0.0f < m_CacAtTick)
            m_CacAtTick = m_CacAtTick - Time.deltaTime;

        if (m_CacAtTick <= 0.0f)
        {
            a_NewObj = (GameObject)Instantiate(m_BulletObj);
            //오브젝트의 클론(복사체) 생성 함수
            a_BulletSC = a_NewObj.GetComponent<BulletCtrl>();
            a_BulletSC.BulletSpawn(this.transform, Vector3.right, 15.0f, 20.0f, "SkillBullet");

            m_CacAtTick = m_AttSpeed;
        }
        //-----
    }

    void MoveObj()
    {
        if (a_Hero == null)
            return;

        m_CurPos.x = a_Hero.transform.position.x - 0.95f;
        m_CurPos.y = a_Hero.transform.position.y + 1.15f;

        this.transform.position = m_CurPos;
    }
}
