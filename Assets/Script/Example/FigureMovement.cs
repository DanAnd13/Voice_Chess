using System.Collections;
using UnityEngine;
using VoiceChess.BoardCellsParameters;
using VoiceChess.FigureParameters;

namespace VoiceChess.Example.FigureMoves
{
    public class FigureMovement : MonoBehaviour
    {
        public static IEnumerator MoveObjectSmoothly(FigureParams selectedFigure, Vector3 targetPosition, System.Action onComplete)
        {
            float duration = 0.5f;
            float elapsed = 0f;
            Vector3 startPosition = selectedFigure.transform.position;

            while (elapsed < duration)
            {
                selectedFigure.transform.position = Vector3.Lerp(startPosition, targetPosition, elapsed / duration);
                elapsed += Time.deltaTime;
                yield return null;
            }

            selectedFigure.transform.position = targetPosition;

            onComplete?.Invoke();
        }

        public static void MovingObject(string newPosition, BoardCellsParams targetCell, FigureParams selectedFigure, System.Action onComplete)
        {
            Vector3 newPositionInWorld = targetCell.CellObject.transform.position;
            newPositionInWorld.y = selectedFigure.transform.position.y;
            selectedFigure.StartCoroutine(MoveObjectSmoothly(selectedFigure, newPositionInWorld, onComplete));
        }


        public static void CaptureFigure(FigureParams capturedFigure, Transform blackCapturedArea, Transform whiteCapturedArea)
        {
            Transform captureArea = (capturedFigure.TeamColor == FigureParams.TypeOfTeam.WhiteTeam) ?
                blackCapturedArea : whiteCapturedArea;

            capturedFigure.transform.SetParent(captureArea);
            capturedFigure.transform.position = GetNextCapturePosition(captureArea);

            //capturedFigure.PreviousPosition = capturedFigure.CurrentPosition;
            capturedFigure.Status = FigureParams.TypeOfStatus.OffGame;
        }



        private static Vector3 GetNextCapturePosition(Transform captureArea)
        {
            int capturedCount = captureArea.childCount;
            float offset = 0.5f; // Відстань між фігурами

            return captureArea.position + new Vector3(capturedCount * offset, 0, 0);
        }
    }
}
