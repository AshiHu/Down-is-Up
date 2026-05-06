using System.Collections;
using UnityEngine;

public class Door : MonoBehaviour
{
    [Header("Animation")]
    public Vector3 openOffset = new Vector3(0f, 3f, 0f);
    public float duration = 1f;
    public AnimationCurve curve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    private bool _opened = false;

    public void Open()
    {
        if (_opened) return;
        _opened = true;
        StartCoroutine(Slide(transform.position + openOffset));
    }

    private IEnumerator Slide(Vector3 target)
    {
        Vector3 start = transform.position;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = curve.Evaluate(elapsed / duration);
            transform.position = Vector3.Lerp(start, target, t);
            yield return null;
        }

        transform.position = target;
    }
}