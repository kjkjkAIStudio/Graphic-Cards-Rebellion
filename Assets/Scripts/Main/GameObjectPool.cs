using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 缓冲池，用于经常加载和卸载的GameObject，可以减少CPU负荷，减少GC。
/// </summary>
public class GameObjectPool : MonoBehaviour
{
    [Tooltip("缓冲池中的prefab")]
    public GameObject prefab;
    [Tooltip("当缓冲池被创建时，预备的GameObject数目")]
    public int initCount;
    [Tooltip("如果使用的对象大于安全范围（initCount-warningRange到initCount+warningRange），把回收的对象卸载。超出安全范围时，卸载缓冲池会报警。")]
    public int warningRange;
    [Tooltip("该数值仅供查看缓冲池的实时负载，修改它是毫无意义的")]
    public int loadCount;

    /// <summary>
    /// prefab中必须附加此类的派生类，用于prefab访问其对应的pool，以及重新使用对象时重置对象到初始状态。
    /// </summary>
    public abstract class PoolElement : MonoBehaviour
    {
        [HideInInspector]
        public GameObjectPool pool;

        public abstract void Reset();
    }

    /// <summary>
    /// 从池中取得缓冲的GameObject上附带的PoolElement的object形态（防止GetComponent），如果池已经空了，则会用Instantiate创建一个并返回。
    /// </summary>
    /// <returns>取出的GameObject</returns>
    public object Get()
    {
        ++cUsing;
        loadCount = cUsing;
        if (maxUsing < cUsing)
            maxUsing = cUsing;

        if (poolElements.Count == 0)
        {
            PoolElement temp = Instantiate(prefab, transform).GetComponent<PoolElement>();
            temp.pool = this;
            return temp;
        }
        else
        {
            int lastIndex = poolElements.Count - 1;
            poolElements[lastIndex].gameObject.SetActive(true);
            PoolElement temp = poolElements[lastIndex];
            poolElements.RemoveAt(lastIndex);
            return temp;
        }
    }

    /// <summary>
    /// 回收指定的GameObject，并启动它的Reset方法。如果池已经到达安全范围上限，这个GameObject会被Destroy
    /// （警告：它必须是从池中取出的。）
    /// </summary>
    /// <param name="handle">所回收的GameObject</param>
    /// <param name="reseter">附在其中的Reset方法</param>
    public void Recycle(PoolElement element)
    {
        if (poolElements.Count < maxWarning)
        {
            element.gameObject.SetActive(false);
            element.Reset();
            poolElements.Add(element);
        }
        else
        {
            Destroy(element.gameObject);
        }

        --cUsing;
        loadCount = cUsing;
    }

    List<PoolElement> poolElements;
    int maxWarning;
    int maxUsing, cUsing;

    void Awake()
    {
        poolElements = new List<PoolElement>();
        maxWarning = initCount + warningRange;
        maxUsing = 0;
        cUsing = 0;

        for (int i = 0; i < initCount; ++i)
        {
            GameObject temp = Instantiate(prefab, transform);
            temp.SetActive(false);
            PoolElement temp2 = temp.GetComponent<PoolElement>();
            temp2.pool = this;
            poolElements.Add(temp2);
        }
    }

    void OnDestroy()
    {
        if (maxUsing > maxWarning)
            Debug.LogWarningFormat("对象池“{0}”负载警报！GameObject的最大使用数为{1}。", gameObject.name, maxUsing);
    }
}
