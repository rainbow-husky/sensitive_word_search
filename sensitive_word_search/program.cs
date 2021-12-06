using System.IO;
using System;
using CommandDotNet;
using System.Text;
using Serilog;

namespace sensitive_word_search
{
    public class program
    {
        static int Main(string[] args)
        {
            // 创建serilog日记全局静态实例
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

            //sensitive_word_search test = new sensitive_word_search("Hello World", "Hello cqf");
            // string test_string = "aaaaHello World Hello a World";
            // string test_file_path = "F://cqf//vs2021project//sensitive_word_search//test.txt";
            // string ouput = test.search_and_replace(test_file_path);
            return new AppRunner<searcher>().Run(args);
            // dataflow testd = new dataflow();
            // testd.nono(args);

            //Console.ReadLine(); // 等待输入，防止输出后丢失结果界面
        }
    }

    [Command(Description = "sensitive words searcher")]
    public class sensitive_word_search
    {
        // 敏感词查找（并替换）机
        public string sensitive_words;
        public string replace_words;
        
        public sensitive_word_search(string sensitive_words, string replace_words)
        {
            // 初始化函数
            this.sensitive_words = sensitive_words; // 敏感词
            this.replace_words = replace_words; // 替换敏感词的新词
            Console.WriteLine("sensitive_word_search instance has been inited!");
        }

        [Command(Description = "search sensitive words and replace them!")]
        public string search_and_replace(string input)
        {
            if(File.Exists(input))
            {
                // 输入为txt文件
                StreamReader sr = new StreamReader(input);  //文件读取流
                string line = null;
                // Path.GetFileNameWithoutExtension会得到不包含后缀的文件名，同时也不会再有前面的路径
                // 所以需要使用Path.GetDirectoryName先获得目录，再用Path.Combine将两者拼接
                string newfilepath = Path.Combine(Path.GetDirectoryName(input), Path.GetFileNameWithoutExtension(input) + "_replace.txt");
                FileStream fs = new FileStream(newfilepath, FileMode.Create);   // 创建文件
                StreamWriter sw = new StreamWriter(fs); //文件写入流
                
                while((line = sr.ReadLine()) != null)
                {
                    line = line.Replace(this.sensitive_words, this.replace_words);
                    //开始写入
                    sw.Write(line);
                }
                //清空缓冲区
                sw.Flush();
                fs.Flush();
                //关闭流
                sw.Close();
                fs.Close();

                Console.WriteLine("the replacements on the file finished!");
                return newfilepath;
            }
            else
            {
                // 输入为字符串
                string output = input.Replace(this.sensitive_words, this.replace_words);
                Console.WriteLine("the replacements on the string finished!");
                Console.WriteLine(output);
                return output; 
            }
        }

    }
}
