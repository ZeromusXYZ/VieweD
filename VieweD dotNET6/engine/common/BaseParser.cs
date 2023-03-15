namespace VieweD.engine.common;

public class BaseParser : IComparable<BaseParser>
{
    public virtual string Name => "Base Parser";
    public virtual string Description => "";
    public virtual string DefaultRulesFile => "rules.xml";
    public ViewedProjectTab? ParentProject { get; set; }
    public RulesReader? Rules { get; protected set; }
    protected List<string> SupportedReaders { get; set; } = new List<string>();
    public virtual int PacketIdMinimum => 0;
    public virtual int PacketIdMaximum => 0xFFFF;
    public virtual int PacketCompressionLevelMaximum => 0;
    public virtual bool AllowSyncSearch => false;

    public BaseParser(ViewedProjectTab parentProject)
    {
        ParentProject = parentProject;
        Rules = new RulesReader(ParentProject);
    }

    public BaseParser()
    {
        //
    }

    public virtual BaseParser CreateNew(ViewedProjectTab parentProject)
    {
        return new BaseParser(parentProject);
    }

    public override string ToString()
    {
        return Name;
    }

    public bool OpenRulesFile(string rulesFileName)
    {
        return Rules?.LoadRulesFromFile(rulesFileName) ?? false;
    }

    /// <summary>
    /// Parses the bytes of packetData to generate ParsedFields
    /// </summary>
    /// <param name="packetData"></param>
    /// <param name="initialLoading">True when it's parsed during the file loading process</param>
    /// <returns></returns>
    public virtual bool ParsePacketData(BasePacketData packetData, bool initialLoading)
    {
        packetData.ParsedData.Clear();
        // In your own parser, replace this check with whatever 
        if (packetData is not BasePacketData data)
            return false;

        // Do actual parsing, you can overwrite packetData values here if you want
        var rule = Rules?.GetPacketRule(data);
        rule?.Build();
        rule?.RunRule(data);

        // Add unparsed data
        data.AddUnparsedFields();
        return true;
    }

    /// <summary>
    /// Parses all packet data, also triggers progressbar update events
    /// </summary>
    /// <param name="initialLoading">Set to true if doing initial parsing during loading of the source</param>
    /// <returns></returns>
    public virtual bool ParseAllData(bool initialLoading)
    {
        var res = true;
        ViewedProjectTab.OnParseProgressUpdate(this, 0, ParentProject?.LoadedPacketList.Count ?? 1);
        for (var i = 0; i < ParentProject?.LoadedPacketList.Count; i++)
        {
            var basePacketData = ParentProject.LoadedPacketList[i];
            var packetRes = ParsePacketData(basePacketData, initialLoading);

            if (!packetRes)
                basePacketData.MarkedAsInvalid = true;

            res &= packetRes;

            if (i % 50 == 0)
                ViewedProjectTab.OnParseProgressUpdate(this, i, ParentProject.LoadedPacketList.Count);
        }
        ViewedProjectTab.OnParseProgressUpdate(this, 1, 1);
        return res;
    }


    int IComparable<BaseParser>.CompareTo(BaseParser? other)
    {
        return other != null ? string.CompareOrdinal(this.Name, other.Name) : 0;
    }

    /// <summary>
    /// Checks if this parser is compatible with the current reader
    /// </summary>
    /// <param name="reader"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public bool CanHandleSource(BaseInputReader? reader)
    {
        if (reader == null)
            return false;

        foreach (var supportedReader in SupportedReaders)
        {
            if (string.Equals(reader.Name, supportedReader, StringComparison.CurrentCultureIgnoreCase))
                return true;
        }
        return false;
    }
}