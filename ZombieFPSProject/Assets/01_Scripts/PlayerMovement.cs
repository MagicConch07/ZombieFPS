using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using Cinemachine;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private InputReader _inputReader;
    [SerializeField] private Transform _virtualCam;
    [SerializeField] private Transform _bodyObj;

    private FPSInput.PlayerActions _inputAction;
    private Rigidbody _myrigid;
    private bool _isCursor = false;
    private float interpolationSpeed = 25.0f;

    public float speed = 15f;
    public float mouseSpeed = 100f;


    void Awake()
    {
        _myrigid = GetComponent<Rigidbody>();
        _inputAction = _inputReader.FPSInputInstance.Player;

        _inputReader.SettingsEvent += SettingsHandle;
    }

    void Start()
    {
        rotationCharacter = _myrigid.transform.localRotation;
        rotationCamera = _virtualCam.transform.localRotation;
    }

    void LateUpdate()
    {
        float x = Input.GetAxis("Mouse X");
        float inputX = _inputAction.MouseView.ReadValue<Vector2>().x;


        Debug.Log($"X : {x} // inputX : {inputX}");

        return;

        if (x == inputX)
        {
            //Debug.Log($"X : {x} // inputX : {inputX}");
        }

        CameraRotation();

        //TODO: 카메라 움직임만 따로 클래스로
        //! 나중에 클래스로 뽑아내야지
        float mouseY = _inputAction.MouseView.ReadValue<Vector2>().x * mouseSpeed;

        Quaternion rotationYaw = Quaternion.Euler(0.0f, mouseY * mouseSpeed, 0.0f);
        rotationCharacter *= rotationYaw;

        _myrigid.MoveRotation(Quaternion.Slerp(_myrigid.rotation, rotationCharacter, Time.deltaTime * interpolationSpeed));
    }

    //TODO : 이거 세팅을 따로 클래스로 뽑아내서 만들어야 함
    //! 일단 여기다가 플레이어 움직임이 아닌 세팅도 넣겠음
    private void SettingsHandle(bool isPush)
    {
        _isCursor = !isPush;
        Debug.Log(_isCursor);
    }

    [SerializeField] private float cameraRotationLimit;
    private float currentCameraRotationX;
    private Quaternion rotationCharacter;
    private Quaternion rotationCamera;

    private Vector2 yClamp = new Vector2(-60, 60);

    private void CameraRotation()
    {
        float _xRotation = _inputAction.MouseView.ReadValue<Vector2>().y;

        Quaternion localRotation = transform.localRotation;
        Quaternion rotationPitch = Quaternion.Euler(-_xRotation, 0.0f, 0.0f);

        //Save rotation. We use this for smooth rotation.
        rotationCamera *= rotationPitch;

        //Local Rotation.

        localRotation = Quaternion.Slerp(localRotation, rotationCamera, Time.deltaTime * interpolationSpeed);

        _virtualCam.localRotation = localRotation;
    }

    private Quaternion Clamp(Quaternion rotation)
    {
        //TODO : 쿼터니언 이해하기
        rotation.Normalize();

        //Pitch.
        float pitch = 2.0f * Mathf.Rad2Deg * Mathf.Atan(rotation.x);

        //Clamp.
        pitch = Mathf.Clamp(pitch, yClamp.x, yClamp.y);

        // 다시 쿼터니언으로 변환
        rotation.x = Mathf.Tan(0.5f * Mathf.Deg2Rad * pitch);

        //Return.
        return rotation;
    }

    void FixedUpdate()
    {
        Vector2 movement = _inputAction.Movement.ReadValue<Vector2>();
        Vector3 moveVec = new Vector3(movement.x, 0, movement.y) * speed;

        moveVec = transform.TransformDirection(moveVec);

        _myrigid.velocity = new Vector3(moveVec.x, _myrigid.velocity.y, moveVec.z);

        //? 이건 절대 잊을 수 없지
    }

}
