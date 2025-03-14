using UnityEngine;
using System.Collections.Generic;

public class CameraObstructionHandler : MonoBehaviour
{
    public Transform ball; // 공
    public LayerMask obstructionMask; // 감지할 오브젝트 레이어
    public float fadeAmount = 0.5f; // 투명도 (0: 완전 투명, 1: 원래 상태)

    private Dictionary<Renderer, Material[]> originalMaterials = new Dictionary<Renderer, Material[]>(); // 원래 머티리얼 저장
    private List<Renderer> fadedObjects = new List<Renderer>(); // 현재 투명한 오브젝트 목록

    void Update()
    {
        // 기존 투명 처리된 오브젝트 원래대로 복구
        RestoreOriginalMaterials();

        // 카메라와 공 사이의 모든 오브젝트 탐색
        Vector3 direction = ball.position - transform.position;
        float distance = direction.magnitude;

        RaycastHit[] hits = Physics.RaycastAll(transform.position, direction.normalized, distance, obstructionMask);

        foreach (RaycastHit hit in hits)
        {
            Renderer renderer = hit.collider.GetComponent<Renderer>();
            if (renderer != null)
            {
                FadeOutObject(renderer);
            }
        }
    }

    // 오브젝트 투명화
    void FadeOutObject(Renderer renderer)
    {
        if (!originalMaterials.ContainsKey(renderer))
        {
            originalMaterials[renderer] = renderer.materials;
        }

        foreach (Material mat in renderer.materials)
        {
            if (mat.HasProperty("_Color"))
            {
                Color color = mat.color;
                color.a = fadeAmount;
                mat.color = color;
            }
        }

        fadedObjects.Add(renderer);
    }

    // 원래 상태 복구
    void RestoreOriginalMaterials()
    {
        foreach (Renderer renderer in fadedObjects)
        {
            if (renderer != null && originalMaterials.ContainsKey(renderer))
            {
                renderer.materials = originalMaterials[renderer];
            }
        }

        fadedObjects.Clear();
    }
}
