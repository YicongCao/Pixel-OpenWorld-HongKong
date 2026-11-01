using System;
using System.IO;
using UnityEngine;
using UnityEngine.InputSystem;

namespace HongKong.Walk
{
    [RequireComponent(typeof(CharacterController))]
    public sealed class StreetWalker : MonoBehaviour
    {
        public Camera viewCamera;
        public float walkSpeed = 2.2f, fastSpeed = 4.0f;
        public Vector3[] route;
        public bool automatic, paused;
        public Vector3 automaticTarget;
        public float distanceWalked;
        public int fallRecoveries;
        public Vector3 Facing { get; private set; } = Vector3.forward;
        public Vector3 Velocity { get; private set; }
        CharacterController body;
        InputAction move, fast;
        float verticalSpeed;
        Vector3 start;

        void Awake()
        {
            body = GetComponent<CharacterController>();
            if(!viewCamera)viewCamera=Camera.main;
            start = transform.position;
            move = new InputAction("Walk", InputActionType.Value);
            move.AddCompositeBinding("2DVector").With("Up", "<Keyboard>/w").With("Down", "<Keyboard>/s")
                .With("Left", "<Keyboard>/a").With("Right", "<Keyboard>/d");
            move.AddCompositeBinding("2DVector").With("Up", "<Keyboard>/upArrow").With("Down", "<Keyboard>/downArrow")
                .With("Left", "<Keyboard>/leftArrow").With("Right", "<Keyboard>/rightArrow");
            fast = new InputAction("Fast", binding: "<Keyboard>/leftShift");
        }
        void OnEnable() { move?.Enable(); fast?.Enable(); }
        void OnDisable() { move?.Disable(); fast?.Disable(); }
        void OnDestroy() { move?.Dispose(); fast?.Dispose(); }
        void Update()
        {
            if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
                paused = !paused;
            if (paused) { Velocity = Vector3.zero; return; }
            Vector3 desired;
            if (automatic)
            {
                desired = automaticTarget - transform.position;
                desired.y = 0;
                desired = desired.magnitude < .09f ? Vector3.zero : desired.normalized;
            }
            else
            {
                var input = move.ReadValue<Vector2>();
                Vector3 right = Vector3.ProjectOnPlane(viewCamera.transform.right, Vector3.up).normalized;
                Vector3 forward = Vector3.ProjectOnPlane(viewCamera.transform.forward, Vector3.up).normalized;
                desired = Vector3.ClampMagnitude(right * input.x + forward * input.y, 1);
            }
            float speed = automatic || fast.IsPressed() ? fastSpeed : walkSpeed;
            if (body.isGrounded && verticalSpeed < 0) verticalSpeed = -2;
            verticalSpeed += Physics.gravity.y * Time.deltaTime;
            Vector3 before = transform.position;
            body.Move((desired * speed + Vector3.up * verticalSpeed) * Time.deltaTime);
            Velocity = (transform.position-before) / Mathf.Max(Time.deltaTime,.0001f);
            distanceWalked += Vector3.ProjectOnPlane(transform.position-before,Vector3.up).magnitude;
            if (desired.sqrMagnitude > .01f) Facing = desired;
            if (transform.position.y < -8) { fallRecoveries++; ResetToStart(); }
        }
        public void ResetToStart()
        {
            body.enabled = false; transform.position = start; body.enabled = true; verticalSpeed=0;
        }
    }
}
