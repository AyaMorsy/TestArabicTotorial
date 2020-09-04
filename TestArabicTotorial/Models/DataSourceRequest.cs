using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TestArabicTotorial.Models
{
    public class DataSourceRequest
    {
        public int Page { get; set; }

        public int PageSize { get; set; }

        public DataSourceRequest()
        {
            Page = 1;
            PageSize = 10;
        }
    

}
}
