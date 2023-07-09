using CommonDomain.DataModels;
using System.Collections.Generic;

namespace Backend.SortModels
{
    public enum ExpertFileSortEnum
    {
        NameAscending,
        NameDescending,
    }
    public class ExpertFileSort
    {
        public static void Initialization(List<SortCondition> SortConditions)
        {
            SortConditions.Clear();
            SortConditions.Add(new SortCondition()
            {
                Id = (int)ExpertFileSortEnum.NameAscending,
                Title = "名稱 遞增"
            });
            SortConditions.Add(new SortCondition()
            {
                Id = (int)ExpertFileSortEnum.NameDescending,
                Title = "名稱 遞減"
            });
        }
    }
}
