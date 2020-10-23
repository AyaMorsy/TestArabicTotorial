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
using Microsoft.Extensions.Hosting.Internal;
using Microsoft.AspNetCore.Hosting;

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
        private readonly IHostingEnvironment _hostEnvironment;

        public  FileUploadController(IHostingEnvironment hostEnvironment)
        {
            TableModel table = new TableModel() { newList = newList, L1List = _LList, L2List = _L2List, Rules = _Rules };
            _hostEnvironment = hostEnvironment;
        }
        [Obsolete]
        public IActionResult Index()
        {
            TableModel table = new TableModel() { newList = newList, L1List = _LList, L2List = _L2List, Rules = _Rules };
           

            return View(table);
        }
        [HttpPost("FileUpload")]
        
        public async Task<IActionResult> Index(List<IFormFile> files )
        {
            var watch = new System.Diagnostics.Stopwatch();

            watch.Start();

            if ( files.First() !=null  )
            {
                var  newFile = files.First();
                TempData["FileUploaded"] = "File Successfully Uploaded";
                AprioriAlog( newFile );
            }
            else
                TempData["FileUploaded"] = "please upload file ";
            TableModel table = new TableModel() { newList = newList, L1List = _LList, L2List = _L2List, Rules = _Rules };

            watch.Stop();
            TimeElapsed=  watch.ElapsedMilliseconds + "  MS";
            TempData["TimeElapsed"] = TimeElapsed + "   Time elapsed ";
            
            if (newList.Count > 0)
            {
                TempData["DataExists"] = "1";
            }
            return View(table);
          

        }
     


        private void AprioriAlog(IFormFile  newFile)
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



           // BAL.Apriori aprioriTid = new BAL.Apriori(tableitemset.ToList());
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


        // this page is for  AprioriTid alogthrim 
        
        [Obsolete]
        public IActionResult AprioriHome(List<IFormFile> files)
        {
            var watch = new System.Diagnostics.Stopwatch();
            watch.Start();
            if (files.Count>0 &&  files.First() != null )
            {
                var newFile = files.First();
                TempData["FileUploaded"] = "File Successfully Uploaded";
                AprioriTid(newFile );
            }
            else
            TempData["FileUploaded"] = "please upload file ";
            watch.Stop();
            TimeElapsed = watch.ElapsedMilliseconds + "  MS";
            TempData["TimeElapsed"] = TimeElapsed + "   Time elapsed ";
           
            TableModel table = new TableModel() { newList = newList, L1List = _LList, L2List = _L2List, Rules = _Rules };
            if (newList.Count > 0)
            {
                TempData["DataExists"] = "1";
            }
            return View(table);
            // return RedirectToAction(  (newList.ToList());
            // return View();
        }

        [Obsolete]
        private void AprioriTid(IFormFile newFile)
        {
            int Support = 2;
            if (newFile != null && newFile.Length > 0)
            {
                using (var tr = new System.IO.StreamReader(newFile.OpenReadStream()))
                {
                    while ((line = tr.ReadLine()) != null)
                    {
                        line.Replace("\t", "#");
                        lines.Add(line);

                    }

                }
            }
            else
            { }
            List<BAL.ItemSet> tableitemset = GetitemList(lines.ToList());
            // Fill table of items 
             newList = tableitemset;

            // write to new file new candiaties 
            string docPath =   Path.Combine(_hostEnvironment.WebRootPath, "File");

            // Write the string array to a new file named "WriteLines.txt".

            using (StreamWriter outputFile = new StreamWriter(Path.Combine(docPath, "TempFile.txt")))
            {
                foreach (ItemSet line in tableitemset)
                {
                    outputFile.WriteLine(line.Label);
                }
            }
                BAL.Apriori apriori = new Apriori(Path.Combine(docPath, "TempFile.txt"));
                int k = 1;
                List<BAL.ItemSet> ItemSets = new List<BAL.ItemSet>();


                // BAL.Apriori aprioriTid = new BAL.Apriori(tableitemset.ToList());


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
                        _LList.Add(new BAL.ItemSet() { Support = 0, Label = "L" + k });
                        TableUserControl(L, rules);

                        next = true;
                        k++;
                        ItemSets.Add(L);

                    }
                } while (next);
            

        }

    }
}