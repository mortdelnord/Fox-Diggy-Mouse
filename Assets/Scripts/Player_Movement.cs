using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

public class Player_Movement : MonoBehaviour
{
    private NavMeshAgent playerAgent;

    [Header ("Cameras")]
    private Camera mainCam;
    public CinemachineVirtualCamera normalCam;
    public CinemachineVirtualCamera zoomInCam;

    [Header ("Bools")]
    private bool foundPrey = false;
    private bool lookingAtPrey = false;
    private bool canMove = true;
    private GameObject currentPrey; // the current prey object
    public Transform orientationLook; // orientation object for rotation check
    public GameManager gameManager; // the game manager script

    public Animator playerAnimator; // the players animator

    public GameObject mousePrefab;
    public Transform bitePoint;
    public AudioSource footstepSound;
    public GameObject snowburst;



    private float backupTimer = 0f; // timer for if rotation check doesn't work
    public float backupTimerMax = 1f; // the max for the timer

    public float jumpAnimTime = 1f;
    public float burstTime = 1.04f;

    private void Start()
    {
        // gets private references that wont need to change
        playerAgent = gameObject.GetComponent<NavMeshAgent>();
        mainCam = Camera.main;

    }



    private void Update()
    {
        if (playerAgent.velocity.magnitude == 0f) //if navmesh agent is not moving play idle animations
        {
            footstepSound.Play();
            playerAnimator.SetBool("IsIdle", true);
            playerAnimator.SetBool("IsJumping", false);
            playerAnimator.SetBool("IsWalking", false);
        }else                                       // else must be walking animation
        {
            footstepSound.Stop();
            playerAnimator.SetBool("IsIdle", false);
            playerAnimator.SetBool("IsJumping", false);
            playerAnimator.SetBool("IsWalking", true);
        }
        if (Input.GetMouseButton(0)) // if left clicking
        {
            Ray clickRay = mainCam.ScreenPointToRay(Input.mousePosition); // a raycast from the screen out into the world from mouse position

            if (Physics.Raycast(clickRay, out RaycastHit hit, 1000, playerAgent.areaMask) && hit.transform != null && canMove) // if player can move, te raycast hit exists and hits an object
            {
                playerAgent.SetDestination(hit.point); //set the players destination to where the raycast hit


                if (hit.transform.gameObject.CompareTag("Prey")) // if the hit has the tag of Prey, set found prey true, make that object the current prey and stop the prey's movement
                {
                    
                    foundPrey = true;
                    currentPrey = hit.transform.gameObject;
                    NavMeshAgent preyAgent = currentPrey.GetComponent<NavMeshAgent>();
                    preyAgent.enabled = false;
                }
                
            }
        }
        if (playerAgent.enabled == true && canMove) // if you have a navagent and can move
        {

            if (foundPrey && !playerAgent.pathPending) // if you have found prey and navagent path is not pending
            {
                if (playerAgent.remainingDistance <= playerAgent.stoppingDistance) // if the remaining distance on navagent is at stopping distance
                {
                    if (playerAgent.hasPath || playerAgent.velocity.sqrMagnitude == 0f) // if the player has a path or velocity is 0 or close to it
                    {
                        foundPrey = false; // no longer found prey
                        lookingAtPrey = true; // now lookig at prey
                        //Attack(currentPrey);
                    }

                }
            }
        }
        if (lookingAtPrey) // if player  is looking at prey
        {
            canMove = false; //you cannot move during this
            backupTimer += Time.deltaTime; // start the backup timer
            
            Vector3 targetPos = new Vector3(currentPrey.transform.position.x, transform.position.y, currentPrey.transform.position.z); // get the target position by making a new vector with the new x and z positions with the players y
            orientationLook.LookAt(targetPos); // make orientation look at that instantly 
            transform.rotation = Quaternion.Slerp(transform.rotation, orientationLook.rotation, Time.deltaTime * 20f); // slerp between players roation and oreintations rotation

            



            if (Mathf.Approximately(transform.rotation.y, orientationLook.rotation.y) || backupTimer >= backupTimerMax) // if the rotations mostly match or the backup timer finsihes
            {
                gameManager.stopTimer = true;
                backupTimer = 0f; // reset timer
                lookingAtPrey = false; // no longer looking at prey
                playerAnimator.SetBool("IsIdle", false); // play jumping animation
                playerAnimator.SetBool("IsWalking", false);
                playerAnimator.SetBool("IsJumping", true);
                Invoke(nameof(AddMouse), jumpAnimTime/2f);
                Invoke(nameof(SnowBurst), burstTime);
                Invoke(nameof(Attack), jumpAnimTime); // use method of attack after animaation finsihes// NOTE THAT THE FLOAT SHOULD MATCH HOW LONG THE ANIMATION IS
                
            }
        }
    }





    private void Attack()
    {
        //int scoreToSend = currentPrey.GetComponent<PreyScore>().Score; // get the score from the prey
        RemoveMouse();
        
        //gameManager.UpdateScore(scoreToSend); // send score to Update scrore function in game manager
        //Destroy(currentPrey); // destroy the prey
        
        gameManager.SpawnPrey(); // spawn new prey
        ResetMove(); // reset being able to move 

        // THESE ARE FOR IF WE ADD THE CAMERA ZOOM IN
        // zoomInCam.Priority = 10;
        // normalCam.Priority = 9;
        // Invoke(nameof(ResetMove), 5f);
    }

    private void SnowBurst()
    {
        Instantiate(snowburst, transform.position, snowburst.transform.rotation);

    }
    private void AddMouse()
    {
        int scoreToSend = currentPrey.GetComponent<PreyScore>().Score; // get the score from the prey
        gameManager.UpdateScore(scoreToSend); // send score to Update scrore function in game manager

        Destroy(currentPrey); // destroy the prey
        Instantiate(mousePrefab, bitePoint);

    }
    private void RemoveMouse()
    {
        foreach (Transform mouse in bitePoint)
        {
            Destroy(mouse.gameObject);
        }
    }
    private void ResetMove() // simply resets the can move bool
    {
        zoomInCam.Priority = 9;
        normalCam.Priority = 10;
        canMove = true;
        gameManager.stopTimer = false;
    }
}
