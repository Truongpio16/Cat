using UnityEngine;

// Gan vao 1 layer nen (may/cay...). Cac tile duoc xep lien tiep theo truc X
// va tu dong "nhay" ve cuoi hang khi troi het man hinh ben trai, tao cam
// giac cuon vo tan ma khong can texture seamless that.
public class CatParallax : MonoBehaviour
{
    [SerializeField] private RectTransform[] tiles;
    [SerializeField] private float scrollSpeed = 6f;
    [SerializeField] private float tileWidth = 600f;

    private void Update()
    {
        if (tiles == null || tiles.Length == 0) return;

        float dx = scrollSpeed * Time.deltaTime;
        float totalWidth = tileWidth * tiles.Length;

        foreach (RectTransform tile in tiles)
        {
            if (tile == null) continue;

            Vector2 pos = tile.anchoredPosition;
            pos.x -= dx;
            if (pos.x <= -tileWidth)
            {
                pos.x += totalWidth;
            }
            tile.anchoredPosition = pos;
        }
    }
}
