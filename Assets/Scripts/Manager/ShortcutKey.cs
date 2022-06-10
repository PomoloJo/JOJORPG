using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShortcutKey : MonoBehaviour
{

    public GameObject playerUI;
    public GameObject bagUI;


    //private Transform playerStatusUI;

    private void Start() 
    {
        //playerStatusUI = playerUI.transform.Find("PlayerStatusUI");    
    }

    // Update is called once per frame
    void Update()
    {
        ChangeWeapon();
        OpenAndCloseStatus();
        OpenAndCloseBag();
    }

    public void ChangeWeapon()
    {
        if(Input.GetKeyDown(KeyCode.C))
        {
            // 交换 武器 与 副手武器 的 UI ，主要是指 item 与 slot 的互换
            var weaponSlot = StatusUIController.Instance.playerUI.transform.Find("WeaponSlot");
            var offhandWeaponSlot = StatusUIController.Instance.playerUI.transform.Find("OffhandWeaponSlot");
            var weaponItem = weaponSlot.GetChild(0);
            var offhandWeaponItem = offhandWeaponSlot.GetChild(0);

            weaponItem.SetParent(offhandWeaponSlot);
            weaponItem.position = offhandWeaponSlot.position;
            offhandWeaponItem.SetParent(weaponSlot);
            offhandWeaponItem.position = weaponSlot.position;


            // 交换 武器 与 副手武器 在 装备字典 中的值
            var temp = StatusUIController.Instance.equipmentDictionary["WeaponSlot"];
            StatusUIController.Instance.equipmentDictionary["WeaponSlot"] = StatusUIController.Instance.equipmentDictionary["OffhandWeaponSlot"];
            StatusUIController.Instance.equipmentDictionary["OffhandWeaponSlot"] = temp;

            // 存在装备变更，刷新 状态 && UI
            Player.Instance.RefreshStatus();
        }
    }

    public void OpenAndCloseStatus()
    {
        if(Input.GetKeyDown(KeyCode.V))
        {
            TipsUIController.Instance.HideTips();
            playerUI.SetActive(!playerUI.activeSelf);
            if(playerUI.activeSelf)
            {
                Player.Instance.RefreshStatus();
            }
        }
    }

    public void OpenAndCloseBag()
    {
        if(Input.GetKeyDown(KeyCode.B))
        {
            TipsUIController.Instance.HideTips();
            bagUI.SetActive(!bagUI.activeSelf);
        }
    }
}
