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
        // 2. 이동방향에 레이캐스트 발사 (장애물 검사)
        RaycastHit2D hit = Physics2D.Raycast(transform.position, moveDirection, rayDistance, tileLayer);

        // 2-1. 장애물 없으면 이동
        if(hit.transform == null)
        {
            //MoveTo() 함수에 이동방향을 매개변수로 전달해 이동
            movement2D.MoveTo(moveDirection);
            // 화면 밖으로 이동시 반대편 등장
            aroundWrap.UpdateAroundWrap();
        }
        else
        {
            StartCoroutine(SetMoveDirectionByTime());
        }
    }

    private void SetMoveDirection(Direction direction)
    {
        //이동 방향 설정
        this.direction = direction;
        // Vector3 타입의 이동 방향 값 설정
        moveDirection = Vector3FromEnum(this.direction);
        // 이동 방향에 따라 이미지 변경
        spriteRenderer.sprite = sprites[(int)this.direction];

        //모든 코루틴 중지
        StopAllCoroutines();
        // 일정 시간동안 같은 방향 이동 시 방향 변경
        StartCoroutine(SetMoveDirectionByTime());
    }

    private IEnumerator SetMoveDirectionByRandom()
    {
        //이동 방향 임의 설정해서 호출
        direction = (Direction)Random.Range(0, (int)Direction.Count);
        SetMoveDirection(direction);
        yield return null; // 코루틴으로 실행되도록 수정
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
            //nextDirection 이동 방향이 오른쪽 또는 왼쪽일 때
            if (directions[0].x != 0)
            {
                directions[1] = directions[0] + new Vector3(0, 0.65f, 0);
                directions[2] = directions[0] + new Vector3(0, -0.65f, 0); 
            }
            //nextDirection 이동 방향이 위 아래 일때
            else if (directions[0].y != 0)
            {
                directions[1] = directions[0] + new Vector3(-0.65f, 0, 0);
                directions[2] = directions[0] + new Vector3(0.65f, 0, 0);
            }

            // nextDirection 이동 방향으로 이동 가능한지 판별하기 위해 3개 레이캐스트 발사
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

            //3개 광선에 부딪히는 오브젝트가 없을 때 (이동 방향에 장애물이 없을 때.
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
