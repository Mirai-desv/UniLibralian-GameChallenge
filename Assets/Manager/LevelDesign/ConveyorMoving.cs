using UnityEngine;

public class ConveyorMoving : MonoBehaviour
{
    private float speed = 2f;
    public Transform OriginPos;
    public Transform EndPos;
    void Update()
    {
        if(transform.position != EndPos.position)
        {
            Vector2 newPos = Vector2.MoveTowards(transform.position, EndPos.position, speed * Time.deltaTime);
            transform.position = newPos;
        }
        else
        {
            Vector2 newPos = OriginPos.position;
            transform.position = newPos;
        }
    }
}
