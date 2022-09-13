using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

public class DragSticker : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    #region 변수
    [SerializeField]
    GameObject again; //다시해볼까요
    //ForCount스크립트 넣어주기
    [SerializeField]
    ForCount countClass; //ForCount스크립트
    [SerializeField]
    GameObject ex; //스티커를 붙여봐요
    [SerializeField]
    GameObject star; //완료용 별
    Vector3 startPosition;

    [SerializeField]
    GameObject invisible; //인터랙션 비활성화용 투명이미지

    public NewRandomTry randomTry;
    #endregion
    void Awake() //Answer스크립트에 AnswerIs함수보다 먼저 실행되기 위해 원래는 Start였다가 Start끼리 싸움나서 Start보다 먼저실행되는 Awake로 바꿔서 디버그해결
    {
        ex.SetActive(true);
        //이벤트 핸들러 방식으로 바꿈 : 람다식 형태로 더한 것
        countClass.Connect += (c, index) => //ForCount스크립트의 Connect이벤트핸들러에 람다식 함수(매개변수2개를 가지고 있는 이름없는 함수)를 넣어줌(+=)
        {
            if (index.transformIndex == transform.GetSiblingIndex()) //하위오브젝트들의 0번부터 시작하는 인덱스를 활용
            {
                print("잘했어요");
                StopAllCoroutines();
                gameObject.transform.position = index.gameObject.transform.position; //충돌된 오브젝트의 위치로 바꿔줌
                index.gameObject.SetActive(false);
                GetComponent<Image>().raycastTarget = false;
                again.SetActive(false);
            }
        };
        startPosition = transform.position; //위치 고정 //cf)startPosition = eventData.position;
        //함수 바구니에 어느 함수를 담을 것인가? : 선택의 문제 : 함수의 형태는 같아야 함 : 매개변수, 리턴 값이 같아야 함       
    }
    #region 함수
    public void OnBeginDrag(PointerEventData eventData)
    {
        transform.SetAsLastSibling(); //맨위로
        StopAllCoroutines();
    }
    public void OnDrag(PointerEventData eventData)
    {
        transform.position = eventData.pressEventCamera.ScreenToWorldPoint(new Vector3(eventData.position.x, eventData.position.y, 90)); //Screen Space - camera 설정시 Ondrag에서만 안보이는 현상:z값 맞춰주기
    }
    //드래그 종료시 충돌처리로 같은 텍스트로 된 자식오브젝트를 가지고 있는지로 맞는 위치 체크
    public void OnEndDrag(PointerEventData eventData)
    {
        GraphicRaycaster raycaster = transform.root.GetComponent<GraphicRaycaster>();
        List<RaycastResult> result = new List<RaycastResult>();
        raycaster.Raycast(eventData, result);
        foreach (var a in result)
        {
            if (a.gameObject != gameObject)
            {
                if (a.gameObject.GetComponentInChildren<Text>().text.Contains(gameObject.GetComponent<Image>().sprite.name)) //충돌된 오브젝트의 자식오브젝트 텍스트와 비교하여 같으면 카운트+1
                {
                    countClass.countParameter.gameObject = a.gameObject;
                    countClass.countParameter.transformIndex = transform.GetSiblingIndex();
                    int index = countClass.CountPro;
                    countClass.CountPro++;
                    star.transform.SetAsLastSibling();
                    star.SetActive(true);
                    star.transform.position = a.gameObject.transform.position;
                    invisible.SetActive(true); //밑에 줄에서 호출하는 별 지연함수보다 먼저 비활성화용 이미지를 활성화시켜야 별애니메이션이 나오자마자 인터랙션 비활성화가능
                    StartCoroutine(randomTry.Speed_forStar(index)); 
                }
                else
                {
                    StartCoroutine(Speed_forStiker()); //틀렸을 경우 다시 원위치로 천천히 이동
                    again.SetActive(true); //again이미지 활성화
                }
            }
            else
            {
                StartCoroutine(Speed_forStiker());
                again.SetActive(true); 
            }
            //Invoke("play", 1f); //invoke는 되도록 쓰지 말자
        }
        
    }
    void play() //invoke가 함수단위로 호출해서 임의로 함수로 만들어둔것
    {
        again.SetActive(false);
    }
    // 스티커 둘사이 거리를 재서 둘사이의 거리가 가까워졌을 때 조금 기다렸다가 가게끔해주는 지연 함수
    IEnumerator Speed_forStiker()   
    {
        while(Vector3.Distance(transform.position, startPosition) > 0) //둘사이의 거리가 있는 동안
        {
            transform.position = Vector3.Lerp(transform.position, startPosition, Time.deltaTime*10); 
            yield return new WaitForSeconds(Time.deltaTime);
            if(Vector3.Distance(transform.position, startPosition) <= 0)
            {
                break;
            }
        }
        transform.position = startPosition;
        yield break;
    }
    #endregion
}




