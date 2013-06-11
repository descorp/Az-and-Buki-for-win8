using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Windows.ApplicationModel.Resources;

namespace levelupspace
{
    public class GameItem : ABCItem
    {
        public GameItem(String uniqueId, String title, String imagePath, String description)
            : base(uniqueId, title, imagePath, description)
        {
        }
    }

    public class DifficultyItem : ABCItem
    {
        public DifficultyItem(String uniqueId, String title, String imagePath, String description)
            : base(uniqueId, title, imagePath, description)
        {
        }
    }   
    
    
    public enum Dificulty { Easy, Medium, Genius };
   
    

    public class GameClass
    {
        private int _score=0;

        private int _levelNum = 0;
        public int LevelNum
        {
            get { return this._levelNum; }
        }

        Random randomizer = new Random();

        public int Score
        {
            get { return _score; }
        }

        public bool IsLastLevel
        {
            get { return _levelNum == LevelsCount; }
        }

        private GameLevel _level;
        public GameLevel Level
        {
            get { return this._level; }
        }
            
        public int MaxScore
        {
            get { return _levelsСount * GameLevel.MaxLevelScore * ((int)this.DifficultyLevel + 1); }
        }

        public Dificulty DifficultyLevel;

        private int _levelsСount=0;

        public int LevelsCount
        {
            get { return this._levelsСount; }
        }

        private AlphabetItem abcToTest;

        public GameClass(Dificulty difficultyLevel, string AlphabetID, String DBPath)
        {
            this.DifficultyLevel = difficultyLevel;
            this.abcToTest = ContentManager.GetAlphabet(AlphabetID, DBPath);

            this._levelsСount = abcToTest.LetterItems.Count/2;

                this._level = new GameLevel(this.randomizer, this.abcToTest, this.DifficultyLevel == Dificulty.Easy);           

            
            this._levelNum = 0;
        }

        public  GameClass() 
        {
            this._level = new GameLevel();
        }

        public void SaveGameState(Dictionary<String, object> pageState)
        {
            pageState["abcToTestID"] = this.abcToTest.UniqueId;
            pageState["LevelNum"]=this._levelNum;
            pageState["LevelsCount"]= this._levelsСount;
            pageState["Score"]=this._score;
            pageState["Difficulty"]=(int)this.DifficultyLevel;
            pageState["ans"] = this._level.GetAns();
            pageState["trial"] = this._level.GetTrial();
            pageState["words"] = this._level.SerilializeWords();
            pageState["letter"] = this._level.GetLetter();
        }

        public bool LoadGameState(Dictionary<String, object> pageState)
        {
            if (pageState == null) return false;
            if (!pageState.ContainsKey("abcToTestID")) return false;
            abcToTest = ContentManager.GetAlphabet((string)pageState["abcToTestID"], DBconnectionPath.Local);
            if (!pageState.ContainsKey("LevelNum")) return false;
            this._levelNum =(int) pageState["LevelNum"];
            if (!pageState.ContainsKey("LevelsCount")) return false;
            this._levelsСount = (int)pageState["LevelsCount"];
            if (!pageState.ContainsKey("Score")) return false;
            this._score = (int)pageState["Score"];
            if (!pageState.ContainsKey("Difficulty")) return false;
            this.DifficultyLevel = (Dificulty)pageState["Difficulty"];
            if (!pageState.ContainsKey("ans")) return false;
            this._level.SetAns((int)pageState["ans"]);
            if (!pageState.ContainsKey("trial")) return false;
            this._level.SetTrial((int)pageState["trial"]);
            if (!pageState.ContainsKey("words")) return false;
            this._level.DeserializeWords((string)pageState["words"]);
            if (!pageState.ContainsKey("letter")) return false;
            this._level.SetLetter((string)pageState["letter"]);
            return true;
        }

        /// <summary>
        /// Переходит на следующий уровень, если текущий пройден
        /// </summary>
        /// <returns>Возвращает false, если уровень не пройден</returns>

        public bool NextLevel(int Answer)
        {           

            var result = this.Level.ValidateAnswer(Answer);
            if (result < 0)
                return false;

            this._score += result * ((int)this.DifficultyLevel + 1);
            this._levelNum++;
            if (_levelNum < LevelsCount)
                this._level = new GameLevel(this.randomizer, this.abcToTest, this.DifficultyLevel == Dificulty.Easy);             
            return true;
        }
        
        
    }

    public class GameLevel
    {
        private LetterItem letter;
        public string GetLetter()
        {
            return letter.UniqueId;
        }

        public void SetLetter(string uniqueID)
        {
            this.letter = ContentManager.GetItem(uniqueID, DBconnectionPath.Local);
            var res = new ResourceLoader();
            this.Question = String.Format(res.GetString("GameQuestion"), this.letter.Description);
        }

        public string Question;        

        public const int MaxLevelScore=3;

        private int Trial = 1;

        public int GetTrial()
        {
            return this.Trial;
        }

        public void SetTrial(int trial)
        {
            this.Trial = trial;
        }

        private int Answer;

        public int GetAns()
        {
            return this.Answer;
        }

        public void SetAns(int ans)
        {
            this.Answer = ans;
        }

        public ObservableCollection<WordItem> words;

        public string SerilializeWords()
        {
            string ans = "";
            foreach (WordItem word in words)
                ans += word.UniqueId+"/";
            return ans;
        }

        public void DeserializeWords(String wordsStr)
        {
            var IDs = wordsStr.Split('/');
            this.words = new ObservableCollection<WordItem>();
            foreach (string id in IDs)
                if (id.Length>0)
                words.Add(ContentManager.GetWordItem(id));
        }


        public GameLevel() { }

        public GameLevel(Random rand, AlphabetItem alphabet, bool LoadWords)
        {
            this.Answer = rand.Next(3);
            this.words = new ObservableCollection<WordItem>();
            var letterAnswerNumber = 0;
            do
            {
                letterAnswerNumber = rand.Next(alphabet.LetterItems.Count - 1);

                this.letter = alphabet.LetterItems[letterAnswerNumber];
            } while (letter.WordItems.Count == 0);

            var res = new ResourceLoader();
            this.Question = String.Format(res.GetString("GameQuestion"), this.letter.Description);
            
            WordItem word;

            for (int i = 0; i < 4; i++)
            {
                
                if (i == this.Answer)
                {
                    word = letter.WordItems[rand.Next(letter.WordItems.Count - 1)].Clone();                    
                }
                else
                {                     
                    do
                    {
                        int letterIndex;
                        do
                            letterIndex = rand.Next(alphabet.LetterItems.Count - 1);
                        while (letterIndex == letterAnswerNumber || alphabet.LetterItems[letterIndex].WordItems.Count==0);


                        var Curletter = alphabet.LetterItems[letterIndex];
                        var wordIndex = rand.Next(Curletter.WordItems.Count - 1);
                        word = Curletter.WordItems[wordIndex].Clone();
                    }
                    while ((words.Where(w=>w.ID==word.ID)).Count()!=0);                   
                }
                if (!LoadWords) word.Image = null;
                words.Add(word);
            }           
        }
        
        /// <summary>
        /// Возвращает очки за уровень , если ответ правильный и отриц. значение иначе
        /// </summary>
        public int ValidateAnswer(int UserAnswer)
        {
            if (UserAnswer == this.Answer)
            {
                return GameLevel.MaxLevelScore / this.Trial;
            }
            else
            {
                this.Trial++;
                return -1;
            }
        }


    }
}
