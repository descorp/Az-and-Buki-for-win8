using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Windows.ApplicationModel.Resources;

namespace levelupspace.DataModel
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
        private int _score;

        private int _levelNum;
        public int LevelNum
        {
            get { return _levelNum; }
        }

        readonly Random _randomizer = new Random();

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
            get { return _level; }
        }
            
        public int MaxScore
        {
            get { return _levelsСount * GameLevel.MaxLevelScore * ((int)DifficultyLevel + 1); }
        }

        public Dificulty DifficultyLevel;

        private int _levelsСount;

        public int LevelsCount
        {
            get { return _levelsСount; }
        }

        private AlphabetItem _abcToTest;

        public GameClass(Dificulty difficultyLevel, string alphabetID, String dbPath)
        {
            DifficultyLevel = difficultyLevel;
            _abcToTest = ContentManager.GetAlphabet(alphabetID, dbPath);

            _levelsСount = _abcToTest.LetterItems.Count/2;

                _level = new GameLevel(_randomizer, _abcToTest, DifficultyLevel == Dificulty.Easy);           

            
            _levelNum = 0;
        }

        public  GameClass() 
        {
            _level = new GameLevel();
        }

        public void SaveGameState(Dictionary<String, object> pageState)
        {
            pageState["abcToTestID"] = _abcToTest.UniqueId;
            pageState["LevelNum"]=_levelNum;
            pageState["LevelsCount"]= _levelsСount;
            pageState["Score"]=_score;
            pageState["Difficulty"]=(int)DifficultyLevel;
            pageState["ans"] = _level.GetAns();
            pageState["trial"] = _level.GetTrial();
            pageState["words"] = _level.SerilializeWords();
            pageState["letter"] = _level.GetLetter();
        }

        public bool LoadGameState(Dictionary<String, object> pageState)
        {
            if (pageState == null) return false;
            if (!pageState.ContainsKey("abcToTestID")) return false;
            _abcToTest = ContentManager.GetAlphabet((string)pageState["abcToTestID"], DBconnectionPath.Local);
            if (!pageState.ContainsKey("LevelNum")) return false;
            _levelNum =(int) pageState["LevelNum"];
            if (!pageState.ContainsKey("LevelsCount")) return false;
            _levelsСount = (int)pageState["LevelsCount"];
            if (!pageState.ContainsKey("Score")) return false;
            _score = (int)pageState["Score"];
            if (!pageState.ContainsKey("Difficulty")) return false;
            DifficultyLevel = (Dificulty)pageState["Difficulty"];
            if (!pageState.ContainsKey("ans")) return false;
            _level.SetAns((int)pageState["ans"]);
            if (!pageState.ContainsKey("trial")) return false;
            _level.SetTrial((int)pageState["trial"]);
            if (!pageState.ContainsKey("words")) return false;
            _level.DeserializeWords((string)pageState["words"]);
            if (!pageState.ContainsKey("letter")) return false;
            _level.SetLetter((string)pageState["letter"]);
            return true;
        }

        /// <summary>
        /// Переходит на следующий уровень, если текущий пройден
        /// </summary>
        /// <returns>Возвращает false, если уровень не пройден</returns>

        public bool NextLevel(int answer)
        {           

            var result = Level.ValidateAnswer(answer);
            if (result < 0)
                return false;

            _score += result * ((int)DifficultyLevel + 1);
            _levelNum++;
            if (_levelNum < LevelsCount)
                _level = new GameLevel(_randomizer, _abcToTest, DifficultyLevel == Dificulty.Easy);             
            return true;
        }
        
        
    }

    public class GameLevel
    {
        private LetterItem _letter;
        public string GetLetter()
        {
            return _letter.UniqueId;
        }

        public void SetLetter(string uniqueID)
        {
            _letter = ContentManager.GetLetterItem(uniqueID, DBconnectionPath.Local);
            var res = new ResourceLoader();
            Question = String.Format(res.GetString("GameQuestion"), _letter.Description);
        }

        public string Question;        

        public const int MaxLevelScore=3;

        private int _trial = 1;

        public int GetTrial()
        {
            return _trial;
        }

        public void SetTrial(int trial)
        {
            _trial = trial;
        }

        private int _answer;

        public int GetAns()
        {
            return _answer;
        }

        public void SetAns(int ans)
        {
            _answer = ans;
        }

        public ObservableCollection<WordItem> Words;

        public string SerilializeWords()
        {
            return Words.Aggregate("", (current, word) => current + (word.UniqueId + "/"));
        }

        public void DeserializeWords(String wordsStr)
        {
            var ds = wordsStr.Split('/');
            Words = new ObservableCollection<WordItem>();
            foreach (var id in ds)
                if (id.Length>0)
                Words.Add(ContentManager.GetWordItem(id));
        }


        public GameLevel() { }

        public GameLevel(Random rand, AlphabetItem alphabet, bool loadWords)
        {
            _answer = rand.Next(3);
            Words = new ObservableCollection<WordItem>();
            int letterAnswerNumber;
            do
            {
                letterAnswerNumber = rand.Next(alphabet.LetterItems.Count - 1);

                _letter = alphabet.LetterItems[letterAnswerNumber];
            } while (_letter.WordItems.Count == 0);

            var res = new ResourceLoader();
            Question = String.Format(res.GetString("GameQuestion"), _letter.Description);

            for (var i = 0; i < 4; i++)
            {
                WordItem word;
                if (i == _answer)
                {
                    word = _letter.WordItems[rand.Next(_letter.WordItems.Count - 1)].Clone();                    
                }
                else
                {                     
                    do
                    {
                        int letterIndex;
                        do
                            letterIndex = rand.Next(alphabet.LetterItems.Count - 1);
                        while (letterIndex == letterAnswerNumber || alphabet.LetterItems[letterIndex].WordItems.Count==0);


                        var curletter = alphabet.LetterItems[letterIndex];
                        var wordIndex = rand.Next(curletter.WordItems.Count - 1);
                        word = curletter.WordItems[wordIndex].Clone();
                    }
                    while ((Words.Where(w=>w.ID==word.ID)).Count()!=0);                   
                }
                if (!loadWords) word.Image = null;
                Words.Add(word);
            }           
        }
        
        /// <summary>
        /// Возвращает очки за уровень , если ответ правильный и отриц. значение иначе
        /// </summary>
        public int ValidateAnswer(int userAnswer)
        {
            if (userAnswer == _answer)
            {
                return MaxLevelScore / _trial;
            }
            _trial++;
            return -1;
        }


    }
}
