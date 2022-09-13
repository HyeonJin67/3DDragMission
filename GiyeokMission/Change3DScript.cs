using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;
using System;
// https://flowtree.tistory.com/46
// https://wolstar.tistory.com/5

public class Change3DScript : MonoBehaviour
{
    public GiyeokMissionManager randomTry;
    [SerializeField]
    ForCount forCount;
    StarCountHandler starCase;
    int count = 0;
    [SerializeField]
    GameObject pico;
    private Animator animatorPico;
    private Animator animatorColl;
    private Animator animatorSave;
    [SerializeField]
    GameObject hand;
    [SerializeField]
    GameObject invisible;
    [SerializeField]
    Image zoomPosition;
    private Vector3 posi;
    private float z_saved;
    new Collider collider;
    GameObject ob;
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
    GameObject savedOb;
    [SerializeField]
    GameObject startRayPosi;
    [SerializeField]
    string obName;
    Vector3 startposi;
    private void Start()
    {
        starCase = FindObjectOfType<StarCountHandler>();
        SoundInterface.instance.SoundPlay(0);
        animatorPico = pico.GetComponent<Animator>();
        StartCoroutine(Speed_StartZoom());
        forCount.Connect += (c, index) => { };
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
    private void OnMouseDown()
    {
        print("OnMouseDown");
        SoundInterface.instance.SoundPlay(2);
        hand.SetActive(false);
        transform.GetChild(0).GetComponent<Image>().gameObject.SetActive(true);
        z_saved = Camera.main.WorldToScreenPoint(gameObject.transform.position).z;
        posi = gameObject.transform.position - GetMouseWorldPosition();
        count = 0;
    }

    private void OnMouseDrag()
    {
        print("OnMouseDrag중");
        SoundInterface.instance.SoundPlay(8);
        transform.position = GetMouseWorldPosition() + posi;
        animatorPico.SetInteger("PicoAction", 4);

        collider = CheckOb();
        if(collider != null)
        {
            SoundInterface.instance.SoundPlay(9);
            animatorColl = collider.gameObject.GetComponent<Animator>();
            if (collider.transform.parent.name.Contains(obName))
            {
                if (savedOb != null && savedOb.GetComponent<CapsuleCollider>().enabled && savedOb.transform.parent.name.Contains(obName))
                {
                    animatorSave = savedOb.GetComponent<Animator>();
                    animatorSave.SetInteger(savedOb.gameObject.name + "Ani", 0);
                }
                savedOb = collider.gameObject;
                animatorColl = collider.gameObject.GetComponent<Animator>();
                animatorColl.SetInteger(collider.name + "Ani", 2);
            }
            else
            {
                if(animatorColl != null) animatorColl.SetInteger(collider.name + "Ani", 0);
                if(savedOb != null) animatorSave.SetInteger(savedOb.gameObject.name + "Ani", 0);
            }
        }
    }
    Collider CheckOb()
    {
        Ray ray = new Ray(startRayPosi.transform.position, transform.forward);
        if (Physics.SphereCast(ray, 0.1f, out RaycastHit hit))
        {
            print("충돌처리 감지" + hit.collider);
            return hit.collider;
        }
        else return null;
    }
    Vector3 GetMouseWorldPosition()
    {
        Vector3 mousePoint = Input.mousePosition;
        mousePoint.z = z_saved;
        return Camera.main.ScreenToWorldPoint(mousePoint);
    }
    private void OnMouseUp()
    {
        print("OnMouseUp");
        collider = CheckOb();
        if (collider != null)
        {
            if (collider.transform.parent.name.Contains(obName))
            {
                SoundInterface.instance.SoundPlay(3);
                invisible.transform.SetAsLastSibling();
                invisible.SetActive(true);
                count++;
                if (count == 1)
                {
                    print("ㄱ포함단어 확인");
                    animatorPico.SetInteger("PicoAction", 1);
                    animatorColl.SetInteger(collider.name + "Ani", 1);
                    starCase.SetStarScore();
                    forCount.countParameter.gameObject = collider.gameObject;
                    forCount.countParameter.transformIndex = transform.GetSiblingIndex();
                    int index = forCount.CountPro;
                    forCount.CountPro++;
                    StartCoroutine(randomTry.Speed_forStar(index, collider.gameObject));
                    collider.gameObject.GetComponent<CapsuleCollider>().enabled = false;
                }
            }
            else
            {
                SoundInterface.instance.SoundPlay(4);
                animatorPico.SetInteger("PicoAction", 2);
                StartCoroutine(Speed_forZoom());
            }
        }
    }
    //돋보기가 씬 안으로 천천히 들어오게 만들어주는 지연 함수
    public IEnumerator Speed_StartZoom()
    {
        invisible.SetActive(true);
        while (Vector3.Distance(transform.position, zoomPosition.transform.position) > 0.2f)//둘사이의 거리가 있는 동안 //첨에 0으로 했다가 너무 느려서 10으로 바꿈
        {
            transform.position = Vector3.Lerp(transform.position, zoomPosition.transform.position, Time.deltaTime * 0.7f);
            yield return new WaitForSeconds(Time.deltaTime); //제자리로 돌아갈때 속도 조절하는 곳
            if (Vector3.Distance(transform.position, zoomPosition.transform.position) <= 0.2f)
            {
                break;
            }
        }
        transform.position = zoomPosition.transform.position;
        invisible.SetActive(false);
        hand.SetActive(true);
        SoundInterface.instance.SoundPlay(1);
        yield break;
    }
    
    //틀렸을 경우에 돋보기가 제자리로 천천히 돌아가게 해주는 지연함수
    public IEnumerator Speed_forZoom()
    {
        //yield return new WaitForSeconds(0.5f); //0.5초정도 기다렸다가 이동
        while (Vector3.Distance(transform.position, zoomPosition.transform.position) > 0.2f) //둘사이의 거리가 있는 동안 //첨에 0으로 했다가 너무 느려서 10으로 바꿈
        {
            transform.position = Vector3.Lerp(transform.position, zoomPosition.transform.position, Time.deltaTime*10); //출발하는 곳과 도착할 곳의 Lerp
            yield return new WaitForSeconds(Time.deltaTime * 0.2f); //제자리로 돌아갈때 속도 조절하는 곳
            if (Vector3.Distance(transform.position, zoomPosition.transform.position) <= 0.2f)
            {
                break;
            }
        }
        transform.position = zoomPosition.transform.position;
        print("제자리로 돌아가기 완료");
        yield break;
    }
    
}
