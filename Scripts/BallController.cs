using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.EventSystems;

public class BallController : MonoBehaviour
{
    private Vector2 dragStartPos;
    private Vector2 dragEndPos;
    private Rigidbody rigid;
    private const InGameStateFlag INPUTFLAG = InGameStateFlag.GameStart | InGameStateFlag.ShootEnd | InGameStateFlag.ShootReady;
    private IngameManager inGmanager;
    [SerializeField] 
    private float maxPower = 20f;   // �ִ� �Ŀ�
    [SerializeField] 
    private float powerMultiplier = 0.1f; // �巡�� ���� ����
    [SerializeField] 
    private const float STOPTHRESHOLD = 0.05f; // ���� ������ �����ϴ� �ӵ� �Ӱ谪

    private bool isShooting = false;
    private bool isMoved = false; // ���� ���������� üũ�ϴ� ����

    private void Start()
    {
        rigid = GetComponent<Rigidbody>();
        inGmanager = IngameManager.Instance;
    }

    private void Update()
    {
        if(Input.touchCount > 0 || Input.GetMouseButton(0) || Input.GetMouseButtonDown(0) || Input.GetMouseButtonUp(0))
        {
#if UNITY_EDITOR
            if (IsIgnoreInput(Input.mousePosition, -1))
                return;
#else
            Touch touch = Input.GetTouch(0);
            if (IsIgnoreInput(touch.position, touch.fingerId))
                return;
#endif
            if ((INPUTFLAG & inGmanager.currentIngameState) != 0)
            {
                inGmanager.ChangeState<ShootReadyState>();
                HandleInput();
            }

        }
    }
    private void FixedUpdate()
    {
        if (isShooting == false)
            return;

        if(rigid.velocity.magnitude > STOPTHRESHOLD)
        {
            isMoved = true;
        }
        if(isMoved && rigid.velocity.magnitude< STOPTHRESHOLD)
        {
            isShooting = false;
            isMoved = false;
            inGmanager.ChangeState<ShootEndState>();
        }
    }
    private void HandleInput()
    {
        if (inGmanager.currentIngameState != InGameStateFlag.ShootReady)
            return;

        // �巡�� ����
        if (Input.GetMouseButtonDown(0))
        {
            dragStartPos = Input.mousePosition;
        }

        // �巡�� ��
        if (Input.GetMouseButtonUp(0))
        {
            inGmanager.ChangeState<ShootingState>();
            dragEndPos = Input.mousePosition;
            ShootBall();
        }
    }
    private void ShootBall()
    {
        inGmanager.ball.chance -= 1;

        rigid.velocity = Vector3.zero; // ���� �ӵ� ����
        rigid.angularVelocity = Vector3.zero; // ȸ�� ����

        // �巡�� ���Ϳ� ���� ���
        Vector2 dragVector = dragStartPos - dragEndPos;
        float dragDistance = dragVector.magnitude;

        Vector3 forceDirection = new Vector3(0, 0, 0);
        // �巡�� ���� ����
        if (dragVector.y < 0)
        {
            forceDirection = new Vector3(dragVector.x, 0f, dragVector.y).normalized;
        }
        else
        {
            forceDirection = new Vector3(dragVector.x, dragVector.y, dragVector.y).normalized;
        }


        // �Ŀ� ��� (�巡�� ���̿� ���, �ִ밪 ����)
        float power = Mathf.Clamp(dragDistance * powerMultiplier, 0, maxPower);

        // ���� �� ����
        rigid.AddForce(forceDirection * power, ForceMode.Impulse);

        Debug.Log($"Drag Distance: {dragDistance}, Power: {power}, Direction: {forceDirection}");
        
        isShooting = true;
    }

    /// <summary>
    /// ��ġ �Ǵ� ���콺 �Է��� �����ؾ� �ϴ��� �˻�
    /// </summary>
    private bool IsIgnoreInput(Vector2 position, int fingerId)
    {
        // UI�� Ŭ���ߴ��� Ȯ��
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject(fingerId))
        {
            return true;
        }

        // ������ �Ͻ����� �������� Ȯ��
        if (inGmanager.isGamePaused)
        {
            return true;
        }

        return false;
    }

    private void OnCollisionEnter(Collision col)
    {
        if (col.transform.tag == "DontTouch")
        {
            IngameManager.Instance.ChangeState<GameOverState>();
        }
        else if (col.transform.tag == "MustTouch")
        {
            IngameManager.Instance.ChangeState<GameClearState>();
        }
    }
}
