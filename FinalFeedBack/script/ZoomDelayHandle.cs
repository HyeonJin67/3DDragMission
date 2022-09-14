using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//지연함수들 따로 빼놓은 스크립트
public class ZoomDelayHandle : MonoBehaviour
{
    #region 변수
    [SerializeField]
    Image zoomPosition; //돋보기가 도착할 곳
    [SerializeField]
    GameObject invisible; //인터랙션 비활성화용 투명이미지
    [SerializeField]
    GameObject hand; //드래그를 유도하는 손 애니메이션용
    #endregion
    #region 지연합수
    private void Awake()
    {
        FindObjectOfType<DragMissionHandle>().Connection += StartZoom;
    }
    void StartZoom()
    {
        StartCoroutine(Speed_StartZoom());
        //if(case == 2)StartCoroutine(Speed_forZoom());
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
    /*
    //틀렸을 경우에 돋보기가 제자리로 천천히 돌아가게 해주는 지연함수
    public IEnumerator Speed_forZoom(Vector3 startposi)
    {
        //yield return new WaitForSeconds(0.5f); //0.5초정도 기다렸다가 이동
        while (Vector3.Distance(zoom.transform.position, startposi) > 10) //둘사이의 거리가 있는 동안 //첨에 0으로 했다가 너무 느려서 10으로 바꿈
        {
            zoom.transform.position = Vector3.Lerp(zoom.transform.position, startposi, Time.deltaTime*10); //출발하는 곳과 도착할 곳의 Lerp
            yield return new WaitForSeconds(Time.deltaTime * 2f); //제자리로 돌아갈때 속도 조절하는 곳
            if (Vector3.Distance(zoom.transform.position, startposi) <= 10)
            {
                break;
            }
        }
        zoom.transform.position = startposi;
        print("제자리로 돌아가기 완료");
        yield break;
    }
    */
    #endregion
}
