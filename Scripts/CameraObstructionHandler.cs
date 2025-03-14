using UnityEngine;
using System.Collections.Generic;

public class CameraObstructionHandler : MonoBehaviour
{
    public Transform ball; // ��
    public LayerMask obstructionMask; // ������ ������Ʈ ���̾�
    public float fadeAmount = 0.5f; // ���� (0: ���� ����, 1: ���� ����)

    private Dictionary<Renderer, Material[]> originalMaterials = new Dictionary<Renderer, Material[]>(); // ���� ��Ƽ���� ����
    private List<Renderer> fadedObjects = new List<Renderer>(); // ���� ������ ������Ʈ ���

    void Update()
    {
        // ���� ���� ó���� ������Ʈ ������� ����
        RestoreOriginalMaterials();

        // ī�޶�� �� ������ ��� ������Ʈ Ž��
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

    // ������Ʈ ����ȭ
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

    // ���� ���� ����
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
