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
    private Rigidbody _myrigid;
    public float speed = 15f;
    public float mouseSpeed = 100f;
    [SerializeField] private Transform _virtualCam;
    [SerializeField] private Transform _bodyObj;

    private bool _isCursor = false;

    void Awake()
    {
        _myrigid = GetComponent<Rigidbody>();
    }

    void Update()
    {
        CameraRotation();

        //* CursorInput
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (_isCursor == false)
                _isCursor = true;
            else
                _isCursor = false;
        }

        //* Cursor
        if (_isCursor)
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
        else
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }

        //! 나중에 클래스로 뽑아내야지
        float mouseY = Input.GetAxis("Mouse X") * mouseSpeed;
        float mouseX = -Input.GetAxis("Mouse Y") * mouseSpeed;

        Quaternion rotationYaw = Quaternion.Euler(0.0f, mouseY * mouseSpeed, 0.0f);
        //Pitch.
        //Quaternion rotationPitch = Quaternion.Euler(-mouseX * mouseSpeed, 0.0f, 0.0f);

        //Save rotation. We use this for smooth rotation.
        //rotationCamera *= rotationPitch;
        rotationCharacter *= rotationYaw;

        //Local Rotation.
        //Quaternion localRotation = _virtualCam.transform.localRotation;
        //Rotate local.
        //localRotation *= rotationPitch;

        //Clamp.
        //localRotation = Clamp(localRotation);

        //Rotate character.
        _myrigid.MoveRotation(_myrigid.rotation * rotationYaw);

        //Set.
        //_virtualCam.transform.localRotation = localRotation;
    }

    [SerializeField]
    private float lookSensitivity;

    [SerializeField]
    private float cameraRotationLimit;
    private float currentCameraRotationX;

    private void CameraRotation()
    {
        float _xRotation = Input.GetAxisRaw("Mouse Y");
        float _cameraRotationX = _xRotation * lookSensitivity;

        currentCameraRotationX -= _cameraRotationX;
        currentCameraRotationX = Mathf.Clamp(currentCameraRotationX, -cameraRotationLimit, cameraRotationLimit);

        _virtualCam.transform.localEulerAngles = new Vector3(currentCameraRotationX, 0f, 0f);
    }

    private Quaternion rotationCharacter;
    private Quaternion rotationCamera;


    private Vector2 yClamp = new Vector2(-60, 60);

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

        //! 이거 리팩토링 조져 new Inputsystem으로
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 moveVec = new Vector3(x, 0, z) * speed;

        moveVec = transform.TransformDirection(moveVec);

        _myrigid.velocity = new Vector3(moveVec.x, _myrigid.velocity.y, moveVec.z);

        //? 이건 절대 잊을 수 없지
    }
}
