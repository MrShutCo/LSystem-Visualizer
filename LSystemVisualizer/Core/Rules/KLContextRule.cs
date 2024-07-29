namespace ShutCo.UI.Core.Rules;

public class KLContextRule : IProductionRule
{
    public string StrictPredecessor { get; }
    public List<string> Successor { get; }
    private List<string> _leftContext;
    private List<string> _rightContext;
    
    public KLContextRule(string leftContext, string strictPredecessor, string rightContext, string successor)
    {
        StrictPredecessor = strictPredecessor;
        _leftContext = LSystem.StringToWord(leftContext);
        _rightContext = LSystem.StringToWord(rightContext);
        Successor = LSystem.StringToWord(successor);
    }
    
    public List<string> ApplyRule(ILSystem system, List<string> word)
    {
        List<string> newWord = [];
        newWord.AddRange(word[.._leftContext.Count]);
        
        for (var i = _leftContext.Count; i < word.Count-_rightContext.Count; i++)
        {
            if (word[i] == StrictPredecessor && SatisfiesLeftContext(system, word, i) && SatisfiesRightContext(system, word, i))
            {
                Console.WriteLine(this + " applied");
                newWord.AddRange(Successor);
            }
            else
            {
                newWord.Add(word[i]);
            }
        }
        newWord.AddRange(word[^_rightContext.Count..]);
        return newWord;
    }

    public override string ToString()
    {
        var s = _leftContext.Aggregate("", (current, left) => current + (left + " < "));
        s += StrictPredecessor;
        s = _rightContext.Aggregate(s, (current, right) => current + (" > " + right));
        s += " \u2192 " + LSystem.WordToString(Successor);
        return s;
    }

    private bool SatisfiesLeftContext(ILSystem system, List<string> word, int i)
    {
        int contextChecked = 0;
        int index = i-1;

        while (index >= 0 && contextChecked < _leftContext.Count)
        {
            var checkedVal = _leftContext[_leftContext.Count - 1 - contextChecked];
            // Wildstar check counts as a context checked
            if (checkedVal == "*" || checkedVal == word[index])
            {
                index--;
                contextChecked++;
                continue;
            }

            // Ignore particular letters
            if (!system.Ignore.Contains(word[index])) return false;
            
            index--;
        }

        return contextChecked == _leftContext.Count;
    }
    
    private bool SatisfiesRightContext(ILSystem system, List<string> word, int i)
    {
        int contextChecked = 0;
        int index = i+1;

        while (index < word.Count && contextChecked < _rightContext.Count)
        {
            var checkedVal = _rightContext[contextChecked];
            // Wildstar check counts as a context checked
            if (checkedVal == "*" || checkedVal == word[index])
            {
                index++;
                contextChecked++;
                continue;
            }

            // Ignore particular letters
            if (!system.Ignore.Contains(word[index])) return false;
            
            index++;
        }

        return contextChecked == _rightContext.Count;
    }

    public bool CanApply(ILSystem system, List<string> word, int index)
    {
        return word[index] == StrictPredecessor && SatisfiesLeftContext(system, word, index) &&
               SatisfiesRightContext(system, word, index);
    }

    public List<string> Apply()
    {
        return Successor;
    }
}