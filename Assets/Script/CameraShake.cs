using System.Collections;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    // 카메라 흔들림 코루틴
    public IEnumerator Shake(float duration, float magnitude)
    {
        Vector3 originalPos = transform.localPosition; // 원래 위치 저장

        float elapsed = 0.0f;

        while (elapsed < duration)
        {
            // 랜덤한 x, y 흔들림 적용
            float x = Random.Range(-0.2f, 0.2f) * magnitude;
            float y = Random.Range(-0.2f, 0.2f) * magnitude;

            transform.localPosition = new Vector3(x, y, originalPos.z); // 흔들린 위치 적용

            elapsed += Time.deltaTime;

            yield return null; // 다음 프레임까지 대기
        }

        // 원래 위치로 복원
        transform.localPosition = originalPos;
    }
}
