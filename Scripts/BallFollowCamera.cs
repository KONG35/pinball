using UnityEngine;

public class BallFollowCamera : MonoBehaviour
{
    private Vector3 offset = new Vector3(0, 3, -5); // ������ �Ÿ�
    private Vector3 velocity = Vector3.zero; // �ӵ� ����
    
    [SerializeField]
    private Transform ball; // ������ ��
    [SerializeField]
    private float followSpeed = 5f; // ī�޶� ���󰡴� �ӵ�
    [SerializeField]
    private float smoothTime = 0.2f; // �ε巴�� �̵��ϴ� �ð�


    void LateUpdate()
    {
        if (ball == null) return;

        Vector3 targetPosition = ball.position + offset;

        // ���� ��ġ�� ���� ���� �̻����θ� ī�޶� �̵��ϵ��� ����
        float distance = Vector3.Distance(transform.position, targetPosition);

        // 0.1f �Ÿ� ���� �־���� �̵�
        if (distance > 0.1f)
        {
            // �ε巴�� �̵� (SmoothDamp)
            transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);
        }

        // ���� �ٶ󺸵��� ȸ��
        transform.LookAt(ball);
        //Quaternion targetRotation = Quaternion.LookRotation(ball.position - transform.position);
        //transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 3f);
    }
}
