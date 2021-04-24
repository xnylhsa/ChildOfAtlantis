using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChildOfAtlantis.Mechanics
{

    [RequireComponent(typeof(Controller2D)), RequireComponent(typeof(SpriteRenderer))]
    public class PlayerController : MonoBehaviour
    {
        /// <summary>
        /// Max horizontal speed of the player.
        /// </summary>
        [SerializeField]
        float maxHorizontalWaterSpeed = 7;
        [SerializeField]
        float maxVerticalWaterSpeed = 7;
        [SerializeField]
        float velocityXSmoothing;
        [SerializeField]
        float velocityYSmoothing;
        [SerializeField]
        float maxGroundSpeed = 7;

        [SerializeField]
        float invulnTime = 1.0f;
        float invulnerableTimer;
        [SerializeField]
        float knockBackSpeed;
        bool knockedBack = false;
        bool invulnerable;
        bool alive = true;
        protected Controller2D controller;
        [SerializeField]
        float maxJumpHeight = 1.75f;
        [SerializeField]
        float minJumpHeight = 1.0f;
        [SerializeField]
        float timeToApex = 0.4f;
        [SerializeField]
        float accelerationTimeAirborne = 0.2f;
        [SerializeField]
        float accelerationTimeGrounded = 0.1f;
        float gravity = 1.0f;

        [SerializeField]
        float waterGravity = 0.0f;

        public bool controlEnabled = true;
        public bool inWater = true;

        Vector2 directionalInput;

        bool jump;
        Vector2 velocity;
        SpriteRenderer spriteRenderer;

        private void Awake()
        {
            //grab components here.
            controller = GetComponent<Controller2D>();
            gravity = -(maxJumpHeight * 2 / (timeToApex * timeToApex));
            alive = true;
        }


        // Update is called once per frame
        void Update()
        {
            HandleInput();
            UpdateInvulnerabilityTimer();
            CalculateVelocity();
            if (knockedBack)
            {
                HandleKnockBack();
            }
            HandleCollisons();
            controller.Move(velocity * Time.deltaTime, directionalInput);
        }

        void HandleInput()
        {
            directionalInput.x = Input.GetAxis("Horizontal");
            directionalInput.y = Input.GetAxis("Vertical");
            //if (inputManager.GetJumpDown())
            //{
            //    player.OnJumpButtonPressed();
            //}
            //if (inputManager.GetJumpUp())
            //{
            //    player.OnJumpButtonReleased();
            //}
        }

        void UpdateInvulnerabilityTimer()
        {
            if (invulnerable)
            {
                invulnerableTimer -= Time.deltaTime;
                if (invulnerableTimer <= 0)
                {
                    invulnerable = false;
                    invulnerableTimer = invulnTime;
                    //animationContr.DeactiveBlink();
                }
            }
        }

        void CalculateVelocity()
        {
            float targetVelocityX = directionalInput.x * maxHorizontalWaterSpeed;

            if (knockedBack)
            {
                targetVelocityX = velocity.x;
            }
            if(!inWater)
            {
                velocity.y += gravity * Time.deltaTime;
            }
            else
            {
                float targetVelocityY = directionalInput.y * maxVerticalWaterSpeed;
                if (controller.collisionInfo.below && targetVelocityY < 0.0f)
                {
                    velocity.y = 0.0f;
                }
                else if(controller.collisionInfo.above && targetVelocityY > 0.0f)
                {
                    velocity.y = 0.0f;
                }
                else
                {
                    velocity.y = Mathf.SmoothDamp(velocity.y, targetVelocityY, ref velocityYSmoothing, (controller.collisionInfo.below) ? accelerationTimeGrounded : accelerationTimeAirborne);
                }
            }
            velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXSmoothing, (controller.collisionInfo.below) ? accelerationTimeGrounded : accelerationTimeAirborne);
        }

        void HandleKnockBack()
        {
            if (controller.collisionInfo.below)
            {
                knockedBack = false;
            }
        }

        void HandleCollisons()
        {
            if (controller.collisionInfo.below || controller.collisionInfo.above)
            {
                if (controller.collisionInfo.slidingDownSlope)
                {
                    if(inWater)
                    {
                        velocity.y += controller.collisionInfo.slopeNormal.y * -waterGravity * Time.deltaTime;
                    }
                    else
                    {
                        velocity.y += controller.collisionInfo.slopeNormal.y * -gravity * Time.deltaTime;
                    }
                }
                else
                {
                    velocity.y = 0;
                }
                if (controller.collisionInfo.below)
                {
                    //animationContr.SetGrounded(true);
                }
            }

        }

    }
}


