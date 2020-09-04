using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using BAL;
using TestArabicTotorial.Models;
using ReflectionIT.Mvc.Paging;

namespace TestArabicTotorial.Controllers
{
    public class FileUploadController : Controller
    {
        public string TimeElapsed;
        public string lblvalidationMessage ;
        List<string> lines = new List<string>();
        string line;
        List<ItemSet> newList = new List<ItemSet>();
        List<ItemSet> _LList = new List<ItemSet>();
        List<ItemSet> _L2List = new List<ItemSet>();
        List<AssociationRule> _Rules = new List<AssociationRule>();
        public IActionResult AprioriHome()
        {
            return View();
        }
        public IActionResult Index()
        {
            TableModel table = new TableModel() { newList = newList, L1List = _LList, L2List = _L2List, Rules = _Rules };
            

            return View(table);
        }
        [HttpPost("FileUpload")]
        
        public async Task<IActionResult> Index(List<IFormFile> files , int page = 1)
        {
            var watch = new System.Diagnostics.Stopwatch();

            watch.Start();

            if ( files.First() !=null  )
            {
                var  newFile = files.First();
                TempData["FileUploaded"] = "File Successfully Uploaded";  
                DoThings( newFile );
            }
            else
                TempData["FileUploaded"] = "please upload file ";
            watch.Stop();
            TimeElapsed=  watch.ElapsedMilliseconds + "  MS";
            TempData["TimeElapsed"] = TimeElapsed + "   Time elapsed ";
            //var qry = newList.ToList().AsQueryable();
            //var model = await PagingList.CreateAsync(qry, 10, page);
            TableModel table = new TableModel() { newList = newList, L1List = _LList, L2List = _L2List, Rules = _Rules };
            if (newList.Count > 0)
            {
                TempData["DataExists"] = "1";
            }
            return View(table);
           // return RedirectToAction(  (newList.ToList());

        }
        [HttpPost]
        public JsonResult Grid1(DataSourceRequest x)
        {
          return Json(newList.Skip(x.Page * 10).Take(10).ToList());

        }



        private void DoThings(IFormFile  newFile)
        {
            int Support = 2;
     

            if (newFile != null && newFile.Length > 0)
            {


                using (var  tr = new System.IO.StreamReader(newFile.OpenReadStream()))
                {
                    while ((line = tr.ReadLine()) != null)
                    {
                        line.Replace("\t", "#");
                        lines.Add(line);
                        
                    }
                  
                }
            }
            else
            {

              


            }
            List<BAL.ItemSet> tableitemset = GetitemList(lines.ToList());
            // Fill table of items 
            newList = tableitemset;
          

            BAL.Apriori apriori = new BAL.Apriori(lines.ToList());
            int k = 1;
            List<BAL.ItemSet> ItemSets = new List<BAL.ItemSet>();
            
            bool next;
            do
            {
                next = false;
                var L = apriori.GetItemSet(k, Support, IsFirstItemList: k == 1);
                if (L.Count > 0)
                {
                    List<AssociationRule> rules = new List<AssociationRule>();
                    if (k != 1)
                        rules = apriori.GetRules(L);
                    //  TableUserControl tableL = new TableUserControl(L, rules);
                    _LList.Add(new BAL.ItemSet() { Support = 0, Label = "L" +k}); 
                   TableUserControl(L, rules);

                    next = true;
                    k++;
                    ItemSets.Add(L);
                  
                }
            } while (next);
           


        }
        public List<BAL.ItemSet> GetitemList(List<string> Values)
        {
            DataTable dt = new DataTable();
            List<BAL.ItemSet> table = new List<BAL.ItemSet> ();
            if (table.Count == 0)
            {
                // table.Add(new BAL.ItemSet() { Label = "itemSet", Support = "Count" });

            }
        
            for (int i = 0; i < Values.Count; i++)
            {
                table.Add(new BAL.ItemSet() { Support = i, Label = Values[i] });
               // dt.Rows.Add(i, Values[i]);
            }

            return table;
        }
        //public void TableUserControl(ItemSet itemSet, List<AssociationRule> rules)
        //{
        //    List<TableModel> table = new List<TableModel>();

        //    List<AssociationRule> Rules = new List<AssociationRule>();
        //    if (table.Count == 0)
        //    {

        //        table.Add(new TableModel (){ llist = "itemSet", Count = "Count" });

        //    }
        //    DataTable dt2 = new DataTable();
        //    if (Rules.Count == 0)
        //    {
        //        //Rules.Add(new AssociationRule() { Label = "item",Confidance = "Confidance", Support = "Support" });
        //        dt2.Columns.Add("item", typeof(string));
        //        dt2.Columns.Add("Confidance", typeof(string));
        //        dt2.Columns.Add("Support", typeof(string));
        //    }

        //    foreach (var item in itemSet)
        //    {
        //        table.Add(new TableModel() { itemSet = item.Key.ToDisplay(), Count = item.Value.ToString()});

        //    }
        //    if (rules.Count == 0)
        //    {
        //      //  ItemSetsDataGrid.Height = 342;
        //        //RulesDataGrid.Visible = false;
        //    }
        //    else
        //    {
        //        //RuleSet.Text = "Rules";
        //        foreach (var item in rules)
        //        {

        //            dt2.Rows.Add(item.Label, item.Confidance.ToPercentString(), item.Support.ToPercentString());
        //           // Rules.Add(new AssociationRule() { Label = item.Label, Confidance = "Confidance", Support = "Support" });
        //        }
        //    }
        //    //ItemSetsDataGrid.Height = 500;
        //    //RulesDataGrid.Height = 500;
        //    //ItemSetsDataGrid.DataSource = dt;
        //    //ItemSetsDataGrid.DataBind();
        //    //RulesDataGrid.DataSource = dt2;
        //    //RulesDataGrid.DataBind();



        //    foreach (var item in itemSet)
        //    {
        //        if (item.Value < itemSet.Support)
        //        { }
        //          // ItemSetsDataGrid.Rows[ItemSetsDataGrid.Rows.Count - 1].BackColor = System.Drawing.Color.LightGray;
        //    }

        //    //var enumerableTable = (dt1 as System.ComponentModel.IListSource).GetList();
        //    //Chart1.DataBindTable(enumerableTable, "Itemset");
        //    //Chart1.Series["Series1"].XValueMember = "Itemset";
        //    //Chart1.Series["Series1"].YValueMembers = "Count";
        //    //Chart1.DataSource = dt1;
        //    //Chart1.DataBind();
        //}
        public void TableUserControl(ItemSet itemSet, List<AssociationRule> rules)
        {
           
           
            foreach (var item in itemSet)
            {
               
                _LList.Add( new ItemSet() { Label= item.Key.ToDisplay(),Support= item.Value });
             
            }
            if (rules.Count == 0)
            {
              
            }
            else
            {
                //RuleSet.Text = "Rules";
                foreach (var item in rules)
                {
                    _Rules.Add(item);
                   // dt2.Rows.Add(item.Label, item.Confidance.ToPercentString(), item.Support.ToPercentString());

                }
            }
          


            //foreach (var item in itemSet)
            //{
            //    if (item.Value < itemSet.Support)
            //        ItemSetsDataGrid.Rows[ItemSetsDataGrid.Rows.Count - 1].BackColor = System.Drawing.Color.LightGray;
            //}

        }

        public IActionResult Btn_click( )
        {
            TempData["buttonval"] = "First click";

            return RedirectToAction("Index"); 
        }
    }
}