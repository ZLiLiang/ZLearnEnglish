using AsmResolver;
using AsmResolver.DotNet;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;
using ModuleDefinition = AsmResolver.DotNet.ModuleDefinition;

namespace Z.Commons
{
    public static class ReflectionHelper
    {
        /// <summary>
        /// 据产品名称获取程序集
        /// </summary>
        /// <param name="productName"></param>
        /// <returns></returns>
        public static IEnumerable<Assembly> GetAssembliesByProductName(string productName)
        {
            var asms = AppDomain.CurrentDomain.GetAssemblies();
            foreach (var asm in asms)
            {
                var asmCompanyAttr = asm.GetCustomAttribute<AssemblyProductAttribute>();
                if (asmCompanyAttr != null && asmCompanyAttr.Product == productName)
                {
                    yield return asm;
                }
            }
        }

        /// <summary>
        /// 是否是微软等的官方Assembly
        /// </summary>
        /// <param name="assembly"></param>
        /// <returns></returns>
        private static bool IsSystemAssembly(Assembly assembly)
        {
            var asmCompanyAttr = assembly.GetCustomAttribute<AssemblyCompanyAttribute>();
            if (asmCompanyAttr == null)
            {
                return false;
            }
            else
            {
                string companyName = asmCompanyAttr.Company;
                return companyName.Contains("Microsoft");
            }
        }

        /// <summary>
        /// 是否是微软等的官方Assembly
        /// </summary>
        /// <param name="asmPath">程序集文件路径</param>
        /// <returns></returns>
        private static bool IsSystemAssembly(string asmPath)
        {
            var moduleDef = ModuleDefinition.FromFile(asmPath);
            var assembly = moduleDef.Assembly;
            if (assembly == null)
            {
                return false;
            }
            var asmCompanyAtrr = assembly.CustomAttributes.FirstOrDefault(c => c.Constructor?.DeclaringType?.FullName == typeof(AssemblyCompanyAttribute).FullName);
            if (asmCompanyAtrr == null)
            {
                return false;
            }
            var companyName = ((Utf8String?)asmCompanyAtrr.Signature?.FixedArguments[0]?.Element)?.Value;
            if (companyName == null)
            {
                return false;
            }
            return companyName.Contains("Microsoft");
        }

        /// <summary>
        /// 判断file这个文件是否是程序集
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        private static bool IsManagedAssembly(string file)
        {
            using var fs = File.OpenRead(file);
            using PEReader peReader = new PEReader(fs);
            return peReader.HasMetadata && peReader.GetMetadataReader().IsAssembly;
        }

        /// <summary>
        /// 尝试通过路径加载程序集
        /// </summary>
        /// <param name="asmPath"></param>
        /// <returns></returns>
        private static Assembly? TryLoadAssembly(string asmPath)
        {
            AssemblyName asmName = AssemblyName.GetAssemblyName(asmPath);
            Assembly? asm = null;
            try
            {
                asm = Assembly.Load(asmName);
            }
            catch (BadImageFormatException ex)
            {
                Debug.WriteLine(ex.Message);
            }
            catch (FileLoadException ex)
            {
                Debug.WriteLine(ex.Message);
            }

            if (asm == null)
            {
                try
                {
                    asm = Assembly.LoadFile(asmPath);
                }
                catch (BadImageFormatException ex)
                {
                    Debug.WriteLine(ex.Message);
                }
                catch (FileLoadException ex)
                {
                    Debug.WriteLine(ex.Message);
                }
            }
            return asm;
        }

        /// <summary>
        /// loop through all assemblies
        /// 获取所有反射程序集
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<Assembly> GetAllReferencedAssemblies(bool skipSystemAssembly = true)
        {
            Assembly? rootAssembly = Assembly.GetEntryAssembly();
            if (rootAssembly == null)
            {
                rootAssembly = Assembly.GetCallingAssembly();
            }

            var returnAssemblies = new HashSet<Assembly>(new AssemblyEquality());//返回程序集
            var loadedAssemblies = new HashSet<string>();//加载程序集
            var assembliesToCheck = new Queue<Assembly>();//检查程序集
            assembliesToCheck.Enqueue(rootAssembly);

            if (skipSystemAssembly && IsSystemAssembly(rootAssembly) != false)
            {
                if (IsValid(rootAssembly))
                {
                    returnAssemblies.Add(rootAssembly);
                }
            }

            while (assembliesToCheck.Any())
            {
                var assemblyToCheck = assembliesToCheck.Dequeue();
                foreach (var reference in assemblyToCheck.GetReferencedAssemblies())
                {
                    if (!loadedAssemblies.Contains(reference.FullName))
                    {
                        var assembly = Assembly.Load(reference);
                        if (skipSystemAssembly && IsSystemAssembly(assembly))
                        {
                            continue;
                        }
                        assembliesToCheck.Enqueue(assembly);
                        loadedAssemblies.Add(reference.FullName);
                        if (IsValid(assembly))
                        {
                            returnAssemblies.Add(assembly);
                        }
                    }
                }
            }

            var asmsInBaseDir = Directory.EnumerateFiles(AppContext.BaseDirectory, "*.dll", new EnumerationOptions { RecurseSubdirectories = true });
            foreach (var asmPath in asmsInBaseDir)
            {
                if (!IsManagedAssembly(asmPath))
                {
                    continue;
                }
                AssemblyName asmName = AssemblyName.GetAssemblyName(asmPath);
                //如果程序集已经加载过了就不再加载
                if (returnAssemblies.Any(x => AssemblyName.ReferenceMatchesDefinition(x.GetName(), asmName)))
                {
                    continue;
                }
                if (skipSystemAssembly && IsSystemAssembly(asmPath))
                {
                    continue;
                }
                Assembly? asm = TryLoadAssembly(asmPath);
                if (asm == null)
                {
                    continue;
                }
                if (!IsValid(asm))
                {
                    continue;
                }
                if (skipSystemAssembly && IsSystemAssembly(asm))
                {
                    continue;
                }
                returnAssemblies.Add(asm);
            }
            return returnAssemblies.ToArray();
        }

        /// <summary>
        /// 验证程序集是否合法
        /// </summary>
        /// <param name="assembly"></param>
        /// <returns></returns>
        private static bool IsValid(Assembly assembly)
        {
            try
            {
                assembly.GetType();
                assembly.DefinedTypes.ToList();
                return true;
            }
            catch (ReflectionTypeLoadException)
            {
                return false;
            }
        }
    }

    /// <summary>
    /// 继承抽象类，重写了程序集比较方法和获取程序集哈希方法
    /// </summary>
    class AssemblyEquality : EqualityComparer<Assembly>
    {
        public override bool Equals(Assembly? x, Assembly? y)
        {
            if (x == null && y == null)
            {
                return true;
            }
            if (x == null || y == null)
            {
                return false;
            }
            return AssemblyName.ReferenceMatchesDefinition(x.GetName(), y.GetName());
        }

        public override int GetHashCode([DisallowNull] Assembly obj)
        {
            return obj.GetName().FullName.GetHashCode();
        }
    }
}
