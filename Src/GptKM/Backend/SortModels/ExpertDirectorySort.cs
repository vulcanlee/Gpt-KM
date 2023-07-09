using CommonDomain.DataModels;
using System.Collections.Generic;

namespace Backend.SortModels
{
    public enum ExpertDirectorySortEnum
    {
        NameAscending,
        NameDescending,
    }
    public class ExpertDirectorySort
    {
        public static void Initialization(List<SortCondition> SortConditions)
        {
            SortConditions.Clear();
            SortConditions.Add(new SortCondition()
            {
                Id = (int)ExpertDirectorySortEnum.NameAscending,
                Title = "名稱 遞增"
            });
            SortConditions.Add(new SortCondition()
            {
                Id = (int)ExpertDirectorySortEnum.NameDescending,
                Title = "名稱 遞減"
            });
        }
    }
}
