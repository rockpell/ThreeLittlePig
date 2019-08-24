using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindEffect : MonoBehaviour
{
    [SerializeField] private Sprite[] windImages;
    //지속시간
    [SerializeField] private float duration;
    //지속시간 카운터
    private float durationCnt;
    //깜박임 주기
    [SerializeField] private float tinkleTime;
    private float tinkleCntTime;
    private int index;
    private MapNode node;
    [SerializeField] private AudioSource audio;

    void Start()
    {
        
    }
    public void initializeNodeTime(float time, MapNode node)
    {
        duration = time;
        this.node = node;
        audio.Play();
        index = 0;
    }
    // Update is called once per frame
    void Update()
    {
        durationCnt += Time.deltaTime;
        tinkleCntTime += Time.deltaTime;
        if(node.WallState == WallType.NONE)
        {
            audio.Stop();
            Destroy(this.gameObject);
        }
        if (durationCnt > duration)
        {
            audio.Stop();
            Destroy(this.gameObject);
        }
        if (tinkleCntTime > tinkleTime)
        {
            index = (index + 1) % windImages.Length;
            this.gameObject.GetComponent<SpriteRenderer>().sprite = windImages[index];
            tinkleCntTime = 0;
        }
    }
}
