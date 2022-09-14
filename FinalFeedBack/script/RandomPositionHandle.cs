using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class RandomPositionHandle : MonoBehaviour
{
    [SerializeField]
    GameObject[] randomOb_inWater;
    [SerializeField]
    GameObject[] randomPosition_inWater;
    [SerializeField]
    GameObject[] randomOb_inLand;
    [SerializeField]
    GameObject[] randomPosition_inLand;
    [SerializeField]
    GameObject[] wrongOb;
    int plus = 0;
    // Start is called before the first frame update
    void Start()
    {
        System.Random random = new System.Random();
        var randomArray1 = Enumerable.Range(0, randomPosition_inWater.Length).ToArray();
        var randomArray2 = Enumerable.Range(0, randomOb_inWater.Length).ToArray();
        var shuffle1 = randomArray1.OrderBy(x => random.Next()).ToArray();
        var shuffle2 = randomArray2.OrderBy(x => random.Next()).ToArray();
        var randomArray5 = Enumerable.Range(0, wrongOb.Length).ToArray();
        var shuffle5 = randomArray5.OrderBy(x => random.Next()).ToArray();
        while (plus < 3)
        {
            if (plus == 0)
            {
                wrongOb[shuffle5[plus]].transform.position = randomPosition_inWater[shuffle1[plus]].transform.position;
                wrongOb[shuffle5[plus]].transform.rotation = randomPosition_inWater[shuffle1[plus]].transform.rotation;
                wrongOb[shuffle5[plus]].SetActive(true);
            }
            else
            {
                randomOb_inWater[shuffle2[plus]].transform.position = randomPosition_inWater[shuffle1[plus]].transform.position;
                randomOb_inWater[shuffle2[plus]].transform.rotation = randomPosition_inWater[shuffle1[plus]].transform.rotation;
                randomOb_inWater[shuffle2[plus]].SetActive(true);
            }
            plus++;
        }
        plus = 0;
        var randomArray3 = Enumerable.Range(0, randomPosition_inLand.Length).ToArray();
        var randomArray4 = Enumerable.Range(0, randomOb_inLand.Length).ToArray();
        var shuffle3 = randomArray3.OrderBy(x => random.Next()).ToArray();
        var shuffle4 = randomArray4.OrderBy(x => random.Next()).ToArray();
        while (plus < 2)
        {
            if (plus == 3)
            {
                wrongOb[shuffle5[plus]].transform.position = randomPosition_inLand[shuffle3[plus]].transform.position;
                wrongOb[shuffle5[plus]].transform.rotation = randomPosition_inLand[shuffle3[plus]].transform.rotation;
                wrongOb[shuffle5[plus]].SetActive(true);
            }
            else
            {
                randomOb_inLand[shuffle4[plus]].transform.position = randomPosition_inLand[shuffle3[plus]].transform.position;
                randomOb_inLand[shuffle4[plus]].transform.rotation = randomPosition_inLand[shuffle3[plus]].transform.rotation;
                randomOb_inLand[shuffle4[plus]].SetActive(true);
            }
            plus++;
        }
    }

}
