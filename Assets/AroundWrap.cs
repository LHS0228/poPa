using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AroundWrap : MonoBehaviour
{
    [SerializeField]
    private StageData stageData;
    [SerializeField]
    private bool isWarp = false;

    public void UpdateAroundWrap()
    {
        Vector3 position = transform.position;

        //왼쪽 끝 오른쪽 끝 도달하면 반대편 이동
        if(position.x < stageData.LimitMin.x || position.x > stageData.LimitMax.x)
        {
            //position.x *= -1; 좌우 광클할 시 버그 걸리는 문제 수정
            position.x = position.x>0 ? -(Mathf.Abs(stageData.LimitMax.x) - 0.1f) : (Mathf.Abs(stageData.LimitMin.x) - 0.1f); 
        }

        //위끝 아래끝 도달하면 반대편 이동
        if(position.y < stageData.LimitMin.y || position.y > stageData.LimitMax.y)
        {
            //position.y *= -1; 좌우 광클할 시 버그 걸리는 문제 수정
            position.y = position.y > 0 ? -(Mathf.Abs(stageData.LimitMax.y) - 0.1f) : (Mathf.Abs(stageData.LimitMin.y) - 0.1f);
        }

        transform.position = position;
    }
}
