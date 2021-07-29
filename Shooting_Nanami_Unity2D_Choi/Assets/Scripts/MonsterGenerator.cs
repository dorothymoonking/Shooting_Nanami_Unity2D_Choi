using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterGenerator : MonoBehaviour
{
    public GameObject[] MonPrefab;

    float m_SpDelta   = 0.0f;
    float m_DiffSpawn = 1.0f;   //난이도에 따른 몬스터 스폰주기 변수
    float m_DiffTick  = 0.0f;
    int   m_DiffLevel = 0;      //

    public static int m_MonNum = 0;
    // Start is called before the first frame update
    void Start()
    {
        m_MonNum = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (m_MonNum > 10)
            return;

        m_DiffTick += Time.deltaTime;
        if(5.0f <= m_DiffTick)
        {
            m_DiffLevel++;
            if (20 < m_DiffLevel)
                m_DiffLevel = 20;

            m_DiffSpawn = 1.0f - (m_DiffLevel * 0.03f);

            m_DiffTick = 0.0f;
        }

        m_SpDelta -= Time.deltaTime;

        if(m_SpDelta < 0.0f)
        {
            GameObject go = null;
            int dice = Random.Range(1, 11);     //1 ~ 10 랜덤값 발생
            if(dice <= 1)
            {
                go = Instantiate(MonPrefab[0]) as GameObject;
            }
            else
            {
                go = Instantiate(MonPrefab[1]) as GameObject;
            }
            float py = Random.Range(-0.3f, 3.0f);
            go.transform.position =
                            new Vector3(InGameMgr.m_ScreenWMax.x + 1.0f, py, 0);
            MonsterCtrl a_Enemy = go.gameObject.GetComponent<MonsterCtrl>();
            if(a_Enemy != null)
            {
                a_Enemy.m_Level = m_DiffLevel;
            }

            m_SpDelta = m_DiffSpawn;
            m_MonNum++;
        }//if(m_SpDelta < 0.0f)
    }
}
