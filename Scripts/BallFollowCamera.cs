using UnityEngine;

public class BallFollowCamera : MonoBehaviour
{
    private Vector3 offset = new Vector3(0, 3, -5); // 공과의 거리
    private Vector3 velocity = Vector3.zero; // 속도 벡터
    
    [SerializeField]
    private Transform ball; // 추적할 공
    [SerializeField]
    private float followSpeed = 5f; // 카메라 따라가는 속도
    [SerializeField]
    private float smoothTime = 0.2f; // 부드럽게 이동하는 시간


    void LateUpdate()
    {
        if (ball == null) return;

        Vector3 targetPosition = ball.position + offset;

        // 공의 위치가 일정 범위 이상으로만 카메라가 이동하도록 제한
        float distance = Vector3.Distance(transform.position, targetPosition);

        // 0.1f 거리 차가 있어야지 이동
        if (distance > 0.1f)
        {
            // 부드럽게 이동 (SmoothDamp)
            transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);
        }

        // 공을 바라보도록 회전
        transform.LookAt(ball);
        //Quaternion targetRotation = Quaternion.LookRotation(ball.position - transform.position);
        //transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 3f);
    }
}
