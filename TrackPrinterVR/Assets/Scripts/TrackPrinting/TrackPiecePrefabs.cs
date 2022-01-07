namespace TrackPrinting
{
    public struct TrackPiecePrefabs
    {
        public TrackType type { get; }
        public TrackPiece first { get; }
        public TrackPiece last { get; }
        
        public TrackPiece ends { get; }
        
        public TrackPiece middle { get; }

        public TrackPiecePrefabs(TrackType type, TrackPiece first, TrackPiece last, TrackPiece middle, TrackPiece ends)
        {
            this.type = type;
            this.first = first;
            this.last = last;
            this.middle = middle;
            this.ends = ends;
        }
    }
}