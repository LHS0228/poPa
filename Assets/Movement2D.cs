using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement2D : MonoBehaviour
{
    [SerializeField]
    private float moveTime = 0.2f;
    private bool isMove = false;

    public bool MoveTo(Vector3 moveDirection)
    {
        if (isMove)
        {
            return false;
        }

        StartCoroutine(SmoothGridMovement(transform.position + moveDirection));

        return true;
    }

    private IEnumerator SmoothGridMovement(Vector2 endPosition)
    {
        Vector2 startPosition = transform.position;
        float percent = 0;

        isMove = true;

        while (percent < 1)
        {
            percent += Time.deltaTime / moveTime;
            transform.position = Vector2.Lerp(startPosition, endPosition, percent);

            yield return null;
        }

        isMove = false;
    }
}
