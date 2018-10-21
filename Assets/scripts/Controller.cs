using System.Collections;
using System.Collections.Generic;
using System.IO.IsolatedStorage;
using DG.Tweening;
using UnityEngine;
using UnityEngine.AI;

/* 
 * controller
 * 		* touchscreen input
 * 		* mouse
 * 		* keyboard
 */

public enum EButton
{
    LEFT_BUTTON = 0,
    RIGHT_BUTTON = 1,
    MIDDLE_BUTTON = 2
}

public enum EControllerType
{
	touch, mouse, keyboard
}

public class Controller {   
	public EControllerType controllerType;

	public Controller()
	{
	}

	public Controller(EControllerType type)
	{
		this.controllerType = type;
	}

    public bool IsScreenPressed(EButton button, out RaycastHit raycast)
    {
        raycast = new RaycastHit();

        if (Input.GetMouseButtonDown((int)button))
        {
            RaycastHit hit;

            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 100))
            {
                raycast = hit;
                
                return true;
            }
        }
    
        return false;
    }


	/*
    public static Bounds CalculateBounds(GameObject go)
    {
        Bounds b = new Bounds(go.transform.position, Vector3.zero);
        foreach (Renderer r in go.GetComponentsInChildren<Renderer>())
        {
            b.Encapsulate(r.bounds);
        }
        return b;
    }
*/
    /*
    public static void FocusCameraOnGameObject(Camera c, GameObject go)
    {
        c.transform.localPosition = new Vector3(camTransform.localPosition.x, camTransform.localPosition.y, camTransform.localPosition.z);

        Bounds b = CalculateBounds(go);
        Vector3 max = b.size;
        float radius = Mathf.Max(max.x, Mathf.Max(max.y, max.z));
        float dist = radius / (Mathf.Sin(c.fieldOfView * Mathf.Deg2Rad / 2f));
        Debug.Log("Radius = " + radius + " dist = " + dist);
        Vector3 pos = c.transform.rotation.eulerAngles.normalized * dist + b.center;
        c.transform.position = pos;
        //c.transform.position = c.transform.rotation * dist + b.center; //new Vector3(c.transform.position.x,c.transform.position.y,dist);
        c.transform.LookAt(b.center);
    }
    */
          
        /*
    public static void FocusCameraOnGameObject(Camera c, GameObject go)
    {
        Bounds b = CalculateBounds(go);
        Vector3 center = b.center;
        Vector3 max = b.size;
        float radius = Mathf.Max(max.x, Mathf.Max(max.y, max.z));
        //float radius = max.magnitude / 2f;
        float fov = c.fieldOfView * Mathf.Deg2Rad;
        float camDistance = (radius*2.0f) / (Mathf.Tan(fov / 2.0f));
        // cameraPos.z = modelPos.z - cameraDistance 
        //c.transform.localPosition =  c.transform.rotation.eulerAngles.normalized * camDistance + b.center;
       // c.fieldOfView = camDistance;


        c.fieldOfView = FOVForHeightAndDistance(FrustumHeightAtDistance(camDistance), Vector3.Distance(c.transform.position, b.center));

        c.transform.LookAt(b.center);
    }
    */

    // Calculate the frustum height at a given distance from the camera.
    static float FrustumHeightAtDistance(float distance)
    {
        return 2.0f * distance * Mathf.Tan(Camera.main.fieldOfView * 0.5f * Mathf.Deg2Rad);
    }

    // Calculate the FOV needed to get a given frustum height at a given distance.
    static float FOVForHeightAndDistance(float height, float distance)
    {
        return 2.0f * Mathf.Atan(height * 0.5f / distance) * Mathf.Rad2Deg;
    }

    
    public static void FocusCameraOnGameObject(Camera c, GameObject go)
    {
        Bounds b = Level.CalculateBounds(go);

        Vector3 max = b.size;
        float radius = max.magnitude / 2f;
        float horizontalFOV = 2f * Mathf.Atan(Mathf.Tan(c.fieldOfView * Mathf.Deg2Rad / 2f) * c.aspect) * Mathf.Rad2Deg;     
        float fov = Mathf.Min(c.fieldOfView, horizontalFOV);
        float dist = radius / (Mathf.Sin(fov * Mathf.Deg2Rad / 2f));
       // Debug.Log("Radius = " + radius + " dist = " + dist);
       
        //c.fieldOfView = FOVForHeightAndDistance(FrustumHeightAtDistance(dist), Vector3.Distance(c.transform.position, b.center));
        float desiredFOV = FOVForHeightAndDistance(FrustumHeightAtDistance(dist), Vector3.Distance(c.transform.position, b.center));

        DOTween.To(() => c.fieldOfView, x => c.fieldOfView = x, desiredFOV, 0.2f).SetEase(Ease.InOutCubic);

        if (c.orthographic)
            c.orthographicSize = radius;

        // Frame the object hierarchy
        //c.transform.LookAt(b.center);
        c.transform.DOLookAt(b.center, 0.2f).SetEase(Ease.OutQuad);
    }
}
