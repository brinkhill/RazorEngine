using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using RazorEngine;
using RazorEngine.Templating;
using System.Reflection;
using RazorEngine.Compilation;
using RazorEngine.Configuration;
using RazorEngine.Compilation.CSharp;

namespace GenerateViewCode
{
    class Program
    {
        static void Main(string[] args)
        {
            string templatebasename = "RazorEngine.Templating.TemplateBase";
            if (args.Length > 0)
            {
                templatebasename = args[0];
            }

            string classNamespace = "Brinkhill.Mobile.Publishing.Views";
            if (args.Length > 1)
            {
                classNamespace = args[1];
            }

            var filecount = 0;

            Console.WriteLine("Generating .cshtml.cs classes based on {0}", templatebasename);

            // for each .cshtml file under the working directory, generate a .cs file if it has changed.
            foreach (var templatepath in Directory.EnumerateFiles(Environment.CurrentDirectory, "*.cshtml", SearchOption.AllDirectories))
            {
                FileInfo fitemplate = new FileInfo(templatepath);
                FileInfo ficode = new FileInfo(templatepath + ".cs");

                try
                {
                    if (!ficode.Exists || ficode.LastWriteTimeUtc < fitemplate.LastWriteTimeUtc)
                    {
                        // get classname from path
                        var cn = fitemplate.Name.Substring(0, fitemplate.Name.IndexOf('.'));
                        var pt = fitemplate.DirectoryName.Split(new char[] { Path.DirectorySeparatorChar }, StringSplitOptions.RemoveEmptyEntries);
                        var ns = pt[pt.Length - 1];
                        for (var i = pt.Length - 2; i > 0; i--)
                        {
                            ns = pt[i] + "." + ns;
                            if (pt[i + 1] == "Views") break;
                        }
                        
                        string template =
                             File.ReadAllText(fitemplate.FullName);
                        //Razor.SetTemplateBaseType(typeof(TemplateBase<>));

                        string viewModelName = string.Empty;

                        string baseTypeName = templatebasename;
                        if (template.StartsWith("@model"))
                        {
                            var l1 = template.IndexOf("\n");
                            viewModelName = template.Substring(6, l1 - 6).Trim();
                            template = template.Substring(l1).Trim();
                            baseTypeName = templatebasename + "<" + viewModelName + ">";
                        }
                        else if (cn == "_ViewStart")
                        {
                            baseTypeName = "System.Web.WebPages.StartPage";
                        }

                        CompilerServiceBuilder.SetCompilerServiceFactory(new DefaultCompilerServiceFactory());
                        RazorEngine.Configuration.DefaultTemplateServiceConfiguration conf = new RazorEngine.Configuration.DefaultTemplateServiceConfiguration();
                        conf.Debug = true;

                        conf.ReplacementClassName = cn;
                        conf.ReplacementViewModelName = viewModelName;
                        conf.ReplacementNamespace = classNamespace;
                        
                        string result = string.Empty;

                        using (var service = new TemplateService(conf))
                        {
                            try
                            {
                                result = service.ParseAndReturnSource(template);
                            }
                            catch (TemplateCompilationException ex)
                            {
                                result = ex.SourceCode;
                            }
                        }

                        File.WriteAllText(ficode.FullName, result);
                        Console.WriteLine("Updated {0}.{1}", ns, cn);
                        filecount++;
                    }
                    else
                    {
                        Console.WriteLine("Skipping unchanged view {0}", fitemplate.Name);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error updating {0} - {1}", ficode.Name, ex.Message);
                    Environment.Exit(1);
                }
            }
            Console.WriteLine("Done - updated {0} files", filecount);
        }
    }


}