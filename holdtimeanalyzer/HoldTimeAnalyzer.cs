using OsuParsers.Enums.Replays;
using OsuParsers.Replays;
using HoldTimes = System.Collections.Generic.Dictionary<int, int>;

namespace holdtimeanalyzer;

/// <summary>
/// Calculates the hold times for replays.
/// </summary>
internal static class HoldTimeAnalyzer
{
  /// <summary>
  /// Calculates the hold times for the given replay, for both keys (K1 & M1, K2 & M2).
  /// </summary>
  /// <param name="replay">The replay to calculate the hold time of.</param>
  /// <returns>The hold times per-key.</returns>
  public static (HoldTimes Key1, HoldTimes Key2) CalculateHoldTimes(Replay replay)
  {
    // Extract key-specific frames for both keys.
    KeyFrame[] key1Frames = ExtractFramesForKey(replay, StandardKeys.K1 & StandardKeys.M1);
    KeyFrame[] key2Frames = ExtractFramesForKey(replay, StandardKeys.K2 & StandardKeys.M2);

    // Calculate the hold times for both keys.
    HoldTimes key1HoldTimes = CalculateHoldTimes(key1Frames);
    HoldTimes key2HoldTimes = CalculateHoldTimes(key2Frames);

    return (key1HoldTimes, key2HoldTimes);
  }

  /// <summary>
  /// Returns the hold times for the given key-specific frames.
  /// </summary>
  /// <param name="frames">The key-specific frames.</param>
  /// <returns>The hold times (key) and amount of occurences (.</returns>
  private static HoldTimes CalculateHoldTimes(KeyFrame[] frames)
  {
    HoldTimes holdTimes = new HoldTimes();
    KeyFrame? pressedFrame = null;

    foreach (var frame in frames)
    {
      // If no pressed frame is set and the current frame is pressed, ´measure time from here.
      if (pressedFrame is null && frame.Pressed)
        pressedFrame = frame;
      // If an unpress frame is encountered, measure the hold time via the pressed frame and increase the amount of occurences.
      else if (pressedFrame is not null && !frame.Pressed)
      {
        int holdTime = frame.Offset - pressedFrame.Offset;
        holdTimes.TryAdd(holdTime, 0); // Ensure the hold time is in the dictionary.
        holdTimes[frame.Offset - pressedFrame.Offset]++;
        pressedFrame = null; // reset the pressed frame again to measure the next hold time.
      }
    }

    return holdTimes;
  }

  /// <summary>
  /// Converts the replay frames in the specified replays into key-specific frames of the given key.
  /// </summary>
  /// <param name="replay">The replay containing the replay frames.</param>
  /// <param name="key">The key to get the specific frames of.</param>
  /// <returns>The key-specific frames.</returns>
  private static KeyFrame[] ExtractFramesForKey(Replay replay, StandardKeys key)
  {
    return replay.ReplayFrames.Select(n => new KeyFrame(n.Time, n.StandardKeys.HasFlag(key))).ToArray();
  }
}

/// <summary>
/// Represents a frame in a replay, with the offset and whether the key this frame is related to was pressed.
/// </summary>
/// <param name="Offset">The offset of the frame from the start.</param>
/// <param name="Pressed">Bool whether the key related to this frame is pressed.</param>
internal record KeyFrame(int Offset, bool Pressed);