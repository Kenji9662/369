using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.InputSystem;

[RequireComponent(typeof(ARRaycastManager))]
[RequireComponent(typeof(PlayerInput))]
public class Manager : MonoBehaviour
{
    //NumberObjs List
    public GameObject[] numPrefabs;
    //Number List
    public static List<string> nList = new List<string>() { "1", "2", "3", "4", "5", "6", "7", "8", "9", "10" };

    // random value & Get Pointer to Get Random NumberObj
    public int ranNum; 
    public string  spawnPointer = "";

    // Get AR Place & Set SpawnObj Transform List 
    public List<Transform> PlanePos = new List<Transform>();
    List<Transform> spawnPos = new List<Transform>();
    
    // Check new AR Plane
    int PlaneCount = 0;

    //Input type
    PlayerInput playerInput;

    [SerializeField]
    InputAction Touch, TouchPos;

    //RayCastEvent
    ARRaycastManager mARRaycastManager;
    List<ARRaycastHit> hits = new List<ARRaycastHit>();

    //Set hit type 
    [SerializeField]
    TrackableType trackableType = TrackableType.Planes;


    //Define Spwan Function
    IEnumerator spawnFunTrigger;
    
    //spawn Delay Time
    public float spawnDelay = 1.25f;


    private void Awake()
    {
        //Run Function when get Touch
        playerInput = GetComponent<PlayerInput>();
        Touch.performed += _ => { GetObj(); };
    }

    void Start()
    {
        mARRaycastManager = GetComponent<ARRaycastManager>();
    }



    void Update()
    {
        if (PlaneCount < PlanePos.Count)
        {
            if (spawnPos.Count == 10)
                return;

            // Set Random Spawn Object Qty
            int ranQty = Random.Range(0.2);

            spawnFunTrigger = SpawnObjFun(ranQty, PlanePos[PlaneCount]);
            StartCoroutine(spawnFunTrigger);

            PlaneCount += 1;
        }

    }

    void GetObj()
    {
        // Get touch position ( Vector2 : screen posistion to Vector3 : Game position) 
        Vector3 pos = Camera.main.ScreenToWorldPoint(TouchPos.ReadValue<Vector2>());

        if (mARRaycastManager.Raycast(pos, hits, trackableType))
        {
            var hitsPose = hits[0].pose;

            objEvent _objEvent = hitsPose.transform.GetComponent<objEvent>();

            if (_objEvent != null)
            {
                //Get Object Event
            }
        }
    }

    // i = Spawn Object Qty 
    IEnumerator SpawnObjFun(int i, Transform pos)
    {
        for (int i2 = 0; i2 < i; i2++)
        {
            //  run programme after "spawnDelay" time
            yield return new WaitForSeconds(spawnDelay);

            // Set Random SpawnObject posistion Value
            float ranX = Random.Range(-0.1f, 0.1f);
            float ranZ = Random.Range(-0.1f, 0.1f);

            //Check Distance Between Objs & cam
            bool checkDistanceBool = false;
            
            ranNum = Random.Range(0, nList.Count);

            // spwanPos.Count = SpawnObj Qty.
            if (spawnPos.Count > 0)
            {
                for (int checkDistanceI = 0; checkDistanceI < spawnPos.Count; checkDistanceI++)
                {
                    if (spawnPos[checkDistanceI] != null)
                    {
                        if (Vector3.Distance(new Vector3(pos.position.x + ranX, pos.position.y, pos.position.z + ranZ), spawnPos[checkDistanceI].position) > .15f)
                            checkDistanceBool = true;
                        else
                            checkDistanceBool = false;
                    }
                }
            }


            if (Vector3.Distance(new Vector3(pos.position.x + ranX, pos.position.y, pos.position.z + ranZ), transform.position) > 1.5f && checkDistanceBool)
            {
                spawnPointer = nList[ranNum];
                nList.Remove(spawnPointer);

                //Spawn Obj
                GameObject spawnedfb = Instantiate(numPrefabs[SpawnI], new Vector3(pos.position.x + ranX, pos.position.y, pos.position.z + ranZ), numPrefabs[ranNum].transform.rotation);
                spawnPos.Add(spawnedfb.transform); 
            }
        }

    }

 }
