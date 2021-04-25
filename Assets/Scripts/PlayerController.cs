using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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
        float velocityXSmoothing;
        float velocityYSmoothing;
        [SerializeField]
        float maxGroundSpeed = 7;

        [SerializeField]
        float invulnTime = 1.0f;
        float invulnerableTimer;
        [SerializeField]
        float knockBackSpeed;
        bool knockedBack = false;
        bool invulnerable = false;

        [SerializeField]
        int maxHealth = 3;
        int health = 3;
        bool alive = true;
        protected Controller2D controller;
        [SerializeField]
        float maxJumpHeight = 1.75f;
        [SerializeField]
        float minJumpHeight = 1.0f;

        float maxJumpVelocity;
        float minJumpVelocity;
        [SerializeField]
        float timeToApex = 0.4f;
        [SerializeField]
        float accelerationTimeAirborne = 0.2f;
        [SerializeField]
        float accelerationTimeGrounded = 0.1f;
        float gravity = 1.0f;

        [SerializeField]
        float waterGravity = 0.0f;

        [HideInInspector]
        public bool controlEnabled = true;
        [HideInInspector]
        public bool inWater = false;
        [HideInInspector]
        public bool canCollect = false;
        [HideInInspector]
        public bool canStore = false;
        [HideInInspector]
        public bool hasInventory = false;
        Vector2 directionalInput;
        public float dashForce = 10.0f;
        bool dashInvuln = false;
        public float dashInvulnTime = 0.4f;
        public float dashInvulnTimer = 0.0f;
        public bool hasDash = false;
        public bool hasShield = false;
        bool isShieldUp = false;
        public float shieldRegenTime = 15.0f;
        float shieldRegenTimer;

        bool facingRight = true;
        bool jump;
        Vector2 velocity;
        SpriteRenderer spriteRenderer;
        Collectible nearbyCollectible;
        CollectionContainer nearbyContainer;
        Collectible storedCollectible;
        private void Awake()
        {
            //grab components here.
            controller = GetComponent<Controller2D>();
            gravity = -(maxJumpHeight * 2 / (timeToApex * timeToApex));
            maxJumpVelocity = timeToApex * Mathf.Abs(gravity);
            minJumpVelocity = Mathf.Sqrt(2 * Mathf.Abs(gravity) * minJumpHeight);
            health = maxHealth;
            invulnerableTimer = invulnTime;
            dashInvulnTimer = dashInvulnTime;
            shieldRegenTimer = shieldRegenTime;
            alive = true;
        }


        // Update is called once per frame
        void Update()
        {
            UpdateInvulnerabilityTimer();
            updateShieldRegen();
            if (dashInvuln)
            {
                directionalInput = Vector2.zero;
            }
            CalculateVelocity();
            if (knockedBack)
            {
                HandleKnockBack();
            }
            controller.Move(velocity * Time.deltaTime, directionalInput);
            HandleCollisons();
        }

        public bool takeDamage()
        {
            if (invulnerable || dashInvuln)
                return false;

            if(isShieldUp)
            {
                isShieldUp = false;
                shieldRegenTimer = 0.0f;
            }
            else
            {
                health--;
            }
            invulnerable = true;
            if (health <= 0)
            {
                alive = false;
                onDeath();
            }
            return true;
        }
        public void dash()
        {
            dashInvuln  = true;
            velocity.x = (facingRight ? 1 : -1) * dashForce;
            velocity.y = 0.0f;
        }
        void onDeath()
        {
            alive = false;
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        public int getHealth()
        {
            return health;
        }

        public bool IsShieldUp()
        {
            return isShieldUp;
        }

        public void heal()
        {
            if (!alive) return;
            if (health < maxHealth)
            {
                health++;
            }
        }
        private void OnTriggerStay2D(Collider2D other)
        {
            if (other.gameObject.layer == 10)
            {
                takeDamage();
            }
        }
        void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.layer == 4)
            {
                inWater = true;
            }
            else if (other.gameObject.layer == 8)
            {
                canStore = true;
                CollectionContainer collectionContainer = other.GetComponent<CollectionContainer>();

                Debug.Assert(collectionContainer, "failed to acquire CollectionContainer component on from a trigger specified as a container.");
                nearbyContainer = collectionContainer;
            }
            else if (other.gameObject.layer == 9)
            {
                canCollect = true;
                Collectible collectible = other.GetComponent<Collectible>();
                Debug.Assert(collectible, "failed to acquire Collectible component on from a trigger specified as a Collectible.");
                nearbyCollectible = collectible;
            }
            if (other.gameObject.layer == 11)
            {
                if(takeDamage())
                    Destroy(other.gameObject);
            }
        }

        void OnTriggerExit2D(Collider2D other)
        {
            if (other.gameObject.layer == 4)
            {
                inWater = false;
                velocity.y += gravity * Time.deltaTime;
            }
            else if (other.gameObject.layer == 8)
            {
                canStore = false;
                nearbyContainer = null;
            }
            else if (other.gameObject.layer == 9)
            {
                canCollect = false;
                nearbyCollectible = null;
            }
        }
        public void OnJumpButtonReleased()
        {
            if (velocity.y > minJumpVelocity)
            {
                velocity.y = minJumpVelocity;
            }
            if (velocity.y <= 0)
            {
                //animationContr.SetFalling(true);
            }
        }

        public void SetDirectionalMovement(Vector2 input)
        {
            directionalInput = input;
            if (directionalInput.x != 0)
            {
                if (directionalInput.x < 0 && transform.localScale.x > 0)
                {
                    facingRight = false;
                    transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
                }
                if (directionalInput.x > 0 && transform.localScale.x < 0)
                {
                    facingRight = true;
                    transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
                }
                if (velocity.y == 0)
                {
                    //animationContr.PlayMoving(true);
                }
            }
            else
            {
                if (velocity.y == 0)
                {
                    //animationContr.PlayIdle();

                }
            }
            if (velocity.y <= 0)
            {
                //animationContr.SetFalling(true);
            }
        }

        public void OnJumpButtonPressed()
        {
            if (controller.collisionInfo.below)
            {
                if (controller.collisionInfo.slidingDownSlope)
                {
                    if (directionalInput.x != -Mathf.Sign(controller.collisionInfo.slopeNormal.x))
                    {
                        velocity.y = maxJumpVelocity * controller.collisionInfo.slopeNormal.y;
                        velocity.x = maxJumpVelocity * controller.collisionInfo.slopeNormal.x;

                    }
                }
                else
                {
                    velocity.y = maxJumpVelocity;

                }
            }
            //animationContr.PlayJump();
            //animationContr.SetGrounded(false);
            //animationContr.SetFalling(false);

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
            if(dashInvuln)
            {
                dashInvulnTimer -= Time.deltaTime;
                if (dashInvulnTimer <= 0)
                {
                    dashInvuln = false;
                    dashInvulnTimer = dashInvulnTime;
                    //animationContr.DeactiveBlink();
                }
            }
        }

        void CalculateVelocity()
        {

            float targetVelocityX;
            if (!inWater)
            {
                targetVelocityX = directionalInput.x * maxGroundSpeed;
                velocity.y += gravity * Time.deltaTime;
                if (knockedBack)
                {
                    targetVelocityX = velocity.x;
                }
            }
            else
            {
                targetVelocityX = directionalInput.x * maxHorizontalWaterSpeed;
                float targetVelocityY = directionalInput.y * maxVerticalWaterSpeed;
                velocity.y += waterGravity * Time.deltaTime;
                velocity.y = Mathf.SmoothDamp(velocity.y, targetVelocityY, ref velocityYSmoothing, (controller.collisionInfo.below) ? accelerationTimeGrounded : accelerationTimeAirborne);
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
                    if (inWater)
                    {
                        velocity.y += Mathf.Clamp(controller.collisionInfo.slopeNormal.y * -waterGravity * Time.deltaTime, -maxVerticalWaterSpeed, maxVerticalWaterSpeed);
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

        public void pickupCollectable()
        {
            Debug.Assert(nearbyCollectible, "there is no nearby collectible to pickup!");
            if(nearbyCollectible.type == CollectibleType.ShieldRelic)
            {
                hasShield = true;
                Destroy(nearbyCollectible);
                return;
            }
            if(nearbyCollectible.type == CollectibleType.DashRelic)
            {
                hasDash = true;
                Destroy(nearbyCollectible);
                return;
            }
            dropCollectable();
            storedCollectible = nearbyCollectible;
            storedCollectible.pickUp(gameObject);
            hasInventory = true;
        }

        public void storeCollectable()
        {
            Debug.Assert(nearbyContainer, "there is no nearbyContainer for storage!");
            Debug.Assert(storedCollectible, "there is no collectible to store!");

            if (nearbyContainer.storeCollectible(storedCollectible))
            {
                storedCollectible = null;
                hasInventory = false;
            }
        }

        public void dropCollectable()
        {
            if (storedCollectible)
            {
                storedCollectible.drop();
                storedCollectible = null;
                hasInventory = false;
            }
        }

        void updateShieldRegen()
        {
            if(hasShield && !isShieldUp)
            {
                shieldRegenTimer += Time.deltaTime;
                if(shieldRegenTimer >= shieldRegenTime)
                {
                    isShieldUp = true;
                }
            }
        }

    }

}


