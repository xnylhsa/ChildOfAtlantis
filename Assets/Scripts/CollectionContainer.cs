using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ChildOfAtlantis.Mechanics
{
    public class CollectionContainer : MonoBehaviour
    {
        public CollectibleType neededCollectible;
        public int numberOfNeededCollectibles = 3;
        public int storedCollectibles = 0;
        public Canvas infoCanvas;
        public Text numberStoredUIText;
        public Text numberStorableUIText;

        public delegate void AllItemsCollectedEvent();
        public event AllItemsCollectedEvent OnAllItemsCollected;
        // Start is called before the first frame update
        void Start()
        {
            numberStorableUIText.text = numberOfNeededCollectibles.ToString();
            infoCanvas.gameObject.SetActive(false);

        }

        // Update is called once per frame
        void Update()
        {
            //if has number of need collectibles do cool upgrade stuff here
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
            if(collectableToStore.type == neededCollectible && storedCollectibles < numberOfNeededCollectibles)
            {
                storedCollectibles++;
                numberStoredUIText.text = storedCollectibles.ToString();
                Destroy(collectableToStore);
                if (storedCollectibles >= numberOfNeededCollectibles)
                {
                    if (OnAllItemsCollected != null)
                    {
                        OnAllItemsCollected();
                    }
                }
                return true;
            }
            return false;
        }
    }
}