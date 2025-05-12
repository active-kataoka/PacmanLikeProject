using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField]
    private float speed = 8.0f;
    [SerializeField]
    private LayerMask stageLayer;

    private Rigidbody2D rb;
    private Vector2 _direction;
    private Vector2 _directionReserve;

    private Vector2 virtualPadInput = Vector2.zero;

    private Animator PlayerAnimator;
    private bool isMovementStopped = true;

    // Start is called before the first frame update
    void Start()
    {
        // nullチェック
        if (GameManager.instance == null)
        {
            Debug.LogWarning("GameManager.instance == null");
            return;
        }

        PlayerAnimator = GetComponent<Animator>();

        rb = GetComponent<Rigidbody2D>();
        _direction = Vector2.right;
    }

    // Update is called once per frame
    void Update()
    {
        // 現在のGameMode
        switch (GameManager.instance.CurrentGameMode)
        {
            case GameManager.GameMode.Playing:
                HandlePlayerMovementInput();
                PlayerAnimator.SetBool("GameStart", true);
                PlayerAnimator.SetBool("GameEnd", false);
                isMovementStopped = false;
                break;
            case GameManager.GameMode.GameOver:
            case GameManager.GameMode.Clear:
                if (!isMovementStopped)
                {
                    StopPlayerMovement();
                    isMovementStopped = true;
                }
                break;
            default:
                break;
        }
    }

    private void FixedUpdate()
    {
        // FixedUpdateで物理演算を扱う
        if (GameManager.instance.CurrentGameMode == GameManager.GameMode.Playing)
        {
            MovePlayer();
        }
    }

    // Rayの取得
    private void CheckDirection(Vector2 direction)
    {
        RaycastHit2D hit = Physics2D.BoxCast(transform.position, Vector2.one * 0.5f, 0.0f, direction, 1.0f, stageLayer);

        if (hit.collider == null)
        {
            _direction = direction;
            _directionReserve = Vector2.zero;
        }
    }

    // プレイヤーの入力処理
    private void HandlePlayerMovementInput()
    {
        if (Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0)
        {
            _directionReserve.x = Input.GetAxisRaw("Horizontal");
            _directionReserve.y = Input.GetAxisRaw("Vertical");
        }

        // バーチャルパッドの入力
        if (virtualPadInput != Vector2.zero)
        {
            _directionReserve = virtualPadInput;
        }

        if (_directionReserve != Vector2.zero)
        {
            CheckDirection(_directionReserve);
        }

        float angle = Mathf.Atan2(_direction.y, _direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }

    // プレイヤーの移動時の物理演算
    private void MovePlayer()
    {
        Vector2 dist = _direction * speed * Time.fixedDeltaTime;
        rb.MovePosition(rb.position + dist);
    }

    // 完全にプレイヤーの動きを停止
    private void StopPlayerMovement()
    {
        rb.velocity = Vector2.zero; // 速度を0
        rb.angularVelocity = 0f;    // 回転速度を0
        rb.freezeRotation = true;   // 回転を固定
        rb.constraints = RigidbodyConstraints2D.FreezeAll; // 移動と回転を制限
    }

    // バーチャルパッド上ボタン
    public void SetDirectionUp()
    {
        virtualPadInput = Vector2.up;
    }

    // バーチャルパッド_下ボタン
    public void SetDirectionDown()
    {
        virtualPadInput = Vector2.down;
    }

    // バーチャルパッド_左ボタン
    public void SetDirectionLeft()
    {
        virtualPadInput = Vector2.left;
    }

    // バーチャルパッド_右ボタン
    public void SetDirectionRight()
    {
        virtualPadInput = Vector2.right;
    }
}