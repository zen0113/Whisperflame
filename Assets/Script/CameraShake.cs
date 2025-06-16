using System.Collections;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    // ī�޶� ��鸲 �ڷ�ƾ
    public IEnumerator Shake(float duration, float magnitude)
    {
        Vector3 originalPos = transform.localPosition; // ���� ��ġ ����

        float elapsed = 0.0f;

        while (elapsed < duration)
        {
            // ������ x, y ��鸲 ����
            float x = Random.Range(-0.2f, 0.2f) * magnitude;
            float y = Random.Range(-0.2f, 0.2f) * magnitude;

            transform.localPosition = new Vector3(x, y, originalPos.z); // ��鸰 ��ġ ����

            elapsed += Time.deltaTime;

            yield return null; // ���� �����ӱ��� ���
        }

        // ���� ��ġ�� ����
        transform.localPosition = originalPos;
    }
}
