using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;
//전체미션을 모두 완수하여 카운트값이 얼마일때 참잘했어요를 띄우는
public class Answer : MonoBehaviour
{
    [SerializeField]
    GameObject pico;
    [SerializeField]
    GameObject zoom;
    [SerializeField]
    Image zoomPosition;
    [SerializeField]
    GameObject invisible;
    [SerializeField]
    GameObject star;
    Animator animator;
    void Start()
    {
        GetComponent<ForCount>().Connect += AnswerIs;
    }
    void AnswerIs(object sender, CountParameter e)
    {
        if (e.count1 == 3)
        {
            SoundInterface.instance.SoundPlay(5);
            //SoundInterface.instance.SoundPlay(6);
            print("미션끝 : 피토 win(hi-host) 애니메이션 플레이");
            StartCoroutine(Delay_forZoom());
            animator = pico.GetComponent<Animator>();
            animator.SetInteger("PicoAction", 3); //피코애니메이션 중에 3번 hi-host켜기
            star.SetActive(false);
        }
    }
    //미션완수시 돋보기가 조금 천천히 제자리로 가게끔
    IEnumerator Delay_forZoom()
    {
        yield return new WaitForSeconds(2.5f);
        zoom.transform.position = zoomPosition.transform.position;
        zoom.GetComponent<CapsuleCollider>().enabled = false;
        yield break;
    }
}
