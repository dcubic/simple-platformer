using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

public class PlayerController : MonoBehaviour {

    private float horizontalInput;
    private float verticalInput;
    private readonly float movementSpeed = 5f;
    private float jumpingPower = 14f;
    private float gravitationalAcceleration = 9.8f;

    private float flipCooldown = 0.1f;
    private float lastFlipFrame = -1f;

    private float ladderGrabCooldown = 0.3f;
    private float lastLadderJumpFrame = -1f;

    private string IS_WALKING_PARAMETER = "isWalking";
    private string IS_ON_LADDER_PARAMETER = "isOnLadder";
    private bool isOnLadder = false;

    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private Rigidbody2D rigidBody;

    [SerializeField] private TileBase openDoorTopTile;
    [SerializeField] private TileBase openDoorBottomTile;
    [SerializeField] private TextMeshProUGUI promptText;
    [SerializeField] public Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;
    private Tilemap interactableTileMap;

    private void Start() {
        spriteRenderer = GetComponent<SpriteRenderer>();
        rigidBody = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        interactableTileMap = GameObject.Find("Interactables_TileMap").GetComponent<Tilemap>();
    }

    void Update() {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");
        Flip();

        Vector3Int cellPosition = interactableTileMap.WorldToCell(transform.position);
        TileBase tile = interactableTileMap.GetTile(cellPosition);
        if (tile != null) {
            switch(tile.name) {
                case "Ladder" or "LadderTop":
                    if (Time.time - lastLadderJumpFrame > ladderGrabCooldown &&
                        !isOnLadder &&
                        Input.GetAxisRaw("Vertical") != 0
                    ) {
                        rigidBody.gravityScale = 0f;
                        rigidBody.velocity = Vector2.zero;
                        isOnLadder = true;
                        animator.SetBool(IS_ON_LADDER_PARAMETER, true);
                    }
                    break;
                case "RedGem" or "GreenGem" or "BlueGem" or "YellowGem":
                    Inventory.Instance.AddGem();
                    interactableTileMap.SetTile(cellPosition, null);
                    break;
                case "RedKey":
                    Inventory.Instance.AddKey();
                    interactableTileMap.SetTile(cellPosition, null);
                    break;
                case "RedDoorTop" or "RedDoorBottom":
                    if (Inventory.Instance.hasKey) {
                        ShowDoorPrompt();
                    }
                    if (Inventory.Instance.hasKey && Input.GetKeyDown(KeyCode.E)) {
                        for (int x = cellPosition.x - 1; x <= cellPosition.x + 1; x++) {
                            for (int y = cellPosition.y - 1; y <= cellPosition.y + 1; y++) {
                                Vector3Int positionCurrent = new Vector3Int(x, y, 0);
                                TileBase adjacentTile = interactableTileMap.GetTile(positionCurrent);
                                if (adjacentTile != null) {
                                    if (adjacentTile.name == "RedDoorTop") {
                                        interactableTileMap.SetTile(positionCurrent, null);
                                        interactableTileMap.SetTile(positionCurrent, openDoorTopTile);
                                    } else if (adjacentTile.name == "RedDoorBottom") {
                                        interactableTileMap.SetTile(positionCurrent, null);
                                        interactableTileMap.SetTile(positionCurrent, openDoorBottomTile);
                                    }
                                }
                            }
                        }
                    }
                    break;
                case "OpenDoorTop" or "OpenDoorBottom":
                    ShowDoorPrompt();
                    if (Input.GetKeyDown(KeyCode.E)) {
                        SceneManager.LoadScene("VictoryScene");
                    }
                    break;
                default: break;
            }
        } else {
            exitLadder();
            HideDoorPrompt();
        }

        if (canJump() && Input.GetButtonDown("Jump")) {
            rigidBody.velocity = new Vector2(rigidBody.velocity.x, jumpingPower);
            if (isOnLadder) {
                setLadderGrabCooldown();
            }
            
            exitLadder();
        }
    }

    private void ShowDoorPrompt() {
        promptText.text = "E";
        promptText.gameObject.SetActive(true);
        UpdatePromptPosition();
    }

    private void UpdatePromptPosition() {
        promptText.transform.position = Camera.main.WorldToScreenPoint(transform.position + Vector3.up / 2 + Vector3.right / 2);
    }
    private void HideDoorPrompt() {
        promptText.gameObject.SetActive(false);
    }

    void OnTriggerEnter2D(Collider2D collision) {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Obstacles")) {
            GameManager.Instance().ResetGame();
        }
    }

    private bool canJump() {
        return IsGrounded() || isOnLadder;
    }

    private bool IsGrounded() {
        return Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);
    }

    private void exitLadder() {
        rigidBody.gravityScale = gravitationalAcceleration;
        isOnLadder = false;
        animator.SetBool(IS_ON_LADDER_PARAMETER, false);
    }

    private void setLadderGrabCooldown() {
        lastLadderJumpFrame = Time.time;
    }

    private void FixedUpdate() {
        if (horizontalInput == 0) {
            animator.SetBool(IS_WALKING_PARAMETER, false);
        } else {
            animator.SetBool(IS_WALKING_PARAMETER, true);
        }
        float updatedVerticalVelocity = isOnLadder ? verticalInput * movementSpeed : rigidBody.velocity.y;
        rigidBody.velocity = new Vector2(horizontalInput * movementSpeed, updatedVerticalVelocity);

    }
    private void Flip() {
        float frameCurrent = Time.time;

        if (frameCurrent - lastFlipFrame > flipCooldown && 
            ((horizontalInput < 0f && !spriteRenderer.flipX) || 
            (horizontalInput > 0f && spriteRenderer.flipX))
        ) {
            spriteRenderer.flipX = !spriteRenderer.flipX;
            lastFlipFrame = frameCurrent;
        }
    }
}
