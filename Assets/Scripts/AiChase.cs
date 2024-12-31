using UnityEngine;

public class AiChase : MonoBehaviour
{
    public GameObject player;
    public float speed;
    private float distance;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        distance = Vector2.Distance(transform.position, player.transform.position);/*avoir la distance entre le joueur et l'ennemi*/
        Vector2 direction = player.transform.position - transform.position;
        direction.Normalize();//ça iniatilse sa longueur à  1 //
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;//ça touve l angle entre les deux //
        if (distance < 4)
        {
            transform.position = Vector2.MoveTowards(this.transform.position, player.transform.position, speed * Time.deltaTime);
            transform.rotation = Quaternion.Euler(Vector3.forward*angle);
        }
        
        
    }
}
