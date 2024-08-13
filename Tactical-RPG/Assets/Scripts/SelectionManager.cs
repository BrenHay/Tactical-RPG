using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectionManager : MonoBehaviour
{
    Transform selectedUnit;
    bool unitSelected = false;

    private void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            bool hasHit = Physics.Raycast(ray, out hit);

            if(hasHit)
            {
                if(hit.transform.tag == "Tile")
                {
                    if(unitSelected)
                    {
                        Vector3 targetCords = new Vector3(hit.transform.position.x, 0.60f, hit.transform.position.z);
                        selectedUnit.transform.position = targetCords;
                        selectedUnit.GetComponent<Unit>().tileCurrentlyOn.GetComponentInChildren<ShowCursor>().selectionCursor.SetActive(false);
                        selectedUnit.GetComponent<Unit>().tileCurrentlyOn = hit.transform.gameObject;
                    }
                    unitSelected = false;
                    selectedUnit = null;
                }

                if(hit.transform.tag == "Unit")
                {
                    if(unitSelected)
                    {
                        selectedUnit.GetComponent<Unit>().tileCurrentlyOn.GetComponentInChildren<ShowCursor>().selectionCursor.SetActive(false);
                    }
                    selectedUnit = hit.transform;
                    unitSelected = true;
                    selectedUnit.GetComponent<Unit>().tileCurrentlyOn.GetComponentInChildren<ShowCursor>().selectionCursor.SetActive(true);
                }
            }
        }
    }
}
