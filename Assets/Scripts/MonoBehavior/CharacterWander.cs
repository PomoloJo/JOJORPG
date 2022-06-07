using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterWander : MonoBehaviour
{

    public float wanderSpeed = 3.0f;
    public float traceSpeed = 5.0f;

    public float detectIntervalTime = 5.0f;

    
    private enum MovementState
    {
        Idle,
        Wander,
        Trace,
        Dead
    }


    private MovementState movementState;
    private Rigidbody2D rigibody;
    private Animator animator;
    private Vector3 targetPosition;



    // Start is called before the first frame update
    void Start()
    {
        rigibody = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

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
    public void Trace()
    {
        Vector3 positionVector = targetPosition - transform.position;
        // 模拟输入
        rigibody.velocity = positionVector.normalized * traceSpeed;
        animator.SetBool("isMoving", true);
        Vector3 newVec = Quaternion.Euler(0,0,-transform.rotation.eulerAngles[2]) * rigibody.velocity;
        animator.SetFloat("inputX", newVec.x);
        animator.SetFloat("inputY", newVec.y);
    }

    private void OnTriggerStay2D(Collider2D other) 
    {
        if(other.tag == "Player")
        {
            targetPosition = other.transform.position;
            movementState = MovementState.Trace;
            Trace();
        }
    }

    private void OnTriggerExit2D(Collider2D other) 
    {
        if(other.tag == "Player")
        {
            movementState = MovementState.Idle;
        }
    }



}
