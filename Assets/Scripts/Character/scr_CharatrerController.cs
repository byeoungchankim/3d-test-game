using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static scr_Models;

public class scr_CharatrerController : MonoBehaviour
{
    private CharacterController characterController;
    private DefaultInput defaultInput;
    public Vector2 input_Movement;
    public Vector2 input_View;

    private Vector3 newCameraRotation;
    private Vector3 newCharacterRotation;


    [Header("Referemces")]
    public Transform cameraHolder;

    [Header("setting")]
    public PlayerSettingsModel playerSettings;
    //ī�޶� �����°� �ִ� 
    public float viewClamYmin = -70;
    public float viewClamYmax = 80;

    [Header("Gravity")]
    public float gravityAmount;
    public float gravityMin;
    private float PlayerGravity;

    public Vector3 jumpingForce;
    private Vector3 jumpingForceVelocity;

    [Header("stance")]
    public PlayerStance playerStance;
    public float playerStanceSmoothing;
    public CharacterStance playerStandStance;
    public CharacterStance playerCrouchStance;
    public CharacterStance playerProneStance;

    private float cameraHeight;
    private float cameraHeightVelocity;

    private Vector3 stanceCapsuleCenter;
    private Vector3 stanceCapsuleCenterVelocity;

    private float stanceCapsuleHeight;
    private float stanceCapsuleHeightVelocity;



    private void Awake()
    {
        defaultInput = new DefaultInput();

        defaultInput.Character.Movement.performed += e => input_Movement = e.ReadValue<Vector2>();
        defaultInput.Character.View.performed += e => input_View = e.ReadValue<Vector2>();
        defaultInput.Character.Jump.performed += e => Jump();

        defaultInput.Enable();

        newCharacterRotation = transform.localRotation.eulerAngles;
        newCameraRotation = cameraHolder.localRotation.eulerAngles;

        characterController = GetComponent<CharacterController>();

        cameraHeight = cameraHolder.localPosition.y;
    }

    private void Update()
    {
        CalculateView();
        CalculateMovement();
        CalculateJump();
        CalculateStance();
    }

    private void CalculateView()
    {
        //ī�޶� �¿� ��
        newCharacterRotation.y += playerSettings.ViewXSensitivity * (playerSettings.ViewXInverted ? input_View .x: input_View.x) * Time.deltaTime;
        transform.rotation = Quaternion.Euler(newCharacterRotation);


        //ī�޶� �� �Ʒ� ��
        newCameraRotation.x += playerSettings.ViewYSensitivity * (playerSettings.ViewYInverted ? input_View.y : -input_View.y) * Time.deltaTime;
        //ī�޶� �ִ� �� �Ʒ� ��
        newCameraRotation.x = Mathf.Clamp(newCameraRotation.x , viewClamYmin, viewClamYmax);

        cameraHolder.localRotation = Quaternion.Euler(newCameraRotation);

    }

    private void CalculateMovement()
    {
        //�̵�
        var verticalSpeed = playerSettings.WalkingForwardSpeed * input_Movement.y * Time.deltaTime;
        var horizontalSpeed = playerSettings.WalkingStrafeSpeed * input_Movement.x * Time.deltaTime;

        var newMovementSpeed = new Vector3(horizontalSpeed, 0, verticalSpeed);

        //ī�޶� ���� �������� �̵�

        newMovementSpeed = transform.TransformDirection(newMovementSpeed);


        //���� �߷� �Ҵ�
        if (PlayerGravity > gravityMin)
        {
            PlayerGravity -= gravityAmount * Time.deltaTime;
        }


        if (PlayerGravity < -0.1f && characterController.isGrounded )
        {
            PlayerGravity = -0.1f;
        }


        newMovementSpeed.y += PlayerGravity;
        newMovementSpeed += jumpingForce * Time.deltaTime;

        defaultInput.Character.Movement.started += ctx => input_Movement = ctx.ReadValue<Vector2>(); defaultInput.Character.Movement.performed += ctx => input_Movement = ctx.ReadValue<Vector2>(); defaultInput.Character.Movement.canceled += ctx => input_Movement = ctx.ReadValue<Vector2>();

        characterController.Move(newMovementSpeed);

    }

    private void CalculateJump()
    {
        jumpingForce = Vector3.SmoothDamp(jumpingForce, Vector3.zero, ref jumpingForceVelocity, playerSettings.JumpingFalloff);
    }
    private void CalculateStance()
    {
        //��ũ���� ī�޶� ����
        var CurrentStance = playerStandStance;

        if(playerStance == PlayerStance.Crouching)
        {
            CurrentStance = playerCrouchStance;
        }
        else if (playerStance == PlayerStance.Prone)
        {
            CurrentStance = playerProneStance;
        }

        cameraHeight = Mathf.SmoothDamp(cameraHolder.localPosition.y, CurrentStance.CamerHeight, ref cameraHeightVelocity, playerStanceSmoothing);

        cameraHolder.localPosition = new Vector3(cameraHolder.localPosition.x, cameraHeight, cameraHolder.localPosition.z);

        characterController.height = Mathf.SmoothDamp(characterController.height, CurrentStance.stanceCollider.height, ref stanceCapsuleHeightVelocity, playerStanceSmoothing);
        characterController.center = Vector3.SmoothDamp(characterController.center, CurrentStance.stanceCollider.center, ref stanceCapsuleCenterVelocity, playerStanceSmoothing);
     }

    private void Jump()
    {
        if(!characterController)
        {
            return;
        }

        //����
        jumpingForce = Vector3.up * playerSettings.JumpingHeight;
        PlayerGravity = 0;

    }
}
