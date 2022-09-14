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
    ForCount forCount; //����ó�� ���� �̼� ���� ī��Ʈ�� ��ũ��Ʈ
    [SerializeField]
    StarCountHandler starCase; //�̼ǿϷ�� ���� �� ������
    [SerializeField]
    GameObject pico; //���� ĳ����
    [SerializeField]
    GameObject hand; //�巡�� ������ ��
    [SerializeField]
    GameObject invisible; //ȭ�� ��Ȱ��ȭ�� ���� ������Ʈ
    [SerializeField]
    Image zoomPosition; //�����Ⱑ �̵��� ��ġ��
    private Vector3 posi; //3d������Ʈ�� ���콺�巡�� ���� ���� �����̰� �ϱ� ���Կ�
    private float z_saved; //3d������Ʈ�� ���콺�巡�� ���� ���� �����̰� �ϱ� ���Կ� //x,y���� ������ �ִ� ���콺�� ��ġ���� z���� �ο����ֱ� ���� ����� ����
    new Collider collider; //�浹ó���� �ݶ��̴� �����
    GameObject savedOb; //�巡�� �� �����⿡ �浹ó���� �ִϸ��̼��� �����ִ� �ϱ� ���� �浹 �ݶ��̴� ����� �Ķ����
    [SerializeField]
    GameObject startRayPosi; //������ ���� �Ѱ������ ���̸� ��� ��ġ�� �������� �ڽ� ������Ʈ
    [SerializeField]
    string obName; //�� ���� ���� �̸�

    private Animator animatorPico;
    private Animator animatorColl;
    private Animator animatorSave;
    int count = 0; //�浹�Ϸ� üũ�� �Ķ����
    bool playstart; //�Ҹ� ��� ���� �ٲ� �� �������� ��������� �ƴ��� üũ�ϴ¿� �Ķ����
    bool check; //���콺�ٿ�ÿ� ó���� �ѹ��� ���� �ȳ� ��Ҹ� �����Բ� �ϴ� �Ķ����
    private void Awake()
    {
        StartCoroutine(Speed_StartZoom());
    }
    //�����Ⱑ �� ������ õõ�� ������ ������ִ� ���� �Լ�
    public IEnumerator Speed_StartZoom()
    {
        while (Vector3.Distance(transform.position, zoomPosition.transform.position) > 0.2f)//�ѻ����� �Ÿ��� �ִ� ���� //÷�� 0���� �ߴٰ� �ʹ� ������ 10���� �ٲ�
        {
            transform.position = Vector3.Lerp(transform.position, zoomPosition.transform.position, Time.deltaTime * 0.7f);
            yield return new WaitForSeconds(Time.deltaTime*0.1f); //���ڸ��� ���ư��� �ӵ� �����ϴ� ��
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
    //���� �Ҹ� ����� ���������� ��� �����ߴٰ� �ٸ� �Ҹ��� ���
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
        print("OnMouseDrag��");
        
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
    //���콺 ���� �����̴� �������̵���
    Vector3 GetMouseWorldPosition()
    {
        Vector3 mousePoint = Input.mousePosition;
        mousePoint.z = z_saved;
        return Camera.main.ScreenToWorldPoint(mousePoint);
    }
    // ������� �浹ó���� �Լ�
    Collider CheckOb()
    {
        Ray ray = new Ray(startRayPosi.transform.position, transform.forward);
        if (Physics.SphereCast(ray, 0.1f, out RaycastHit hit))
        {
            print("�浹ó�� ����" + hit.collider);
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
                    print("�����Դܾ� Ȯ��");
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
    //Ʋ���� ��쿡 �����Ⱑ ���ڸ��� õõ�� ���ư��� ���ִ� �����Լ�
    public IEnumerator Speed_forZoom()
    {
        SoundInterface.instance.SoundPlay(10);
        //yield return new WaitForSeconds(0.5f); //0.5������ ��ٷȴٰ� �̵�
        while (Vector3.Distance(transform.position, zoomPosition.transform.position) > 0.2f) //�ѻ����� �Ÿ��� �ִ� ���� //÷�� 0���� �ߴٰ� �ʹ� ������ 10���� �ٲ�
        {
            transform.position = Vector3.Lerp(transform.position, zoomPosition.transform.position, Time.deltaTime*10); //����ϴ� ���� ������ ���� Lerp
            yield return new WaitForSeconds(Time.deltaTime * 0.2f); //���ڸ��� ���ư��� �ӵ� �����ϴ� ��
            if (Vector3.Distance(transform.position, zoomPosition.transform.position) <= 0.2f)
            {
                break;
            }
        }
        transform.position = zoomPosition.transform.position;
        print("���ڸ��� ���ư��� �Ϸ�");
        yield break;
    }
    
}