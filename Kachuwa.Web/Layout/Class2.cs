using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kachuwa.Web.Theme
{



    public class Row
    {
        public int RowId { get; set; }
        public string ClassName { get; set; }
        public string RowName { get; set; }
        public int Order { get; set; }
        public List<Column> Columns { get; set; }
    }

    public class Column
    {
        public int ColumnId { get; set; }
        public string Name { get; set; }
        public int Order { get; set; }
        public string ClassName { get; set; }
        public string Content { get; set; }
        public string Plugin { get; set; }
    }

    public class LayoutContentResource
    {
        public string Type { get; set; }
        public string Resource { get; set; }
    }

    public interface ILayoutContent
    {
        string Name { get; set; }
        List<LayoutContentResource> Resources { get; set; }
        List<Row> Rows { get; set; }
    }

    public interface ILayoutContentProvider
    {
        Task<ILayoutContent> Get(string name);
        Task<bool> Delete(string name);
        Task<bool> Save(string name);
        Task<bool> Reset(string name);
        Task<List<ILayoutContent>> GetAll();
    }

    public class LayoutContentRenderer
    {
        private ILayoutContent _layout;
        public LayoutContentRenderer(ILayoutContent layout)
        {
            _layout = layout;
        }

        private string _row = "<div rid={0} rname={1} class={2} >{3}</div>";
        private string _column = "<div cid={0} cname={1} class={2} >{3}</div>";
        public string Render()
        {
            StringBuilder layout = new StringBuilder();

            foreach (var resource in _layout.Resources)
            {
                switch (resource.Type)
                {
                    case "css":
                        break;
                    case "script":
                        break;
                }
            }

            var rows = _layout.Rows.OrderBy(e => e.Order);
            foreach (var row in rows)
            {
                var columns = row.Columns.OrderBy(x => x.Order);
                // var rowColums = "";
                StringBuilder rowColums = new StringBuilder();

                foreach (var column in columns)
                {
                    rowColums.AppendFormat(_column, column.ColumnId, column.Name, column.ClassName, column.Content);
                }
                layout.AppendFormat(_row, row.RowId, row.RowName, row.ClassName, rowColums.ToString());


            }
            return layout.ToString();

        }
    }

    public class FileBaseLayoutProvider : ILayoutContentProvider
    {//store in json format for each file


        public Task<ILayoutContent> Get(string name)
        {
            throw new System.NotImplementedException();
        }

        public Task<bool> Delete(string name)
        {
            throw new System.NotImplementedException();
        }

        public Task<bool> Save(string name)
        {
            throw new System.NotImplementedException();
        }

        public Task<bool> Reset(string name)
        {
            throw new System.NotImplementedException();
        }

        public Task<List<ILayoutContent>> GetAll()
        {
            throw new System.NotImplementedException();
        }
    }

    public class LayoutManager
    {
        public ILayoutContentProvider Provider { get; set; }


        public LayoutManager(ILayoutContentProvider provider)
        {
            Provider = provider;
        }

        public async Task<bool> Save(string name)
        {
            return await Provider.Save(name);

        }

        public Task<List<ILayoutContent>> GetAll()
        {
            throw new System.NotImplementedException();
        }

        public async Task<bool> Reset(string name)
        {
            return true;
        }

        public async Task<ILayoutContent> Get(string name)
        {
            throw new System.NotImplementedException();
        }

        public async Task<bool> Delete(string name)
        {
            return true;
        }
    }



}
