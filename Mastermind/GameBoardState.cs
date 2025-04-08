using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mastermind
{
    class GameBoardState
    {
        public int[] rowFields;
        public int[] secretFields;
        public int actualField;
        public int actualRow;
        private List<int> matchedPositions;
        private List<int> matchedSecretpositions;
        public bool IsRightCombination;
        public const int rowsCount = 12;
        public const int fieldsCount = 5;
        public GameBoardState()
        {
            rowFields = new int[fieldsCount];
            secretFields = new int[fieldsCount];
            actualField = 0;
            actualRow = 0;
            matchedPositions = new List<int>();
            matchedSecretpositions = new List<int>();
            IsRightCombination = false;
        }
        public void FillSecretFields(int colorsCount)
        {
            Random random = new Random();
            for (int i = 0; i < fieldsCount; i++)
            {
                secretFields[i] = random.Next(1, colorsCount);
            }
        }
        public bool AddColorAndMoveToNext(int color)
        {
            rowFields[actualField] = color;
            while (rowFields[actualField] > 0)
            {
                actualField++;
                if (actualField == fieldsCount)
                {
                    return false;
                }
            }
            return true;
        }
        public void MoveToNextRow()
        {
            actualRow++;
            actualField = 0;
            rowFields = new int[fieldsCount];
        }
        public Pins EvaluateRow()
        {
            matchedPositions.Clear();
            matchedSecretpositions.Clear();
            Pins evaluationPins = new Pins();
            for(int i = 0; i < fieldsCount; i++)
            {
                if (secretFields[i] == rowFields[i])
                {
                    matchedPositions.Add(i);
                    matchedSecretpositions.Add(i);
                    evaluationPins.BlackPins++;
                }
            }
            for(int i = 0; i < fieldsCount; i++)
                for (int j = 0; j < fieldsCount; j++)
                {
                    if (secretFields[i] == rowFields[j] && !matchedSecretpositions.Contains(i) && !matchedPositions.Contains(j))
                    {
                        matchedPositions.Add(j);
                        matchedSecretpositions.Add(i);
                        evaluationPins.WhitePins++;
                    }
                }
            return evaluationPins;
        }
        public void ReturnActualField(int fieldPosition)
        {
            if (fieldPosition < actualField)
            {
                actualField = fieldPosition;
            }
            rowFields[fieldPosition] = 0;
        }
        public void ResetState()
        {
            rowFields = new int[fieldsCount];
            actualField = 0;
            actualRow = 0;
            IsRightCombination = false;
        }
    }
}
