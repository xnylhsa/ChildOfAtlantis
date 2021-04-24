using System.Collections;
using System.Collections.Generic;
using UnityEngine;



namespace ChildOfAtlantis.Mechanics
{
    public class Controller2D : RayCastController
    {
        //Max Slope AngleIn Degrees
        public float maxSlope = 80;
        [HideInInspector]
        public Vector2 playerInput;
        protected override void Start()
        {
            base.Start();
            collisionInfo.faceDir = 1;
        }
        [HideInInspector]
        public CollisionInfo collisionInfo;
        public void Move(Vector2 moveVector, bool onPlatform = false)
        {
            Move(moveVector, Vector2.zero, onPlatform);
        }
        public void Move(Vector2 moveVector, Vector2 input, bool onPlatform = false)
        {
            playerInput = input;
            UpdateRayCastOrigin();
            collisionInfo.reset();
            collisionInfo.lastVelocity = moveVector;

            if (moveVector.y < 0)
            {
                DescendSlope(ref moveVector);
            }
            if (moveVector.x != 0)
            {
                collisionInfo.faceDir = Mathf.Sign(moveVector.x);
            }
            HorizontalCollisions(ref moveVector);

            if (moveVector.y != 0)
            {
                VerticalCollisions(ref moveVector);
            }
            if (onPlatform)
            {
                collisionInfo.below = true;
            }
            transform.Translate(moveVector);

        }

        void HorizontalCollisions(ref Vector2 velocity)
        {
            float directionX = collisionInfo.faceDir;
            float rayLength = Mathf.Abs(velocity.x) + skinWidth;
            if (Mathf.Abs(velocity.x) < skinWidth)
            {
                rayLength = 2 * skinWidth;
            }
            for (int i = 0; i < horizontalRayCount; i++)
            {
                Vector2 rayOrigin = (directionX == -1) ? raycastOrigins.bottomLeft : raycastOrigins.bottomRight;
                rayOrigin += Vector2.up * (i * horizontalRaySpacing);
                RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength, collisionMask);
                if (hit)
                {
                    if (hit.distance == 0 || hit.transform.gameObject.layer == 10)
                    {
                        continue;
                    }
                    float SlopeAngle = Vector2.Angle(hit.normal, Vector2.up);

                    if (i == 0 && SlopeAngle <= maxSlope)
                    {
                        if (collisionInfo.descendingSlope)
                        {
                            collisionInfo.descendingSlope = false;
                            velocity = collisionInfo.lastVelocity;
                        }
                        float distanceToSlope = 0;
                        if (SlopeAngle != collisionInfo.lastSlopeAngle)
                        {
                            distanceToSlope = hit.distance - skinWidth;
                            velocity.x -= distanceToSlope * directionX;
                        }
                        ClimbSlope(ref velocity, SlopeAngle, hit.normal);
                        velocity.x += distanceToSlope * directionX;

                    }
                    if (SlopeAngle > maxSlope || !(collisionInfo.cimbingSlope))
                    {
                        velocity.x = (hit.distance - skinWidth) * directionX;
                        rayLength = (hit.distance);

                        if (collisionInfo.cimbingSlope)
                        {
                            velocity.y = Mathf.Tan(collisionInfo.SlopeAngle * Mathf.Deg2Rad * Mathf.Abs(velocity.x));
                        }
                        collisionInfo.left = directionX == -1;
                        collisionInfo.right = directionX == 1;
                    }
                }
                Debug.DrawRay(rayOrigin, Vector2.right * directionX);

            }
        }
        void VerticalCollisions(ref Vector2 velocity)
        {
            float directionY = Mathf.Sign(velocity.y);
            float rayLength = Mathf.Abs(velocity.y) + skinWidth;
            for (int i = 0; i < verticalRayCount; i++)
            {
                Vector2 rayOrigin = (directionY == -1) ? raycastOrigins.bottomLeft : raycastOrigins.topLeft;
                rayOrigin += Vector2.right * (i * verticalRaySpacing + velocity.x);
                RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up * directionY, rayLength, collisionMask);
                if (hit)
                {
                    if (hit.transform.gameObject.layer == 10) //if layer == fallthrough platform
                    {
                        if (directionY == 1 || hit.distance == 0 || collisionInfo.fallingThroughPlatform)
                            continue;
                        if (playerInput.y == -1)
                        {
                            collisionInfo.fallingThroughPlatform = true;
                            Invoke("ResetFallingThroughPlatform", 0.25f);
                            continue;
                        }
                    }
                    velocity.y = (hit.distance - skinWidth) * directionY;

                    rayLength = (hit.distance);
                    if (collisionInfo.cimbingSlope)
                    {
                        velocity.x = velocity.y / Mathf.Tan(collisionInfo.SlopeAngle * Mathf.Deg2Rad) * Mathf.Sign(velocity.x);
                    }
                    collisionInfo.below = directionY == -1;
                    collisionInfo.above = directionY == 1;
                }
                Debug.DrawRay(rayOrigin, Vector2.up * directionY);
                Debug.DrawRay(rayOrigin, velocity * 10, Color.red);

            }

            if (collisionInfo.cimbingSlope)
            {
                float directionX = Mathf.Sign(velocity.x);
                rayLength = Mathf.Abs(velocity.x + skinWidth);
                Vector2 rayorigin = ((directionX == -1) ? raycastOrigins.bottomLeft : raycastOrigins.bottomRight) + Vector2.up * velocity.y;
                RaycastHit2D hit = Physics2D.Raycast(rayorigin, Vector2.right * directionX, rayLength, collisionMask);
                if (hit)
                {
                    float SlopeAngle = Vector2.Angle(hit.normal, Vector2.up);
                    if (SlopeAngle != collisionInfo.SlopeAngle)
                    {
                        velocity.x = (hit.distance - skinWidth) * directionX;
                        collisionInfo.SlopeAngle = SlopeAngle;
                        collisionInfo.slopeNormal = hit.normal;
                    }
                }
            }
        }

        void ClimbSlope(ref Vector2 deltaMovement, float slopeangle, Vector2 slopeNormal)
        {
            float distance = Mathf.Abs(deltaMovement.x);
            float y = distance * Mathf.Sin(slopeangle * Mathf.Deg2Rad);
            if (!(deltaMovement.y > y))
            {
                deltaMovement.y = y;
                deltaMovement.x = distance * Mathf.Cos(slopeangle * Mathf.Deg2Rad) * Mathf.Sign(deltaMovement.x);
                collisionInfo.below = true;
                collisionInfo.cimbingSlope = true;
                collisionInfo.SlopeAngle = slopeangle;
                collisionInfo.slopeNormal = slopeNormal;

            }
        }
        void DescendSlope(ref Vector2 deltaMovement)
        {
            RaycastHit2D maxSlopeHitLeft = Physics2D.Raycast(raycastOrigins.bottomLeft, -Vector2.up, Mathf.Abs(deltaMovement.y) + skinWidth, collisionMask);
            RaycastHit2D maxSlopeHitRight = Physics2D.Raycast(raycastOrigins.bottomLeft, -Vector2.up, Mathf.Abs(deltaMovement.y) + skinWidth, collisionMask);
            if (maxSlopeHitLeft ^ maxSlopeHitRight)
            {
                SlideDownMaxSlope(maxSlopeHitLeft, ref deltaMovement);
                SlideDownMaxSlope(maxSlopeHitRight, ref deltaMovement);
            }
            if (!collisionInfo.slidingDownSlope)
            {
                float directionX = Mathf.Sign(deltaMovement.x);
                Vector2 rayOrigin = (directionX == -1) ? raycastOrigins.bottomRight : raycastOrigins.bottomLeft;
                RaycastHit2D hit = Physics2D.Raycast(rayOrigin, -Vector2.up, Mathf.Infinity, collisionMask);
                if (hit)
                {
                    float SlopeAngle = Vector2.Angle(hit.normal, Vector2.up);
                    if (SlopeAngle != 0 && SlopeAngle <= maxSlope)
                    {
                        if (Mathf.Sign(hit.normal.x) == directionX)
                        {
                            if (hit.distance - skinWidth <= Mathf.Tan(SlopeAngle * Mathf.Deg2Rad) * Mathf.Abs(deltaMovement.x))
                            {

                                float moveDistance = Mathf.Abs(deltaMovement.x);
                                float descendVelocityY = Mathf.Sin(SlopeAngle * Mathf.Deg2Rad) * moveDistance;
                                deltaMovement.x = moveDistance * Mathf.Cos(SlopeAngle * Mathf.Deg2Rad) * Mathf.Sign(deltaMovement.x);
                                deltaMovement.y -= descendVelocityY;
                                collisionInfo.SlopeAngle = SlopeAngle;
                                collisionInfo.below = true;
                                collisionInfo.descendingSlope = true;
                                collisionInfo.slopeNormal = hit.normal;

                            }

                        }
                    }
                }

            }


        }

        void SlideDownMaxSlope(RaycastHit2D hit, ref Vector2 deltaMovement)
        {
            if (hit)
            {
                float SlopeAngle = Vector2.Angle(hit.normal, Vector2.up);
                if (SlopeAngle >= maxSlope)
                {
                    deltaMovement.x = hit.normal.x * (Mathf.Abs(deltaMovement.y) - hit.distance / Mathf.Tan(SlopeAngle * Mathf.Deg2Rad));
                    collisionInfo.SlopeAngle = SlopeAngle;
                    collisionInfo.slidingDownSlope = true;
                    collisionInfo.slopeNormal = hit.normal;
                }
            }
        }
        void ResetFallingThroughPlatform()
        {
            collisionInfo.fallingThroughPlatform = false;
        }


        public RaycastHit2D[] BoxCastAll(Vector2 size, bool forward, LayerMask thingsToHit, int castDistance)
        {
            Vector2 direction = Vector2.zero;
            direction.x = (forward) ? collisionInfo.faceDir : -collisionInfo.faceDir;
            return Physics2D.BoxCastAll(transform.position, size, transform.eulerAngles.z, direction, castDistance, thingsToHit);
        }
        public int BoxCastNonAlloc(Vector2 size, bool forward, LayerMask thingsToHit, float castDistance, ref RaycastHit2D[] hits)
        {
            Vector2 direction = Vector2.zero;
            direction.x = (forward) ? collisionInfo.faceDir : -collisionInfo.faceDir;
            return Physics2D.BoxCastNonAlloc(transform.position, size, transform.eulerAngles.z, direction, hits, castDistance, thingsToHit);
        }

        public Collider2D[] OverlapAreaAll(Vector2 areaSize, LayerMask thingsToHit)
        {
            Vector2 topLeft;
            topLeft.x = transform.position.x - (areaSize.x / 2);
            topLeft.y = transform.position.y + (areaSize.y / 2);

            Vector2 bottomRight;
            bottomRight.x = transform.position.x + (areaSize.x / 2);
            bottomRight.y = transform.position.y - (areaSize.y / 2);
            return Physics2D.OverlapAreaAll(topLeft, bottomRight, thingsToHit);
        }
        public int OverlapAreaNonAlloc(Vector2 areaSize, LayerMask thingsToHit, ref Collider2D[] results)
        {
            Vector2 topLeft;
            topLeft.x = transform.position.x - (areaSize.x / 2);
            topLeft.y = transform.position.y + (areaSize.y / 2);

            Vector2 bottomRight;
            bottomRight.x = transform.position.x + (areaSize.x / 2);
            bottomRight.y = transform.position.y - (areaSize.y / 2);
            return Physics2D.OverlapAreaNonAlloc(topLeft, bottomRight, results, thingsToHit);
        }
        public Collider2D[] OverlapCircleAll(LayerMask thingsToHit, float radius)
        {
            return Physics2D.OverlapCircleAll(transform.position, radius, thingsToHit);
        }
        public int OverlapCircleNonAlloc(LayerMask thingsToHit, float radius, ref Collider2D[] results)
        {
            return Physics2D.OverlapCircleNonAlloc(transform.position, radius, results, thingsToHit);
        }
        public struct CollisionInfo
        {
            public bool above, below;
            public bool left, right;
            public bool cimbingSlope, descendingSlope;
            public bool slidingDownSlope;
            public bool fallingThroughPlatform;
            public float SlopeAngle, lastSlopeAngle;
            public Vector2 lastVelocity;
            public Vector2 slopeNormal;
            public float faceDir;
            public void reset()
            {
                above = below = left = right = false;
                cimbingSlope = false;
                descendingSlope = false;
                slidingDownSlope = false;
                lastSlopeAngle = SlopeAngle;
                slopeNormal = Vector2.zero;
                SlopeAngle = 0.0f;
            }


        }


    }
}



