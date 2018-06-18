using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpecLab.Business.Report
{
    public class ReportGridTable<T>
    {
        public List<GridColumn> Columns = new List<GridColumn>();

        public List<T> Datas = new List<T>();

        public List<CellDisplay> HeaderCellDisplay
        {
            get
            {
                return (from u in Columns select u.CellDisplay).ToList();
            }
        }

        public List<CellDisplay> GetDataDisplay(int index)
        {
            List<CellDisplay> listCellDisplay = new List<CellDisplay>();
            for (int j = 0; j < Columns.Count; j++)
            {
                var displayText = "";
                if (Columns[j].DataSourceName == "{{index}}")
                {
                    displayText = (index + 1).ToString();
                }
                else if (!string.IsNullOrEmpty(Columns[j].DataSourceName))
                {
                    var propertyGet = Datas[index].GetType().GetProperty(Columns[j].DataSourceName);
                    if (propertyGet == null)
                    {
                        throw new Exception(string.Format("{0} dataSourceName is invalid", Columns[j].DataSourceName));
                    }
                    displayText = (string)propertyGet.GetValue(Datas[index], null);
                }

                listCellDisplay.Add(new CellDisplay()
                {
                    Ratio = Columns[j].Ratio,
                    DisplayText = displayText,
                    PaddingLeft = Columns[j].PaddingLeft,
                    CellAlign = Columns[j].DataAlign
                });
            }

            return listCellDisplay;
        }
    }
}
