using holdtimeanalyzer;
using OsuParsers.Decoders;
using OsuParsers.Enums;
using OsuParsers.Replays;
using ScottPlot;
using ScottPlot.Plottables;
using System.Globalization;
using HoldTimes = System.Collections.Generic.Dictionary<int, int>;

public class Program
{
  /// <summary>
  /// The one-based index of the currently processed replay.
  /// </summary>
  private static int _current = 0;
  
  /// <summary>
  /// The total amount of replays to process.
  /// </summary>
  private static int _total = 0;

  public static void Main(string[] args)
  {
    Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;

    // Ensure that a replays & output folder exists.
    Directory.CreateDirectory("output");
    if (!Directory.Exists("replays"))
    {
      Directory.CreateDirectory("replays");
      Console.WriteLine("A replays folder was created. Put your replays in there and run the program again.");
      Console.ReadKey();
      return;
    }

    // go through all replay files in the replays folder and process them.
    string[] replayFiles = Directory.GetFiles("replays", "*.osr");
    _total = replayFiles.Length;
    foreach (string replayFile in replayFiles)
    {
      try
      {
        _current++;
        ProcessReplayFile(replayFile);
      }
      catch (Exception ex)
      {
        // If an error occurs, notify the user and write the error to a file.
        Write("Error", $"{Path.GetFileName(replayFile)} => {ex.Message}", ConsoleColor.Red);
        File.WriteAllText($"output\\error_{Path.GetFileNameWithoutExtension(replayFile)}.txt", $"{ex.Message}\n{ex.StackTrace}");
      }
    }

    Console.WriteLine();
    Console.WriteLine("Done.");
    Console.ReadKey();
  }

  /// <summary>
  /// Processes the specified replay file and generates a plot for the hold times.
  /// </summary>
  /// <param name="replayFile">The replay file.</param>
  private static void ProcessReplayFile(string replayFile)
  {
    // Calculate the hold times for the replay and filter them to only include hold times below 100ms.
    Replay replay = ReplayDecoder.Decode(replayFile);
    (HoldTimes Key1, HoldTimes Key2) holdTimes = HoldTimeAnalyzer.CalculateHoldTimes(replay);
    HoldTimes filteredKey1 = new HoldTimes(holdTimes.Key1.Where(x => x.Key <= 100));
    HoldTimes filteredKey2 = new HoldTimes(holdTimes.Key2.Where(x => x.Key <= 100));

    // If no significant hold times were found, skip the replay.
    if (!filteredKey1.Any() && !filteredKey2.Any())
    {
      Write("Skipped", $"{Path.GetFileName(replayFile)} (no significant hold times found)", ConsoleColor.Yellow);
      return;
    }

    // Setup the plot.
    Plot plot = new Plot();
    string mods = string.Join(", ", Enum.GetValues<Mods>().Where(m => replay.Mods.HasFlag(m) && m > Mods.None).Select(m => m.ToString()));
    string id = $"{(replay.OnlineId == 0 ? "Offline" : replay.OnlineId)}";
    plot.Title($"({id}) {replay.PlayerName} at {replay.ReplayTimestamp:dd/MM/yyyy HH:mm} +{mods} ({replay.ReplayLength}ms)");
    plot.ScaleFactor = 2;
    plot.Axes.Left.Label.Text = "Amount of Occurrences";
    plot.Axes.Bottom.Label.Text = "Milliseconds";

    plot.Legend.Alignment = Alignment.UpperRight;

    // Configure the limits for the axes, depending on the bounds of the hold times.
    plot.Axes.Bottom.Min = filteredKey1.Concat(filteredKey2).Min(x => x.Key) - 1;
    plot.Axes.Bottom.Max = filteredKey1.Concat(filteredKey2).Max(x => x.Key) + 1;
    plot.Axes.Left.Min = 0;
    plot.Axes.Left.Max = (int)(filteredKey1.Concat(filteredKey2).Max(x => x.Value) * 1.05) + 1;

    // Add the exponential average lines for both keys.
    double exponentialAvg1 = filteredKey1.Sum(x => x.Key * Math.Pow(x.Value, 2)) / filteredKey1.Sum(x => Math.Pow(x.Value, 2));
    double exponentialAvg2 = filteredKey2.Sum(x => x.Key * Math.Pow(x.Value, 2)) / filteredKey2.Sum(x => Math.Pow(x.Value, 2));
    plot.Add.VerticalLine(exponentialAvg1, color: new Color(121, 159, 203), pattern: LinePattern.Dotted);
    plot.Add.VerticalLine(exponentialAvg2, color: new Color(249, 102, 94), pattern: LinePattern.Dotted);

    // Add the bar plots for both keys.
    BarPlot key1Bar = plot.Add.Bars(filteredKey1.Keys.Select(x => x * 1d), filteredKey1.Values.Select(x => x * 1d));
    BarPlot key2Bar = plot.Add.Bars(filteredKey2.Keys.Select(x => x * 1d), filteredKey2.Values.Select(x => x * 1d));
    key1Bar.Color = new Color(121, 159, 203, 100);
    key2Bar.Color = new Color(249, 102, 94, 100);
    key1Bar.LegendText = $"Key 1 ({filteredKey1.Sum(x => x.Value)} presses, {Math.Round(exponentialAvg1, 2)} eavg.)";
    key2Bar.LegendText = $"Key 2 ({filteredKey2.Sum(x => x.Value)} presses, {Math.Round(exponentialAvg2, 2)} eavg.)";

    // Render the plot and save it to the output folder.
    plot.SavePng($"output\\{Path.GetFileNameWithoutExtension(replayFile)}.png", 2000, 1200);
    string progress = $"({_current}/{_total} {(int)(_current * 100d / _total)}%)";
    Write("Processed", $"{Path.GetFileName(replayFile)} => output\\{Path.GetFileNameWithoutExtension(replayFile)}.png {progress}", ConsoleColor.Green);
  }

  /// <summary>
  /// Writes a message to the console with the specified prefix color.
  /// </summary>
  /// <param name="prefix">The prefix of the message.</param>
  /// <param name="text">The text.</param>
  /// <param name="color">The prefix color.</param>
  private static void Write(string prefix, string text, ConsoleColor color)
  {
    Console.ForegroundColor = color;
    Console.Write(prefix + " ");
    Console.ForegroundColor = ConsoleColor.Gray;
    Console.WriteLine(text);
  }
}