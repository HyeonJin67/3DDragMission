using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
//랜덤 및 정답처리
public class GiyeokMissionManager : MonoBehaviour
{
    #region 변수
    [SerializeField]
    RandomStar randomStar; //Scriptable Object의 파일을 불러올 변수
    [Tooltip("씬별로 필요한 미션 완료 별의 갯수만큼만 넣어주세요")]
    [SerializeField]
    GameObject[] starPosi; //별이 이동할 위치
    [Tooltip("랜덤으로 바뀔 곳의 위치를 넣어주세요")]
    [SerializeField]
    Image[] randomPosi;
    [SerializeField]
    GameObject star; //완료용 별
    [SerializeField]
    GameObject invisible; //인터랙션 비활성화용 투명이미지
    [SerializeField]
    AudioSource narrations; //AudioSource는 컴포넌트, AudioClip은 오디오파일자체
    //int plus = 0;
    [Tooltip("이 씬의 자음의 이름을 입력해주세요 ex)Giyeok")]
    [SerializeField]
    string obName;
    #endregion
    //랜덤으로 이미지와 위치를 바꿔주는 스크립트
    void Start() 
    {


        //var randomArray = Enumerable.Range(0, randomPosi.Length).ToArray(); //자동으로 배열 형태로 범위내의 숫자들을 넣어준 sprite의 갯수 "순서대로" 넣어줌
        //var randomArrayGiyeok = Enumerable.Range(0, randomStar.EveryImages_Giyeok.Length).ToArray();
        //System.Random random = new System.Random(); //시스템에 있는 랜덤 생성
        ////OrderBy를 랜덤으로 하겠다는 의미 : 순서대로 이미 들어가 있는 거를 랜덤으로 섞어주게 됨
        //var shuffle1 = randomArray.OrderBy( x => random.Next()).ToArray(); //랜덤위치용 
        //var shuffle2 = randomArrayGiyeok.OrderBy( x => random.Next()).ToArray(); //ㄱ포함 단어용
        //while(plus < 2)
        //{
        //    //print("거리계산용"+Vector3.Distance(randomPosi[shuffle1[plus]].gameObject.transform.position, randomPosi[shuffle1[plus+1]].gameObject.transform.position));
        //    randomPosi[shuffle1[plus]].sprite = randomStar.EveryImages_Giyeok[shuffle2[plus]];
        //    Image im = randomPosi[shuffle1[plus]].GetComponent<Image>();
        //    Color col = im.color;
        //    col.a = 1f; //알파값 0에서 1로 변경ㄴ
        //    im.color = col;
        //    randomPosi[shuffle1[plus]].gameObject.name = "Contains_" + obName+ "_" + randomStar.EveryImages_Giyeok[shuffle2[plus]];
        //    plus++;
        //}
        //while (plus < 4)
        //{
        //    randomPosi[shuffle1[plus]].sprite = randomStar.EveryImages_Nieun[shuffle2[plus]];
        //    Image im = randomPosi[shuffle1[plus]].GetComponent<Image>();
        //    Color col = im.color;
        //    col.a = 1f;
        //    im.color = col;
        //    randomPosi[shuffle1[plus]].gameObject.name = "Contains_Nieun";
        //    plus++;
        //}
        //if(plus == 4)
        //{
        //    randomPosi[shuffle1[plus]].gameObject.SetActive(false); //랜덤 선정되지 않은 오브젝트는 충돌처리 및 애니메이션 플레이를 막기 위해 아예 꺼주기
        //}
    }
    #region 함수
    //오디오 재생시 모든 인터랙션(터치나 드래그) 비활성화용
    IEnumerator StartClip(int a) 
    {
        invisible.gameObject.SetActive(false);
        //narrations.Stop();
        //narrations.clip = randomStar.EveryAudios_Giyeok[a];
        //narrations.Play();
        //yield return new WaitUntil(() => !narrations.isPlaying); //yield return new WaitUntil(() => randomStar.EveryAudio[a].isPlaying); //오디오가 플레이될때까지 기다렸다가
        invisible.gameObject.SetActive(true); //오디오가 플레이되면 투명오브젝트 활성화
        yield break; //코루틴 끝내기가 있어줘야 안그럼 안끝나고 머물러있음
    }
    //미션성공시 별이 나타나서 화면에 정해진 위치로 가는 동안에 지연시켜주는 함수
    public IEnumerator Speed_forStar(int index1, GameObject ob)
    {
        star.transform.position = ob.gameObject.transform.position; //숨겨둔 별의 시작 위치가 충돌오브젝트에서 시작 되게끔 //별 하나를 가지고 돌려서씀
        star.transform.SetAsLastSibling();
        star.SetActive(true);
        invisible.transform.SetAsLastSibling();
        invisible.SetActive(true); //별이 움직이는 동안 비활성화용 투명이미지이용
        yield return new WaitForSeconds(1); //1초 정도 있다가 별 등장
        while (Vector3.Distance(star.transform.position, starPosi[index1].transform.position) > 0.05f)
        {
            star.transform.position = Vector3.Lerp(star.transform.position, starPosi[index1].transform.position, Time.deltaTime *2);
            yield return new WaitForSeconds(Time.deltaTime * 0.3f); //yield return new WaitForFixedUpdate();
            if (Vector3.Distance(star.transform.position, starPosi[index1].transform.position) <= 0.05f)
            {
                invisible.SetActive(false);
                break;
            }
        }
        //starPosi[index1].GetComponent<Image>().sprite = randomStar.star[0]; //투명외곽별을 노란별로 교체
        starPosi[index1].GetComponent<Image>().sprite = randomStar.Stars[0]; //투명외곽별을 노란별로 교체 //프로퍼티로 별 가져오기
        invisible.SetActive(false); //별의 움직임이 끝나면 인터랙션 다시 활성화
        star.SetActive(false);
        yield break;
    }
    #endregion
}
