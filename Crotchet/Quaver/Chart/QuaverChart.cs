#nullable enable
using System;
using System.Collections.Generic;
using System.IO;
using Crotchet.Quaver.Chart.Models;
using Crotchet.Quaver.Chart.Objects;
using Crotchet.Quaver.Exceptions;

namespace Crotchet.Quaver.Chart
{
    public class QuaverChart
    {
        public string AudioFile;
        public int SongPreviewTime;
        public string BackgroundFile;
        public int MapId;
        public int MapSetId;
        public Mode Mode;
        public string Title;
        public string Artist;
        public string Source;
        public string Tags;
        public string Creator;
        public string DifficultyName;
        public string Description;
        public List<EditorLayer> EditorLayers;
        public List<TimingPoint> TimingPoints;
        public List<SliderVelocity> SliderVelocities;
        public List<HitObject> HitObjects;

        public QuaverChart()
        {
            EditorLayers = new List<EditorLayer>();
            TimingPoints = new List<TimingPoint>();
            SliderVelocities = new List<SliderVelocity>();
            HitObjects = new List<HitObject>();
        }
        
        public void Parse(string? filePath)
        {
            ParsingStage currentStage = ParsingStage.Header;
            
            string[] fileContents = File.ReadAllLines(filePath);
            for (var index = 0; index < fileContents.Length; index++) {
                var line = fileContents[index];

                #region Header Loading

                switch (line.Split(':')[0]) {
                    case "AudioFile":
                        AudioFile = line.Split(new[] {": "}, StringSplitOptions.None)[1];
                        break;
                    case "SongPreviewTime":
                        SongPreviewTime = Convert.ToInt32(line.Split(new[] {": "}, StringSplitOptions.None)[1]);
                        break;
                    case "BackgroundFile":
                        BackgroundFile = line.Split(new[] {": "}, StringSplitOptions.None)[1];
                        break;
                    case "MapId":
                        MapId = Convert.ToInt32(line.Split(new[] {": "}, StringSplitOptions.None)[1]);
                        break;
                    case "MapSetId":
                        MapSetId = Convert.ToInt32(line.Split(new[] {": "}, StringSplitOptions.None)[1]);
                        break;
                    case "Mode":
                        Mode = (Mode) Enum.Parse(typeof(Mode), line.Split(new[] {": "}, StringSplitOptions.None)[1]);
                        break;
                    case "Title":
                        Title = line.Split(new[] {": "}, StringSplitOptions.None)[1];
                        break;
                    case "Artist":
                        Artist = line.Split(new[] {": "}, StringSplitOptions.None)[1];
                        break;
                    case "Source":
                        Source = line.Split(new[] {": "}, StringSplitOptions.None)[1];
                        break;
                    case "Tags":
                        Tags = line.Split(new[] {": "}, StringSplitOptions.None)[1];
                        break;
                    case "Creator":
                        Creator = line.Split(new[] {": "}, StringSplitOptions.None)[1];
                        break;
                    case "DifficultyName":
                        DifficultyName = line.Split(new[] {": "}, StringSplitOptions.None)[1];
                        break;
                    case "Description":
                        Description = line.Split(new[] {": "}, StringSplitOptions.None)[1];
                        break;
                    case "EditorLayers":
                        currentStage = ParsingStage.EditorLayers;
                        break;
                    case "TimingPoints":
                        currentStage = ParsingStage.TimingPoints;
                        break;
                    case "SliderVelocities":
                        currentStage = ParsingStage.SliderVelocities;
                        break;
                    case "HitObjects":
                        currentStage = ParsingStage.HitObjects;
                        break;
                    default:
                        switch (currentStage) {
                            case ParsingStage.Header:
                                throw new UnknownHeaderTagException();
                            case ParsingStage.TimingPoints: {
                                string startTime = line;
                                string bpm = fileContents[++index];
                                
                                TimingPoint tp = new TimingPoint {
                                    StartTime = Convert.ToInt32(startTime.Split(new[] {": "}, StringSplitOptions.None)[1]),
                                    BPM = Convert.ToDouble(bpm.Split(new[] {": "}, StringSplitOptions.None)[1])
                                };
                                TimingPoints.Add(tp);
                                break;
                            }
                            case ParsingStage.SliderVelocities: {
                                string startTime = line;
                                string multiplier = fileContents[++index];
                                
                                SliderVelocity sv = new SliderVelocity {
                                    StartTime = Convert.ToInt32(startTime.Split(new[] {": "}, StringSplitOptions.None)[1]),
                                    Multiplier = Convert.ToDouble(multiplier.Split(new[] {": "}, StringSplitOptions.None)[1])
                                };
                                SliderVelocities.Add(sv);
                                break;
                            }
                            case ParsingStage.HitObjects: {
                                string startTime = line;
                                string lane = fileContents[++index];
                                string endTime = "55";
                                if (fileContents[index + 1].Contains("EndTime"))
                                    endTime = fileContents[++index].Split(new[] {": "}, StringSplitOptions.None)[1];
                                else
                                    endTime =
                                        (Convert.ToInt32(startTime.Split(new[] {": "}, StringSplitOptions.None)[1]) + 30).ToString();
                                HitObject ho = new HitObject {
                                    StartTime = Convert.ToInt32(startTime.Split(new[] {": "}, StringSplitOptions.None)[1]),
                                    Lane = (Lane) Enum.Parse(typeof(Lane), lane.Split(new[] {": "}, StringSplitOptions.None)[1]),
                                    EndTime = Convert.ToInt32(endTime)
                                };
                                ho.EndTime = ho.EndTime - ho.StartTime;
                                HitObjects.Add(ho);
                                break;
                            }
                        }

                        break;
                }

                #endregion
            }
        }
    }
}