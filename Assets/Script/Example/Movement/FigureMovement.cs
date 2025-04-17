using ChessSharp;
using System.Collections;
using UnityEngine;
using VoiceChess.BoardCellsParameters;
using VoiceChess.Example.Manager;
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
                // Плавно переміщаємо фігуру, але зберігаємо її поточне значення Y
                selectedFigure.transform.position = new Vector3(
                    Mathf.Lerp(startPosition.x, targetPosition.x, elapsed / duration),
                    startPosition.y, // Завжди залишаємо Y без змін
                    Mathf.Lerp(startPosition.z, targetPosition.z, elapsed / duration)
                );

                elapsed += Time.deltaTime;
                yield return null;
            }

            // Після завершення анімації встановлюємо точну кінцеву позицію
            selectedFigure.transform.position = new Vector3(targetPosition.x, startPosition.y, targetPosition.z);

            onComplete?.Invoke();
        }


        public static void MovingObject(string newPosition, BoardCellsParams targetCell, FigureParams selectedFigure, System.Action onComplete)
        {
            // Зберігаємо поточне значення Y фігури
            float currentY = selectedFigure.transform.position.y;

            // Отримуємо нову позицію клітинки
            Vector3 newPositionInWorld = targetCell.CellPrefab.transform.position;

            // Встановлюємо Y на збережене значення
            newPositionInWorld.y = currentY;

            // Викликаємо метод для плавного переміщення
            selectedFigure.StartCoroutine(MoveObjectSmoothly(selectedFigure, newPositionInWorld, onComplete));
        }



        public static void CaptureFigure(FigureParams capturedFigure, Transform blackCapturedArea, Transform whiteCapturedArea)
        {
            Transform captureArea;
            FigureParams.TypeOfTeam typeOfTeam;
            if (capturedFigure.TeamColor == FigureParams.TypeOfTeam.WhiteTeam)
            {
                captureArea = blackCapturedArea;
                typeOfTeam = FigureParams.TypeOfTeam.BlackTeam;
            }
            else
            {
                captureArea = whiteCapturedArea;
                typeOfTeam = FigureParams.TypeOfTeam.WhiteTeam;
            }
             
            capturedFigure.transform.position = GetNextCapturePosition(captureArea, typeOfTeam);
            capturedFigure.Status = FigureParams.TypeOfStatus.OffGame;
        }



        private static Vector3 GetNextCapturePosition(Transform captureArea, FigureParams.TypeOfTeam typeOfTeam)
        {
            int capturedCount = 0;
            foreach (FigureParams figureParams in GameManager.Figures)
            {
                if (figureParams.Status == FigureParams.TypeOfStatus.OffGame && figureParams.TeamColor != typeOfTeam)
                {
                    capturedCount++;
                }
            }
            float offset = 0.5f; // Відстань між фігурами

            return captureArea.position + new Vector3(capturedCount * offset, 0, 0);
        }
    }
}
