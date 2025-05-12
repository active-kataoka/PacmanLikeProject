using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Eye : MonoBehaviour
{
    [SerializeField]
    private Sprite eye_up;
    [SerializeField]
    private Sprite eye_down;
    [SerializeField]
    private Sprite eye_left;
    [SerializeField]
    private Sprite eye_right;

    private SpriteRenderer _sr;

    // Start is called before the first frame update
    void Start()
    {
        _sr = GetComponent<SpriteRenderer>();   
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ChangeEye(Vector2 direction)
    {
        if(direction == Vector2.up)
        {
            _sr.sprite = eye_up;
        }
        else if(direction == Vector2.down)
        {
            _sr.sprite = eye_down;
        }
        else if (direction == Vector2.left)
        {
            _sr.sprite = eye_left;
        }
        else if (direction == Vector2.right)
        {
            _sr.sprite = eye_right;
        }
    }
}
