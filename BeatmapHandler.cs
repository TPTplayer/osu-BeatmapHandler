using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Security.Cryptography;
using System.Runtime.InteropServices;
using static System.Windows.Forms.LinkLabel;

namespace _7kConvertor {
    public class BeatmapHandler {
        public delegate void ExceptionHandler(object sender, Exception ex);
        public event ExceptionHandler ExceptionEvent;

        public delegate void MessageHandler(string type, string message);
        public event MessageHandler MessageEvent;

        private string _beatmapFilePath;
        private string _rawBuffer;

        // [HitObjects]
        // x,y,time,type,hitSound,endTime:hitSample
        // 64,192,47097,128,0,47751:0:0:0:35:drum.wav

        // laneIndex = floor(x * columnCount / 512)

        #region File Section
        private enum SectionIdx {
            //IDX_HEADER, 
            GENERAL, 
            EDITOR,
            METADATA, 
            DIFF, 
            EVENTS,
            TIMINGPOINTS,
            HITOBJ
        }
        private const int N_SECTION = 7;
        private string[] _sectionParsed;
        #endregion

        // Difficulty
        private struct Difficulty {
            public float hpDrainRate;
            public int circleSize;
            public float overallDifficulty;
            public float approachRate;
            public float sliderMultiplier;
            public float sliderTickRate;
        }
        private const int N_DIFFELEM = 6;
        private Difficulty _difficulty;

        // HitObjects
        private struct HitObject {
            public int x;
            public int y;
            public int time;
            public int type;
            public int hitsound;
            public int endTime;
            public string hitSample;
        }
        private List<HitObject> hitObjectsList; 

        private enum HitObjectFormatIdx {
            X,
            Y, // don't care
            TIME,
            TYPE, // don't care
            HITSOUND, // don't care
            ENDTIME,
            HITSAMPLE // don't care
        }
        private const int N_HITOBJ_FORMAT = 6;
        
        public BeatmapHandler() {
            _difficulty = new Difficulty();
            hitObjectsList = new List<HitObject>();
        }

        public bool LoadBeatmap(string path) {
            try {
                _beatmapFilePath = path;

                if (!ReadBeatmapFile(_beatmapFilePath)) return false;
                if (!SectionParser()) return false;


            }
            catch (Exception ex) {
                ExceptionEvent(this, ex);
                return false;
            }
            return true;
        }

        private bool ReadBeatmapFile(string path) {
            StreamReader streamReader;

            try {
                streamReader = new StreamReader(path);
                _rawBuffer = streamReader.ReadToEnd();
                streamReader.Close();
            }
            catch (Exception ex) {
                ExceptionEvent(this, ex);
                return false;
            }
            return true;
        }

        private bool SectionParser() {
            string[] lines;
            int idx = 0, section = 0;

            bool flag_reqNextSection = false;

            try {
                lines = _rawBuffer.Split('\n');
                if (!FindBeatmapHeader(lines)) return false;

                // parse section
                _sectionParsed = new string[N_SECTION];
                for (idx = 2; idx < lines.Length; idx++) {
                    if (lines[idx] == "\r") {
                        flag_reqNextSection = true;
                        continue;
                    }

                    if(flag_reqNextSection) {
                        flag_reqNextSection = false;
                        section++;
                    }
                    _sectionParsed[section] += lines[idx];
                }
            }
            catch(Exception ex) {
                ExceptionEvent(this, ex);
                return false;
            }

            return true;
        }

        private bool DifficultyParser() {
            string[] lines;
            string key;
            
            try {
                lines = _sectionParsed[(int)SectionIdx.DIFF].Split('\r');
                for(int i = 1; i < lines.Length; i++) {
                    key = lines[i].Split(':')[0];
                    switch (key) {
                        case "HPDRainRate":
                            break;
                        case "CircleSize":
                            break;
                        case "OverallDifficulty":
                            break;
                        case "ApproachRate":
                            break;
                        case "SliderMultiplier":
                            break;
                        case "SliderTickRate":
                            break;
                        default:
                            MessageEvent("WARN", "unknown key: " + key);
                            break;
                    }
                }
            }
            catch (Exception ex) {
                ExceptionEvent(this, ex);
                return false;
            }

            return true;
        }

        private bool HitObjectParser() {

            try {
                
            }
            catch(Exception ex) {
                ExceptionEvent(this, ex);
                return false;
            }

            return true;
        }

        private bool FindBeatmapHeader(string[] rawBeatmapLines) {
            bool flag_findHeader = false;

            try {
                // find header
                foreach (string line in rawBeatmapLines) {
                    string[] tmp = line.Split(' ');
                    if (tmp.Length == 4 && tmp[0] == "osu" && tmp[1] == "file" && tmp[2] == "format") {
                        flag_findHeader = true;
                        break;
                    }

                    // to do: version check
                    //
                }

                if (!flag_findHeader) {
                    MessageEvent("-", "--");
                    return false;
                }
            }
            catch (Exception ex) {
                ExceptionEvent(this, ex);
                return false;
            }
            return true;
        }
    }
}
