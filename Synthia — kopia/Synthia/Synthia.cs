using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace Synthia
{
    class Synthia
    {
        public string TextInput { set; get; }
        public string TextOutput { set; get; }
        public bool useVoice = false;
        private string lastQuestion = "";

        Timer respondTimer { get; }
        int commands = 0;
        string memoryFile = @"memory.dat";
        string[][] memoryQuestions;
        string[] memoryAnwsers;
        List<string> synthiaQuestions = new List<string>(); //pytania Synthi 
        List<string> questionsAsked = new List<string>();
        int askTime = 0;

        public Synthia()
        {
            Memory memory = new Memory(memoryFile);         
            LoadCommands(ref commands);
            SetTimer(respondTimer);
            synthiaQuestions = Questions.Load(@"questions.dat");
        }
        private void LoadCommands(ref int command)
        {
            Memory memory = new Memory(memoryFile);
            if (memory.KeyExists("commands", "command"))
            {                   
                commands =  int.Parse(memory.Read("commands", "command"));
                memoryQuestions = new string[commands][];
                memoryAnwsers = new string[commands];
                for (int i = 0; i < commands; i++)
                {
                    string str = memory.Read((i+1).ToString(), "questions");
                    var question = SplitText(str, new List<string>());
                    memoryQuestions[i] = question;
                    memoryAnwsers[i] = memory.Read((i + 1).ToString(), "anwsers");
                }
            }
        }

        private void SetTimer(Timer timer)
        {
            timer = new Timer(1000);
            timer.AutoReset = true;
            timer.Enabled = true;
        }
        private void WriteToMemory(string anwser, string question)
        {
            commands += 1;
            Memory memory = new Memory(memoryFile);
            memory.Write(commands.ToString(), anwser, "anwsers");
            memory.Write(commands.ToString(), question, "questions");
            memory.Write("commands", commands.ToString(), "command");
        }

        public string Response(string text)
        {
            if (text == "")
            {
                return "Write anything...";
            }

            if (text == "!!") //Jeśli chce dać inną odpowiedź
            {
                Console.Write("Daj odpowiedź: ");
                string anw = Console.ReadLine();
                SaveAnwser(lastQuestion, anw);
                return "Zapytaj o coś innego.";
            }
            lastQuestion = text;
            text = StringToLower(text);
            bool userAsks = text[text.Length - 1] == '?' ? true : Probability(0.35);
            string response = "";
            askTime++;
            //TEN ETAP SZUKA NAJBARDZIEJ TRAFNYCH ODPOWIEDZI SPOŚRÓD CAŁEJ PAMIĘCI
            ShuffleQuestions(memoryQuestions, memoryAnwsers);
            var question = SplitText(text, new List<string>());

            string anwser = SearchInMemory(question, memoryAnwsers, memoryQuestions);
            ///KONIEC ETAPU WYSZUKIWANIA ODPOWIEDZI

            if (anwser != "")
            {
                response = anwser;
            }
            else //Zapisywanie do pamięci
            {
                string say = "I do not know the anwser to this question, please tell me how should I response.";
                Console.WriteLine(say);
                anwser = Console.ReadLine();
                SaveAnwser(lastQuestion, anwser);
            }
            if (userAsks && !string.IsNullOrEmpty(response) && response[response.Length-1] != '?')
            {
                askTime = 0;
            }       
            return response;
        }

        public string AskQuestion()
        {
            if (synthiaQuestions.Count > 0)
            {
                var rnd = new Random();
                int q = rnd.Next(0, synthiaQuestions.Count);
                string response = synthiaQuestions[q];
                synthiaQuestions.RemoveAt(q);
                return " " + response;
            }
            else
            {
                return "I have nothing more to say.";
            }
        }

        private void SaveAnwser(string question, string anwser)
        {
            var list = SplitText(question, new List<string>());
            string quest = ListToString(list);
            quest = quest.ToLower();
            WriteToMemory(anwser, quest);
            LoadCommands(ref commands);
        }
        void ShuffleQuestions(string[][] questions, string[] anwsers)
        {
            int n = questions.Length;
            Random rnd = new Random();
            while (n > 1)
            {
                n--;
                int k = rnd.Next(n + 1);
                string[] value = questions[k];
                questions[k] = questions[n];
                questions[n] = value;

                string str = anwsers[k];
                anwsers[k] = anwsers[n];
                anwsers[n] = str;
            }
            memoryQuestions = questions;
            memoryAnwsers = anwsers;
        }

        private string ListToString(string[] list)
        {
            string word = "";
            foreach (string str in list)
            {
                if (word == "")
                {
                    word += str;
                }
                else
                {
                    word += " " + str;
                }
            }
            return word;
        }

        private string[] SplitText(string text, List<string> list)
        {
            string word = "";
            for (int i = 0; i < text.Length; i++)
            {
                if (text[i] == ' ' || (i+1 == text.Length))
                {
                    if ((i + 1 == text.Length)) word += text[i];
                    list.Add(word);
                    word = "";
                }
                else
                {
                    word += text[i];
                }
            }
            try
            {
                for (int i = 0; i < list.Count; i++)
                {
                    string str = list[i];
                    while (str[str.Length - 1] == '.' || str[str.Length - 1] == '?' || str[str.Length - 1] == ',' 
                        || str[str.Length - 1] == '!') //póki jest niewlasciwy znak
                    {
                        str = str.Remove(str.Length - 1);
                        list[i] = str;
                    }
                }
            }
            catch (Exception ex)
            {
                return new string[] { "dd" };
            }
            return list.ToArray();
        }


        private string StringToLower(string a)
        {
            a = a.ToLower();
            char[] b = a.ToCharArray();
            return new string(b);
        }


        private string SearchInMemory(string[] question, string[] anwsers, string[][] memory)
        {
            int counterUltimate = 0;
            string anwser = "";
            
            var questionAskedStr = String.Join(" ", question);
            foreach (string q in questionsAsked)
            {
                if (q == questionAskedStr && q.Length > 10)
                {
                    return "Powtarzasz się, już mówiłam"; //"You are repeating yourself.";
                }
            }
            for (int i = 0; i < memory.Length; i++)
            {
                int counter = 0;
                foreach (string str in memory[i])
                {                   
                    foreach(string word in question)
                    {
                        //Console.WriteLine(word + " : " + str);
                        if (word == str)
                        {
                            counter++;                            
                        }
                    }
                    if (counter >= counterUltimate)
                    {                        
                        if (counter > counterUltimate) //Jeśli ilość wyrazów w pytaniu jest bardziej celna od obecnie znalezionej?
                        {
                            counterUltimate = counter;
                           // Console.WriteLine(ListToString(memoryQuestions[i]) + " : " + memoryAnwsers[i]);
                            //Console.WriteLine(counterUltimate + " : " + memoryQuestions[i].Length);
                            if((anwsers[i].Length < anwser.Length || string.IsNullOrEmpty(anwser)))
                                anwser = anwsers[i];
                        }
                        else if (counter == memory[i].Length) //Jeśli ilość wyrazów jest taka sama, ale wszystkie w liście odpowiadają wszystkim w pytaniu -> Tzn. że pytanie jest najbardziej celne, lepiej być nie może
                        {
                            counterUltimate = counter;
                            //Console.WriteLine(ListToString(memoryQuestions[i]) + " : " + memoryAnwsers[i]);
                            //Console.WriteLine(counterUltimate + " : " + memoryQuestions[i].Length);
                            anwser = anwsers[i];                           
                        }

                    }
                }
            }
            
            if (question.Length < 3 && anwser =="")
            {
                var rnd = new Random();
                int q = rnd.Next(0, 5);
                switch(q)
                {
                    case 0:
                    case 1:
                    case 2:
                        anwser = "Ok"; //"Ok.";
                        break;
                    case 3:
                        anwser = "Yhm";//"Yhm.";
                        break;
                    case 4:
                        anwser = "To dobrze"; //"Good.";
                        break;
                    case 5:
                        anwser = "Rozumiem"; //"Right.";
                        break;

                }
            }
            if (anwser == "")
                return "Co?";//"What?";

            questionsAsked.Add(questionAskedStr);
            return anwser;
        }

        public static bool Probability(double probability)
        {
            Random rnd = new Random();
            return rnd.NextDouble() < probability ? true : false;
        }

    }

}

