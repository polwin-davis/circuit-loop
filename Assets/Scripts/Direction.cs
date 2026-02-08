public enum Direction
{
    Up,
    Right,
    Down,
    Left
}

public enum TileType
{
    End,        // 1 connection
    Straight,   // 2 opposite connections
    Corner,     // 2 adjacent connections
    Cross       // 4 connections
}
