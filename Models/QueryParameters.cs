using System;
namespace asp_net_web_api.API.Models
{
    public class QueryParameters
    {
        /*Pagination*/

        const int _maxSize = 100;
        private int _size = 50;

        public int Size {
            get { return _size; } 
            set { _size = Math.Min(value, _maxSize); }
        }

        public int Page {get; set;} = 1;


        /*Filter*/

        public string SortBy { get; set; } = "Id";

        private string _sortOrder = "asc";

        public string SortOrder { 
            get { return _sortOrder; } 
            set { if(value == "asc" || value == "desc") _sortOrder=value; }
        }
    }
}