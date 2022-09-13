using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
//주고 받을 이벤트 Args;
public class CountParameter : EventArgs
{
    public GameObject gameObject;
    public int count1;
    public int transformIndex;
}

public class ForCount : MonoBehaviour
{
    public event EventHandler<CountParameter> Connect; //<매개변수> : CountParameter라는 클래스를 매개변수로 하는 이벤트핸들러
    public CountParameter countParameter = new CountParameter(); //CountParameter클래스안에 있는 변수들을 사용하고자

    int count2;
    
    public int CountPro //몇개의 미션을 완수했는지 카운트하는 프로퍼티
    {
        get => count2; //읽어오기:count값을 읽어온다.
        set //쓰기:사용한다
        {
            count2 = value;
            countParameter.count1 = value;
            Connect?.Invoke(this, countParameter);
        }
    }
    
}

