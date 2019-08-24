using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SummonWolf : MonoBehaviour
{
    [SerializeField] private float timer;
    private float timeCnt;
    private MapNode node;
    public MapNode Node
    {
        set { node = value; }
    }
    [SerializeField] private AudioSource audio;

    void Start()
    {
        audio.Play();
    }

    // Update is called once per frame
    void Update()
    {
        timeCnt += Time.deltaTime;
        if(timeCnt > timer)
        {
            if(node != null)
            {
                node.changeState(WallType.NONE);
            }
            audio.Stop();
            Destroy(this.gameObject);
        }
    }

}
