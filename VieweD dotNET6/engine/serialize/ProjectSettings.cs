using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using VieweD.engine.common;

namespace VieweD.engine.serialize;

[XmlRoot("Settings")]
public class ProjectSettings
{
    public string ProjectFile { get; set; } = "";
    public string ProjectUrl { get; set; } = "";
    public string LogFile { get; set; } = "";
    public string InputReader { get; set; } = "";
    public string Parser { get; set; } = "";
    public string RulesFile { get; set; } = "";
    public string DecryptionName { get; set; } = "";

    public ProjectVideoSettings VideoSettings { get; set; } = new ();
    public List<string> Tags { get; set; } = new();
    public PacketListFilter Filter { get; set; } = new();
    public SearchParameters Search { get; set; } = new();
    public TimeSpan LastTimeOffset { get; set; } = TimeSpan.Zero;
}

public class ProjectVideoSettings
{
    public string VideoFile { get; set; } = "";
    public string VideoUrl { get; set; } = "";
    public TimeSpan VideoOffset { get; set; } = TimeSpan.Zero;
}
