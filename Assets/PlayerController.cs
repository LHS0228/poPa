using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayController : MonoBehaviour
{
    [SerializeField]
    private LayerMask tileLayer;

    private Vector2 moveDirection = Vector2.right;
    private float rayDistance = 0.55f;
    private Direction direction = Direction.Right;
    
    private Movement2D movement2D;
    private AroundWrap aroundWrap;
    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        //tileLayer = 1<<LayerMask.NameToLayer("Tile");
        movement2D = GetComponent<Movement2D>();
        aroundWrap = GetComponent<AroundWrap>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    private void Update()
    {
        //0. �÷��̾� ����ĳ��Ʈ ���� ����.
        RaycastHit2D upHit = Physics2D.Raycast(transform.position, Vector2.up, rayDistance, tileLayer);
        RaycastHit2D downHit = Physics2D.Raycast(transform.position, Vector2.down, rayDistance, tileLayer);
        RaycastHit2D leftHit = Physics2D.Raycast(transform.position, Vector2.left, rayDistance, tileLayer);
        RaycastHit2D rightHit = Physics2D.Raycast(transform.position, Vector2.right, rayDistance, tileLayer);

        //1. ����Ű �̵�
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            if (upHit.transform == null)
            {
                moveDirection = Vector2.up;
                direction = Direction.Up;
            }
        }
        else if(Input.GetKeyDown(KeyCode.LeftArrow))
        {
            if (leftHit.transform == null)
            {
                moveDirection = Vector2.left;
                direction = Direction.Left;
            }
        }
        else if(Input.GetKeyDown(KeyCode.RightArrow))
        {
            if (rightHit.transform == null)
            {
                moveDirection = Vector2.right;
                direction = Direction.Right;
            }
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            if (downHit.transform == null)
            {
                moveDirection = Vector2.down;
                direction = Direction.Down;
            }
        }

        // 2. �̵����⿡ ����ĳ��Ʈ �߻� (��ֹ� �˻�)
        RaycastHit2D hit = Physics2D.Raycast(transform.position, moveDirection, rayDistance, tileLayer);
        Debug.DrawLine(transform.position, transform.position + (Vector3)moveDirection * rayDistance, Color.cyan);

        // 2-1 ��ֹ� ���� �� �̵�
        if(hit.transform == null)
        {
            // MoveTo() �Լ��� �̵������� �Ű������� ������ �̵�
            bool movePossible = movement2D.MoveTo(moveDirection);

            //�̵��� �����ϰ� �Ǹ�
            if(movePossible)
            {
                if (direction == Direction.Left)
                {
                    transform.localEulerAngles = Vector3.forward * 90 * (int)direction;
                    spriteRenderer.flipY = true;
                }
                else
                {
                    transform.localEulerAngles = Vector3.forward * 90 * (int)direction;
                    spriteRenderer.flipY = false;
                }
            }

            //ȭ�� ������ ������ �ݴ��� ����
            aroundWrap.UpdateAroundWrap();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Item"))
        {
            // ������ ȹ�� ó��
            Destroy(collision.gameObject);
        }
        else if (collision.CompareTag("Enemy"))
        {
            // �÷��̾� ĳ���� ü�� ���� �� ó��
            StopCoroutine(nameof(OnHit));
            StartCoroutine(nameof(OnHit));

            Destroy(collision.gameObject);
        }
    }

    private IEnumerator OnHit()
    {
        spriteRenderer.color = Color.red;

        yield return new WaitForSeconds(0.1f);

        spriteRenderer.color = Color.white;
    }
}
