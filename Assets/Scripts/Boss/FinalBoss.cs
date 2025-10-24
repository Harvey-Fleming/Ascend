using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Yarn.Unity;

public class FinalBoss : Boss
{
    bool isActive = false;

    [SerializeField] State currentState;

    [SerializeField] float phase = 1;

    Dictionary<GameObject, bool> leverStates = new();
    public UnityEvent ResetLevers;

    public GameObject debugSphere;
    public State CurrentState { get => currentState; set => currentState = value; }
    public float Phase { get => phase; set => phase = value; }

    #region - Arrow Variables
    [Header("Arrow Attack")]

    [Tooltip("Minimum Position for arrows shooting across the screen (Left to Right)")]
    [SerializeField] private Transform hMin;
    [Tooltip("Maximum Position for arrows shooting across the screen (Left to Right)")]
    [SerializeField] private Transform hMax;
    [Tooltip("Minimum Position for arrows shooting down the screen (Top to Bottom)")]
    [SerializeField] private Transform vMin;
    [Tooltip("Maximum Position for arrows shooting down the screen (Top to Bottom)")]
    [SerializeField] private Transform vMax;


    [Space]
    [SerializeField] private GameObject arrowPrefab;

    public GameObject ArrowPrefab { get => arrowPrefab; set => arrowPrefab = value; }
    public Transform HMin { get => hMin; set => hMin = value; }
    public Transform HMax { get => hMax; set => hMax = value; }
    public Transform VMin { get => vMin; set => vMin = value; }
    public Transform VMax { get => vMax; set => vMax = value; }

    #endregion

    #region - Beam Variables

    Coroutine beam1 = null;
    Coroutine beam2 = null;
    Coroutine beam3 = null;
    Coroutine beam4 = null;
    [Header("Beam Attack")]
    [SerializeField] LayerMask beamlayerMask;
    [SerializeField] LayerMask playerMask;
    [SerializeField] private float beamSpeed = 10f;

    [Space]
    [SerializeField] private GameObject heart;

    public LayerMask BeamlayerMask { get => beamlayerMask; set => beamlayerMask = value; }
    public LayerMask PlayerMask { get => playerMask; set => playerMask = value; }
    public float BeamSpeed { get => beamSpeed; set => beamSpeed = value; }

    public Coroutine Beam1 { get => beam1; set => beam1 = value; }
    public Coroutine Beam2 { get => beam2; set => beam2 = value; }
    public Coroutine Beam3 { get => beam3; set => beam3 = value; }
    public Coroutine Beam4 { get => beam4; set => beam4 = value; }
    public bool IsActive { get => isActive; set => isActive = value; }

    #endregion

    private void Start()
    {
        //StartFight();
    }

    // Update is called once per frame
    void Update()
    {
        if (isActive)
        {
            currentState.Execute();
        }
    }

    [ContextMenu("Start Fight")]
    [YarnCommand("StartFinalBoss")]
    public void StartFight()
    {
        if (!isActive)
        {
            isActive = true;
            GameObject.Find("Boss Door").GetComponent<Animator>().SetTrigger("CloseFinalDoor");
            if(currentState != null)
            {
                currentState.EndState();
            }
            currentState = new FinalIdleState(this);
        }

        ResetLeverDictionary();
    }

    private void ResetLeverDictionary()
    {
        FinalBossLever[] levers = GameObject.FindObjectsOfType<FinalBossLever>();
        leverStates.Clear();
        foreach (FinalBossLever lever in levers)
        {
            leverStates.Add(lever.gameObject, false);
        }

        foreach (KeyValuePair<GameObject, bool> pair in leverStates)
        {
            Debug.Log(pair.Key + " has a value of " + pair.Value);

        }
    }

    public void FlipLever(GameObject lever, bool isFlipped)
    {
        if (leverStates.ContainsKey(lever))
        {
            leverStates[lever] = isFlipped;
        }

        foreach(KeyValuePair<GameObject, bool> pair in leverStates)
        {
            if (pair.Value == false)
            {
                return;
            }  
        }
        TakeDamage();
        ResetLevers?.Invoke();
    }

    [ContextMenu("Damage Heart")]
    public override void TakeDamage()
    {
        Debug.Log(currentHealth);
        StopAllCoroutines();
        currentState.EndState();
        base.TakeDamage();

        heart.GetComponent<Animator>().SetTrigger("takedamage");

        if (currentHealth == maxHealth - 1)
        {         
            GameObject.FindObjectOfType<DialogueRunner>().StartDialogue("bossphasewarning");
            ResetLeverDictionary();
            return;
        }

        if(currentHealth <= 0)
        {
            currentState.EndState();
            heart.GetComponent<Animator>().SetTrigger("Death");
            GameObject.Find("HeartDeathParticles").GetComponent<ParticleSystem>().Play();
            currentState = new FinalDeathState(this);
            GameObject.FindObjectOfType<DialogueRunner>().StartDialogue("postfinalboss");
        }

        if (currentState is FinalBeamsState)
            currentState = new FinalArrowState(this);
        else if (currentState is FinalArrowState)
            currentState = new FinalBeamsState(this);
        ResetLeverDictionary();
    }

    [YarnCommand("TriggerNextFinalBossPhase")]
    public void TriggerNextPhase()
    {
        currentState.EndState();
        currentState = new FinalSprayState(this, currentState);
        phase = 2;
    }

    private void OnEnable()
    {
        FinalBossLever.flipLever.AddListener(FlipLever);
    }

    [YarnCommand("TransformHeart")]
    public void TransformHeart()
    {
        if(heart.TryGetComponent<Animator>(out Animator heartAnimator))
        {
            heartAnimator.SetBool("isCombined", true);
        }
        else
        {
            Debug.Log("Heart Does not have an animator");
        }
        
    }
}

public class FinalIdleState : State
{
    public FinalIdleState(Boss controller) : base(controller)
    {

    }

    public override void EndState()
    {
        throw new System.NotImplementedException();
    }

    public override void Execute()
    {
        ((FinalBoss)controller).CurrentState = new FinalArrowState(controller);
    }
}

public class FinalDeathState : State
{
    public FinalDeathState(Boss controller) : base(controller)
    {

    }

    public override void EndState()
    {
        
    }

    public override void Execute()
    {
        
    }
}

public class FinalBeamsState : State
{
    int i = 0;
    List<LineRenderer> renderers = new();
    public FinalBeamsState(Boss controller) : base(controller)
    {
        foreach (Transform child in controller.transform.GetChild(0).transform)
        {
            switch (i)
            {
                case 0:
                    ((FinalBoss)controller).Beam1 = controller.StartCoroutine(BeamAttack(child.gameObject.GetComponent<LineRenderer>(), i * 90));
                    break;
                case 1:
                    if(((FinalBoss)controller).Phase > 1)
                    ((FinalBoss)controller).Beam2 = controller.StartCoroutine(BeamAttack(child.gameObject.GetComponent<LineRenderer>(), i * 90));
                    break;
                case 2:
                    ((FinalBoss)controller).Beam3 = controller.StartCoroutine(BeamAttack(child.gameObject.GetComponent<LineRenderer>(), i * 90));
                    break;
                case 3:
                    if(((FinalBoss)controller).Phase > 1)
                    ((FinalBoss)controller).Beam4 = controller.StartCoroutine(BeamAttack(child.gameObject.GetComponent<LineRenderer>(), i * 90));
                    break;
            }

            i++;
        }
    }

    public override void Execute()
    {

    }

    IEnumerator BeamAttack(LineRenderer renderer, float baseOffset)
    {
        renderers.Add(renderer);
        renderer.enabled = true;
        float beamCounter = 0;
        while (beamCounter <= 6.283)
        {
            //Get a direction based off the desired angle (Incremented at the end of the while loop)
            Vector3 v = Quaternion.AngleAxis((beamCounter + (baseOffset * Mathf.PI / 180)) * 180 / Mathf.PI, Vector3.forward) * Vector3.up;
            Debug.DrawLine(renderer.GetPosition(0), renderer.GetPosition(0) + v.normalized, Color.red, 0.5f);

            RaycastHit2D hit;
            //Set Line renderer endpoint to ray end point.
            if (hit = Physics2D.Raycast(renderer.GetPosition(0), v.normalized, Mathf.Infinity, ((FinalBoss)controller).BeamlayerMask))
            {
                renderer.SetPosition(1, hit.point);
                //if (baseOffset / 90 == 0)
                //    GameObject.Instantiate(((FinalBoss)controller).debugSphere, hit.point, Quaternion.identity);
            }

            RaycastHit2D[] hitAll;
            //Detects if player is in ray's path
            if ((hitAll = Physics2D.RaycastAll(renderer.GetPosition(0), v.normalized, 25, ((FinalBoss)controller).PlayerMask)).Length != 0)
            {
                GameObject closestObject = null;
                float closestDistance = float.MaxValue;

                foreach (RaycastHit2D hits in hitAll)
                {
                    //Ignore this object if it is one of these
                    if (hits.collider.isTrigger || hits.collider.name == controller.gameObject.name || (!hits.collider.CompareTag("Ground") && !hits.collider.CompareTag("Player")))
                    {
                        continue;
                    }
                    //Check if this object is the closest to the ray origin
                    if ((renderer.GetPosition(0) - hits.collider.transform.position).sqrMagnitude < closestDistance)
                    {
                        closestObject = hits.collider.gameObject;
                        closestDistance = (renderer.GetPosition(0) - hits.collider.transform.position).sqrMagnitude;
                    }

                }
                
                //If it is the player, deal damage
                if (closestObject != null && closestObject.CompareTag("Player"))
                {

                    closestObject.GetComponent<PlayerHealth>().TakeDamage();
                }
            }

            //increment the beam angle
            if(((FinalBoss)controller).Phase == 1)
                beamCounter += ((FinalBoss)controller).BeamSpeed * 0.15f;
            else
                beamCounter += (((FinalBoss)controller).BeamSpeed * 1.25f) * 0.15f;
            yield return new WaitForSeconds(0.15f);
        }
        renderer.enabled = false;
        ((FinalBoss)controller).CurrentState = new FinalArrowState(controller);
    }

    public override void EndState()
    {
        foreach(LineRenderer renderer in renderers)
        {
            renderer.enabled = false;
        }
    }
}

public class FinalArrowState : State
{
    List<GameObject> arrows = new();
    FinalBoss boss;

    bool hasShotHorizontal = false;

    float arrowNumber = 0;

    public List<GameObject> Arrows { get => arrows; set => arrows = value; }

    public FinalArrowState(Boss controller) : base(controller)
    {
        boss = ((FinalBoss)controller);
        arrowNumber = boss.Phase > 1 ? 4 : 3;

        SpawnVerticalArrows(controller);
    }

    public override void EndState()
    {
        foreach (GameObject arrow in arrows)
        {
            GameObject.Destroy(arrow);
        }
        arrows.Clear();
    }

    public override void Execute()
    {
    }

    private void SpawnVerticalArrows(Boss controller)
    {
        FinalBoss boss = ((FinalBoss)controller);
        SpawnArrows(180f, boss.VMin.position, boss.VMax.position);

        boss.StartCoroutine(ShootArrows(Vector3.down));
    }

    private void SpawnHorizontalArrows()
    {
        hasShotHorizontal = true;
        SpawnArrows(-90, boss.HMin.position, boss.HMax.position);

        boss.StartCoroutine(ShootArrows(Vector3.right));
    }

    private void SpawnArrows(float angle, Vector3 minPos, Vector3 maxPos)
    {
        for (int i = 0; i < (arrowNumber); i++)
        {
            //Debug.Log("Spawning Arrow Number: " + i);
            GameObject obj = GameObject.Instantiate(boss.ArrowPrefab, Vector3.zero, Quaternion.identity);
            obj.GetComponent<ArrowCollision>().CanHitGround = false;
            obj.GetComponent<ArrowCollision>().DisableGroundCollision();

            obj.transform.position = Vector3.Lerp(minPos, maxPos, (1 / (arrowNumber - 1)) * i);
            obj.transform.rotation = Quaternion.Euler(obj.transform.rotation.x, obj.transform.rotation.y, angle);

            arrows.Add(obj);
        }
    }

    IEnumerator ShootArrows(Vector3 direction)
    {
        foreach(GameObject arrow in arrows)
        {
            arrow.GetComponent<ArrowCollision>().ShootInDirection(direction, 10f, 5f);
            yield return new WaitForSeconds(0.5f);
        }
        yield return new WaitForSeconds(2f);
        foreach (GameObject arrow in arrows)
        {
            GameObject.Destroy(arrow);
        }
        arrows.Clear();

        if (!hasShotHorizontal)
            SpawnHorizontalArrows();
        else
            boss.CurrentState = new FinalBeamsState(controller);
    }
}

public class FinalSprayState : State
{
    GameObject heart;

    GameObject bloodPool;
    Vector3 poolDefaultPos;
    public FinalSprayState(Boss controller, State previousState) : base(controller)
    {
        if(previousState is FinalArrowState)
        {
            ((FinalArrowState)previousState).EndState();
        }
        else if(previousState is FinalBeamsState)
        {
            ((FinalBeamsState)previousState).EndState();
        }

        heart = controller.gameObject.transform.GetChild(1).gameObject;

        for(int i = 0; i < 2; i++)
        {
            heart.transform.GetChild(i).GetComponent<ParticleSystem>().Play();
        }

        bloodPool = GameObject.Find("BloodPool");
        poolDefaultPos = bloodPool.transform.position;
    }

    public override void EndState()
    {
        for (int i = 0; i < 2; i++)
        {
            heart.transform.GetChild(i).GetComponent<ParticleSystem>().Stop();
        }
    }

    public override void Execute()
    {
        if(bloodPool.transform.position.y <= poolDefaultPos.y + 1f)
        {
            bloodPool.transform.position = new Vector3(bloodPool.transform.position.x, bloodPool.transform.position.y + (0.5f * Time.deltaTime), bloodPool.transform.position.z);
        }
        else
        {
            EndState();
            ((FinalBoss)controller).CurrentState = new FinalBeamsState(controller);
        }
    }
}

