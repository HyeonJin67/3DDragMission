using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class PicoWalkHandle : MonoBehaviour
{
    [SerializeField]
    GameObject picoPosition;
    private Animator animator;
    int[] clickArray = { 6, 7, 8, 9, 10 };
    System.Random random = new System.Random();

    void Start()
    {
        animator = GetComponent<Animator>();
        animator.SetInteger("PicoAction", 5);
        StartCoroutine(Speed_forPico());
    }
    //피코 걸어서 씬에 천천히 들어오게끔 해주는 지연 함수
    IEnumerator Speed_forPico()
    {
        while (Vector3.Distance(transform.position, picoPosition.transform.position) > 0.2f) //둘사이의 거리가 있는 동안 //첨에 0으로 했다가 너무 느려서 10으로 바꿈
        {
            transform.position = Vector3.Lerp(transform.position, picoPosition.transform.position, Time.deltaTime * 0.7f);
            yield return new WaitForSeconds(Time.deltaTime);
            if (Vector3.Distance(transform.position, picoPosition.transform.position) <= 0.2f)
            {
                break;
            }
        }
        transform.position = picoPosition.transform.position;
        transform.Rotate(0, -70, 0);
        animator.SetInteger("PicoAction", 4); //피코애니메이션 중에 4번 두리번두리번켜기
        yield break;
    }
    private void OnMouseDown()
    {
        print("피코 클릭2");
        int[] shuffle = clickArray.OrderBy(x => random.Next()).ToArray();
        animator.SetInteger("PicoAction", shuffle[0]);
    }
}
