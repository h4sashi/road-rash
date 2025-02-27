using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIScreen : MonoBehaviour
{
    public Screens screenType;
    [HideInInspector] public string screenName;

    void Awake()
    {
        Init();
    }

    public void Init()
    {
        screenName = screenType.ToString();
        Debug.Log(screenName);
    }

    void Start()
    {

    }

    void Update()
    {

    }
}
