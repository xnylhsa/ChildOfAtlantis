using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChildOfAtlantis.Mechanics
{
    /// <summary>
    /// Implements game physics for some in game entity.
    /// </summary>
    public class RayCastController : MonoBehaviour
    {

        public BoxCollider2D boxCollider;
        const float distBetweenRays = 0.005f;

        protected const float skinWidth = 0.0015f;
        [HideInInspector]
        public int horizontalRayCount = 4;
        public LayerMask collisionMask;
        [HideInInspector]
        public int verticalRayCount = 4;
        protected float verticalRaySpacing;
        protected float horizontalRaySpacing;
        protected RaycastOrigins raycastOrigins;
        // Use this for initialization
        protected virtual void Awake()
        {
            Debug.Assert(boxCollider);
        }
        protected virtual void Start()
        {
            UpdateRayCastOrigin();
            CalculateRaySpacing();
        }
        protected void UpdateRayCastOrigin()
        {
            Bounds bounds = boxCollider.bounds;
            bounds.Expand(skinWidth * -2);
            raycastOrigins.bottomLeft = new Vector2(bounds.min.x, bounds.min.y);
            raycastOrigins.bottomRight = new Vector2(bounds.max.x, bounds.min.y);

            raycastOrigins.topLeft = new Vector2(bounds.min.x, bounds.max.y);
            raycastOrigins.topRight = new Vector2(bounds.max.x, bounds.max.y);


        }
        protected void CalculateRaySpacing()
        {
            Bounds bounds = boxCollider.bounds;
            bounds.Expand(skinWidth * -2);
            float boundsWidth = bounds.size.x;
            float boundsHeight = bounds.size.y;

            horizontalRayCount = Mathf.RoundToInt(boundsHeight / distBetweenRays);
            verticalRayCount = Mathf.RoundToInt(boundsWidth / distBetweenRays);


            horizontalRaySpacing = bounds.size.y / (horizontalRayCount - 1);
            verticalRaySpacing = bounds.size.x / (verticalRayCount - 1);


        }


        // Update is called once per frame
        protected virtual void Update()
        {
            UpdateRayCastOrigin();

        }

        protected struct RaycastOrigins
        {
            public Vector2 topLeft, topRight;
            public Vector2 bottomLeft, bottomRight;
        }


    }

}