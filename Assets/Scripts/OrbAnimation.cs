using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;

public class OrbAnimation : MonoBehaviour
{
    Vector3 startPos;

    [SerializeField] float floatSpeed;
    [SerializeField] float floatDistance;
    [SerializeField] float crystalLerpSpeed;

    [SerializeField] GameObject[] pedestals;
    List<GameObject> crystals = new();
    [SerializeField] Vector3[] crystalRestingPos;
    List<LineRenderer> lightningRenderers = new();
    int pedestalIncrement = 0;


    [SerializeField] GameObject crystalPrefab;
    [SerializeField] Material lightningMat;

    void Start()
    {
        startPos = transform.position;
    }

    void Update()
    {
        Vector3 newPos = new Vector3(0.0f, Mathf.Sin(Time.time * floatSpeed) * floatDistance, 0.0f);
        transform.position = startPos + newPos;

        foreach(LineRenderer lineRenderer in lightningRenderers)
        {
            lineRenderer.SetPosition(1, this.transform.position);
        }
    }

    [YarnCommand("SpawnInitialOrbs")]
    public void OnLevelStart()
    {
        for (int i = 0; i < 2; i++)
        {
            GameObject crystal = Instantiate(crystalPrefab, pedestals[i].transform.GetChild(0).transform.position, Quaternion.identity, pedestals[i].transform);
            crystals.Add(crystal);
            crystal.GetComponent<Collider2D>().enabled = false;

            var child = new GameObject();
            child.transform.parent = pedestals[i].transform;

            LineRenderer lightning = child.AddComponent<LineRenderer>();
            lightningRenderers.Add(lightning);
            lightning.SetPosition(0, crystal.transform.position);
            lightning.SetPosition(1, this.transform.position);

            List<Material> materials = new List<Material>() { lightningMat };
            lightning.SetMaterials(materials);
        }
    }

    [ContextMenu("Break Pedestal")]
    public void OnPedestalBreak()
    {
        StartCoroutine(BreakPedestal());
    }

    IEnumerator BreakPedestal()
    {
        foreach (LineRenderer lineRenderer in lightningRenderers)
        {
            Destroy(lineRenderer.gameObject);
        }

        lightningRenderers.Clear();

        for (int i = 0; i < 2; i++)
        {
            Vector3 startingPos = crystals[i].transform.position;
            float t = 0f;
            while (Vector3.Distance(crystals[i].transform.position, crystalRestingPos[i]) > 0.5f)
            {
                crystals[i].transform.position = Vector3.Lerp(startingPos, crystalRestingPos[i], t);
                t += Time.deltaTime * crystalLerpSpeed;
                yield return new WaitForEndOfFrame();
            }
        }

        foreach (GameObject crystal in crystals)
        {
            Destroy(crystal.gameObject);
        }
        crystals.Clear();
        yield return null;
    }

    void OnKeyCollected(object sender, KeyCollectEventArgs args)
    {
        if(args.parentDoor.name == "Final Door" && pedestalIncrement < pedestals.Length)
        {
            GameObject crystal = Instantiate(crystalPrefab, pedestals[pedestalIncrement].transform.GetChild(0).transform.position, Quaternion.identity, pedestals[pedestalIncrement].transform);
            crystal.GetComponent<Collider2D>().enabled = false;

            LineRenderer lightning = gameObject.AddComponent<LineRenderer>();
            lightningRenderers.Add(lightning);
            lightning.SetPosition(0, crystal.transform.position);
            lightning.SetPosition(1, this.transform.position);

            List<Material> materials = new List<Material>() { lightningMat };
            lightning.SetMaterials(materials);
            pedestalIncrement++;
        }
    }

    private void OnEnable()
    {
        LevelEvents.KeyCollected += OnKeyCollected;
    }

    private void OnDisable()
    {
        LevelEvents.KeyCollected -= OnKeyCollected;
    }
}
