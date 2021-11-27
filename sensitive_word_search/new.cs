using System.Diagnostics;
using System.Diagnostics.Tracing;
using System.Runtime.Serialization;
using System.IO;
using System;
using System.Threading.Tasks.Dataflow;
using System.Threading.Tasks;
using Serilog;
using System.Text;

namespace TPL_sensitive_word_search
{
    public class dataflow
    {
        public void nono(string[] args)
        {
            // 创建全局静态实例
            Log.Logger = new LoggerConfiguration()
                //设置最低等级
                .MinimumLevel.Verbose()
                //将事件发送到文件
                .WriteTo.File(@"F:\cqf\vs2021project\sensitive_word_search\log_output\Log.txt",     // 日志文件名
                    outputTemplate:                    // 设置输出格式，显示详细异常信息
                    @"{Timestamp:yyyy-MM-dd HH:mm-ss.fff }[{Level:u3}] {Message:lj}{NewLine}{Exception}", 
                    rollingInterval: RollingInterval.Day,   // 日志按月保存
                    rollOnFileSizeLimit: true,              // 限制单个文件的最大长度
                    encoding:Encoding.UTF8,                 // 文件字符编码
                    retainedFileCountLimit:10,              // 最大保存文件数
                    fileSizeLimitBytes:10*1024)                // 最大单个文件长度
                .CreateLogger();
            
            var stopWatch = new Stopwatch();    // 计时器
            Log.Verbose("开始敏感词查找机流程测试：3 2 1 ");
            Log.Information("开始测试");
            TPL_sensitive_word_search project_test = new TPL_sensitive_word_search("Hello World", "Hello cqf");
            string line;
            string test_file_path = "F://cqf//vs2021project//sensitive_word_search//test.txt";
            Task.Run(() =>
            {    
                try
                {
                    using (StreamReader sr = new StreamReader(test_file_path))
                    {
                        while ((line = sr.ReadLine()) != null)
                        {
                            project_test.transformBlock.Post(line);
                            
                        }
                        stopWatch.Stop();
                        Log.Information($"结束测试， 共运行{stopWatch.ElapsedMilliseconds}ms。");
                        Log.CloseAndFlush();
                    }
                }
                catch(Exception e)
                {
                    Log.Error(e,"捕获异常:文件读取错误！");
                }                           
                
            });
        }
    }

    public class TPL_sensitive_word_search
    {
        public string sensitive_words;
        public string replace_words;

        BroadcastBlock<string> broadcastBlock = new BroadcastBlock<string>(p=>p);
        public TransformBlock<string, string> transformBlock;
        // 字段初始化时,不能引用非静态字段、方法或属性

        public ActionBlock<string> consoleBlock = new ActionBlock<string>((input) =>
        {
            Console.WriteLine(input);
        });

        public ActionBlock<string> fileBlock = new ActionBlock<string>((input) =>
        {
            using (StreamWriter sw = new StreamWriter("output.txt",true))
            { 
                sw.WriteLine(input); 
            }
        });

        public TPL_sensitive_word_search(string sensitive_words, string replace_words)
        {
            // 初始化函数
            this.sensitive_words = sensitive_words; // 敏感词
            this.replace_words = replace_words; // 替换敏感词的新词
            transformBlock = new TransformBlock<string, string>((input) =>
            {
                return input.Replace(this.sensitive_words, this.replace_words);
            });

            // 流程定义
            transformBlock.LinkTo(broadcastBlock);
            broadcastBlock.LinkTo(consoleBlock);
            broadcastBlock.LinkTo(fileBlock);
            Console.WriteLine("sensitive_word_search instance has been inited!");
            Log.Information("敏感词查找机类对象已初始化！");
        }
    }
}
