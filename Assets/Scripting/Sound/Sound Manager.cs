using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    [SerializeField] private GameObject waterSFX;
    private float period;
    private void PlayWaterDrips()
    {
        StopAllCoroutines();
        GameObject waterClone = Instantiate(waterSFX);
        Destroy(waterClone, 1f);
        StartCoroutine(WaterCycle());
    }
    private void Start()
    {
        period = Random.Range(1f, 10f);
        StartCoroutine(WaterCycle());
    }

    private IEnumerator WaterCycle()
    {
        yield return new WaitForSeconds(period);
        PlayWaterDrips();
        period = Random.Range(1f, 10f);
    }
}
