using System.Collections.Generic;

namespace eDriven.Gui.Data.eDriven.Gui.Data.Collections
{
    /// <summary>
    /// Sort compare function signature
    /// </summary>
    /// <param name="item1"></param>
    /// <param name="item2"></param>
    /// <returns></returns>
    public delegate int SortCompareFunction(object item1, object item2, List<SortField> fields = null);
}