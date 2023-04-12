using System.Collections.Generic;
using System.Drawing;

namespace VieweD.engine.common;

public static class PacketColors
{
    public static Color ColorBackIn { get; private set; }
    public static Color ColorBackOut { get; private set; }
    public static Color ColorBackUnknown { get; private set; }
    public static Color ColorBarIn { get; private set; }
    public static Color ColorBarOut { get; private set; }
    public static Color ColorBarUnknown { get; private set; }
    public static Color ColorFontIn { get; private set; }
    public static Color ColorFontOut { get; private set; }
    public static Color ColorFontUnknown { get; private set; }
    public static Color ColorSelectedFontIn { get; private set; }
    public static Color ColorSelectedFontOut { get; private set; }
    public static Color ColorSelectedFontUnknown { get; private set; }
    public static Color ColorSelectIn { get; private set; }
    public static Color ColorSelectOut { get; private set; }
    public static Color ColorSelectUnknown { get; private set; }
    public static Color ColorSyncIn { get; private set; }
    public static Color ColorSyncOut { get; private set; }
    public static Color ColorSyncUnknown { get; private set; }
    public static int PacketListStyle { get; private set; }
    public static List<Color> DataColors { get; } = new();

    public static void UpdateColorsFromSettings()
    {
        ColorBackIn = Properties.Settings.Default.ColBackIN;
        ColorBackOut = Properties.Settings.Default.ColBackOUT;
        ColorBackUnknown = Properties.Settings.Default.ColBackUNK;
        ColorBarIn = Properties.Settings.Default.ColBarIN;
        ColorBarOut = Properties.Settings.Default.ColBarOUT;
        ColorBarUnknown = Properties.Settings.Default.ColBarUNK;
        ColorFontIn = Properties.Settings.Default.ColFontIN;
        ColorFontOut = Properties.Settings.Default.ColFontOUT;
        ColorFontUnknown = Properties.Settings.Default.ColFontUNK;
        ColorSelectedFontIn = Properties.Settings.Default.ColSelectedFontIN;
        ColorSelectedFontOut = Properties.Settings.Default.ColSelectedFontOUT;
        ColorSelectedFontUnknown = Properties.Settings.Default.ColSelectedFontUNK;
        ColorSelectIn = Properties.Settings.Default.ColSelectIN;
        ColorSelectOut = Properties.Settings.Default.ColSelectOUT;
        ColorSelectUnknown = Properties.Settings.Default.ColSelectUNK;
        ColorSyncIn = Properties.Settings.Default.ColSyncIN;
        ColorSyncOut = Properties.Settings.Default.ColSyncOUT;
        ColorSyncUnknown = Properties.Settings.Default.ColSyncUNK;
        PacketListStyle = Properties.Settings.Default.PacketListStyle;

        // Default Field Colors
        var n = Properties.Settings.Default.ColFieldCount;
        DataColors.Clear();
        DataColors.Add(SystemColors.WindowText);
        if (n >= 2)
            DataColors.Add(Properties.Settings.Default.ColField1);

        if (n >= 3)
            DataColors.Add(Properties.Settings.Default.ColField2);

        if (n >= 4)
            DataColors.Add(Properties.Settings.Default.ColField3);

        if (n >= 5)
            DataColors.Add(Properties.Settings.Default.ColField4);

        if (n >= 6)
            DataColors.Add(Properties.Settings.Default.ColField5);

        if (n >= 7)
            DataColors.Add(Properties.Settings.Default.ColField6);

        if (n >= 8)
            DataColors.Add(Properties.Settings.Default.ColField7);

        if (n >= 9)
            DataColors.Add(Properties.Settings.Default.ColField8);

        if (n >= 10)
            DataColors.Add(Properties.Settings.Default.ColField9);

        if (n >= 11)
            DataColors.Add(Properties.Settings.Default.ColField10);

        if (n >= 12)
            DataColors.Add(Properties.Settings.Default.ColField11);

        if (n >= 13)
            DataColors.Add(Properties.Settings.Default.ColField12);

        if (n >= 14)
            DataColors.Add(Properties.Settings.Default.ColField13);

        if (n >= 15)
            DataColors.Add(Properties.Settings.Default.ColField14);

        if (n >= 16)
            DataColors.Add(Properties.Settings.Default.ColField15);
    }
}