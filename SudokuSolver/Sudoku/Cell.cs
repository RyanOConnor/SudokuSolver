using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sudoku
{
    [Serializable]
    public class Cell
    {
        public int value { get; private set; }
        public bool isBlank { get; set; }
        public List<int> possibilities { get; private set; }
        public int _x { get; private set; }
        public int _y { get; private set; }

        public Section row { get; private set; }
        public Section column { get; private set; }
        public Section region { get; private set; }

        public Cell(int v, bool b, int x, int y)
        {
            value = v;
            isBlank = b;
            _x = x;
            _y = y;

            if (!isBlank)
                possibilities = new List<int>();
            else
                possibilities = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
        }

        public void SetSections(Section r, Section c, Section reg)
        {
            row = r;
            column = c;
            region = reg;
        }

        public void AssignCellValue()
        {
            // Only continue if cell isn't a preset value
            if (isBlank)
            {
                // If answer found, block cell from use and modify peer region possibilities
                if (possibilities.Distinct().Count() == 1)
                {
                    value = possibilities.First();                   // Set value to only possibility
                    possibilities.Clear();
                    isBlank = false;

                    row.RefactorPossibilities();     // In peer regions change each cell's possibilities
                    column.RefactorPossibilities();  //     to reflect value change
                    region.RefactorPossibilities();

                    row.SingleOut();
                    column.SingleOut();
                    region.SingleOut();
                }
            }
        }

        public void ForceCellValue(int value)
        {
            if (isBlank)
            {
                possibilities.Clear();
                possibilities.Add(value);
                AssignCellValue();
            }
        }

        /*public bool CheckValidPlacement(int value)
        {
            if (row.items.Any(x => x.value == value))
                return false;
            else if (column.items.Any(x => x.value == value))
                return false;
            else if (region.items.Any(x => x.value == value))
                return false;
            else
                return true;
        }*/

    }
}
