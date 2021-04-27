using System.Collections;
using System.Collections.Generic;
using UnityEngine;



namespace ChildOfAtlantis.Mechanics 
{ 
    public class CameraController : MonoBehaviour
    {

        public Controller2D followTarget;
        public Vector2 focusAreaSize;
        public float verticalOffset;
        public float lookAheadDstX;
        public float lookSmoothTimeX;
        public float verticalSmoothTime;
        public float minY;
        public float maxY;
        public float minX;
        public float maxX;

        float currentLookAheadX;
        float targetLookAheadX;
        float lookAheadDirX;
        float smoothLookVelocityX;
        float smoothLookVelocityY;
        Camera cam;
        FocusArea focusArea;
        bool lookAheadStopped = false;
        // Use this for initialization
        void Start()
        {
            focusArea = new FocusArea(followTarget.boxCollider.bounds, focusAreaSize);
            cam = GetComponent<Camera>();
        }

        private void LateUpdate()
        {
            focusArea.Update(followTarget.boxCollider.bounds);
            Vector2 focusPosition = focusArea.Center + Vector2.up * verticalOffset;
            if (focusArea.velocity.x != 0)
            {
                lookAheadDirX = Mathf.Sign(focusArea.velocity.x);
                lookAheadStopped = false;
                if (Mathf.Sign(followTarget.playerInput.x) == Mathf.Sign(focusArea.velocity.x) && followTarget.playerInput.x != 0)
                {
                    targetLookAheadX = lookAheadDirX * lookAheadDstX;
                }
                else if (!lookAheadStopped)
                {
                    lookAheadStopped = true;
                    targetLookAheadX = currentLookAheadX + (lookAheadDirX * lookAheadDstX - currentLookAheadX) / 4f;
                }
            }
            currentLookAheadX = Mathf.SmoothDamp(currentLookAheadX, targetLookAheadX, ref smoothLookVelocityX, lookSmoothTimeX);
            focusPosition.y = Mathf.SmoothDamp(transform.position.y, focusPosition.y, ref smoothLookVelocityY, verticalSmoothTime);
            focusPosition += Vector2.right * currentLookAheadX;
            transform.position = (Vector3)focusPosition + Vector3.forward * -10;
            if (transform.position.y - (cam.orthographicSize) < minY)
            {
                transform.position = new Vector3(transform.position.x, minY + cam.orthographicSize, transform.position.z);
            }
            else if (transform.position.y + (cam.orthographicSize) > maxY)
            {
                transform.position = new Vector3(transform.position.x, maxY - cam.orthographicSize, transform.position.z);
            }

            if (transform.position.x - (2f * cam.orthographicSize) < minX)
            {
                transform.position = new Vector3(minX + (2f * cam.orthographicSize), transform.position.y , transform.position.z);
            }
            else if (transform.position.x + (2f * cam.orthographicSize) > maxX)
            {
                transform.position = new Vector3(maxX - (2f * cam.orthographicSize), transform.position.y, transform.position.z);
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = new Color(1, 0, 0, 0.5f);
            Gizmos.DrawCube(focusArea.Center, focusAreaSize);
            Gizmos.DrawLine(new Vector3(-100.0f, minY), new Vector3(100.0f, minY));

            Gizmos.DrawLine(new Vector3(minX, -100.0f), new Vector3(minX, 100.0f));
            Gizmos.DrawLine(new Vector3(-100.0f, maxY), new Vector3(100.0f, maxY));
            Gizmos.DrawLine(new Vector3(maxX, -100.0f), new Vector3(maxX, 100.0f));


        }
        // Update is called once per frame
        void Update()
        {

        }
        struct FocusArea
        {
            public Vector2 Center, velocity;
            float left, right;
            float top, bottom;
            public FocusArea(Bounds targetBounds, Vector2 size)
            {
                left = targetBounds.center.x - size.x / 2;
                right = targetBounds.center.x + size.x / 2;
                bottom = targetBounds.min.y;
                top = targetBounds.min.y + size.y;
                velocity = Vector2.zero;
                Center = new Vector2((left + right) / 2, (top + bottom) / 2);
            }
            public void Update(Bounds targetBounds)
            {
                float shiftX = 0;
                if (targetBounds.min.x < left)
                {
                    shiftX = targetBounds.min.x - left;
                }
                else if (targetBounds.max.x > right)
                {
                    shiftX = targetBounds.max.x - right;
                }
                left += shiftX;
                right += shiftX;

                float shiftY = 0;
                if (targetBounds.min.y < bottom)
                {
                    shiftY = targetBounds.min.y - bottom;
                }
                else if (targetBounds.max.y > top)
                {
                    shiftY = targetBounds.max.y - top;
                }
                top += shiftY;
                bottom += shiftY;
                Center = new Vector2((left + right) / 2, (top + bottom) / 2);
                velocity = new Vector2(shiftX, shiftY);
            }
        }
    }

}