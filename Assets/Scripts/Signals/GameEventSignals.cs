using UnityEngine;

public static class GameEventSignals
{
    public static event MapResizeHandler OnMapResize;
    public delegate void MapResizeHandler(int _rowCount, int _colCount);
    public static void DoMapResize(int _rowCount, int _colCount) { if (OnMapResize != null) OnMapResize(_rowCount, _colCount); }

    public static event MapRescaleHandler OnMapRescale;
    public delegate void MapRescaleHandler(float _blockSeparation);
    public static void DoMapRescale(float _blockSeparation) { if (OnMapRescale != null) OnMapRescale(_blockSeparation); }
}
