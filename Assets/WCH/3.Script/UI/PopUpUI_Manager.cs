using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopUpUI_Manager : MonoBehaviour
{

    [SerializeField] public GameObject ExitPopUp;
    public void OnExitPopUp()
    {
        ExitPopUp.SetActive(true);
    }

    public void OffExitPopUp()
    {
        ExitPopUp.SetActive(false);
    }

}
