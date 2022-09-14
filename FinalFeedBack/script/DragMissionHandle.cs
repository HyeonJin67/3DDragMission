using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.Events;
using System;
//돋보기에 들어갈 드래그기능 총괄 스크립트
public class DragMissionHandle : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    #region 변수
    [SerializeField]
    ForCount forCount; //정답처리 관련 미션 갯수 카운트용 스크립트
    public GiyeokMissionManager randomTry; //랜덤과 정답처리 관련, 소리 설정을 담당하는 스크립트 //Scriptable Object에는 넣지 못하는 스크립트에서 필요로 하는 오브젝트들을 가지고 있음
    public ZoomDelayHandle delayFunc; //지연관련 함수 모음 스크립트
    [SerializeField]
    GameObject invisible; //인터랙션 비활성화용 투명이미지
    [SerializeField]
    GameObject pico;
    [SerializeField]
    Image zoomPosition; //돋보기가 도착할 곳
    [SerializeField]
    GameObject hand; //드래그를 유도하는 손 애니메이션용
    [SerializeField]
    Image cloud;
    [SerializeField]
    Image cloudPosition; //구름이 찍고 돌아오는 곳의 위치
    [Tooltip("이 씬의 자음의 이름을 입력해주세요 ex)Giyeok")]
    [SerializeField]
    string obName; //오브젝트를 판별할 자음의 이름을 입력받는 곳
    Vector3 startPosition; //돋보기가 원래 자리로 돌아가기 위한 처음 위치 저장용
    int count = 0; //드래그가 끝나도 기역포함 단어를 판별할 수 있는 용도
    private Animator animator; //특정 애니메이션을 지정해주기 위한 용도
    GameObject savedOb;
    #endregion
    #region 이벤트
    public event Action Connection;
    #endregion
    void Start() //Answer스크립트에 AnswerIs함수보다 먼저 실행되기 위해 원래는 Start였다가 Start끼리 싸움나서 Start보다 먼저실행되는 Awake로 바꿔서 디버그해결
    {
        SoundInterface.instance.SoundPlay(0);
        StartCoroutine(Speed_forCloud());
        animator = pico.GetComponent<Animator>();
        animator.SetInteger("PicoAction", 5); //피코애니메이션 중에 5번 Walk켜기
        Connection?.Invoke();
        //이벤트 핸들러 방식으로 바꿈 : 람다식 형태로 더한 것
        forCount.Connect += (c, index) => { }; //ForCount스크립트의 Connect이벤트핸들러에 람다식 함수(매개변수2개를 가지고 있는 이름없는 함수)를 넣어줌(+=)
        //startPosition = zoomPosition.transform.position; //위치 고정 //cf)startPosition = eventData.position;
        //함수 바구니에 어느 함수를 담을 것인가? : 선택의 문제 : 함수의 형태는 같아야 함 : 매개변수, 리턴 값이 같아야 함       
    }
    #region 함수
    public void OnBeginDrag(PointerEventData eventData)
    {
        SoundInterface.instance.SoundPlay(2);
        count = 0; //드래그가 끝나고 다시 드래그가 시작될 때도 "ㄱ포함단어를 만날때마자" 충돌체크를 위한 count값 지정
        hand.SetActive(false);
        //StopAllCoroutines();
        animator = pico.GetComponent<Animator>();
        animator.SetInteger("PicoAction", 4); //피코애니메이션 중에 4번 idle(두리번두리번)켜기
        GetComponent<Animator>().enabled = true; //드래그 시작시 돋보기가 커지는 애니메이션 켜주기
        transform.GetChild(0).GetComponent<Image>().gameObject.SetActive(true); //마스크씌워둔 첫번째 자식오브젝트 켜서 드래그가 시작되면 돋보기 주면 어둡게 만들기
    }
    public void OnDrag(PointerEventData eventData)
    {
        //드래그 중 오브젝트 따라오게끔
        transform.position = eventData.pressEventCamera.ScreenToWorldPoint(new Vector3(eventData.position.x, eventData.position.y, 1)); //Screen Space - camera 설정시 Ondrag에서만 안보이는 현상:z값(camera의 ) 고정
        GameObject ob = MeetObject(eventData); //충돌체크하는 함수 호출
        if (ob != null)
        {
            if (ob.gameObject.name.Contains(obName)) //입력받은 자음을 포함하는 단어를 골랐을 경우
            {
                if(savedOb != null && savedOb.GetComponent<Image>().raycastTarget == true && savedOb.gameObject.name.Contains(obName))
                {
                    animator = savedOb.GetComponent<Animator>();
                    animator.SetInteger("BigAction", 2);
                }
                savedOb = ob;
                animator = ob.GetComponent<Animator>();
                animator.SetInteger("BigAction", 1); //맞췄을 때 재생되는 흔들흔들 애니메이션 지정 및 재생
                //print("savedOb1" + savedOb);
            }
            else if (savedOb != null && savedOb.GetComponent<Image>().raycastTarget == true)
            {
                print("savedOb2" + savedOb);
                animator = savedOb.GetComponent<Animator>();
                animator.SetInteger("BigAction", 2);
            }
        }
    }
    //드래그 종료시 충돌처리로 같은 텍스트로 된 자식오브젝트를 가지고 있는지로 맞는 위치 체크
    public void OnEndDrag(PointerEventData eventData)
    {
        GameObject ob = MeetObject(eventData); //충돌체크하는 함수 호출
        if (ob != null)
        {
            print("충돌체크" + ob.name);
            if (ob.gameObject.name.Contains(obName)) //입력받은 자음을 포함하는 단어를 골랐을 경우
            {
                SoundInterface.instance.SoundPlay(3);
                //SoundInterface.instance.SoundPlay(6);
                invisible.transform.SetAsLastSibling(); 
                invisible.SetActive(true); //맞춘단어의 애니메이션과 별 이동이 나오는 동안 비활성화용
                count++;
                if (count == 1)
                {
                    print("ㄱ포함단어 확인");
                    if (ob.gameObject.name.Contains("Cloud")) //구름이 하늘에서 움직이는거 멈추기 위함
                    {
                        //StopCoroutine(Speed_forCloud()); //StopCoroutine("Speed_forCloud"); //"같은 스크립트"에 있는 "특정" 코루틴 끄기 //근데 안됨
                        StopAllCoroutines(); //"같은 스크립트"에서 실행중인 모든 코루틴 끄기
                    }
                    animator = pico.GetComponent<Animator>();
                    animator.SetInteger("PicoAction", 1); //피코애니메이션 중에 1번 win켜기
                    animator = ob.GetComponent<Animator>();
                    animator.SetInteger("BigAction", 1); //맞췄을 때 재생되는 흔들흔들 애니메이션 지정 및 재생
                    //이벤트핸들러로 단어갯수 카운트
                    forCount.countParameter.gameObject = ob.gameObject;
                    forCount.countParameter.transformIndex = transform.GetSiblingIndex();
                    int index = forCount.CountPro;
                    forCount.CountPro++;
                    StartCoroutine(randomTry.Speed_forStar(index, ob)); //맞았을 때의 별 부르기
                    ob.GetComponent<Image>().raycastTarget = false; //맞췄을 때 더이상 충돌처리되서 인식되지 않게 
                }
            }
            else //틀린단어를 골랐을 경우
            {
                SoundInterface.instance.SoundPlay(4);
                //Connection += 
                //SoundInterface.instance.SoundPlay(7);
                animator = pico.GetComponent<Animator>();
                animator.SetInteger("PicoAction", 2); //피코애니메이션 중에 2번 lose켜기
                //StartCoroutine(delayFunc.Speed_forZoom(startPosition));
            }
        }
    }

    //충돌 체크를 위한 함수
    GameObject MeetObject(PointerEventData eventData)
    {
        GraphicRaycaster raycaster = transform.root.GetComponent<GraphicRaycaster>();
        List<RaycastResult> result = new List<RaycastResult>();
        raycaster.Raycast(eventData, result);
        foreach (var a in result)
        {
            if (a.gameObject != gameObject) //자기자신이 아닐 경우에만
            {
                print("자기자신이 아닌거");
                return a.gameObject;
            }
        }
        return null;
    }
    //구름이 하늘에서 움직이는 애니메이션 대신 쓰는 지연 함수
    public IEnumerator Speed_forCloud()
    {
        yield return new WaitForSeconds(0.5f);
        while (Vector3.Distance(cloud.transform.position, cloudPosition.transform.position) > 0.7f)
        {
            cloud.transform.position = Vector3.Lerp(cloud.transform.position, cloudPosition.transform.position, Time.deltaTime * 0.7f);
            yield return new WaitForSeconds(Time.deltaTime * 15f);
            if (Vector3.Distance(cloud.transform.position, cloudPosition.transform.position) <= 0.7f)
            { break; }
        }
        cloud.transform.position = cloudPosition.transform.position;
        yield break;
    }
    
    #endregion
}
