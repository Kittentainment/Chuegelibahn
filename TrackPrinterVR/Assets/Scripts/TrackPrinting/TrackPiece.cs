using UnityEngine;

namespace TrackPrinting;

public class TrackPiece : MonoBehaviour
{
    [SerializeField] private TrackType _type;
    public TrackType type => _type;
}