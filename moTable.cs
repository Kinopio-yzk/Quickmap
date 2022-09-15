using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace MyMapObjects
{
    public class moTable
    {
        public DataTable dt;

        public List<short> columnsLength;
        public List<string> columnsName;
        public List<string> ID;
        public int rowCount, columnsCount;

        public moTable()
        {
            dt = new DataTable();
            columnsLength = new List<short>();
            columnsName = new List<string>();
            ID = new List<string>();
        }
    }
}
