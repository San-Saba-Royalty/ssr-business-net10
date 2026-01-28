using System;
using System.Text;

namespace SSRBusiness.BusinessClasses;

/// <summary>
/// Converts numeric values to their English word representation.
/// Used for legal documents requiring spelled-out dollar amounts.
/// </summary>
public static class NumberToWordConverter
{
    private static readonly string[] Ones =
    {
        "", "One", "Two", "Three", "Four", "Five", "Six", "Seven", "Eight", "Nine",
        "Ten", "Eleven", "Twelve", "Thirteen", "Fourteen", "Fifteen", "Sixteen",
        "Seventeen", "Eighteen", "Nineteen"
    };

    private static readonly string[] Tens =
    {
        "", "", "Twenty", "Thirty", "Forty", "Fifty", "Sixty", "Seventy", "Eighty", "Ninety"
    };

    private static readonly string[] Thousands =
    {
        "", "Thousand", "Million", "Billion", "Trillion"
    };

    /// <summary>
    /// Converts a decimal dollar amount to words.
    /// Example: 1234.56 -> "One Thousand Two Hundred Thirty-Four Dollars and 56/100"
    /// </summary>
    public static string ConvertDollars(decimal amount)
    {
        if (amount == 0) return "Zero Dollars and 00/100";

        var isNegative = amount < 0;
        amount = Math.Abs(amount);

        var dollars = (long)Math.Floor(amount);
        var cents = (int)Math.Round((amount - dollars) * 100);

        var sb = new StringBuilder();

        if (isNegative) sb.Append("Negative ");

        if (dollars == 0)
        {
            sb.Append("Zero");
        }
        else
        {
            sb.Append(ConvertWholeNumber(dollars));
        }

        sb.Append(dollars == 1 ? " Dollar" : " Dollars");
        sb.Append($" and {cents:D2}/100");

        return sb.ToString();
    }

    /// <summary>
    /// Converts a decimal amount to words without currency formatting.
    /// Example: 1234.56 -> "One Thousand Two Hundred Thirty-Four and 56/100"
    /// </summary>
    public static string ConvertDecimal(decimal amount)
    {
        if (amount == 0) return "Zero";

        var isNegative = amount < 0;
        amount = Math.Abs(amount);

        var wholePart = (long)Math.Floor(amount);
        var decimalPart = amount - wholePart;

        var sb = new StringBuilder();

        if (isNegative) sb.Append("Negative ");

        if (wholePart == 0)
        {
            sb.Append("Zero");
        }
        else
        {
            sb.Append(ConvertWholeNumber(wholePart));
        }

        if (decimalPart > 0)
        {
            // Get decimal digits as string
            var decimalStr = decimalPart.ToString("0.##########").TrimStart('0', '.');
            sb.Append(" and ");
            sb.Append(decimalStr);
            sb.Append("/");
            sb.Append(new string('0', decimalStr.Length).Replace("0", "").PadLeft(decimalStr.Length, '1')[0..1]);
            // Simpler: append the fractional representation
        }

        return sb.ToString();
    }

    /// <summary>
    /// Converts a whole number to words.
    /// Example: 1234 -> "One Thousand Two Hundred Thirty-Four"
    /// </summary>
    public static string ConvertWholeNumber(long number)
    {
        if (number == 0) return "Zero";
        if (number < 0) return "Negative " + ConvertWholeNumber(-number);

        var words = new StringBuilder();
        var groupIndex = 0;

        while (number > 0)
        {
            var group = (int)(number % 1000);
            number /= 1000;

            if (group > 0)
            {
                var groupWords = ConvertGroup(group);
                if (groupIndex > 0)
                {
                    groupWords += " " + Thousands[groupIndex];
                }

                if (words.Length > 0)
                {
                    words.Insert(0, groupWords + " ");
                }
                else
                {
                    words.Append(groupWords);
                }
            }

            groupIndex++;
        }

        return words.ToString().Trim();
    }

    private static string ConvertGroup(int number)
    {
        var result = new StringBuilder();

        var hundreds = number / 100;
        var remainder = number % 100;

        if (hundreds > 0)
        {
            result.Append(Ones[hundreds]);
            result.Append(" Hundred");
            if (remainder > 0) result.Append(' ');
        }

        if (remainder > 0)
        {
            if (remainder < 20)
            {
                result.Append(Ones[remainder]);
            }
            else
            {
                var tens = remainder / 10;
                var ones = remainder % 10;

                result.Append(Tens[tens]);
                if (ones > 0)
                {
                    result.Append('-');
                    result.Append(Ones[ones]);
                }
            }
        }

        return result.ToString();
    }

    /// <summary>
    /// Converts an integer to ordinal words.
    /// Example: 1 -> "First", 2 -> "Second", 21 -> "Twenty-First"
    /// </summary>
    public static string ConvertToOrdinal(int number)
    {
        if (number <= 0) return number.ToString();

        // Special cases for 1-19
        string[] ordinalOnes =
        {
            "", "First", "Second", "Third", "Fourth", "Fifth", "Sixth", "Seventh", "Eighth", "Ninth",
            "Tenth", "Eleventh", "Twelfth", "Thirteenth", "Fourteenth", "Fifteenth", "Sixteenth",
            "Seventeenth", "Eighteenth", "Nineteenth"
        };

        string[] ordinalTens =
        {
            "", "", "Twentieth", "Thirtieth", "Fortieth", "Fiftieth", "Sixtieth", "Seventieth", "Eightieth", "Ninetieth"
        };

        if (number < 20)
        {
            return ordinalOnes[number];
        }

        if (number < 100)
        {
            var tens = number / 10;
            var ones = number % 10;

            if (ones == 0)
            {
                return ordinalTens[tens];
            }

            return Tens[tens] + "-" + ordinalOnes[ones];
        }

        // For larger numbers, convert normally and add ordinal suffix
        var words = ConvertWholeNumber(number);
        
        // Replace last word with ordinal form
        if (words.EndsWith("One")) return words[..^3] + "First";
        if (words.EndsWith("Two")) return words[..^3] + "Second";
        if (words.EndsWith("Three")) return words[..^5] + "Third";
        if (words.EndsWith("Four")) return words[..^4] + "Fourth";
        if (words.EndsWith("Five")) return words[..^4] + "Fifth";
        if (words.EndsWith("Six")) return words[..^3] + "Sixth";
        if (words.EndsWith("Seven")) return words[..^5] + "Seventh";
        if (words.EndsWith("Eight")) return words[..^5] + "Eighth";
        if (words.EndsWith("Nine")) return words[..^4] + "Ninth";
        if (words.EndsWith("Ten")) return words[..^3] + "Tenth";
        if (words.EndsWith("Eleven")) return words[..^6] + "Eleventh";
        if (words.EndsWith("Twelve")) return words[..^6] + "Twelfth";
        if (words.EndsWith("y")) return words[..^1] + "ieth";
        
        return words + "th";
    }
}
