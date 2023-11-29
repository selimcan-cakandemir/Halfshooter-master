using System;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerMovement : MonoBehaviour 
{
    public Vector3 move;
    Vector3 velocity;
    bool isGrounded;

    public Terrain defaultTerrain; //make this dynamic so you don't have to assign in if you move the character
    Collider defaultTerrainCollider;
    TerrainDetector terrainDetector;

    public float speed = 12f;
    public float gravity = -9.81f;
    public float jumpHeight = 3f;
    public float groundDistance = 0.4f;
    public float footStepTimer = 1f;
    public LayerMask groundMask;
    public Transform groundCheck;
    public CharacterController controller;

    [SerializeField] AudioSource footStepAudioSource = default;
    [SerializeField] AudioClip[] dirtClips = default;
    [SerializeField] AudioClip[] sandClips = default;
    [SerializeField] AudioClip[] stoneClips = default;
    [SerializeField] AudioClip[] woodClips = default;
    [SerializeField] AudioClip[] metalClips = default;
    [SerializeField] AudioClip[] jumpSounds = default;

    void Awake()
    {
        terrainDetector = new TerrainDetector();
        defaultTerrainCollider = defaultTerrain.GetComponent<Collider>();
        terrainDetector.ReceiveActiveTerrain(defaultTerrain);
    }

    void Update()
    {
        isGrounded = CheckIfGrounded();

        ResetGravity();
        GetMoveInput();
        HandleFootSteps();
        GetJumpInput();
        ApplyGravity();
        SendActiveTerrainData();
    }

    void HandleFootSteps()
    {
        if(!isGrounded) return;
        if (move == Vector3.zero) return;
        
        footStepTimer -= Time.deltaTime;

        if (footStepTimer <= 0)
        {
            if (Physics.Raycast(groundCheck.transform.position, Vector3.down, out RaycastHit hit, 3))
            {               
                switch (hit.collider.tag)
                {
                    case "Footsteps/Terrain/Sand":
                        footStepAudioSource.PlayOneShot(GetRandomSandClip());
                        break;
                    case "Footsteps/Terrrain/Dirt":
                        footStepAudioSource.PlayOneShot(GetRandomDirtClip());
                        break;
                    case "Footsteps/Stone":
                        footStepAudioSource.PlayOneShot(stoneClips[UnityEngine.Random.Range(0, stoneClips.Length)]);
                        break;
                    default:
                        break;
                }
            }
            footStepTimer = 0.5f;
        }   
    }

    AudioClip GetRandomJumpClip()
    {
        return jumpSounds[UnityEngine.Random.Range(0, jumpSounds.Length)];
    }

    AudioClip GetRandomDirtClip()
    {
        int terrainTextureIndex = terrainDetector.GetActiveTerrainTextureIdx(transform.position);

        switch (terrainTextureIndex)
        {
            case 0:
                return dirtClips[UnityEngine.Random.Range(0, dirtClips.Length)];
            case 1:
                return stoneClips[UnityEngine.Random.Range(0, stoneClips.Length)];
            case 2:
            default:
                return dirtClips[UnityEngine.Random.Range(0, dirtClips.Length)];
        }
    }

    AudioClip GetRandomSandClip()
    {
        int terrainTextureIndex = terrainDetector.GetActiveTerrainTextureIdx(transform.position);

        if (terrainTextureIndex != 4) { return sandClips[UnityEngine.Random.Range(0, sandClips.Length)]; } else { return stoneClips[UnityEngine.Random.Range(0, stoneClips.Length)]; }
    }

    void ApplyGravity()
    {
        velocity.y += gravity * Time.deltaTime;

        controller.Move(velocity * Time.deltaTime);
    }   

    void GetJumpInput() 
    {
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2 * gravity);

            footStepAudioSource.PlayOneShot(GetRandomJumpClip());
        }
    }

    public void GetMoveInput()
    {
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        move = transform.right * x + transform.forward * z;

        controller.Move(move * speed * Time.deltaTime);
    }

    public bool CheckIfGrounded()
    {
        return Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
    }

    void ResetGravity()
    {
        if (isGrounded & velocity.y < 0)
        {
            velocity.y = -2f;
        }
    }

    //This need to be changed into dynamic, default terrain is assigned from the editor and needs to be changed to be dynamic (if you move character to another terrain at the start, it will break

    void SendActiveTerrainData()
    {
        if (Physics.Raycast(groundCheck.transform.position, Vector3.down, out RaycastHit hit, 3))
        {
            if (hit.collider != defaultTerrainCollider && hit.transform.TryGetComponent<Terrain>(out var terrain))
            {
                defaultTerrainCollider = hit.collider;
                defaultTerrain = terrain;
                terrainDetector.ReceiveActiveTerrain(defaultTerrain);
            }
        }
    }
}