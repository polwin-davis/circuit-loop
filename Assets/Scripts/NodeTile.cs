using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;

[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(SpriteRenderer))]
public class NodeTile : MonoBehaviour, IPointerClickHandler
{
    public TileType tileType;
    public List<Direction> connections;

    private SpriteRenderer sr;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        SetInvalidColor();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (LevelManager.Instance != null &&
            LevelManager.Instance.IsLevelCompleted)
            return;

        RotateTile();
    }

    void RotateTile()
    {
        transform.Rotate(0, 0, -90f);

        for (int i = 0; i < connections.Count; i++)
            connections[i] = (Direction)(((int)connections[i] + 1) % 4);

        LevelManager.Instance?.tileRotateAudio?.Play();
        LevelManager.Instance?.CheckLevel();
    }

    public NodeTile GetNeighbor(Direction dir)
    {
        Vector2 dirVec = dir switch
        {
            Direction.Up => Vector2.up,
            Direction.Down => Vector2.down,
            Direction.Left => Vector2.left,
            Direction.Right => Vector2.right,
            _ => Vector2.zero
        };

        RaycastHit2D hit = Physics2D.Raycast(
            transform.position + (Vector3)dirVec * 0.6f,
            dirVec,
            0.6f
        );

        return hit.collider ? hit.collider.GetComponent<NodeTile>() : null;
    }

    public void SetValidColor() => sr.color = Color.green;
    public void SetInvalidColor() => sr.color = Color.white;
}
