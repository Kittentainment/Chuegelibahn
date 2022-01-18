namespace TrackPrinting
{
    public struct TrackPiecePrefabs
    {
        public TrackType type { get; }
        public TrackPiece first { get; }
        public TrackPiece last { get; }
        public TrackPiece middle { get; }

        public TrackPiecePrefabs(TrackType type, TrackPiece first, TrackPiece last, TrackPiece middle)
        {
            this.type = type;
            this.first = first;
            this.last = last;
            this.middle = middle;
        }
    }
}