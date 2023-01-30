using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// use this to spawn growers when clicking on nothing, despawn when clicking on something
/// growers are little loops with one sustained note. they start in silence, get louder, and get quieter
/// how loud they get depends on how long the mouse is held
/// 
/// inspired by Electroplankton -- Sun Amaculae (Nintendo) and Bloom (Eno/Chilvers)
/// </summary>
public class GrowerSpawner : MonoBehaviour
{

    Grower currentGrower;

    public PositionToPitch posToPitch;
    public GameObject growerPrefab;

    public float maxGrowerTime = 5f;
    public float maxGrowerScale = 2f;



    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            Collider2D targetObject = Physics2D.OverlapPoint(mousePosition);
            if (targetObject && targetObject.gameObject.GetComponent<Grower>() != null)
            {
                targetObject.gameObject.GetComponent<Grower>().PopGrower();
            }
            else
            {
                Vector3 startPos = (Camera.main.ScreenToWorldPoint(Input.mousePosition));
                startPos.z = 0f;
                GameObject newObject = Instantiate(growerPrefab, startPos, Quaternion.identity);
                currentGrower = newObject.GetComponent<Grower>();
                currentGrower.posToPitch = posToPitch;
                currentGrower.maxScale = maxGrowerScale;
                currentGrower.maxTime = maxGrowerTime;
                currentGrower.startScale = new Vector3(0.5f, 0.5f, 0.5f);
            }


        }

        if (Input.GetMouseButton(0) && currentGrower != null)
        {
            currentGrower.Grow();
        }

        if(Input.GetMouseButtonUp(0) && currentGrower != null)
        {
            currentGrower.StopGrowing();
            currentGrower = null;
        }
    }
}
