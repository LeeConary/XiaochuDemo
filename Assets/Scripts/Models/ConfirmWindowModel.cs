using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ConfirmWindowModel : MonoBehaviour {

	public void OnClickToClose()
    {
        Destroy(this);
        
    }
}
