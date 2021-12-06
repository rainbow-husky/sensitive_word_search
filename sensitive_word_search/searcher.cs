using System.Runtime.CompilerServices;
using System;
using System.IO;
using System.Threading.Tasks.Dataflow;
using System.Threading.Tasks;
using System.Threading;
using Serilog;
using System.Text;
using CommandDotNet;
using System.Diagnostics;
using Spectre.Console;

namespace sensitive_word_search
{
    [Command(Description = "Search and replace sensitive words in strings or file...")]
    public class searcher
    {
        [DefaultCommand,
        Command(Name = "search and replace",
        Description = "Search and replace sensitive words",
        ExtendedHelpText = "more details and examples")
        ]
        public int SearchAndReplace(
            [Option(LongName = "type", ShortName = "t",
                    Description = "type of input, 0 for strings, 1 for txt file")]
            int type = 0,
            [Option(LongName = "input_path", ShortName = "i",
                    Description = "the path of input file")]
            string inputPath = "input.txt",
            [Option(LongName = "output_path", ShortName = "o",
                    Description = "the path of output file")]
            string outputPath = "output.txt",
            [Option(LongName = "the sensitive_words",
                    Description = "the sensitive words")]
            string sensitive_words = "Hello World",
            [Option(LongName = "the replace_words",
                    Description = "the words that replace sensitive words")]
            string replace_words = "Hello cqf")
        {
            AnsiConsole.Progress()
                .Start(ctx=>
                {
            
                    var task1=ctx.AddTask("[bold red]Please wait patiently[/]");
                    while(!ctx.IsFinished)
                    {
                        task1.Increment(1.5);
                        Thread.Sleep(10);
                    }
                });
            // String a;
            // a= AnsiConsole.Prompt(
            // new SelectionPrompt<string>()
            // .Title("[green]input string or read file[/]")
            // .PageSize(5)
            // .MoreChoicesText("[grey](reveal more datastream)[/]")
            // .AddChoices(new[] {
            //     "string", "file",
            // }));

            var table=new Table();
            table.AddColumn("date");
            table.AddColumn("auther");
            table.AddColumn("version");
            table.AddRow("2021.12.05","Cqf","1.0");
            table.Border(TableBorder.Ascii);
            AnsiConsole.Write(table);

            var stopWatch = new Stopwatch();    // 计时器
            Log.Verbose("开始敏感词查找机流程测试：3 2 1 ");
            Log.Information("开始测试");

            if((type ==1) && (!File.Exists(inputPath)))
            {
                Log.Error("运行终止：输入文件路径不存在!!!");
                stopWatch.Stop();
                Log.Information($"结束测试， 共运行{stopWatch.ElapsedMilliseconds}ms。");
                Log.CloseAndFlush();
                return 0;
            }

            TPL_sensitive_word_search tpl_finder = new TPL_sensitive_word_search(sensitive_words, replace_words, outputPath);
            string line;
            if(type == 0)
            {
                //输入数据为字符串
                
                Console.WriteLine("plz input strings...");
                line = Console.ReadLine();
                Log.Information($"输入数据为字符串{line}，接下来进行字符串处理！");
                Task.Run(() =>
                {
                    tpl_finder.transformBlock.Post(line);
                }
                );
                stopWatch.Stop();
                Console.WriteLine("the string has been replaced!");
                Log.Information($"字符串处理成功，结束测试， 共运行{stopWatch.ElapsedMilliseconds}ms。");
                Log.CloseAndFlush();
            }
            else
            {
                
                Console.WriteLine("plz input file's path...");
                Log.Information($"输入数据为文件，路径为：{inputPath}");
                Task.Run(() =>    
                {
                    try{
                        using(StreamReader sr = new StreamReader(inputPath))

                        while ((line= sr.ReadLine())!= null)
                        {
                            tpl_finder.transformBlock.Post(line); 
                        }
                        stopWatch.Stop();
                        Console.WriteLine("the file has been replaced!");
                        Log.Information($"文件处理成功，结束测试， 共运行{stopWatch.ElapsedMilliseconds}ms。");
                        Log.CloseAndFlush();
                    }
                    catch
                    {
                        Log.Error($"Can't read {inputPath}");
                        stopWatch.Stop();
                        Log.Information($"文件处理出错，结束测试， 共运行{stopWatch.ElapsedMilliseconds}ms。");
                        Log.CloseAndFlush();
                    }
                });
            }

            return 1;
        }
    }
}
