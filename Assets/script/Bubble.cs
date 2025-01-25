using UnityEngine;

public class Bubble : MonoBehaviour
{
    public Transform player; // The player to follow
    public Vector3 teleportTo = new Vector3(0, 0, 0);
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        player.position = teleportTo;
    }
}
