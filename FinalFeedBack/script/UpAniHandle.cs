using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//씬 시작시 단어들이 파도 위로 슥 올라오는 애니메이션대신 쓰이는 지연함수
public class UpAniHandle : MonoBehaviour
{
    [Tooltip("각각의 위치에 맞는 위로 올라갈 위치 오브젝트를 넣어주세요")]
    [SerializeField]
    Image upPosi;
    void Start()
    {
        StartCoroutine(Speed_forWord());
    }
    IEnumerator Speed_forWord()
    {
        //yield return new WaitForSeconds(0.5f);
        while (Vector3.Distance(transform.position, upPosi.transform.position) > 0.7f)
        {
            transform.position = Vector3.Lerp(transform.position, upPosi.transform.position, Time.deltaTime * 0.7f);
            yield return new WaitForSeconds(Time.deltaTime * 0.8f);
            if (Vector3.Distance(transform.position, upPosi.transform.position) <= 0.7f)
            { break; }
        }
        transform.position = upPosi.transform.position;
        yield break;
    }
}
