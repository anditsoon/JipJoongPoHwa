using System.Collections;
using UnityEngine;
using UnityEngine.Pool;

public class Feather : MonoBehaviour
{
    private GameObject ally;
    public IObjectPool<GameObject> pool;

    void Start()
    {
        ally = GameObject.Find("Ally");
        pool = ally.GetComponent<BSkill>().featherPool;
    }

    public void StopFeatherCoroutine()
    {
        StopAllCoroutines();
    }

    public IEnumerator AutoDestroy(float time)
    {
        yield return new WaitForSeconds(time);
        if(gameObject.activeSelf) pool.Release(gameObject);
    }
}
