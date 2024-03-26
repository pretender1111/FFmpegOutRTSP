using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using UnityEngine;

namespace RTSP
{  

    public class RTSPServerLoader : MonoBehaviour
    {
        public static RTSPServerLoader instance;
        public bool RTSPServerloaded = false;
        public bool CoroutineStarted = false;
        private Process process;
        private StreamWriter messageStream;

        public RTSPServerLoader()
        {
            try
            {
                process = new Process();
                process.EnableRaisingEvents = false;
                process.StartInfo.FileName = Application.dataPath + "./RTSPServer/rtsp-simple-server.exe";
                process.StartInfo.WorkingDirectory = Application.dataPath + "./RTSPServer";
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.CreateNoWindow = true;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.RedirectStandardInput = true;
                process.StartInfo.RedirectStandardError = true;
                process.OutputDataReceived += new DataReceivedEventHandler(DataReceived);
                process.ErrorDataReceived += new DataReceivedEventHandler(ErrorReceived);
                process.Start();
                process.BeginOutputReadLine();
                messageStream = process.StandardInput;

                UnityEngine.Debug.Log("Starting RTSP Server");
            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogError("Unable to launch app: " + e.Message);
            }
        }

        /// <summary>
        /// Sets a boolean when the server has started
        /// </summary>
        /// <returns></returns>
        public IEnumerator WaitForServerToStart()
        {
            CoroutineStarted = true;
            yield return (new WaitForSeconds(2));

            RTSPServerloaded = true;
            UnityEngine.Debug.Log("Started RTSP Server");
        }

        /// <summary>
        /// Handles stdout messages from the process
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventArgs"></param>
        void DataReceived(object sender, DataReceivedEventArgs eventArgs)
        {
            UnityEngine.Debug.Log(eventArgs.Data);
        }

        /// <summary>
        /// Handles stderr messages from the process
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventArgs"></param>
        void ErrorReceived(object sender, DataReceivedEventArgs eventArgs)
        {
            UnityEngine.Debug.LogError(eventArgs.Data);
        }

        /// <summary>
        /// Kills the RTSP server
        /// </summary>
        public void Kill(string name)
        {
            Process[] processes = Process.GetProcesses();
            foreach (Process process in processes)
            {
                try
                {
                    if (!process.HasExited && process.ProcessName == name)
                        process.Kill();
                }
                catch (InvalidOperationException ex)
                {
                    UnityEngine.Debug.Log(ex);
                }
            }
        }

        void OnApplicationQuit()
        {
            Kill("rtsp-simple-server");
        }

        /// <summary>
        /// Gets instance of rstp server. if it doesnt exist, creates it.
        /// </summary>
        /// <returns></returns>
        public static RTSPServerLoader GetInstance()
        {
            if (instance == null)
            {
                instance = new RTSPServerLoader();
                UnityEngine.Debug.Log("new RTSPServerLoader");
            }
            return (instance);
        }
    }
}

