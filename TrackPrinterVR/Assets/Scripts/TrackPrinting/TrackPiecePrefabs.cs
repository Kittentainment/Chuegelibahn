
namespace TrackPrinting
{
    public struct TrackPiecePrefabs
    {
        public TrackType Type { get; }
        public TrackPiece First { get; }
        public TrackPiece Last { get; }
        
        public TrackPiece EndPieces { get; }
        
        public TrackPiece Middle { get; }

        public TrackPiecePrefabs(TrackType type, TrackPiece first, TrackPiece last, TrackPiece middle, TrackPiece endPieces)
        {
            this.Type = type;
            this.First = first;
            this.Last = last;
            this.Middle = middle;
            this.EndPieces = endPieces;
        }
    }
}