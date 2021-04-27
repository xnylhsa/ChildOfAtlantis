using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChildOfAtlantis.Mechanics
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class Collectible : MonoBehaviour
    {

        public CollectibleType type;
        SpriteRenderer spriteRenderer;
        // Start is called before the first frame update
        void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        // Update is called once per frame
        void Update()
        {

        }

        public void drop()
        {
            spriteRenderer.enabled = true;
            transform.SetParent(transform.parent.parent.parent);
            Collider2D collider = GetComponent<Collider2D>();
            collider.enabled = true;
        }
        public void pickUp(GameObject owner)
        {
            spriteRenderer.enabled = false;
            transform.SetParent(owner.transform);
            Collider2D collider = GetComponent<Collider2D>();
            collider.enabled = false;
        }
    }

    public enum CollectibleType
    {
        ShieldRelic = 0,
        DashRelic,
        ShipPeice,
        Wood,
        SeaWeed,
        Shell,
        SeaFern,
        ShipPiece,
        Metal,
        Rope,
        MAX
    }

}

