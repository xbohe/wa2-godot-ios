using YamlDotNet.RepresentationModel;
using System.Collections.Generic;
using System.Globalization;
using Godot;
using StringReader = System.IO.StringReader;
public class ContentSegment
{
  public int Begin { get; set; }
  public int End { get; set; }
  public string Text { get; set; }
}
public class SoundSubtitle
{
  public int Id { get; set; }
  public List<ContentSegment> Content { get; set; }
}
public class VoiceSubtitle
{
  public int Id { get; set; }
  public int Scene { get; set; }
  public List<ContentSegment> Content { get; set; }
}
public class SubtitleRoot
{
  public List<SoundSubtitle> SoundSubtitle { get; set; }
  public List<VoiceSubtitle> VoiceSubtitle { get; set; }
}

public partial class SubtitleMgr : Node
{
  public List<SoundSubtitle> SoundSubtitleList = new();
  public List<VoiceSubtitle> VoiceSubtitleList = new();
  public Wa2Audio ListenAudio;
  public List<ContentSegment> ListenContent;
  [Export]
  public Label TextLabel;
  public override void _Ready()
  {
    using var file = FileAccess.Open("res://assets/sub.yaml", FileAccess.ModeFlags.Read);
    if (file == null)
    {
      return;
    }

    var yaml = new YamlStream();
    using var reader = new StringReader(file.GetAsText());
    yaml.Load(reader);

    var root = (YamlMappingNode)yaml.Documents[0].RootNode;
    SoundSubtitleList = ParseSoundSubtitles(GetSequence(root, "soundSubtitle"));
    VoiceSubtitleList = ParseVoiceSubtitles(GetSequence(root, "voiceSubtitle"));

    // foreach (var se in VoiceSubtitleList)
    // {
    //   GD.Print($"SoundEffect ID: {se.Id}");
    //   foreach (var segment in se.Content)
    //   {
    //     GD.Print($"  {segment.Begin}-{segment.End}: {segment.Text}");
    //   }
    // }
  }
  private static YamlSequenceNode GetSequence(YamlMappingNode root, string key)
  {
    return (YamlSequenceNode)root.Children[new YamlScalarNode(key)];
  }
  private static int GetInt(YamlMappingNode node, string key)
  {
    string value = ((YamlScalarNode)node.Children[new YamlScalarNode(key)]).Value;
    return int.Parse(value, CultureInfo.InvariantCulture);
  }
  private static string GetText(YamlMappingNode node, string key)
  {
    return ((YamlScalarNode)node.Children[new YamlScalarNode(key)]).Value ?? "";
  }
  private static List<ContentSegment> ParseContent(YamlMappingNode entry)
  {
    var result = new List<ContentSegment>();
    var content = (YamlSequenceNode)entry.Children[new YamlScalarNode("content")];
    foreach (YamlMappingNode segment in content.Children)
    {
      result.Add(new ContentSegment
      {
        Begin = GetInt(segment, "begin"),
        End = GetInt(segment, "end"),
        Text = GetText(segment, "text")
      });
    }
    return result;
  }
  private static List<SoundSubtitle> ParseSoundSubtitles(YamlSequenceNode entries)
  {
    var result = new List<SoundSubtitle>();
    foreach (YamlMappingNode entry in entries.Children)
    {
      result.Add(new SoundSubtitle
      {
        Id = GetInt(entry, "id"),
        Content = ParseContent(entry)
      });
    }
    return result;
  }
  private static List<VoiceSubtitle> ParseVoiceSubtitles(YamlSequenceNode entries)
  {
    var result = new List<VoiceSubtitle>();
    foreach (YamlMappingNode entry in entries.Children)
    {
      result.Add(new VoiceSubtitle
      {
        Id = GetInt(entry, "id"),
        Scene = GetInt(entry, "scene"),
        Content = ParseContent(entry)
      });
    }
    return result;
  }
  public void ListenVoice(int scene, int id, Wa2Audio audio)
  {
    for (int i = 0; i < VoiceSubtitleList.Count; i++)
    {
      if (VoiceSubtitleList[i].Id == id && VoiceSubtitleList[i].Scene == scene)
      {
        ListenContent = VoiceSubtitleList[i].Content;
        ListenAudio = audio;
        return;
      }
    }
    if (ListenAudio == null)
    {
      TextLabel.Text = "";
      ListenAudio = null;
      ListenContent = null;
    }
  }
  public void ListenSe(int id, Wa2Audio audio)
  {
    for (int i = 0; i < SoundSubtitleList.Count; i++)
    {
      if (SoundSubtitleList[i].Id == id)
      {
        ListenContent = SoundSubtitleList[i].Content;
        ListenAudio = audio;
        return;
      }
    }
    if (ListenAudio == null)
    {
      TextLabel.Text = "";
      ListenAudio = null;
      ListenContent = null;
    }

  }
  public override void _Process(double delta)
  {
    if (ListenAudio != null && ListenAudio.Stream != null && ListenAudio.Playing && ListenContent != null)
    {
      foreach (ContentSegment segment in ListenContent)
      {
        if (ListenAudio.GetPlaybackPosition() * 1000 >= segment.Begin && ListenAudio.GetPlaybackPosition() * 1000 <= segment.End)
        {
          TextLabel.Text = segment.Text;
        }
      }
    }
    else
    {
      TextLabel.Text = "";
      ListenAudio = null;
      ListenContent = null;
    }
  }
}
