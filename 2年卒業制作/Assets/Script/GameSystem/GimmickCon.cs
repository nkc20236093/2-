using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GimmickCon : MonoBehaviour
{
    originGimmick[] originGimmicks = new originGimmick[4];
    [SerializeField] int gimickNumber;

    [Header("�z�d�՗plinerenderer\n�ԁA�΁A���̏���")]
    [SerializeField] GameObject[] pillers;
    [SerializeField] GameObject[] cableObj;
    [SerializeField] LineRenderer[] cables;
    [Header("�ŏ��̈ʒu�A���Z�b�g���邽�тɐݒ�\n���̈ʒu�Ƃ���")]
    [SerializeField] Vector3[] firstSetPosition;
    public class originGimmick
    {
        protected int myNumber = 0;
        public originGimmick(){}
        public virtual void Operation() { }
    }
    public class Gimmick01 : originGimmick
    {
        public Gimmick01()
        {

        }
        public override void Operation()
        {
            Debug.Log("���C�g�q�b�g!");
        }
    }
    public class Gimmick02 : originGimmick
    {
        Vector3[] firstPosition;
        GameObject[] cables;
        LineRenderer[] colorLineRenderer = new LineRenderer[3];
        GameObject[] pillers;

        bool first = false;
        bool rayHit = false;
        int colorNumber = 0;
        RaycastHit hit;
        public Gimmick02(LineRenderer[] lineRenderer, GameObject[] cables, Vector3[] first, GameObject[] pillers)
        {
            colorLineRenderer = lineRenderer;
            this.cables = cables;
            firstPosition = first;
            this.pillers = pillers;
        }
        public override void Operation()
        {
            Debug.Log("����Ƃ�");
            // ����Escape�L�[����������I��
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                for (int i = 0; i < cables.Length; i++)
                {
                    colorLineRenderer[i].enabled = false;
                    pillers[i].SetActive(false);
                }
                // �v���C���[������\�ɖ߂�
                PlayerController.stop = false;
                first = false;
            }
            else
            {
                // �N�����Ɉ�񂾂����s
                if (!first)
                {
                    for (int i = 0; i < cables.Length; i++)
                    {
                        if (!colorLineRenderer[i].enabled)
                        {
                            colorLineRenderer[i].enabled = true;
                        }
                        colorLineRenderer[i].SetPosition(colorLineRenderer[i].positionCount - 1, firstPosition[i]);
                        pillers[i].SetActive(true);
                    }
                    first = true;
                }

                // ����F��H�ŘM�����ς���Ƃ�����
                if (Input.GetKeyDown(KeyCode.H))
                {
                    colorNumber++;
                }
                else if (Input.GetKeyDown(KeyCode.F))
                {
                    colorNumber--;
                }
                colorNumber = Mathf.Clamp(colorNumber, 0, 2);

                float value = colorLineRenderer[colorNumber].GetPosition(colorLineRenderer[colorNumber].positionCount - 1).x;
                // ����M��B�Ő��𓮂����Ƃ�����
                if (Input.GetKey(KeyCode.M))
                {
                    value ++;
                }
                else if (Input.GetKey(KeyCode.B))
                {
                    value --;
                }
                value = Mathf.Clamp(value, -100, 100);
                colorLineRenderer[colorNumber].SetPosition(colorLineRenderer[colorNumber].positionCount - 1, new Vector3(value, 150, 0));

                Vector3 startPos = colorLineRenderer[colorNumber].GetPosition(0);
                Vector3 endPos = colorLineRenderer[colorNumber].GetPosition(colorLineRenderer[colorNumber].positionCount - 1);
                Vector3 localStartPos = colorLineRenderer[colorNumber].transform.TransformPoint(colorLineRenderer[colorNumber].GetPosition(0));
                Vector3 localEndPos = colorLineRenderer[colorNumber].transform.TransformPoint(colorLineRenderer[colorNumber].GetPosition(colorLineRenderer[colorNumber].positionCount - 1));
                Vector3 direction = (localEndPos - localStartPos).normalized;

                Ray ray = new Ray(localStartPos, direction);
                Debug.DrawRay(ray.origin, ray.direction * 1000000, Color.blue);
                // Ray���q�b�g�������W�����[�J�����W�ɕϊ����Ēǉ�
                if (Physics.Raycast(ray, out hit, LayerMask.GetMask("Pillar")))
                {
                    Vector3[] points = new Vector3[colorLineRenderer[colorNumber].positionCount + 1];
                    points[0] = startPos;
                    // �q�b�g�������W�����[�J�����W�ɕϊ�
                    Vector3 localHitPoint = hit.transform.InverseTransformPoint(hit.point * 100); 
                    Debug.Log(localHitPoint);
                    points[1] = new Vector3(localHitPoint.x, localHitPoint.x, 0);
                    points[2] = endPos;
                    colorLineRenderer[colorNumber].positionCount = 3;
                    colorLineRenderer[colorNumber].SetPositions(points);
                }
                else
                {
                    Vector3[] points = new Vector3[2] { startPos, endPos };
                    colorLineRenderer[colorNumber].positionCount = 2;
                    colorLineRenderer[colorNumber].SetPositions(points);
                }
                return;
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        originGimmicks[0] = new Gimmick01();
        if (gimickNumber == 1)
        {
            originGimmicks[1] = new Gimmick02(cables, cableObj, firstSetPosition, pillers);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void LightHit()
    {
        originGimmicks[gimickNumber].Operation();
    }
}
