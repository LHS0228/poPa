using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyFSM : MonoBehaviour
{
    [SerializeField]
    private LayerMask tileLayer;
    [SerializeField]
    private Sprite[] sprites;
    [SerializeField]
    private StageData stageData;
    [SerializeField]
    private float delayTime = 3f;

    private Vector2 moveDirection = Vector2.right;
    private Direction direction = Direction.Right;
    private Direction nextDirection = Direction.None;
    private float rayDistance = 0.55f;

    private Movement2D movement2D;
    private AroundWrap aroundWrap;
    private SpriteRenderer spriteRenderer;

    void Awake()
    {
        movement2D = GetComponent<Movement2D>();
        aroundWrap = GetComponent<AroundWrap>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        StartCoroutine(SetMoveDirectionByRandom());
    }

    // Update is called once per frame
    void Update()
    {
        // 2. �̵����⿡ ����ĳ��Ʈ �߻� (��ֹ� �˻�)
        RaycastHit2D hit = Physics2D.Raycast(transform.position, moveDirection, rayDistance, tileLayer);

        // 2-1. ��ֹ� ������ �̵�
        if(hit.transform == null)
        {
            //MoveTo() �Լ��� �̵������� �Ű������� ������ �̵�
            movement2D.MoveTo(moveDirection);
            // ȭ�� ������ �̵��� �ݴ��� ����
            aroundWrap.UpdateAroundWrap();
        }
        else
        {
            StartCoroutine(SetMoveDirectionByTime());
        }
    }

    private void SetMoveDirection(Direction direction)
    {
        //�̵� ���� ����
        this.direction = direction;
        // Vector3 Ÿ���� �̵� ���� �� ����
        moveDirection = Vector3FromEnum(this.direction);
        // �̵� ���⿡ ���� �̹��� ����
        spriteRenderer.sprite = sprites[(int)this.direction];

        //��� �ڷ�ƾ ����
        StopAllCoroutines();
        // ���� �ð����� ���� ���� �̵� �� ���� ����
        StartCoroutine(SetMoveDirectionByTime());
    }

    private IEnumerator SetMoveDirectionByRandom()
    {
        //�̵� ���� ���� �����ؼ� ȣ��
        direction = (Direction)Random.Range(0, (int)Direction.Count);
        SetMoveDirection(direction);
        yield return null; // �ڷ�ƾ���� ����ǵ��� ����
    }

    private IEnumerator SetMoveDirectionByTime()
    {
        yield return new WaitForSeconds(delayTime);

        nextDirection = (Direction)(Random.Range(0, 2) * 2 + 1 - (int)direction % 2);
        StartCoroutine(CheckBlockedNextMoveDirection());
    }

    private IEnumerator CheckBlockedNextMoveDirection()
    {
        while (true)
        {
            Vector3[] directions = new Vector3[3];
            bool[] isPossibleMoves = new bool[3];

            directions[0] = Vector3FromEnum(nextDirection);
            //nextDirection �̵� ������ ������ �Ǵ� ������ ��
            if (directions[0].x != 0)
            {
                directions[1] = directions[0] + new Vector3(0, 0.65f, 0);
                directions[2] = directions[0] + new Vector3(0, -0.65f, 0); 
            }
            //nextDirection �̵� ������ �� �Ʒ� �϶�
            else if (directions[0].y != 0)
            {
                directions[1] = directions[0] + new Vector3(-0.65f, 0, 0);
                directions[2] = directions[0] + new Vector3(0.65f, 0, 0);
            }

            // nextDirection �̵� �������� �̵� �������� �Ǻ��ϱ� ���� 3�� ����ĳ��Ʈ �߻�
            int possibleCount = 0;
            for(int i=0; i<3; ++i)
            {
                if(i == 0)
                {
                    isPossibleMoves[i] = Physics2D.Raycast(transform.position, directions[i], 0.5f, tileLayer);
                    Debug.DrawLine(transform.position, transform.position + directions[i] * 0.5f, Color.yellow);
                }
                else
                {
                    isPossibleMoves[i] = Physics2D.Raycast(transform.position, directions[i], 0.7f, tileLayer);
                    Debug.DrawLine(transform.position, transform.position + directions[i] * 0.7f, Color.yellow);
                }

                if (isPossibleMoves[i] == false)
                {
                    possibleCount++;
                }
            }

            //3�� ������ �ε����� ������Ʈ�� ���� �� (�̵� ���⿡ ��ֹ��� ���� ��.
            if(possibleCount == 3)
            {
                if(transform.position.x > stageData.LimitMin.x && transform.position.x < stageData.LimitMax.x &&
                    transform.position.y > stageData.LimitMin.y && transform.position.y < stageData.LimitMax.y)
                {
                    SetMoveDirection(nextDirection);
                    nextDirection = Direction.None;
                    break;
                }
            }

            yield return null;
        }
    }

    private Vector3 Vector3FromEnum(Direction state)
    {
        Vector3 direction = Vector3.zero;

        switch (state)
        {
            case Direction.Up: direction = Vector3.up; break;
            case Direction.Left: direction = Vector3.left; break;
            case Direction.Right: direction = Vector3.right; break;
            case Direction.Down: direction = Vector3.down; break;
        }

        return direction;
    }
}
