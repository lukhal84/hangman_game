using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

class Hangman {
  static void Main() {

    var totalNumberOfAttempts = 10;
    var records = new List<DataRecord>();

    var fileName = "countries_and_capitals.txt";
    var content = ReadAllText(fileName);

    Random random = new Random();
    Stopwatch stopwatch = new Stopwatch();

    var countryList = SplitContentToCountriesAndCapitals(content);

    WriteIntroduction(totalNumberOfAttempts);

    do {
      var randomNumber = random.Next(0, countryList.Count - 1);

      string guessedCountry = countryList[randomNumber].getCountry().ToUpper();
      string guessedCapital = countryList[randomNumber].getCapital().ToUpper();
      var printedWord = CreateDashesString(guessedCapital.Length);
      string notInWordLetters = "";

      AskUserToStart(guessedCountry, stopwatch);

      var userAttemptsLeft = totalNumberOfAttempts;
      bool isCapitalGuessed = false;
      int lettersToGuess = guessedCapital.Length;
      int currentScore = 0;

      while(userAttemptsLeft > 0) {
        Console.Clear();
        Console.WriteLine("What is the capital of: '{0}'?", guessedCountry);
        Console.WriteLine(printedWord);
        Console.WriteLine("Not in Word letters: '{0}'", notInWordLetters);
        Console.WriteLine("You have currently '{0}' more attempts!", userAttemptsLeft);

        var option = ChooseGuessOption();
        if(option == GuessOption.Letter) {
          if(RequestLetterAndCheckIfExists(guessedCapital, ref printedWord, ref notInWordLetters, ref lettersToGuess)) {
            if(lettersToGuess == 0) {
              isCapitalGuessed = true;
            }
          }
          else {
            userAttemptsLeft--;
          }
        }
        else if(option == GuessOption.WholeWord) {
          if(RequestWordAndCheckIfCorrect(guessedCapital, ref printedWord)) {
            isCapitalGuessed = true;
          }
          else {
            userAttemptsLeft = userAttemptsLeft - 2;
          }
        }
        else {
          continue;
        }

        currentScore++;

        if(isCapitalGuessed || userAttemptsLeft < 1) {
          stopwatch.Stop();
          AddHighscore(guessedCountry, guessedCapital, isCapitalGuessed, currentScore, stopwatch, ref records);
          break;
        }
      }

    }
    while(UserWantsPlayAgain());

    PrintHighscoreList(records);
  }
  
  public static string ReadAllText(string path)
  {
  	return File.ReadAllText(path);
  }
  
  static List<CountriesCapitals> SplitContentToCountriesAndCapitals(string content) 
  {
      var capitals = new List<CountriesCapitals>();
      
      string[] lines = content.Split('\n');

      foreach (string line in lines) {
        string[] pair = line.Split('|');
        capitals.Add(new CountriesCapitals(pair[0].Trim(), pair[1].Trim()));
      }

      return capitals;
  }
  static string CreateDashesString(int dash_amount) {
    StringBuilder stringBuilder = new StringBuilder();

    for(int i = 0; i < dash_amount; i++) {
        stringBuilder.Append("_");
        stringBuilder.Append(" ");
    }
    return stringBuilder.ToString();
  }
  static void WriteIntroduction(int numberOfAttempts) 
  {
      Console.WriteLine("Hello my friend!");
      Console.WriteLine("Let's play a game.");
      Console.WriteLine("I'll give you a country and you have to guess its capital.");
      Console.WriteLine("You can enter one letter or write the whole word.");
      Console.WriteLine("Remember! You can make a better score entering the whole word, however in case of mistake you will lose 2 points instead of 1.");
      Console.WriteLine("You have {0} attempts!", numberOfAttempts);
  }

  static void AskUserToStart(string country, Stopwatch stopwatch) {
      Console.WriteLine("Press any key to start.");

      while(true) {
        var input = Console.ReadLine();

        if (string.IsNullOrEmpty(input))
        {
          Console.WriteLine("You haven't entered an input. Try again.");
          continue;
        } 
        else {
          stopwatch.Start();
          break;
        }
      }

  }

  static string AskForUsername() {
      Console.WriteLine("If you want to add your score to list, please enter your name. Otherwise press enter.");

      while(true) {
        var input_name = Console.ReadLine();

        if (string.IsNullOrEmpty(input_name))
        {
          Console.WriteLine("You haven't entered an input. Result won't be added.");
          break;
        } 
        else {
          return input_name;
        }
      }
      return "";

  }

  static bool UserWantsPlayAgain() {
      Console.WriteLine("Do you want to play again? (type 'yes' if so)");
      var result = Console.ReadLine();

      if("YES" == result.ToUpper()) {
        return true;
      }
      return false;
  }

  static GuessOption ChooseGuessOption() {
      Console.WriteLine("Press 1 to guess the letter, 2 to guess the whole word.");
      var input = Console.ReadLine();
    
      if (string.IsNullOrEmpty(input))
      {
          Console.WriteLine("You haven't entered an input.  Try once again");
          return GuessOption.InvalidValue;
      }
      else {
        int option;
        bool parseSuccess = int.TryParse(input, out option);
        if (parseSuccess)
          if(option == (int)GuessOption.Letter) {
              return GuessOption.Letter;
          }
          else if (option == (int)GuessOption.WholeWord) {
              return GuessOption.WholeWord;
          }
          else {
            Console.WriteLine("This is wrong number. Try once again");
            return GuessOption.InvalidValue;
          }
        else
          Console.WriteLine("This is not a number.  Try once again");
          return GuessOption.InvalidValue;
        }
  }

  static bool RequestLetterAndCheckIfExists(string guessedWord, ref string printedWord, ref string notInWordLetters, ref int lettersToGuess) {
      bool result = false;
      
      Console.WriteLine("Enter the letter...");
      var input = Console.ReadLine();

      char letter;
      bool parseSuccess = char.TryParse(input, out letter);
      string newLetter = letter.ToString().ToUpper();
      if (parseSuccess) {
        int startIndex = 0;
        int returnIndex = 0;
        while ( returnIndex != -1) {
          StringBuilder builder = new StringBuilder(printedWord);
          returnIndex = guessedWord.IndexOf(newLetter, startIndex, guessedWord.Length - startIndex);
          if(returnIndex != -1) {
            builder.Remove(returnIndex*2,1);
            builder.Insert(returnIndex*2, newLetter);
            printedWord = builder.ToString();
            startIndex = returnIndex + 1;
            lettersToGuess--;
            result = true;
          }
        }
        if(result == false && notInWordLetters.IndexOf(newLetter, 0, notInWordLetters.Length) == -1){
          notInWordLetters += newLetter;
        }
      }
        return result;
  }

  static bool RequestWordAndCheckIfCorrect(string guessedWord, ref string printedWord) {
      bool result = false;
      
      Console.WriteLine("Enter the word...");
      var word = Console.ReadLine();

      if(guessedWord == word.ToUpper()) {
        result = true;
        printedWord = guessedWord;
      }
      return result;
    }

  static void AddHighscore(string guessedCountry, string guessedCapital, bool isCapitalGuessed, int currentScore, Stopwatch stopwatch, ref List<DataRecord> records) {
    DateTime localDate = DateTime.Now;

    if(isCapitalGuessed) {
      Console.WriteLine("Congratulations! You have guessed the capital!");
      Console.WriteLine("The capital of '{0}' is '{1}'!", guessedCountry, guessedCapital);
      Console.WriteLine("The final score is {0} and you have used {1} attemps", isCapitalGuessed, currentScore);
      Console.WriteLine("It took you {0} seconds", stopwatch.ElapsedMilliseconds / 1000);

      string username = AskForUsername();
      DataRecord record = new DataRecord(username, localDate.ToString(), stopwatch.ElapsedMilliseconds / 1000, guessedCapital);
      records.Add(record);
    }
    else {
      Console.WriteLine("Unfortunetely you missed! You have not guessed the capital!");
      Console.WriteLine("The capital of '{0}' is '{1}'!", guessedCountry, guessedCapital);
      Console.WriteLine("The final score is {0} and you have used {1} attemps", isCapitalGuessed, currentScore);
      Console.WriteLine("It took you {0} seconds", stopwatch.ElapsedMilliseconds / 1000);   
    }
  }

  static void PrintHighscoreList(List<DataRecord> records) { 
    records.Sort((r1, r2) => r1.getTime().CompareTo(r2.getTime()));
    Console.WriteLine("-----------------------------------------------------------------------------------");
    foreach(var element in records) {
      Console.WriteLine("| Date: {0} | User: {1} | Guessing Time {2} | Capital {3} |", element.getDate(), element.getUsername(), element.getTime(), element.getCapital());
    }
    Console.WriteLine("-----------------------------------------------------------------------------------");
  }
}