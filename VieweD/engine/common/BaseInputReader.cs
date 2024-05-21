using System;
using System.Collections.Generic;
using System.IO;
using VieweD.Helpers.System;
using VieweD.Properties;

namespace VieweD.engine.common;

public class BaseInputReader : IComparable<BaseInputReader>
{
    /// <summary>
    /// Name of this reader to display to the user
    /// </summary>
    public virtual string Name => "Base Reader";

    /// <summary>
    /// Description for this input reader, if empty it will not show up in the selection list
    /// </summary>
    public virtual string Description => "";

    public virtual string DataFolder => "base";
    
    /// <summary>
    /// Owning project
    /// </summary>
    public ViewedProjectTab? ParentProject { get; set; }

    /// <summary>
    /// Is the source opened
    /// </summary>
    public bool IsOpened { get; protected set; }

    /// <summary>
    /// Stream of the opened source that is used for reading
    /// </summary>
    public Stream? SourceStream { get; protected set; }

    /// <summary>
    /// Holds a list of file extensions that this input reader expects to handle
    /// All entries must be added in lowercase during construction
    /// </summary>
    public List<string> ExpectedFileExtensions { get; protected set; } = new ();

    public BaseInputReader(ViewedProjectTab parentProject)
    {
        ParentProject = parentProject;
    }

    public BaseInputReader()
    {
        //
    }

    ~BaseInputReader()
    {
        if (IsOpened) Close();
    }

    public virtual BaseInputReader CreateNew(ViewedProjectTab parentProject)
    {
        return new BaseInputReader(parentProject);
    }

    int IComparable<BaseInputReader>.CompareTo(BaseInputReader? other)
    {
        return other != null ? string.CompareOrdinal(this.Name, other.Name) : 0;
    }

    public override string ToString()
    {
        return Name;
    }

    /// <summary>
    /// Checks if this input reader can likely handle the source file
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    public virtual bool CanHandleSource(string source)
    {
        var ext = Path.GetExtension(source).ToLower();
        foreach (var fileExtension in ExpectedFileExtensions)
        {
            if (ext == fileExtension) 
                return true;
        }
        return false;
    }

    /// <summary>
    /// Closes the reader
    /// </summary>
    public virtual void Close()
    {
        ParentProject?.OnInputSourceClosing(this);
    }

    /// <summary>
    /// Opens the source stream and read the headers if needed
    /// </summary>
    /// <param name="source"></param>
    /// <param name="fileName">Originally opened file name (if any)</param>
    /// <returns></returns>
    public virtual bool Open(Stream source, string fileName)
    {
        ParentProject?.OnInputError(this, "Function not Implemented!");
        return false;
    }


    /// <summary>
    /// Opens a source file and notifies the project if successful
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    public virtual bool OpenFile(string source)
    {
        if (ParentProject == null)
            return false;

        try
        {
            SourceStream = new FileStream(source, FileMode.Open, FileAccess.Read, FileShare.Read);
            var res = Open(SourceStream, source);
            if (res)
            {
                ParentProject.Settings.LogFile = source;
                ParentProject.Text = Helper.MakeTabName(source);
            }
            else
            {
                ParentProject.Settings.LogFile = string.Empty;
                ParentProject.Text = Resources.TypeUnknown;
            }

            return res;
        }
        catch (Exception ex)
        {
            ParentProject.OnInputError(this, ex.Message);
            return false;
        }
    }

    public virtual int ReadAllData()
    {
        if (!IsOpened)
        {
            return -1;
        }

        var dataCount = 0;
        return dataCount;
    }

    public bool DateTimeParse(string s, out DateTime res)
    {
        res = DateTime.MinValue;
        if (s.Length != 19) return false;
        try
        {
            var yyyy = int.Parse(s.Substring(0, 4));
            var mm = int.Parse(s.Substring(5, 2));
            var dd = int.Parse(s.Substring(8, 2));
            var hh = int.Parse(s.Substring(11, 2));
            var nn = int.Parse(s.Substring(14, 2));
            var ss = int.Parse(s.Substring(17, 2));
            res = new DateTime(yyyy, mm, dd, hh, nn, ss);
            return true;
        }
        catch
        {
            //
        }

        return false;
    }
}