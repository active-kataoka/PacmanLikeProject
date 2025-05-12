using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Point : MonoBehaviour
{
    [SerializeField]
    private LayerMask stageLayer;

    public List<Vector2> Directions;

    // Start is called before the first frame update
    void Start()
    {
        Directions = new List<Vector2>();

        CheckDirection(Vector2.up);
        CheckDirection(Vector2.down);
        CheckDirection(Vector2.left);
        CheckDirection(Vector2.right);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void CheckDirection(Vector2 direction)
    {
        // Ray
        RaycastHit2D hit = Physics2D.BoxCast(this.transform.position, Vector2.one * 0.5f, 0.0f, direction, 1.0f, this.stageLayer);

        if(hit.collider == null)
        {
            Directions.Add(direction);
        }
    }
}
