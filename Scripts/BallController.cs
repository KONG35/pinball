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
    private float maxPower = 20f;   // 최대 파워
    [SerializeField] 
    private float powerMultiplier = 0.1f; // 드래그 길이 보정
    [SerializeField] 
    private const float STOPTHRESHOLD = 0.05f; // 멈춘 것으로 간주하는 속도 임계값

    private bool isShooting = false;
    private bool isMoved = false; // 공이 움직였음을 체크하는 변수

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

        // 드래그 시작
        if (Input.GetMouseButtonDown(0))
        {
            dragStartPos = Input.mousePosition;
        }

        // 드래그 끝
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

        rigid.velocity = Vector3.zero; // 기존 속도 제거
        rigid.angularVelocity = Vector3.zero; // 회전 제거

        // 드래그 벡터와 길이 계산
        Vector2 dragVector = dragStartPos - dragEndPos;
        float dragDistance = dragVector.magnitude;

        Vector3 forceDirection = new Vector3(0, 0, 0);
        // 드래그 방향 설정
        if (dragVector.y < 0)
        {
            forceDirection = new Vector3(dragVector.x, 0f, dragVector.y).normalized;
        }
        else
        {
            forceDirection = new Vector3(dragVector.x, dragVector.y, dragVector.y).normalized;
        }


        // 파워 계산 (드래그 길이에 비례, 최대값 제한)
        float power = Mathf.Clamp(dragDistance * powerMultiplier, 0, maxPower);

        // 공에 힘 적용
        rigid.AddForce(forceDirection * power, ForceMode.Impulse);

        Debug.Log($"Drag Distance: {dragDistance}, Power: {power}, Direction: {forceDirection}");
        
        isShooting = true;
    }

    /// <summary>
    /// 터치 또는 마우스 입력을 무시해야 하는지 검사
    /// </summary>
    private bool IsIgnoreInput(Vector2 position, int fingerId)
    {
        // UI를 클릭했는지 확인
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject(fingerId))
        {
            return true;
        }

        // 게임이 일시정지 상태인지 확인
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
