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

        //���� �� ������ �� �����ϸ� �ݴ��� �̵�
        if(position.x < stageData.LimitMin.x || position.x > stageData.LimitMax.x)
        {
            //position.x *= -1; �¿� ��Ŭ�� �� ���� �ɸ��� ���� ����
            position.x = position.x>0 ? -(Mathf.Abs(stageData.LimitMax.x) - 0.1f) : (Mathf.Abs(stageData.LimitMin.x) - 0.1f); 
        }

        //���� �Ʒ��� �����ϸ� �ݴ��� �̵�
        if(position.y < stageData.LimitMin.y || position.y > stageData.LimitMax.y)
        {
            //position.y *= -1; �¿� ��Ŭ�� �� ���� �ɸ��� ���� ����
            position.y = position.y > 0 ? -(Mathf.Abs(stageData.LimitMax.y) - 0.1f) : (Mathf.Abs(stageData.LimitMin.y) - 0.1f);
        }

        transform.position = position;
    }
}
