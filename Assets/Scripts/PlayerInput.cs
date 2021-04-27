using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChildOfAtlantis.Mechanics
{

    public class PlayerInput : MonoBehaviour
    {
        public PlayerController player;
        public float dashCooldown = 1.0f;

        [HideInInspector]
        public float dashCooldownTimer = 0.0f;
        public float dashActivationInterval = 0.25f;

        // Start is called before the first frame update
        void Start()
        {
        }

        // Update is called once per frame
        void Update()
        {
            Vector2 directionalInput;

            directionalInput.x = Input.GetAxis("Horizontal");
            if (player.inWater)
            {
                directionalInput.y = Input.GetAxis("Vertical");
                if (Input.GetButton("Jump"))
                {
                    directionalInput.y = 1.0f;
                }
                if(player.hasDash)
                {
                    updateDashCooldown();
                    if (dashCooldownTimer <= 0.0f)
                    {
                        if (Input.GetButtonDown("Dash") && directionalInput.x != 0.0f)
                        {
                            player.dash();
                            dashCooldownTimer = dashCooldown;
                        }
                    }
                }
            }
            else
            {
                directionalInput.y = 0.0f;
                if (Input.GetButtonDown("Jump"))
                {
                    player.OnJumpButtonPressed();
                }
                if (Input.GetButtonUp("Jump"))
                {
                    player.OnJumpButtonReleased();
                }
            }
            player.SetDirectionalMovement(directionalInput);
            if(Input.GetButtonDown("Interact"))
            {
                if (player.canCollect)
                {
                    player.pickupCollectable();
                }
                else if (player.hasInventory)
                {
                    if(player.canStore)
                    {
                        player.storeCollectable();
                    }
                    else
                    {
                        player.dropCollectable();
                    }
                }
            }
        }

        void updateDashCooldown()
        {
            if(dashCooldownTimer > 0.0f)
            {
                dashCooldownTimer -= Time.deltaTime;
            }
        }
    }

}