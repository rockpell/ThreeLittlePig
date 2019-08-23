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

    void Start()
    {
        
    }
    public void initializeTime(float time)
    {
        duration = time;
        index = 0;
    }
    // Update is called once per frame
    void Update()
    {
        durationCnt += Time.deltaTime;
        tinkleCntTime += Time.deltaTime;
        if (durationCnt > duration)
        {
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
