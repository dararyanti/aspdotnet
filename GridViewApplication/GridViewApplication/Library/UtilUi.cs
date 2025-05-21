using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace GridViewApplication.Library
{
    public class UtilUi
    {
        /// <summary>
        /// Bind GridView dan Generate Paging
        /// </summary>
        /// <param name="Page">System.Web.UI.Page</param>
        /// <param name="gv">System.Web.UI.GridView</param>
        /// <param name="dt">System.Data.DataTable</param>
        /// <param name="RepeaterPage">System.Web.UI.RepeaterPage</param>
        /// <param name="lbPage">System.Web.UI.Label</param>
        public static void BindGrid(Page Page, GridView gv, DataTable dt, Repeater RepeaterPage, Label lbPage)
        {
            try
            {
                gv.DataSource = dt;
                gv.DataBind();
                if (dt.Rows.Count > 0)
                {
                    UtilUi.PageNumber(gv.PageIndex, dt.Rows.Count, gv, RepeaterPage, lbPage);
                    ScriptManager.RegisterStartupScript(Page, Page.GetType(), "NoData", "" +
                        "$('#divshowingdatapage').show();" +
                        "$('#dgDataPaging').show();", true);
                }
                else
                {
                    ScriptManager.RegisterStartupScript(Page, Page.GetType(), "NoData", "" +
                        "$('#divshowingdatapage').hide();" +
                        "$('#dgDataPaging').hide();", true);
                }
            }
            catch (Exception ex)
            {
                string exErrorDetail = Util.getDetail(ex);
                Util.CreateLog(exErrorDetail);
            }
        }

        /// <summary>
        /// Load Page Number dan Set Informasi Status Paging
        /// </summary>
        /// <param name="PageNumberIndex"></param>
        /// <param name="ItemCount"></param>
        /// <param name="GridView"></param>
        /// <param name="RepeaterPage"></param>
        /// <param name="Label"></param>
        private static void PageNumber(int PageNumberIndex, int ItemCount, GridView GridView, Repeater RepeaterPage, Label Label)
        {
            UtilUi.LoadPageNumber(PageNumberIndex, ItemCount, RepeaterPage, 10);
            if (ItemCount > 0)
            {
                string PageInfo = "Showing " + (GridView.PageIndex + 1).ToString("N0") + " from " + RepeaterPage.Items.Count.ToString("N0") + ". (" + ItemCount.ToString("N0") + " data)";
                Label.Text = PageInfo;
            }
        }

        /// <summary>
        /// Generate Repeater Page Number
        /// </summary>
        /// <param name="PageNumberIndex"></param>
        /// <param name="ItemCount"></param>
        /// <param name="RepeaterPage"></param>
        /// <param name="MaxItemPerPage"></param>
        private static void LoadPageNumber(int PageNumberIndex, int ItemCount, Repeater RepeaterPage, int MaxItemPerPage)
        {
            if (PageNumberIndex <= 0)
            {
                PageNumberIndex = 1;
            }
            int currentpage = PageNumberIndex;
            PageNumberIndex = PageNumberIndex + 1;
            DataTable dtPageNumber = new DataTable();
            dtPageNumber.Columns.Add("PageNumber");
            dtPageNumber.Columns.Add("Value");
            DataRow rowPageNumber;
            int LastPageNumber = (int)Math.Ceiling((double)ItemCount / MaxItemPerPage);
            PageNumberIndex = (int)Math.Ceiling((double)PageNumberIndex / 10);
            int StartIndex = (10 * (PageNumberIndex - 1) + 1);
            int LastIndex = (10 * PageNumberIndex);
            if (PageNumberIndex <= 0)
            {
                PageNumberIndex = 1;
            }
            if (LastIndex > LastPageNumber)
            {
                LastIndex = LastPageNumber;
            }
            if (StartIndex <= 0)
            {
                goto EndRoutine;
            }
            DataRow row = null;
            row = dtPageNumber.NewRow();
            row[0] = "<<";
            row[1] = StartIndex - 1;
            dtPageNumber.Rows.Add(row);
            for (int i = StartIndex; i <= LastIndex; i++)
            {
                rowPageNumber = dtPageNumber.NewRow();
                rowPageNumber[0] = i;
                rowPageNumber[1] = i;
                dtPageNumber.Rows.Add(rowPageNumber);
            }
            row = dtPageNumber.NewRow();
            row[0] = ">>";
            row[1] = LastIndex + 1;
            dtPageNumber.Rows.Add(row);
            if (currentpage < 10)
            {
                DataRow[] dr = dtPageNumber.Select("PageNumber='<<'");
                foreach (var drow in dr)
                {
                    drow.Delete();
                }
                dtPageNumber.AcceptChanges();
            }
            if (LastIndex == LastPageNumber)
            {
                DataRow[] dr = dtPageNumber.Select("PageNumber='>>'");
                foreach (var drow in dr)
                {
                    drow.Delete();
                }
                dtPageNumber.AcceptChanges();
            }
            EndRoutine:
            RepeaterPage.DataSource = dtPageNumber;
            RepeaterPage.DataBind();
        }

    }
}