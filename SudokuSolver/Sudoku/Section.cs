using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sudoku
{
    public enum SectionType { ROW, COLUMN, REGION }

    [Serializable]
    public class Section
    {
        public List<Cell> items;
        public SectionType type;

        public Section(SectionType t)
        {
            type = t;
        }

        public void AssignCells(List<Cell> cells)
        {
            items = cells;
        }

        // Step through each cell, if a hardcoded cell is found then retrace
        //      the section and remove that possibility from each cell
        public void RefactorPossibilities()
        {
            for (int i = 0; i < items.Count; i++)
            {
                if (!items[i].isBlank)                          // if cell has a set value
                {
                    for (int j = 0; j < items.Count; j++)       // retrace each cell in region and remove set value
                        if (items[j].isBlank)                   // from blank cells possibility list
                        {
                            if (items[j].possibilities.Count > 1)
                                items[j].possibilities.Remove(items[i].value);

                            if (items[j].possibilities.Distinct().Count() == 1)     // if cell has only 1 possibility value left (including zero), 
                            {
                                if (!items.Any(x => x.value == items[j].possibilities.First()))
                                    items[j].AssignCellValue();                         // set cell to that value
                                //Console.WriteLine("One Possibility left");
                            }
                        }
                }
            }
        }

        // Search the region for possibility values occurring once, and if found set it to specified cell
        public void SingleOut()
        {
            for (int i = 0; i < items.Count; i++)
            {
                int numSpecificPoss = items.OfType<Cell>()
                    .Where(x => x.isBlank)
                    .Where(x => x.possibilities.Contains(i + 1))
                    .Count();

                if (numSpecificPoss == 1)
                {
                    Cell SingledOut = items.OfType<Cell>()
                        .Where(x => x.isBlank)
                        .Where(x => x.possibilities.Contains(i + 1))
                        .Single();

                    //Console.WriteLine("Singled out");
                    SingledOut.ForceCellValue(i + 1);
                }
            }
        }

        public bool SectionVerified()
        {
            int allValues = items.OfType<Cell>()
                .Where(x => x.value > 0)
                .Select(x => x.value).ToArray()
                .Count();

            int distinctValues = items.OfType<Cell>()
                .Where(x => x.value > 0)
                .Select(x => x.value).ToArray()
                .Distinct()
                .Count();

            if (allValues == distinctValues)
                return true;
            else
                return false;
        }
    }
}
