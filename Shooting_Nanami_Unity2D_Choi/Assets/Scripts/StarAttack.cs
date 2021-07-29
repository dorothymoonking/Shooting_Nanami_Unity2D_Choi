using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarAttack : MonoBehaviour
{
    GameObject m_Hero;
    Vector2 m_CurPos;
    [Header("속도, 반지름")]

    private float speed = 10.0f;
    private float radius = 2.0f;

    private float runningTime = 0;
    private Vector2 newPos = new Vector2();

    // Use this for initialization
    void Start()
    {
        m_Hero = GameObject.Find("HeroRoot");
    }
    void Update()
    {
        m_CurPos = m_Hero.transform.position;

        runningTime += Time.deltaTime * speed;
        float x = m_CurPos.x + radius * Mathf.Cos(runningTime);
        float y = m_CurPos.y + radius * Mathf.Sin(runningTime);
        newPos = new Vector2(x, y);
        this.transform.position = newPos;
    }

    public void Spwan(float _data)
    {
        radius = radius * _data;
    }
}

