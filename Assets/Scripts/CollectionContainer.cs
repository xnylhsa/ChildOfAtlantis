using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ChildOfAtlantis.Mechanics
{
    [System.Serializable]
    public struct CollectionRequirement
    {
        public int numberOfNeededCollectibles;
        public int storedCollectibles;
        public CollectibleType neededCollectible;
        public Text numberStoredUIText;
        public Text numberStorableUIText;
    }

    public class CollectionContainer : MonoBehaviour
    {
        public List<CollectionRequirement> collectionRequirements;

        public Canvas infoCanvas;

        public delegate void AllItemsCollectedEvent();
        public event AllItemsCollectedEvent OnAllItemsCollected;
        // Start is called before the first frame update
        void Start()
        {
            infoCanvas.gameObject.SetActive(false);
        }

        // Update is called once per frame
        void Update()
        {
            bool allItemsStored = true;
            //if has number of need collectibles do cool upgrade stuff here
            for (int i = 0; i < collectionRequirements.Count; i++)
            {
                CollectionRequirement collectionReq = collectionRequirements[i];
                collectionReq.numberStoredUIText.text = collectionRequirements[i].storedCollectibles.ToString();
                collectionReq.numberStorableUIText.text = collectionRequirements[i].numberOfNeededCollectibles.ToString();

                collectionRequirements[i] = collectionReq;
                if (collectionRequirements[i].storedCollectibles < collectionRequirements[i].numberOfNeededCollectibles)
                {
                    allItemsStored = false;
                }

            }
            if (OnAllItemsCollected != null && allItemsStored)
            {
                OnAllItemsCollected();
                OnAllItemsCollected = null;
            }
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.gameObject.layer == 3)
            {
                infoCanvas.gameObject.SetActive(true);
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.gameObject.layer == 3)
            {
                infoCanvas.gameObject.SetActive(false);
            }
        }

        public bool storeCollectible(Collectible collectableToStore)
        {
            for (int i = 0; i < collectionRequirements.Count; i++)
            {
                if (collectionRequirements[i].neededCollectible == collectableToStore.type && collectionRequirements[i].storedCollectibles < collectionRequirements[i].numberOfNeededCollectibles)
                {
                    CollectionRequirement collectionReq = collectionRequirements[i];
                    collectionReq.storedCollectibles++;
                    collectionRequirements[i] = collectionReq;
                    collectionRequirements[i].numberStoredUIText.text = collectionRequirements[i].storedCollectibles.ToString();
                    Destroy(collectableToStore);
                    return true;
                }

            }

            return false;


        }
    }
}