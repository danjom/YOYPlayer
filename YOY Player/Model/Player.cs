using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media;
using YOYPlayer.Model.Data;
using YOYPlayer.Model.Helpers;
using YOYPlayer.Model.Utils.Exceptions;

namespace YOYPlayer.Model
{
    public class Player : INotifyPropertyChanged
    {
        private static WaveOut _player;
        private static WaveStream _reader;
        private static LoopStream _loop;

        public bool IsActive
        {
            get => (_player?.PlaybackState ?? PlaybackState.Stopped) == PlaybackState.Playing;
        }

        #region Sinleton

        private static Player _instance = null;
        public static Player Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new Player();

                return _instance;
            }
        }

        #endregion

        #region Notifier

        public event PropertyChangedEventHandler PropertyChanged;

        private void Notify(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

        public static async void RunLoop(string fileName)
        {
            await Task.Factory.StartNew(() =>
            {
                _reader = new WaveFileReader(fileName);
                _loop = new LoopStream(_reader);

                _player = new WaveOut();
                _player.Init(_loop);
                _player.Play();
                Instance.Notify("IsActive");
            });

            var ai = Properties.Settings.Default.AudioInfo;

            LogsService.WriteLog(new LocalLogItem()
            {
                AccessKey = ai.AccessKey,
                CommerceName = ai.CommerceName,
                BranchName = ai.BranchName,
                DepartmentName = ai.DepartmentName,
                CommerceId = ai.CommerceId,
                BranchId = ai.BranchId,
                DepartmentId = ai.DepartmentId,
                EventDate = DateTime.Now.ToUniversalTime(),
                EventType = 1,
                FileID = ai.FileID,
                Username = Properties.Settings.Default.Username
            });

            await WebHelper.SendLogs();
        }

        public static void Stop()
        {
            if (_player != null)
            {
                _player.Stop();

                _player.Dispose();
                _loop.Dispose();
                _reader.Dispose();

                Instance.Notify("IsActive");
            }
        }

        private class LoopStream : WaveStream
        {
            WaveStream sourceStream;

            /// <summary>
            /// Creates a new Loop stream
            /// </summary>
            /// <param name="sourceStream">The stream to read from. Note: the Read method of this stream should return 0 when it reaches the end
            /// or else we will not loop to the start again.</param>
            public LoopStream(WaveStream sourceStream)
            {
                this.sourceStream = sourceStream;
                this.EnableLooping = true;
            }

            /// <summary>
            /// Use this to turn looping on or off
            /// </summary>
            public bool EnableLooping { get; set; }

            /// <summary>
            /// Return source stream's wave format
            /// </summary>
            public override WaveFormat WaveFormat
            {
                get { return sourceStream.WaveFormat; }
            }

            /// <summary>
            /// LoopStream simply returns
            /// </summary>
            public override long Length
            {
                get { return sourceStream.Length; }
            }

            /// <summary>
            /// LoopStream simply passes on positioning to source stream
            /// </summary>
            public override long Position
            {
                get { return sourceStream.Position; }
                set { sourceStream.Position = value; }
            }

            public override int Read(byte[] buffer, int offset, int count)
            {
                int totalBytesRead = 0;

                while (totalBytesRead < count)
                {
                    int bytesRead = sourceStream.Read(buffer, offset + totalBytesRead, count - totalBytesRead);
                    if (bytesRead == 0)
                    {
                        if (sourceStream.Position == 0 || !EnableLooping)
                        {
                            // something wrong with the source stream
                            break;
                        }
                        // loop
                        Thread.Sleep(TimeSpan.FromSeconds(2));
                        sourceStream.Position = 0;
                    }
                    totalBytesRead += bytesRead;
                }
                return totalBytesRead;
            }
        }
    }

}