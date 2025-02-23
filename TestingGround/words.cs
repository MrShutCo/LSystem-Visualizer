namespace TestingGround;

public class Words
{
    /*
     * Steps taken
     * 1. Downloaded the list, filtered down to unique letters, then max 2 vowels, then 1, and keeping word length betweeen 2 and 5
     * 2. Realized at each step that the brute force solution was much too large, so we needed to be smarter about it
     * 3. Checked how many words are in each length, figured it makes sense to sort the words by longest to shortest to prune search space as much as possible
     */
    
    public static void Solve()
    {
        var words = File.ReadLines("popular.txt");
        Console.WriteLine(words.Count());
        words = words.Where(FilterMaxTwoUniqueVowels);
        Console.WriteLine(words.Count());
        words = words.Where(FilterUniqueLetters);
        Console.WriteLine(words.Count());
        //
        //
        Console.WriteLine(words.Count(w => w.Length > 6));
        Console.WriteLine(words.Count(w => w.Length == 6));
        Console.WriteLine(words.Count(w => w.Length == 5));
        Console.WriteLine(words.Count(w => w.Length == 4));
        Console.WriteLine(words.Count(w => w.Length == 3));
        Console.WriteLine(words.Count(w => w.Length == 2));
        Console.WriteLine(words.Count(w => w.Length == 1));

        var w = words.OrderByDescending(w => w.Length);

        foreach (var word in words.Where(w => w.Length == 6))
        {
            Console.WriteLine(word);
        }

        string[] chosen = ["","","","","",""];
        HashSet<char> seen = [];
        
    }
    
    

    private static char[] vowels = ['a', 'e', 'i', 'o', 'u', 'y'];

    private static bool FilterUniqueLetters(string word)
    {
        HashSet<char> seen = [];
        return word.All(ch => seen.Add(ch));
    }

    private static bool FilterMaxTwoUniqueVowels(string word)
    {
        int[] vowelsFound = [0, 0, 0, 0, 0, 0, 0];
        int totalFound = 0;
        foreach (var t in word)
        {
            for (var j = 0; j < vowels.Length; j++)
            {
                if (t != vowels[j]) continue;
                vowelsFound[j]++;
                totalFound++;
                if (vowelsFound[j] > 1) return false;
                if (totalFound > 1) return false;
                break;
            }
        }

        return true;
    }
}