// FFmpegOut - FFmpeg video encoding plugin for Unity
// https://github.com/keijiro/KlakNDI

using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEditor;
using RTSP;
using System.Net;
using System.Net.NetworkInformation;

namespace FFmpegOut
{
    [AddComponentMenu("FFmpegOut/Camera Capture")]
    public sealed class CameraCapture : MonoBehaviour
    {
        #region Public properties

        [SerializeField] int _width = 1920;

        public int width {  
            get { return _width; }
            set { _width = value; }
        }

        [SerializeField] int _height = 1080;

        public int height {
            get { return _height; }
            set { _height = value; }
        }

        //ѡ�������
        [SerializeField] FFmpegPreset _preset;

        public FFmpegPreset preset {
            get { return _preset; }
            set { _preset = value; }
        }

        [SerializeField] float _frameRate = 60;

        public float frameRate {
            get { return _frameRate; }
            set { _frameRate = value; }
        }

        [SerializeField] string _url = "rtsp://";

        public string url {
            get { return _url; }
            set { _url = value; }
        }
        #endregion



        #region Private members

        FFmpegSession _session;
        RenderTexture _tempRT;
        GameObject _blitter;
        RTSPServerLoader loader;

        bool isServerAccesible = false;
        string ipv4Address;

        //��ȡ�������Ŀ����Ⱦ������ʽ
        RenderTextureFormat GetTargetFormat(Camera camera)
        {
            return camera.allowHDR ? RenderTextureFormat.DefaultHDR : RenderTextureFormat.Default;
        }

        //��ȡ������Ŀ���ݼ���
        int GetAntiAliasingLevel(Camera camera)
        {
            return camera.allowMSAA ? QualitySettings.antiAliasing : 1;
        }

        #endregion

        #region Time-keeping variables

        int _frameCount;
        float _startTime;
        int _frameDropCount;

        float FrameTime {
            get { return _startTime + (_frameCount - 0.5f) / _frameRate; }
        }

        void WarnFrameDrop()
        {
            if (++_frameDropCount != 10) return;

            Debug.LogWarning(
                "Significant frame droppping was detected. This may introduce " +
                "time instability into output video. Decreasing the recording " +
                "frame rate is recommended."
            );
        }

        #endregion

        #region MonoBehaviour implementation

        void OnValidate()
        {
            _width = Mathf.Max(8, _width);
            _height = Mathf.Max(8, _height);
        }

        void OnDisable()
        {
            if (_session != null)
            {
                // Close and dispose the FFmpeg session.
                _session.Close();
                _session.Dispose();
                _session = null;
            }

            if (_tempRT != null)
            {
                // Dispose the frame texture.
                GetComponent<Camera>().targetTexture = null;
                Destroy(_tempRT);
                _tempRT = null;
            }

            if (_blitter != null)
            {
                // Destroy the blitter game object.
                Destroy(_blitter);
                _blitter = null;
            }

            loader.Kill("rtsp-simple-server");//�رճ���ʱɱ��process
        }

        IEnumerator Start()
        {
            loader = RTSPServerLoader.GetInstance();

            if (!loader.CoroutineStarted)
            {
                StartCoroutine(loader.WaitForServerToStart());
            }

            //��ȡ����ip����һ���򵥵�������ַ��飬��ֹ�����ַ�������
            // ��ȡ���������������ӿ�
            NetworkInterface[] interfaces = NetworkInterface.GetAllNetworkInterfaces();
            // ����ÿ���ӿ�
            foreach (NetworkInterface iface in interfaces)
            {
                // ����ҵ�WiFi����̫���ӿڣ����Ҹýӿڴ��ڻ״̬
                if (iface.OperationalStatus == OperationalStatus.Up 
                    && (iface.NetworkInterfaceType == NetworkInterfaceType.Wireless80211 || iface.NetworkInterfaceType == NetworkInterfaceType.Ethernet))
                {
                    // �����ýӿ��ϵ����� IP ��ַ
                    foreach (UnicastIPAddressInformation addr in iface.GetIPProperties().UnicastAddresses)
                    {
                        // ����ҵ� IPv4 ��ַ�����������
                        if (addr.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                        {
                            Debug.Log("IPv4 Address: " + addr.Address.ToString());
                            ipv4Address = addr.Address.ToString();
                            break;
                        }
                    }
                }
            }
            //�ж������rtsp�Ƿ������ip
            if(url.Contains(ipv4Address))
            {
                isServerAccesible = true;
            }
            else
            {
                isServerAccesible = false;
            }

            // Sync with FFmpeg pipe thread at the end of every frame.
            for (var eof = new WaitForEndOfFrame();;)
            {
                yield return eof;
                _session?.CompletePushFrames();
            }
        }

        void Update()
        {
            var camera = GetComponent<Camera>();

            // Lazy initialization �����ʼ��
            if (_session == null && loader.RTSPServerloaded && isServerAccesible)
            {
                // Give a newly created temporary render texture to the camera
                // if it's set to render to a screen. Also create a bli)ter
                // object to keep frames presented on the screen.
                if (camera.targetTexture == null)
                {
                    _tempRT = new RenderTexture(_width, _height, 24, GetTargetFormat(camera)); 
                    _tempRT.antiAliasing = GetAntiAliasingLevel(camera);
                    camera.targetTexture = _tempRT;
                    _blitter = Blitter.CreateInstance(camera);
                }

                // Start an FFmpeg session.
                _session = FFmpegSession.Create(
                    gameObject.name,
                    camera.targetTexture.width,
                    camera.targetTexture.height,
                    _frameRate, url, preset
                );

                _startTime = Time.time;
                _frameCount = 0;
                _frameDropCount = 0;    
            }

            var gap = Time.time - FrameTime;
            var delta = 1 / _frameRate;

            if (loader.RTSPServerloaded && isServerAccesible)
            {
                if (gap < 0)
                {
                    // Update without frame data.
                    _session.PushFrame(null);
                }
                else if (gap < delta)
                {
                    // Single-frame behind from the current time:
                    // Push the current frame to FFmpeg.
                    _session.PushFrame(camera.targetTexture);
                    _frameCount++;
                }
                else if (gap < delta * 2)
                {
                    // Two-frame behind from the current time:
                    // Push the current frame twice to FFmpeg. Actually this is not
                    // an efficient way to catch up. We should think about
                    // implementing frame duplication in a more proper way. #fixme
                    _session.PushFrame(camera.targetTexture);
                    _session.PushFrame(camera.targetTexture);
                    _frameCount += 2;
                }
                else
                {
                    // Show a warning message about the situation.
                    WarnFrameDrop();

                    // Push the current frame to FFmpeg.
                    _session.PushFrame(camera.targetTexture);

                    // Compensate the time delay.
                    _frameCount += Mathf.FloorToInt(gap * _frameRate);
                }
            }            
        }
        #endregion
    }
}