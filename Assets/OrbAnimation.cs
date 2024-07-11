using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbAnimation : MonoBehaviour
{
    Vector3 startPos;

    [SerializeField] float floatSpeed;

    [SerializeField] GameObject[] pedestals;
    [SerializeField] List<LineRenderer> lightningRenderers;
    int pedestalIncrement = 0;
    [SerializeField] GameObject crystalPrefab;
    [SerializeField] Material lightningMat;
    // Start is called before the first frame update
    void Start()
    {
        startPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 newPos = new Vector3(0.0f, Mathf.Sin(Time.time * floatSpeed), 0.0f);
        transform.position = startPos + newPos;

        foreach(LineRenderer lineRenderer in lightningRenderers)
        {
            lineRenderer.SetPosition(1, this.transform.position);
        }
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
