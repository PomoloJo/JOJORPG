using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterState : MonoBehaviour
{
    // 暴露在 inspector 的属性, 必须要从面板中填入
    public float wanderSpeed;
    public float detectIntervalTime;
    public float chaseSpeed;
    public float chaseDetectRadius;

    
    private enum MovementState
    {
        Idle,
        Wander,
        Chase,
        Dead
    }


    private MovementState movementState;
    private Rigidbody2D rigibody;
    private Animator animator;
    private Vector3 targetPosition;
    private CircleCollider2D circleCollider;



    // Start is called before the first frame update
    void Start()
    {
        rigibody = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        circleCollider = GetComponent<CircleCollider2D>();
        circleCollider.radius = chaseDetectRadius;

        ChooseRandomTargetPosition();
        StartCoroutine(DetectState());
    }

    // Update is called once per frame
    void Update()
    {
        Debug.DrawLine(transform.position, targetPosition, Color.red);    
    }

    private void FixedUpdate() 
    {
        Vector3 positionVector = targetPosition - transform.position;
        // 这个停止的判断值可以再稍微设置大一点，避免因到达不了精确坐标出现的鬼畜现象
        if((Mathf.Abs(positionVector.x) < 0.3f && Mathf.Abs(positionVector.y) < 0.3f) || (Mathf.Abs(rigibody.velocity.x) < 0.3f && Mathf.Abs(rigibody.velocity.y) < 0.3f))
        {
            movementState = MovementState.Idle;
            animator.SetBool("isMoving", false);
            rigibody.velocity = new Vector2(0,0);
        }
    }

    // 漫游协程，idle状态经过 wanderIntervalTime 时间后开始漫游
    private IEnumerator DetectState()
    {
        while(movementState != MovementState.Dead)
        {
            yield return new WaitForSeconds(detectIntervalTime);
            if(movementState == MovementState.Idle)
            {
                Wander();
            }
        }
    }


    // 以自己为圆心，随机选择一个角度与一个移动距离 进行漫游
    public void ChooseRandomTargetPosition()
    {
        float wanderAngle = Random.Range(0,360);
        float wanderRadius = Random.Range(2,5);
        // 角度转弧度
        wanderAngle *= Mathf.Deg2Rad;
        targetPosition = rigibody.position + wanderRadius * new Vector2(Mathf.Cos(wanderAngle), Mathf.Sin(wanderAngle));
    }

    // 漫游到随机目标位置
    public void Wander()
    {
        ChooseRandomTargetPosition();
        Vector3 positionVector = targetPosition - transform.position;
        // 模拟输入
        rigibody.velocity = positionVector.normalized * wanderSpeed;
        animator.SetBool("isMoving", true);
        Vector3 newVec = Quaternion.Euler(0,0,-transform.rotation.eulerAngles[2]) * rigibody.velocity;
        animator.SetFloat("inputX", newVec.x);
        animator.SetFloat("inputY", newVec.y);
    }

    // 追逐到指定目标位置
    public void Chase()
    {
        Vector3 positionVector = targetPosition - transform.position;
        // 模拟输入
        rigibody.velocity = positionVector.normalized * chaseSpeed;
        animator.SetBool("isMoving", true);
        Vector3 newVec = Quaternion.Euler(0,0,-transform.rotation.eulerAngles[2]) * rigibody.velocity;
        animator.SetFloat("inputX", newVec.x);
        animator.SetFloat("inputY", newVec.y);
    }


    // 以下为进入侦察范围后的反应

    // 检测 tag 后期如果 tag 多了会很麻烦，应该改为接口实现
    private void OnTriggerStay2D(Collider2D other) 
    {
        if(other.tag == "Player")
        {
            targetPosition = other.transform.position;
            movementState = MovementState.Chase;
            Chase();
        }
    }

    // 检测 tag 后期如果 tag 多了会很麻烦，应该改为接口实现
    private void OnTriggerExit2D(Collider2D other) 
    {
        if(movementState == MovementState.Chase && other.tag == "Player")
        {
            movementState = MovementState.Idle;
        }
    }



}
