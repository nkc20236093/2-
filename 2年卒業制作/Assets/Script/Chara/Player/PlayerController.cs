using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    Rigidbody rigid;
    Janken[] jankens = new Janken[3];
    int select = 0;
    [SerializeField] AudioSource audioSource;
    [SerializeField] GameObject Finger;
    LineRenderer lineRenderer;

    public class Janken
    {
        public virtual void HandEffect(AudioSource audio, Rigidbody rigid, Transform transform, GameObject Finger, LineRenderer lineRenderer) { }
    }

    public class Rock : Janken
    {
        public override void HandEffect(AudioSource audio, Rigidbody rigid, Transform transform, GameObject Finger, LineRenderer lineRenderer)
        {
            Debug.Log("�O�[");

            lineRenderer.enabled = false;

            // ����񂯂񔭓��L�[��j���Ɖ��肵��
            if (Input.GetKeyDown(KeyCode.J))
            {
                audio.Play();
                Collider[] hitColliders = Physics.OverlapSphere(transform.position, 100f);
                foreach (Collider col in hitColliders)
                {
                    if (col.CompareTag("Enemy"))
                    {
                        float distance = Vector3.Distance(new Vector3(transform.position.x, 0, transform.position.z), new Vector3(col.transform.position.x, 0, col.transform.position.z));
                        col.gameObject.GetComponent<EnemyCon>().Induction(transform.position, distance);
                    }
                }
            }
        }
    }

    public class Paper : Janken
    {
        public override void HandEffect(AudioSource audio, Rigidbody rigid, Transform transform, GameObject Finger, LineRenderer lineRenderer)
        {
            Debug.Log("�p�[");

            lineRenderer.enabled = false;

            float horizontalInput = Input.GetAxis("Horizontal");
            float verticalInput = Input.GetAxis("Vertical");

            Vector3 moveDirection = new Vector3(horizontalInput, 0, verticalInput).normalized;

            if (moveDirection != Vector3.zero)
            {
                float targetAngle = Mathf.Atan2(moveDirection.x, moveDirection.z) * Mathf.Rad2Deg + Camera.main.transform.eulerAngles.y;
                Vector3 moveDir = Quaternion.Euler(0, targetAngle, 0) * Vector3.forward;
                rigid.AddForce(moveDir * 5f);
            }
        }
    }

    public class Scissors : Janken
    {
        RaycastHit hit;
        float lazerDistance = 10f;
        bool LightHit = false;
        public override void HandEffect(AudioSource audio, Rigidbody rigid, Transform transform, GameObject Finger, LineRenderer lineRenderer)
        {
            Debug.Log("�`���L");
            // ����񂯂񔭓��L�[��j���Ɖ��肵��
            if (Input.GetKey(KeyCode.J)) 
            {
                lineRenderer.enabled = true;
                Vector3 direction = Finger.transform.InverseTransformDirection(Camera.main.transform.forward); // Finger�̃��[�J�����W�n�ɕϊ�
                Ray ray = new Ray(Finger.transform.position, direction);
                //Debug.DrawRay(ray.origin, ray.direction * lazerDistance, Color.red);

                lineRenderer.SetPosition(1, Finger.transform.InverseTransformPoint(Finger.transform.position + direction * lazerDistance)); // Finger�̃��[�J�����W�n�Őݒ�                
                if (Physics.Raycast(ray, out hit, lazerDistance))
                {
                    Debug.Log("�q�b�g");
                    float distance = Vector3.Distance(Finger.transform.position, hit.point);
                    lazerDistance = distance;
                    if (hit.collider.gameObject.CompareTag("LightGimmick") && Mathf.Approximately(distance, lazerDistance))
                    {
                        LightHit = true;
                        if (LightHit)
                        {
                            // �q�b�g������쓮
                            hit.collider.gameObject.GetComponent<LightGimmick>().LightHit();
                        }
                        Debug.Log("�q�b�g");
                    }
                    else
                    {
                        lazerDistance = 10.0f;
                        LightHit = false;
                    }
                }
            }
            else
            {
                lineRenderer.enabled = false;
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        Camera.main.transform.localEulerAngles = Vector3.zero;
        rigid = GetComponent<Rigidbody>();
        jankens[0] = new Rock();
        jankens[1] = new Scissors();
        jankens[2] = new Paper();
        lineRenderer = Finger.GetComponent<LineRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        CameraCon();
        Action();
    }
    void Action()
    {
        // ���Ƀ`���L��i�A�O�[��u�A�p�[��p�Ƃ����Ƃ�
        if (Input.GetKeyDown(KeyCode.U))
        {
            select = 0;
        }
        else if (Input.GetKeyDown(KeyCode.I))
        {
            select = 1;
        }
        else if (Input.GetKeyDown(KeyCode.P))
        {
            select = 2;
        }
        jankens[select].HandEffect(audioSource, rigid, transform, Finger, lineRenderer);
    }

    void CameraCon()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            Camera.main.transform.localEulerAngles = Vector3.zero;
        }
        else
        {
            Vector3 direction = Camera.main.transform.localEulerAngles;
            direction.x -= Input.GetAxis("Mouse Y") * 2.0f;
            direction.y += Input.GetAxis("Mouse X") * 2.0f;

            // �C���O��Y���̓�����ۑ�
            float CameraY = direction.y;

            if (direction.x > 180)
            {
                direction.x -= 360;
            }

            direction.x = Mathf.Clamp(direction.x, -15, 45);
            direction.z = 0;
            Camera.main.transform.localEulerAngles = direction;

            // �v���C���[�̌������J�����̌����ɍ��킹��
            transform.eulerAngles = new Vector3(0, CameraY, 0);
        }
    }
}
