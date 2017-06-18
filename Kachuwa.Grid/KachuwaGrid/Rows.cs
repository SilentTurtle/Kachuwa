using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Kachuwa.Grid;
using Kachuwa.KGrid;

namespace Kachuwa.KGrid
{
    public interface IKachuwaGridRow
    {

    }
    public interface IKachuwaGridRow<out T>
    {
        string CssClasses { get; set; }
        T Model { get; }
    }
    public interface IKachuwaGridRows<out T> : IEnumerable<IKachuwaGridRow<T>>
    {
    }

    public interface IKachuwaGridRowsOf<T> : IKachuwaGridRows<T>
    {
        Func<T, string> CssClasses { get; set; }
        IKachuwaGrid<T> Grid { get; }
    }
    public interface IKachuwaGridRows
    {

    }
    public class KachuwaGridRow<T> : IKachuwaGridRow<T>
    {
        public string CssClasses { get; set; }
        public T Model { get; set; }

        public KachuwaGridRow(T model)
        {
            Model = model;
        }
    }
    public class KachuwaGridRows<T> : IKachuwaGridRowsOf<T>
    {
        public IEnumerable<IKachuwaGridRow<T>> CurrentRows { get; set; }
        public Func<T, string> CssClasses { get; set; }
        public IKachuwaGrid<T> Grid { get; set; }

        public KachuwaGridRows(IKachuwaGrid<T> grid)
        {
            Grid = grid;
        }

        public virtual IEnumerator<IKachuwaGridRow<T>> GetEnumerator()
        {
            if (CurrentRows == null)
            {
                var items = Grid.Source;
                CurrentRows = items
                  .ToList()
                  .Select(model => new KachuwaGridRow<T>(model)
                  {
                      CssClasses = CssClasses?.Invoke(model)
                  });
                //IQueryable<T> items = Grid.Source;
                //foreach (IGridProcessor<T> processor in Grid.Processors.Where(proc => proc.ProcessorType == GridProcessorType.Pre))
                //    items = processor.Process(items);

                //foreach (IGridProcessor<T> processor in Grid.Processors.Where(proc => proc.ProcessorType == GridProcessorType.Post))
                //    items = processor.Process(items);

                //CurrentRows = items
                //    .ToList()
                //    .Select(model => new GridRow<T>(model)
                //    {
                //        CssClasses = CssClasses?.Invoke(model)
                //    });
            }

            return CurrentRows.GetEnumerator();
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

}
