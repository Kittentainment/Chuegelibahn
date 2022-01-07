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

    // Straight Pieces
    [SerializeField] private TrackPiece straightPiece;
    public TrackPiece StraightPiece => straightPiece;
    
    [SerializeField] private TrackPiece straightPieceSnappingFirst;
    public TrackPiece StraightPieceSnappingFirst => straightPieceSnappingFirst;
    
    [SerializeField] private TrackPiece straightPieceSnappingLast;
    public TrackPiece StraightPieceSnappingLast => straightPieceSnappingLast;
    
    // Curve Pieces
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


    public TrackPiece GetPieceOfType(TrackType type)
    {
        return type switch
        {
            TrackType.Straight => StraightPiece,
            TrackType.Left => CurvePiece,
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
        };
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
            TrackType.Left => 0.01f,
            // TrackType.Right => 0.1f,
            // TrackType.Up => 0.1f,
            // TrackType.Down => 0.1f,
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
        };
    }

    public static Vector3 GetVectorFromPivotToCenterBottom(TrackType type, Transform startPos)
    {
        return type switch
        {
            TrackType.Straight => startPos.right * 0.05f,
            TrackType.Left => startPos.right * 0.05f,
            // TrackType.Right => Vector3.left * 0.05f,
            // TrackType.Up => Vector3.left * 0.05f,
            // TrackType.Down => Vector3.left * 0.05f,
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
        };
    }

    public TrackPiecePrefabs GetTrackPrefabsForType(TrackType type)
    {
        return type switch
        {
            TrackType.Straight => new TrackPiecePrefabs(type, StraightPieceSnappingFirst, StraightPieceSnappingLast, StraightPiece, StraightPiece),
            TrackType.Left => new TrackPiecePrefabs(type, StraightPieceSnappingFirst, StraightPieceSnappingLast, CurvePiece, StraightPiece),
            // TrackType.Right => throw new ArgumentOutOfRangeException(nameof(type)),
            // TrackType.Up => throw new ArgumentOutOfRangeException(nameof(type)),
            // TrackType.Down => throw new ArgumentOutOfRangeException(nameof(type)),
            _ => throw new ArgumentOutOfRangeException(nameof(type))
        };
    }

    /// <summary>
    /// The rotation of one piece in degree(?).
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public static float GetRotationOfTrackPiece(TrackType type)
    {
        return type switch
        {
            TrackType.Straight => 0,
            TrackType.Left => 5f,
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
        };   
    }

    public static int GetMinimumNumberOfPieces(TrackType type)
    {
        return type switch
        {
            TrackType.Straight => 3,
            TrackType.Left => 3,
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
        };
    }

    public static int GetMaximumNumberOfPieces(TrackType type)
    {
        return type switch
        {
            TrackType.Straight => 50,
            TrackType.Left => 39,
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
        };
    }
}
