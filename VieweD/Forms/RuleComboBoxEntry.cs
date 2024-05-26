namespace VieweD.Forms;

public class RuleComboBoxEntry(string display, string value)
{
    public string Display { get; set; } = display;
    public string Value { get; set; } = value;

    public RuleComboBoxEntry() : this(string.Empty, string.Empty)
    {
        //
    }
}