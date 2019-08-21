using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SVFixMaxCamera : MonoBehaviour
{
    public bool follow = true; //Whether the camera should follow the player
    private Vector3 moveTo; //Target vector
    private Pig player;

    [SerializeField] private GameObject bottomBorder = null; 
    [SerializeField] private GameObject upBorder = null; 
    [SerializeField] private GameObject leftBorder = null;
    [SerializeField] private GameObject rightBorder = null;

    private float xMin;
    private float xMax;
    private float yMin;
    private float yMax;

    // Start is called before the first frame update
    void Start()
    {
        player = FindObjectOfType<Pig>();
        InitBorder();
    }

    // Update is called once per frame
    void Update()
    {
        float x = Mathf.Clamp(player.transform.position.x, xMin, xMax);
        float y = Mathf.Clamp(player.transform.position.y, yMin, yMax);
        transform.position = new Vector3(x, y, transform.position.z);
    }

    private void InitBorder()
    {
        xMin = leftBorder.transform.position.x;
        xMax = rightBorder.transform.position.x;
        yMin = bottomBorder.transform.position.y;
        yMax = upBorder.transform.position.y;
    }
}
