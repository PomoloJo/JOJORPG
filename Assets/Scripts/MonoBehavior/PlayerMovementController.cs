using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 与人物运动的控制有关，挂载到 player
public class PlayerMovementController : MonoBehaviour
{
    public float speed = 6.0f;
    private float inputX, inputY;
    private Rigidbody2D PlayerRigibody;
    private Animator PlayerAnimator;

    // Start is called before the first frame update 
    void Start()
    {
        PlayerRigibody = GetComponent<Rigidbody2D>();
        PlayerAnimator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
    }
    // Update 可能在不同计算机或不同平台帧率不同，刚体的更新最好放于 FixedUpdate
    private void FixedUpdate() {
        UpdateMovement();
    } 

    private void UpdateMovement()
    {
        inputX = Input.GetAxisRaw("Horizontal");
        inputY = Input.GetAxisRaw("Vertical");
        //Vector2 input = new Vector2(inputX, inputY).normalized;
        Vector2 input = (transform.right * inputX + transform.up * inputY).normalized;
        PlayerRigibody.velocity = input * speed;

        // 如果有输入
        if(input != Vector2.zero)
        {
            PlayerAnimator.SetBool("IsMoving", true);
            PlayerAnimator.SetFloat("InputX", inputX);
            PlayerAnimator.SetFloat("InputY", inputY);
        }
        else
        {
            PlayerAnimator.SetBool("IsMoving", false);
        }

        // 更合理的浮点数比较，但是上面那种用法用的时候也没发现问题，先这样吧
        // if(Mathf.Approximately(inputX, 0.0f) && Mathf.Approximately(inputY, 0.0f))
        // {
        //     PlayerAnimator.SetBool("IsMoving", false);
        // }
        // else
        // {
        // PlayerAnimator.SetBool("IsMoving", true);
        //     PlayerAnimator.SetFloat("InputX", inputX);
        //     PlayerAnimator.SetFloat("InputY", inputY);
        // }
    }
}
