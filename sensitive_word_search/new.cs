using System.IO;
using System;
using System.Threading.Tasks.Dataflow;
using System.Threading.Tasks;

namespace TPL_sensitive_word_search
{
    public class dataflow
    {
        public void nono(string[] args)
        {
            TPL_sensitive_word_search project_test = new TPL_sensitive_word_search("Hello World", "Hello cqf");
            string line;
            string test_file_path = "F://cqf//vs2021project//sensitive_word_search//test.txt";
            Task.Run(() =>
            {                               
                using (StreamReader sr = new StreamReader(test_file_path))
                {
                    while ((line = sr.ReadLine()) != null)
                    {
                        project_test.transformBlock.Post(line);
                        
                    }
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
        }
    }
}
