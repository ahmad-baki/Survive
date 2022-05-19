using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    public float walkSpeed;
    public float sprintSpeed;
    public float jumpHeight;
    public float interactDistance;

    public float mass = 9.81f;
    public float groundCheckOffset;
    public float groundCheckRadius;
    public float terminalVelocity;

    public float cameraRotationSpeed;
    public float playerCameraCatchupSpeed;
    public GameObject cameraFollowGO;


    //[HideInInspector]
    public List<Collider> interactables;
    
    bool _grounded;
    CharacterController _playerControl;
    PlayerCombatControl _combatControl;
    InventarManager _inventarManager;
    Animator _animator;
    float _verticalVelocity;
    Vector3 _relTargetDir;
    bool _courutineRunning = false;
    float _playerRelCameraRotation = 0f;


    // Start is called before the first frame update
    void Start()
    {
        _playerControl = GetComponent<CharacterController>();
        _combatControl = GetComponent<PlayerCombatControl>();
        _animator = GetComponent<Animator>();
        _inventarManager = GetComponent<InventarManager>();
        interactables = new();
    }

    // Update is called once per frame
    void Update()
    {
        CheckGround();
        GravityAndJump();
        Move();
        Rotate();
        Interact();
    }

    void CheckGround()
    {
        Vector3 center = new(transform.position.x, transform.position.y + groundCheckOffset, transform.position.z);
        _grounded = Physics.CheckSphere(center, groundCheckRadius);
    }

    void GravityAndJump()
    {
        if (Mathf.Abs(_verticalVelocity) < terminalVelocity)
        {
            _verticalVelocity += Physics2D.gravity.y * mass * Time.deltaTime;
        }
        if (_grounded) 
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                _verticalVelocity = Mathf.Sqrt(jumpHeight * -2 * Physics2D.gravity.y * mass);
            }
        }
    }

    void Move()
    {
        Vector3 move = new();
        if (!_combatControl.isAttacking)
        {
            _relTargetDir = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
            Vector3 absoluteTargetDir = _relTargetDir.x * Camera.main.transform.right + _relTargetDir.z * Camera.main.transform.forward;
            float speed = (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) ? sprintSpeed : walkSpeed;
            move = absoluteTargetDir.normalized * speed;

            // remap horVel between 0 and 1
            float horVel = Mathf.Sqrt(Mathf.Pow(move.x, 2) + Mathf.Pow(move.z, 2));
            float remappedVel = 0;
            if (horVel >  walkSpeed - 0.1f)
            {
                remappedVel = 0.1f + (horVel - walkSpeed) / (sprintSpeed - walkSpeed) * 0.9f;
            }
            _animator.SetFloat("Velocity", remappedVel);
        }
        _playerControl.Move((move + new Vector3(0, _verticalVelocity, 0)) * Time.deltaTime);
    }

    void Rotate()
    {
        float mouseX = Input.GetAxis("Mouse X") * cameraRotationSpeed * Time.deltaTime;

        // if player doesnt move, only move camera
        if (_relTargetDir == Vector3.zero) {
            cameraFollowGO.transform.Rotate(Vector3.up * mouseX);
        }
        else
        {
            transform.Rotate(Vector3.up * mouseX);
            float camerLocalRotation = cameraFollowGO.transform.localRotation.y;
            _playerRelCameraRotation = Mathf.Atan2(_relTargetDir.x, _relTargetDir.z) * Mathf.Rad2Deg;
            if (!_courutineRunning && Mathf.Abs(GetTargetRotation(cameraFollowGO.transform.localEulerAngles.y, -_playerRelCameraRotation)) > 5)
            {
                StartCoroutine(RotatePlayerRelCameraFollow(_playerRelCameraRotation));
            }
        }
    }

    void Interact()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (interactables.Count > 0)
            {
                if (interactables[0]) {
                    interactables[0].GetComponent<Interactable>().Interact(this.gameObject);
                }
                else
                {
                    Debug.LogWarning("Tried to interact with object, which doesnt have colliders");
                    interactables.RemoveAt(0);
                }
            }
        }
    }

    IEnumerator RotatePlayerRelCameraFollow(float delta = 0)
    {
        _courutineRunning = true;
        Transform cFTransform = cameraFollowGO.transform;
        while (Mathf.Abs(GetTargetRotation(cFTransform.localEulerAngles.y, -delta)) > 5)
        {
            float targetRotation = GetTargetRotation(cFTransform.localEulerAngles.y, -delta);
            float rotationAngle = targetRotation * playerCameraCatchupSpeed * Time.deltaTime;

            // just set rotation directly
            if (Mathf.Abs(rotationAngle) >= 180) break;

            transform.Rotate(new Vector3(0, -rotationAngle, 0));
            cFTransform.Rotate(new Vector3(0, rotationAngle, 0));
            yield return null;
        }
        Vector3 cameraFollowEulerAnlges = cFTransform.localEulerAngles;
        transform.Rotate(new Vector3(0, cameraFollowEulerAnlges.y + delta, 0));
        cameraFollowGO.transform.localEulerAngles = new Vector3(cameraFollowEulerAnlges.x, -delta, cameraFollowEulerAnlges.z);
        _courutineRunning = false;
    }

    float GetTargetRotation(float source, float target)
    {
        source = (Mathf.Abs(source) < .1f) ? 360 : source;
        target = (Mathf.Abs(target) < .1f) ? 360 : target;

        if (source == target)
        {
            return 0;
        }

        if (target > source)
        {
            if (target - source <= 180)
            {
                return target - source;
            }
            return target - 360 - source;
        }
        else
        {
            if (360 - source + target <= 180)
            {
                return 360 - source + target;
            }
            return target - source;
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        // layer 6 == interactable
        if(other.gameObject.layer == 6)
        {
            Interactable interactable = other.GetComponent<Interactable>();
            if (interactable)
            {
                interactables.Add(other);
                interactable.Activate();
            }
            else
            {
                Debug.LogWarning($"{other.name} has Interact layer, but no Interact-Component");
            }
        }
    }


    private void OnTriggerExit(Collider other)
    {
        // layer 6 == interactable
        if (other.gameObject.layer == 6)
        {
            if (other.GetComponent<Interactable>() is var interactable)
            {
                interactables.Remove(other);
                interactable.DeActivate();
            }
            else
            {
                Debug.LogWarning($"{other.name} has Interact layer, but no Interact-Component");
                interactables.Remove(other);
            }
        }
    }


    float AddAngles(float a, float b)
    {
        float sum = a + b;
        if (sum < 0) return 360 + sum;
        if (sum > 360) return sum - 360;
        return sum;
    }
}