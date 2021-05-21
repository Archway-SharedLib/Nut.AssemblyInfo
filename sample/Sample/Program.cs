using System;
using System.Reflection;

[assembly: AssemblyMetadata("Sample", "Sampleの値です。")]

namespace Sample
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine(Nut.AssemblyInfo.Title);
            Console.WriteLine(Nut.AssemblyInfo.Version);
            Console.WriteLine(Nut.AssemblyInfo.FileVersion);
            Console.WriteLine(Nut.AssemblyInfo.InformationalVersion);
            Console.WriteLine(Nut.AssemblyInfo.Company);
            Console.WriteLine(Nut.AssemblyInfo.Configuration);
            Console.WriteLine(Nut.AssemblyInfo.Product);

            Console.WriteLine(Nut.AssemblyInfo.Metadata.Sample);

            Console.WriteLine(Nut.AssemblyInfo.Project.AssemblyName);
            Console.WriteLine(Nut.AssemblyInfo.Project.RootNamespace);
            Console.WriteLine(Nut.AssemblyInfo.Project.TargetFrameworkIdentifier);
            Console.WriteLine(Nut.AssemblyInfo.Project.TargetFrameworkMoniker);
            Console.WriteLine(Nut.AssemblyInfo.Project.TargetFrameworkVersion);
        }
    }
}
