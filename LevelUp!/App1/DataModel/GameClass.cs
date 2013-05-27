using System;
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

        private int _level = 0;      

        public int Score
        {
            get { return _score; }
        }

        public bool IsLastLevel
        {
            get { return _level == LevelsCount; }
        }

        public GameLevel Level;
            
        public int MaxScore
        {
            get { return _levels.Count*GameLevel.MaxLevelScore*((int)this.DifficultyLevel+1); }
        }

        public Dificulty DifficultyLevel;

        private ObservableCollection<GameLevel> _levels;

        public int LevelsCount
        {
            get { return this._levels.Count; }
        }

        private AlphabetItem abcToTest;

        public GameClass(Dificulty difficultyLevel, string AlphabetID)
        {
            this.DifficultyLevel = difficultyLevel;
            this.abcToTest = ContentManager.GetAlphabet(AlphabetID);

            Random r = new Random();

            this._levels = new ObservableCollection<GameLevel>();

            int LevelCount = abcToTest.LetterItems.Count/2;

            for (int i = 0; i < LevelCount; i++)
            {
                this._levels.Add(new GameLevel(r, this.abcToTest, this.DifficultyLevel == Dificulty.Easy));
            }

            Level = this._levels[0];
            this._level = 0;
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
            this._level++;
            if (_level < LevelsCount)
                this.Level = _levels[this._level];            
            return true;
        }
        
        
    }

    public class GameLevel
    {
        private LetterItem letter;

        public string Question;

        public const int MaxLevelScore=3;

        private int Trial = 1;

        private int Answer;

        public ObservableCollection<WordItem> words;
        

        public GameLevel(Random rand, AlphabetItem alphabet, bool LoadWords)
        {
            this.Answer = rand.Next(3);
            this.words = new ObservableCollection<WordItem>();
            var letterAnswerNumber = rand.Next(alphabet.LetterItems.Count - 1);
            this.letter = alphabet.LetterItems[letterAnswerNumber];
            var res = new ResourceLoader();
            this.Question = String.Format(res.GetString("GameQuestion"),this.letter.Description);
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
                        while (letterIndex == letterAnswerNumber);


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
