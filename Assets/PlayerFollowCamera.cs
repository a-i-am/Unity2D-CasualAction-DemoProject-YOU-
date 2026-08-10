using UnityEngine;

namespace Script
{
    public class PlayCameraFollow : MonoBehaviour
    {
        [SerializeField] private GameObject player;

        [SerializeField] private float minX = -50f;
        [SerializeField] private float minY = -50f;
        [SerializeField] private float maxX = 50f;
        [SerializeField] private float maxY = 50f;

        private void Start()
        {
            if (player != null)
            {
                transform.position = player.transform.position;
            }
        }

        private void LateUpdate()
        {
            if (player != null)
            {
                Vector3 position = player.transform.position;
                float posX = Mathf.Clamp(position.x, minX, maxX);
                float posY = Mathf.Clamp(position.y, minY, maxY);
                transform.position = new Vector3(posX, posY, -26f);
            }
        }
    }
}