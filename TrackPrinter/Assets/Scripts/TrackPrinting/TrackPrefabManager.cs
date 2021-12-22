using System;
using System.Collections;
using System.Collections.Generic;
using Snapping;
using TrackPrinting;
using UnityEngine;

public class TrackPrefabManager : MonoBehaviour
{
    public static TrackPrefabManager instance { get; private set; }
    
    [SerializeField] private SnappingObjWrapper _baseObjectWrapperPrefab;
    public SnappingObjWrapper baseObjectWrapperPrefab => _baseObjectWrapperPrefab;

    [SerializeField] private TrackPiece straightPiece;
    public TrackPiece StraightPiece => straightPiece;
    
    [SerializeField] private TrackPiece straightPieceSnappingFirst;
    public TrackPiece StraightPieceSnappingFirst => straightPieceSnappingFirst;
    
    [SerializeField] private TrackPiece straightPieceSnappingLast;
    public TrackPiece StraightPieceSnappingLast => straightPieceSnappingLast;

    
    [SerializeField] private TrackPiece curvePiece;
    public TrackPiece CurvePiece => curvePiece;



    private void Awake()
    {
        if (instance != null)
        {
            throw new Exception("TrackPrefabs should only exist once!");
        }
        instance = this;
    }

    /// <summary>
    /// The distance from the previous track piece to the next one if put together.
    /// </summary>
    /// <param name="type">Specify which TrackType you want the length of.</param>
    /// <returns>The distance from the previous track piece to the next one if put together.</returns>
    /// <exception cref="ArgumentOutOfRangeException">When type is out of range of the existing enums.</exception>
    public static float GetLengthOfTrackPiece(TrackType type)
    {
        return type switch
        {
            TrackType.Straight => 0.01f,
            TrackType.Left => 0.1f,
            TrackType.Right => 0.1f,
            TrackType.Up => 0.1f,
            TrackType.Down => 0.1f,
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
        };
    }

    public static Vector3 GetVectorFromPivotToCenterBottom(TrackType type)
    {
        return type switch
        {
            TrackType.Straight => Vector3.right * 0.05f,
            TrackType.Left => Vector3.left * 0.05f,
            TrackType.Right => Vector3.left * 0.05f,
            TrackType.Up => Vector3.left * 0.05f,
            TrackType.Down => Vector3.left * 0.05f,
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
        };
    }

    public TrackPiecePrefabs GetTrackPrefabsForType(TrackType type)
    {
        return type switch
        {
            TrackType.Straight => new TrackPiecePrefabs(type, StraightPieceSnappingFirst, StraightPieceSnappingLast, StraightPiece),
            TrackType.Left => throw new ArgumentOutOfRangeException(nameof(type)),
            TrackType.Right => throw new ArgumentOutOfRangeException(nameof(type)),
            TrackType.Up => throw new ArgumentOutOfRangeException(nameof(type)),
            TrackType.Down => throw new ArgumentOutOfRangeException(nameof(type)),
            _ => throw new ArgumentOutOfRangeException(nameof(type))
        };
    }
    
}
