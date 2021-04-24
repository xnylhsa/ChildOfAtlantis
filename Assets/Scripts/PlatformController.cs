using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace ChildOfAtlantis.Mechanics
{ 

    public class Platform2D : RayCastController
    {

        public LayerMask passengerMask;
        public float waitTime = 0.0f;
        public List<Vector3> localMovementLocations = new List<Vector3>();
        public int fromIndex = 0;
        public float movementSpeed;
        List<Vector3> globalMoveLocations = new List<Vector3>();
        public bool cyclic = false;
        public List<PassengerMoveInfo> moveInfo = new List<PassengerMoveInfo>();
        public bool drawDebug;
        float percentBetweenWaypoints;
        public Color debugColor = Color.red;
        public float debugPointSize = 2f;
        float nextMoveTime = 0.0f;
        Dictionary<Transform, Controller2D> controllerDictionary = new Dictionary<Transform, Controller2D>();
        [Range(0, 2)]
        public float EaseAmount = 0;
        // Use this for initialization
        protected override void Start()
        {
            base.Start();
            foreach (Vector3 loc in localMovementLocations)
            {
                Vector3 worldPos = loc + transform.position;
                globalMoveLocations.Add(worldPos);
            }
        }

        // Update is called once per frame
        protected override void Update()
        {
            base.Update();
            moveInfo.Clear();
            Vector3 Velocity = CalculatePlatformMovement();
            CalculatePassengerMovement(Velocity);
            MovePassengers(true);
            transform.Translate(Velocity);
            MovePassengers(false);
        }
        float Ease(float x)
        {
            float a = EaseAmount + 1;
            float xtoA = Mathf.Pow(x, a);
            return xtoA / (xtoA + Mathf.Pow(1 - x, a));
        }
        Vector3 CalculatePlatformMovement()
        {
            if (Time.time < nextMoveTime)
            {
                return Vector3.zero;
            }
            int toIndex = (fromIndex + 1) % globalMoveLocations.Count;
            if (!cyclic)
            {
                toIndex = fromIndex + 1;

                if (toIndex >= globalMoveLocations.Count)
                {
                    fromIndex = 0;
                    globalMoveLocations.Reverse();
                    toIndex = fromIndex + 1;
                }
            }
            float distance = Vector3.Distance(globalMoveLocations[fromIndex], globalMoveLocations[toIndex]);
            percentBetweenWaypoints += Time.deltaTime * movementSpeed / distance;
            percentBetweenWaypoints = Mathf.Clamp01(percentBetweenWaypoints);
            float easedPercent = Ease(percentBetweenWaypoints);
            Vector3 velocity = Vector3.Lerp(globalMoveLocations[fromIndex], globalMoveLocations[toIndex], easedPercent);
            if (percentBetweenWaypoints >= 1.0f)
            {
                fromIndex = toIndex;
                percentBetweenWaypoints = 0.0f;
                nextMoveTime = Time.time + waitTime;
            }
            return velocity - transform.position;
        }

        void MovePassengers(bool beforeMovePlatform)
        {
            foreach (PassengerMoveInfo info in moveInfo)
            {
                if (!(controllerDictionary.ContainsKey(info.transform)))
                {
                    controllerDictionary.Add(info.transform, info.transform.GetComponent<Controller2D>());
                }
                if (info.moveBeforePlatform == beforeMovePlatform)
                {
                    controllerDictionary[info.transform].Move(info.targetVelocity, info.onPlatform);
                }
            }
        }
        void CalculatePassengerMovement(Vector3 Velocity)
        {
            HashSet<Transform> movedPassengers = new HashSet<Transform>();
            float directionX = Mathf.Sign(Velocity.x);
            float directionY = Mathf.Sign(Velocity.y);

            if (Velocity.y != 0)
            {
                float rayLength = Mathf.Abs(Velocity.y) + skinWidth;
                for (int i = 0; i < verticalRayCount; i++)
                {
                    Vector2 rayOrigin = (directionY == -1) ? raycastOrigins.bottomLeft : raycastOrigins.topLeft;
                    rayOrigin += Vector2.right * (i * verticalRaySpacing);
                    RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up * directionY, rayLength, passengerMask);
                    if (hit && hit.distance != 0)
                    {

                        if (!(movedPassengers.Contains(hit.transform)))
                        {
                            float pushX = (directionY == 1) ? Velocity.x : 0;
                            float pushY = Velocity.y - (hit.distance - skinWidth) * directionY;
                            moveInfo.Add(new PassengerMoveInfo(hit.transform, new Vector3(pushX, pushY, 0), directionY == 1, true));
                            movedPassengers.Add(hit.transform);
                        }
                    }
                }
            }
            if (Velocity.x != 0)
            {
                float rayLength = Mathf.Abs(Velocity.x) + skinWidth;
                for (int i = 0; i < horizontalRayCount; i++)
                {
                    Vector2 rayOrigin = (directionX == -1) ? raycastOrigins.bottomLeft : raycastOrigins.bottomRight;
                    rayOrigin += Vector2.up * (i * horizontalRaySpacing);
                    RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength, passengerMask);
                    if (hit && hit.distance != 0)
                    {
                        float pushX = Velocity.x - (hit.distance - skinWidth) * directionX;
                        float pushY = -skinWidth;
                        moveInfo.Add(new PassengerMoveInfo(hit.transform, new Vector3(pushX, pushY, 0), false, true));
                        movedPassengers.Add(hit.transform);
                    }
                }
            }

            if (directionY == -1 || Velocity.y == 0 && Velocity.x != 0)
            {
                float rayLength = skinWidth * 2;
                for (int i = 0; i < verticalRayCount; i++)
                {
                    Vector2 rayOrigin = raycastOrigins.topLeft + Vector2.right * (i * verticalRaySpacing);
                    RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up, rayLength, passengerMask);
                    if (hit && hit.distance != 0)
                    {
                        if (!(movedPassengers.Contains(hit.transform)))
                        {
                            float pushX = Velocity.x;
                            float pushY = Velocity.y;

                            movedPassengers.Add(hit.transform);
                            moveInfo.Add(new PassengerMoveInfo(hit.transform, new Vector3(pushX, pushY, 0), true, false));

                        }
                    }
                }
            }
        }
        public struct PassengerMoveInfo
        {
            public Transform transform;
            public Vector3 targetVelocity;
            public bool onPlatform;
            public bool moveBeforePlatform;
            public PassengerMoveInfo(Transform _transform, Vector3 velocity, bool playerOnPlatform, bool movePlayerBeforePlatform)
            {
                transform = _transform;
                targetVelocity = velocity;
                onPlatform = playerOnPlatform;
                moveBeforePlatform = movePlayerBeforePlatform;
            }
        }
        private void OnDrawGizmos()
        {
            if (drawDebug)
            {
                Gizmos.color = debugColor;
                int index = 0;
                foreach (Vector3 loc in localMovementLocations)
                {
                    Vector3 globalPos = (Application.isPlaying) ? globalMoveLocations[index] : loc + transform.position;
                    Gizmos.DrawLine(globalPos - Vector3.up * debugPointSize, globalPos + Vector3.up * debugPointSize);
                    Gizmos.DrawLine(globalPos - Vector3.right * debugPointSize, globalPos + Vector3.right * debugPointSize);
                    index++;

                }
            }
        }
    }
}
