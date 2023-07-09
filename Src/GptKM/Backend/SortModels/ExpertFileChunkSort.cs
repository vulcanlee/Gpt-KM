using CommonDomain.DataModels;
using System.Collections.Generic;

namespace Backend.SortModels
{
    public enum ExpertFileChunkSortEnum
    {
        NameAscending,
        NameDescending,
    }
    public class ExpertFileChunkSort
    {
        public static void Initialization(List<SortCondition> SortConditions)
        {
            SortConditions.Clear();
            SortConditions.Add(new SortCondition()
            {
                Id = (int)ExpertFileChunkSortEnum.NameAscending,
                Title = "名稱 遞增"
            });
            SortConditions.Add(new SortCondition()
            {
                Id = (int)ExpertFileChunkSortEnum.NameDescending,
                Title = "名稱 遞減"
            });
        }
    }
}
