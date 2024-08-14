using System;
using System.Text.RegularExpressions;

public class CPHInline
{
    public bool Execute()
    {
        string message = args["rawInput"].ToString();
        try
        {
            double result = EvaluateExpression(message);
            CPH.SendMessage($"Das Ergebnis ist: {result}", false);
        }
        catch (Exception e)
        {
            CPH.SendMessage($"Fehler: {e.Message}", false);
        }
        return true;
    }

    private double EvaluateExpression(string expression)
    {
        expression = Regex.Replace(expression, @"\s+", "");
        
        // Check for square root
        var sqrtMatch = Regex.Match(expression, @"^sqrt\((-?\d+\.?\d*)\)$");
        if (sqrtMatch.Success)
        {
            double num = Convert.ToDouble(sqrtMatch.Groups[1].Value);
            if (num < 0)
                throw new ArgumentException("Quadratwurzel aus negativer Zahl nicht möglich");
            return Math.Sqrt(num);
        }

        // Check for exponentiation
        var expMatch = Regex.Match(expression, @"^(-?\d+\.?\d*)\^(-?\d+\.?\d*)$");
        if (expMatch.Success)
        {
            double baseNum = Convert.ToDouble(expMatch.Groups[1].Value);
            double exponent = Convert.ToDouble(expMatch.Groups[2].Value);
            return Math.Pow(baseNum, exponent);
        }

        // Check for other operations
        var match = Regex.Match(expression, @"^(-?\d+\.?\d*)([\+\-\*/])(-?\d+\.?\d*)$");
        if (!match.Success)
        {
            throw new ArgumentException("Ungültiges Ausdrucksformat");
        }

        double num1 = Convert.ToDouble(match.Groups[1].Value);
        char operation = match.Groups[2].Value[0];
        double num2 = Convert.ToDouble(match.Groups[3].Value);

        switch (operation)
        {
            case '+': return num1 + num2;
            case '-': return num1 - num2;
            case '*': return num1 * num2;
            case '/':
                if (num2 == 0)
                    throw new DivideByZeroException("Division durch Null ist nicht erlaubt");
                return num1 / num2;
            default:
                throw new ArgumentException("Ungültiger Operator");
        }
    }
}