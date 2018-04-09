using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyModel;

namespace Kachuwa.Reflection
{

    //public class AppDomain
    //{
    //    public static AppDomain CurrentDomain { get; private set; }

    //    static AppDomain()
    //    {
    //        CurrentDomain = new AppDomain();
    //    }

    //    public Assembly[] GetAssemblies()
    //    {
    //        var assemblies = new List<Assembly>();
    //        var dependencies = DependencyContext.Default.RuntimeLibraries;
    //        foreach (var library in dependencies)
    //        {
    //            if (IsCoreLibrary(library))
    //            {
    //                var assembly = Assembly.Load(new AssemblyName(library.Name));
    //                assemblies.Add(assembly);
    //            }
    //        }
    //        return assemblies.ToArray();
    //    }
    //    public Assembly[] GetAssemblies(string includeAssemblyStartWith)
    //    {
    //        var assemblies = new List<Assembly>();
    //        var dependencies = DependencyContext.Default.RuntimeLibraries;
    //        foreach (var library in dependencies)
    //        {
    //            if (IsCoreLibrary(library) || IncludeLibrary(library, includeAssemblyStartWith))
    //            {
    //                var assembly = Assembly.Load(new AssemblyName(library.Name));
    //                assemblies.Add(assembly);
    //            }
    //        }
    //        return assemblies.ToArray();
    //    }

    //    private static bool IsCoreLibrary(RuntimeLibrary compilationLibrary)
    //    {
    //        return compilationLibrary.Name.Contains("Kachuwa")
    //               || compilationLibrary.Dependencies.Any(d => d.Name.StartsWith("Kachuwa"));
    //    }
    //    private static bool IncludeLibrary(RuntimeLibrary compilationLibrary, string librayName)
    //    {
    //        return compilationLibrary.Name.Contains(librayName)
    //               || compilationLibrary.Dependencies.Any(d => d.Name.StartsWith(librayName));
    //    }

    //    private static IEnumerable<Assembly> GetReferencingAssemblies()
    //    {
    //        var assemblies = new List<Assembly>();
    //        var dependencies = DependencyContext.Default.RuntimeLibraries;

    //        foreach (var library in dependencies)
    //        {
    //            try
    //            {
    //                var assembly = Assembly.Load(new AssemblyName(library.Name));
    //                assemblies.Add(assembly);
    //            }
    //            catch (FileNotFoundException)
    //            { }
    //        }
    //        return assemblies;
    //    }
    //}
}