using System.ComponentModel.DataAnnotations;

public class NumericValidation : ValidationAttribute
{
    public override bool IsValid(object? value)
    {
        if (value == null)
            return true;

        if (int.TryParse(value.ToString(), out int intValue))
            return true;

        return false;
    }
}
