using System;
using Snapping;
using TrackPrinting.TrackBuilderComponents;
using UnityEngine;

namespace TrackPrinting
{
    public class TrackBuilder
    {
    
        private readonly NumberOfElementsCalcComponent _numOfElemCalcComponent;
        private readonly TrackSegmentCreatorComponent _trackSegmentCreatorComponent;

        public TrackType type { get; }
        private Vector3 lastTrackPrinterPos { get; set; }
        private Vector3 lastDraggablePos { get; set; }

        private int _lastNumberOfElements = 0;

        private TrackSegment _trackBuilderSegment;
        private static TrackPrefabManager prefabManager => TrackPrefabManager.instance;

        private TrackBuilder(TrackType type, Vector3 trackPrinterPos, Vector3 draggablePos,
            NumberOfElementsCalcComponent numOfElemCalcComponent,
            TrackSegmentCreatorComponent trackSegmentCreatorComponent)
        {
            _numOfElemCalcComponent = numOfElemCalcComponent;
            _trackSegmentCreatorComponent = trackSegmentCreatorComponent;

            this.type = type;
            UpdateLastPositions(trackPrinterPos, draggablePos);
            _trackBuilderSegment = new GameObject().AddComponent<TrackSegment>();
            _trackBuilderSegment.transform.position = trackPrinterPos;
        }

        /// <summary>
        /// Factory Method to get the correct <code>TrackBuilder</code>.
        /// </summary>
        /// <param name="type">The type of pieces we want to print with the created TrackBuilder.</param>
        /// <param name="trackPrinterPos"></param>
        /// <param name="draggablePos"></param>
        /// <returns>A TrackBuilder which prints track pieces for the given TrackType</returns>
        public static TrackBuilder CreateTrackBuilderForType(TrackType type, Vector3 trackPrinterPos, Vector3 draggablePos)
        {
            return type switch {
                TrackType.Straight => new TrackBuilder(type, trackPrinterPos, draggablePos, 
                    new LengthBasedNoECalcComponent(),
                    new StraightTrackSegmentCreatorComponent()),
            
                TrackType.Left => new TrackBuilder(type, trackPrinterPos, draggablePos,
                    new AngleBasedNoECalcComponent(),
                    new CurvedTrackSegmentCreatorComponent()),
            
                _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
            };
        }

    
    
        private void UpdateLastPositions(Vector3 trackPrinterPos, Vector3 draggablePos)
        {
            lastTrackPrinterPos = trackPrinterPos;
            lastDraggablePos = draggablePos;
        }
    

        /// <summary>
        /// This method should be called, whenever there is a chance that the preview should be updated.
        /// This will most likely be, because the <code>Draggable</code> or the <code>TrackPrinter</code> is moved.
        /// </summary>
        /// <param name="trackPrinter"></param>
        /// <param name="draggable"></param>
        public void CheckForPreviewUpdate(Transform trackPrinter, Transform draggable)
        {
            var trackPrinterPos = trackPrinter.position;
            var draggablePos = draggable.position;
            var drawLine = draggablePos - trackPrinterPos;
            var outputDirection = trackPrinter.forward;
            var upwardsDirection = trackPrinter.up;
            var distance = Vector3.Dot(drawLine, outputDirection);
            var horizontalAngle = Vector3.SignedAngle(outputDirection, draggable.forward, upwardsDirection);
            var numberOfNeededElements = _numOfElemCalcComponent.CalculateNumberOfNeededElements(type, distance, horizontalAngle);
            if (numberOfNeededElements <= 0)
            {
                DeleteTrackPreview();
                return;
            }

            if (numberOfNeededElements != _lastNumberOfElements) // TODO We Can't really check for that, as we also need to account for rotation changes of the Track Printer, and in VR this probably happens constantly. But it's helpful for Debug,
            { 
                Debug.Log("numberOfNeededElements = " + numberOfNeededElements);
                UpdateTrackPreview(numberOfNeededElements, outputDirection, upwardsDirection, trackPrinter);
            }
            else
            {
                UpdateTrackPreview(numberOfNeededElements, outputDirection, upwardsDirection, trackPrinter);
                // TODO: replace this with logic which moves the existing preview to the new position of the track printer, (and only in case it changed).
            }

            _lastNumberOfElements = numberOfNeededElements;
            UpdateLastPositions(trackPrinterPos, draggablePos);

            // if (numberOfNeededElements > 30 && _draggable.isGrabbed)
            // {
            //     // _draggable.LetGo(); // DEBUG only for testing
            //     // _draggable.trackPrinter!.PrintCurrentTrack(); // DEBUG only for testing
            // }
        }

    



        /// <summary>
        /// Update the pieces which show where the piece to create will be created.
        /// They will also be used to create the actual pieces.
        /// </summary>
        /// <param name="numberOfElements">How many Pieces should be created.</param>
        /// <param name="outputDirection">The direction the printer is facing, where at least the first piece should also face to.</param>
        /// <param name="upwardsDirection">The upward direction of the printer.</param>
        /// <param name="printerTransform">The position of the printer, where we start printing the objects from.</param>
        private void UpdateTrackPreview(int numberOfElements, Vector3 outputDirection, Vector3 upwardsDirection, Transform printerTransform)
        {
            var startPos = printerTransform.position + TrackPrefabManager.GetLengthOfTrackPiece(TrackType.Straight) * outputDirection;
            var trackPrinterRotation = Quaternion.LookRotation(outputDirection, upwardsDirection);// Quaternion.FromToRotation(Vector3.forward, outputDirection); // The rotation of the TrackPrinter in WorldCoordinates.
            DeleteTrackPreview();
            _trackBuilderSegment = _trackSegmentCreatorComponent.CreateSegment(type, numberOfElements, outputDirection, upwardsDirection, printerTransform.right, startPos, trackPrinterRotation);
        
        }
    

        public SnappingObjWrapper PrintCurrentTrack()
        {
        
            if (_lastNumberOfElements < TrackPrefabManager.GetMinimumNumberOfPieces(type))
            {
                return null;
            }
            var currentTrack = _trackBuilderSegment;
            _trackBuilderSegment = null;
            return TrackFinisher.FinishTrackPiece(currentTrack);
        }
    


        private void DeleteTrackPreview()
        {
            if (_trackBuilderSegment == null) return;
            _trackBuilderSegment.trackPieces.Clear();
            GameObject.Destroy(_trackBuilderSegment.gameObject);
        }

        public void DestroyYourself()
        {
            DeleteTrackPreview();
        }
    }
}