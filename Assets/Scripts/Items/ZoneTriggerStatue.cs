using Unity.VisualScripting;
using UnityEngine;

public class ZoneTriggerStatue : MonoBehaviour
{
    public StatueZones statue;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            statue.isPlayerInZone = true; // Игрок входит в зону
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            statue.isPlayerInZone = false; // Игрок выходит из зоны
        }
    }
}