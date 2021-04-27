using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ChildOfAtlantis.Mechanics;
public class GameController : MonoBehaviour
{
    public PlayerController player;
    public PlayerInput playerInputController;
    public RectTransform healthPanel;
    public List<GameObject> heartUIObjects;
    public Image dashCooldownIndicator;
    public Image shipCompass;
    public CollectionContainer shipBuildLocation;
    public CollectionContainer firstUpgradeLocation;
    public CollectionContainer finalUpgradeLocation;
    public GameObject shieldEffect;
    public GameObject firstProgressGate;
    public GameObject secondProgressGate;
    int lastPlayerHealth = 0;
    bool lastShieldState = false;
    int buildProgress = 0;
    // Start is called before the first frame update
    void Start()
    {
        int playersHealth = player.getHealth();
        for(int i = playersHealth; i < heartUIObjects.Count; i++)
        {
            heartUIObjects[i].SetActive(false);
        }
        dashCooldownIndicator.gameObject.SetActive(player.hasDash);
        lastPlayerHealth = playersHealth;
        if (firstUpgradeLocation)
        {
            firstUpgradeLocation.gameObject.SetActive(false);
            firstUpgradeLocation.OnAllItemsCollected += OnAllItemsCollected;

        }
        if (finalUpgradeLocation)
        {
            finalUpgradeLocation.gameObject.SetActive(false);
            finalUpgradeLocation.OnAllItemsCollected += OnAllItemsCollected;
        }
        if (shipBuildLocation)
        {
            shipBuildLocation.gameObject.SetActive(true);
            shipBuildLocation.OnAllItemsCollected += OnAllItemsCollected;
        }

    }

    // Update is called once per frame
    void Update()
    {
        if (player.hasDash)
        {
            if (!dashCooldownIndicator.isActiveAndEnabled)
                dashCooldownIndicator.gameObject.SetActive(player.hasDash);
            if (playerInputController.dashCooldownTimer <= 0.0f)
            {
                dashCooldownIndicator.color = Color.cyan;
            }
            else
            {
                dashCooldownIndicator.color = Color.Lerp(Color.black, Color.blue, 1 - (playerInputController.dashCooldownTimer / playerInputController.dashCooldown));
            }
        }
        if (player.hasShield && player.IsShieldUp() && shieldEffect.activeInHierarchy == false)
        {
            shieldEffect.SetActive(true);
        }
        else if(!player.hasShield || !player.IsShieldUp())
        {
            shieldEffect.SetActive(false);
        }
        int playersHealth = player.getHealth();
        if (playersHealth != lastPlayerHealth)
        {
            lastPlayerHealth = playersHealth;

            for (int i = 0; i < heartUIObjects.Count; i++)
            {
                if (i < playersHealth)
                {
                    heartUIObjects[i].SetActive(true);
                }
                else if (heartUIObjects[i].activeInHierarchy)
                {
                    heartUIObjects[i].SetActive(false);
                }
            }
        }
        if(shipCompass)
        {
            Vector3 currentShipPosition;
            if(buildProgress == 0)
            {
                currentShipPosition = shipBuildLocation.transform.position;
            }
            else if (buildProgress == 1)
            {
                currentShipPosition = firstUpgradeLocation.transform.position;
            }
            else 
            {
                currentShipPosition = finalUpgradeLocation.transform.position;
            }

            Vector3 vectorToTarget = currentShipPosition - player.transform.position;
            float angle = (Mathf.Atan2(vectorToTarget.y, vectorToTarget.x) * Mathf.Rad2Deg);
            shipCompass.rectTransform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }
    }

    void OnAllItemsCollected()
    {
        if(buildProgress == 0)
        {
            if (shipBuildLocation)
                shipBuildLocation.gameObject.SetActive(false);
            if (finalUpgradeLocation)
                firstUpgradeLocation.gameObject.SetActive(true);
            if (firstProgressGate)
                firstProgressGate.gameObject.SetActive(false);
            buildProgress++;
        }
        else if (buildProgress == 1)
        {
            if (shipBuildLocation)
                firstUpgradeLocation.gameObject.SetActive(false);
            if (finalUpgradeLocation)
                finalUpgradeLocation.gameObject.SetActive(true);
            if (firstProgressGate)
                secondProgressGate.gameObject.SetActive(false);

        }
    }
}
