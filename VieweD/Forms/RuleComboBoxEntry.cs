namespace VieweD.Forms;

public class RuleComboBoxEntry
{
    public string Display { get; set; }
    public string Value { get; set; }

    public RuleComboBoxEntry()
    {
        Display = string.Empty;
        Value = string.Empty;
    }

    public RuleComboBoxEntry(string display, string value)
    {
        Display = display;
        Value = value;
    }
}