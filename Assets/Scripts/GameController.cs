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
    public CollectionContainer shipBuildLocation;
    public CollectionContainer firstUpgradeLocation;
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
        firstUpgradeLocation.gameObject.SetActive(false);
        firstUpgradeLocation.OnAllItemsCollected += OnAllItemsCollected;
        shipBuildLocation.OnAllItemsCollected += OnAllItemsCollected;
    }

    // Update is called once per frame
    void Update()
    {
        if(player.hasDash)
        {
            if(!dashCooldownIndicator.isActiveAndEnabled)
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
        int playersHealth = player.getHealth();
        bool isShieldUp = player.IsShieldUp();
        if (playersHealth != lastPlayerHealth || lastShieldState != isShieldUp)
        {
            lastPlayerHealth = playersHealth;
            lastShieldState = isShieldUp;
            for (int i = 0; i < heartUIObjects.Count; i++)
            {
                if (i < playersHealth)
                {
                    heartUIObjects[i].SetActive(true);
                    if (isShieldUp)
                    {
                        heartUIObjects[i].GetComponent<Image>().color = Color.cyan;
                    }
                    else
                    {
                        heartUIObjects[i].GetComponent<Image>().color = Color.red;
                    }
                }
                else if (heartUIObjects[i].activeInHierarchy)
                {
                    heartUIObjects[i].SetActive(false);
                }
            }
        }
    }

    void OnAllItemsCollected()
    {
        if(buildProgress == 0)
        {
            shipBuildLocation.gameObject.SetActive(false);
            firstUpgradeLocation.gameObject.SetActive(true);
            buildProgress++;
        }
        else if (buildProgress == 1)
        {
            firstUpgradeLocation.gameObject.SetActive(false);
        }
    }
}
