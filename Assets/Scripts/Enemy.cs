using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField]
    private LayerMask stageLayer;
    [SerializeField]
    private Eye eye;
    [SerializeField]
    private Transform bodyTransform;
    [SerializeField]
    private Transform playerTransform;
    [SerializeField]
    private float speed = 7.5f;

    private Animator bodyAnimator; // 子オブジェクトのAnimatorを保持する変数
    private Rigidbody2D rb;
    private Vector2 _direction;
    private Vector2 _directionReserve;
    private bool isMovementStopped = true;

    // Start is called before the first frame update
    void Start()
    {
        if (GameManager.instance == null)
        {
            Debug.LogWarning("GameManager.instance == null");
            return;
        }

        if (bodyTransform == null)
        {
            Debug.LogWarning("bodyTransform == null");
            return;
        }
        else
        {
            bodyAnimator = bodyTransform.GetComponent<Animator>();
        }

        if (bodyAnimator == null)
        {
            Debug.LogWarning("bodyAnimator == null");
            return;
        }

        if (playerTransform == null) return;

        rb = GetComponent<Rigidbody2D>();
        _direction = Vector2.left;
    }

    // Update is called once per frame
    void Update()
    {
        // 現在のGameMode
        switch (GameManager.instance.CurrentGameMode)
        {
            case GameManager.GameMode.Playing:
                if (_directionReserve != Vector2.zero)
                {
                    CheckDirection(_directionReserve);
                }
                bodyAnimator.SetBool("GameStart", true);
                bodyAnimator.SetBool("GameEnd", false);
                isMovementStopped = false;
                break;
            case GameManager.GameMode.GameOver:
            case GameManager.GameMode.Clear:
                if (!isMovementStopped)
                {
                    StopEnemyMovement();
                    isMovementStopped = true;
                }
                break;
            default:
                break;
        }
    }

    private void FixedUpdate()
    {
        if (GameManager.instance.CurrentGameMode == GameManager.GameMode.Playing)
        {
            MoveEnemy();
        }
    }

    private void CheckDirection(Vector2 direction)
    {
        // レイを飛ばす
        RaycastHit2D hit = Physics2D.BoxCast(transform.position, Vector2.one * 0.5f, 0.0f, direction, 1.0f, stageLayer);

        if (hit.collider == null)
        {
            _direction = direction;
            eye.ChangeEye(_direction);
            _directionReserve = Vector2.zero;
        }
    }

    //private void OnTriggerEnter2D(Collider2D other)
    //{
    //    Point point = other.GetComponent<Point>();

    //    if (point != null)
    //    {
    //        // 進行方向をランダムに決める
    //        int index = Random.Range(0, point.Directions.Count);
    //        _directionReserve = point.Directions[index]; // 次の進行方向を設定
    //    }
    //}

    private void OnTriggerEnter2D(Collider2D other)
    {
        Point point = other.GetComponent<Point>();

        if (point != null)
        {
            Vector2 bestDirection = Vector2.zero;
            float bestDist = float.MaxValue;

            // 進める候補方向(上・下・左・右など)をすべて調べる
            foreach (var dir in point.Directions)
            {
                // dir 方向に少し進んだ位置を仮定
                Vector2 nextPos = (Vector2)transform.position + dir;

                // その位置とプレイヤーの距離を計算
                float distToPlayer = Vector2.Distance(nextPos, playerTransform.position);

                // 最小距離を更新する方向を探す
                if (distToPlayer < bestDist)
                {
                    bestDist = distToPlayer;
                    bestDirection = dir;
                }
            }

            // ベストな方向を予約
            _directionReserve = bestDirection;
        }
    }


    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            // 衝突時に「回転速度リセット・回転凍結」
            rb.angularVelocity = 0f;
            rb.freezeRotation = false;

            other.gameObject.SetActive(false);
            GameManager.instance.GameOver();
        }
    }

    // 敵の自動移動
    private void MoveEnemy()
    {
        Vector2 dist = _direction * speed * Time.fixedDeltaTime;
        rb.MovePosition(rb.position + dist);
    }

    // 完全に敵の動きを停止
    private void StopEnemyMovement()
    {
        rb.linearVelocity = Vector2.zero; // 速度を0
        rb.angularVelocity = 0f;    // 回転速度を0
        rb.freezeRotation = true;   // 回転を固定
        rb.constraints = RigidbodyConstraints2D.FreezeAll; // 移動と回転を制限
    }
}