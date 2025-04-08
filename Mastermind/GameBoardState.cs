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
        public bool IsRightCombination;
        public const int rowsCount = 15;
        public const int fieldsCount = 5;
        public GameBoardState()
        {
            rowFields = new int[fieldsCount];
            secretFields = new int[fieldsCount];
            actualField = 0;
            actualRow = 0;
            IsRightCombination = false;
        }
        public bool MoveActualField(int color)
        {
            rowFields[actualField] = color;
            if (actualField == fieldsCount -1)
            {               
                IsRightCombination = EvaluateRow();
                if (IsRightCombination)
                {
                    return true;
                }
                if (actualRow < rowsCount - 1)
                {
                    actualRow++;
                    actualField = 0;
                    rowFields = new int[fieldsCount];
                    return true;
                }
                return false;
            }
            while (rowFields[actualField] > 0)
            {
                actualField++;
            }
            return true;
        }
        private bool EvaluateRow()
        {
            return false;
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
