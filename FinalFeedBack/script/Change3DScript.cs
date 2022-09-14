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
    [SerializeField]
    ForCount forCount; //정답처리 관련 미션 갯수 카운트용 스크립트
    [SerializeField]
    StarCountHandler starCase; //미션완료시 공통 별 프리팹
    [SerializeField]
    GameObject pico; //피코 캐릭터
    [SerializeField]
    GameObject hand; //드래그 유도용 손
    [SerializeField]
    GameObject invisible; //화면 비활성화용 투명 오브젝트
    [SerializeField]
    Image zoomPosition; //돋보기가 이동할 위치용
    private Vector3 posi; //3d오브젝트를 마우스드래그 따라 같이 움직이게 하기 위함용
    private float z_saved; //3d오브젝트를 마우스드래그 따라 같이 움직이게 하기 위함용 //x,y값만 가지고 있는 마우스의 위치에게 z값을 부여해주기 위한 저장용 변수
    new Collider collider; //충돌처리시 콜라이더 저장용
    GameObject savedOb; //드래그 중 돋보기에 충돌처리시 애니메이션을 껏다켯다 하기 위한 충돌 콜라이더 저장용 파라미터
    [SerializeField]
    GameObject startRayPosi; //돋보기 렌즈 한가운데에서 레이를 쏘는 위치용 돋보기의 자식 오브젝트
    [SerializeField]
    string obName; //이 씬의 자음 이름

    private Animator animatorPico;
    private Animator animatorColl;
    private Animator animatorSave;
    int count = 0; //충돌완료 체크용 파라미터
    bool playstart; //소리 재생 파일 바꿀 때 전에꺼가 재생중인지 아닌지 체크하는용 파라미터
    bool check; //마우스다운시에 처음에 한번만 성우 안내 목소리 나오게끔 하는 파라미터
    private void Awake()
    {
        StartCoroutine(Speed_StartZoom());
    }
    //돋보기가 씬 안으로 천천히 들어오게 만들어주는 지연 함수
    public IEnumerator Speed_StartZoom()
    {
        while (Vector3.Distance(transform.position, zoomPosition.transform.position) > 0.2f)//둘사이의 거리가 있는 동안 //첨에 0으로 했다가 너무 느려서 10으로 바꿈
        {
            transform.position = Vector3.Lerp(transform.position, zoomPosition.transform.position, Time.deltaTime * 0.7f);
            yield return new WaitForSeconds(Time.deltaTime*0.1f); //제자리로 돌아갈때 속도 조절하는 곳
            if (Vector3.Distance(transform.position, zoomPosition.transform.position) <= 0.2f)
            {
                break;
            }

        }
        transform.position = zoomPosition.transform.position;
        invisible.SetActive(false);
        hand.SetActive(true);
        yield break;
    }
    private void Start()
    {
        invisible.SetActive(true);
        SoundInterface.instance.SoundPlay(0);
        StartCoroutine(SoundCheck(1));
        animatorPico = pico.GetComponent<Animator>();
        check = true;
    }
    //앞의 소리 재생이 끝날때까지 잠시 지연했다가 다름 소리로 재생
    IEnumerator SoundCheck(int index)
    {
        yield return new WaitForSeconds(7);
        while(SoundInterface.instance.Source.isPlaying)
        {
            if (!SoundInterface.instance.Source.isPlaying)
            {
                SoundInterface.instance.SoundPlay(index);
                break;
            }
        }
        yield break;
    }
    private void OnMouseDown()
    {
        print("OnMouseDown");
        if (check)
        {
            if (!SoundInterface.instance.Source.isPlaying) SoundInterface.instance.SoundPlay(2);
        }
        check = false;
        hand.SetActive(false);
        transform.GetChild(0).GetComponent<Image>().gameObject.SetActive(true);
        z_saved = Camera.main.WorldToScreenPoint(gameObject.transform.position).z;
        posi = gameObject.transform.position - GetMouseWorldPosition();
        count = 0;
    }
    
    private void OnMouseDrag()
    {
        print("OnMouseDrag중");
        
        if (!SoundInterface.instance.Source.isPlaying)
        {
            SoundInterface.instance.SoundPlay(8);
        }
        transform.position = GetMouseWorldPosition() + posi;
        animatorPico.SetInteger("PicoAction", 4);
        collider = CheckOb();
        if(collider != null)
        {
            if (!playstart)
            {
                SoundInterface.instance.Source.Stop();
                SoundInterface.instance.SoundPlay(9);
                if (collider.GetComponentInChildren<AudioSource>())
                {
                    collider.GetComponentInChildren<AudioSource>().Play();
                }
                playstart = true;
            }
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
                if (animatorColl != null) animatorColl.SetInteger(collider.name + "Ani", 0);
                if(savedOb != null) animatorSave.SetInteger(savedOb.gameObject.name + "Ani", 0);
            }
        }
        else
        {
            playstart = false;
        }
    }
    //마우스 따라 움직이는 돋보기이동용
    Vector3 GetMouseWorldPosition()
    {
        Vector3 mousePoint = Input.mousePosition;
        mousePoint.z = z_saved;
        return Camera.main.ScreenToWorldPoint(mousePoint);
    }
    // 돋보기와 충돌처리용 함수
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
    private void OnMouseUp()
    {
        playstart = false;
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
                    //StartCoroutine(randomTry.Speed_forStar(index, collider.gameObject));
                    collider.gameObject.GetComponent<CapsuleCollider>().enabled = false;
                }
            }
            else
            {
                animatorPico.SetInteger("PicoAction", 2);
                StartCoroutine(Speed_forZoom());
            }
        }
        invisible.SetActive(false);
    }
    private void OnMouseExit()
    {
        playstart = false;
    }
    //틀렸을 경우에 돋보기가 제자리로 천천히 돌아가게 해주는 지연함수
    public IEnumerator Speed_forZoom()
    {
        SoundInterface.instance.SoundPlay(10);
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
