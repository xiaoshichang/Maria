﻿
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Maria.Core.GM
{
	public enum TerminalLogItemType
	{
		Error = LogType.Error,
		Assert = LogType.Assert,
		Warning = LogType.Warning,
		Log = LogType.Log,
		Exception = LogType.Exception,
		Input,
		Shell,
	}


	public class TerminalLogItem
	{
		public readonly TerminalLogItemType LogItemType;
		private readonly string Message;

		public TerminalLogItem(TerminalLogItemType logType, string message)
		{
			LogItemType = logType;
			Message = message;
		}

		public override string ToString()
		{
			return $"{LogItemType} >> {Message}";
		}
	}


	public enum TerminalState
	{
		Open,
		Close,
	}

	public class GMTerminal : MonoBehaviour
	{
		private void Awake()
		{
			DontDestroyOnLoad(gameObject);
			
			_InitGeometry();
			_InitStyles();
			_state = TerminalState.Close;

			// https://docs.unity3d.com/Manual/DomainReloading.html
			StopListening();
			StartListening();
		}

		private void StartListening()
		{
			if (!_Listening)
			{
				_Listening = true;
				Application.logMessageReceived += _OnUnityMessage;
			}
		}

		private void StopListening()
		{
			if (_Listening)
			{
				_Listening = false;
				Application.logMessageReceived -= _OnUnityMessage;
				_items.Clear();
			}
		}
		
		private void OnDisable()
		{
			StopListening();
		}

		void OnGUI()
		{
			if (Event.current.Equals(Event.KeyboardEvent(HotKey)))
			{
				StartCoroutine(_ToggleState());
				return;
			}

			if (_state == TerminalState.Close)
			{
				return;
			}

			GUI.Window(1, _windowRect, _DrawWindow, Title, _windowStyle);
		}


		private IEnumerator _ToggleState()
		{
			yield return null;
			if (_state == TerminalState.Close)
			{
				_FirstDraw = true;
				_CommandText = "";

			}
			_state = _state == TerminalState.Close ? TerminalState.Open : TerminalState.Close;
		}

		private void _InitGeometry()
		{
			_windowRect = new Rect(0, 0, Screen.width, Screen.height);
			_ScrollPosition.y = int.MaxValue;
		}

		private void _InitStyles()
		{
			var backgroundTexture = new Texture2D(1, 1);
			var backgroundColor = Color.black;
			backgroundColor.a = 0.7f;
			backgroundTexture.SetPixel(0, 0, backgroundColor);
			backgroundTexture.Apply();

			_windowStyle = new GUIStyle
			{
				normal =
				{
					background = backgroundTexture,
					textColor = ForegroundColor
				},
				padding = new RectOffset(4, 4, 4, 4),
			};

			_InputStyle = new GUIStyle
			{
				padding = new RectOffset(4, 4, 4, 4),
				normal =
				{
					textColor = ForegroundColor,
				}
			};

			_styles = new Dictionary<TerminalLogItemType, GUIStyle>();

			var errorStyle = new GUIStyle
			{
				padding = new RectOffset(4, 4, 4, 4),
				normal =
				{
					textColor = Color.red
				}
			};
			_styles[TerminalLogItemType.Error] = errorStyle;

			var assertStyle = new GUIStyle
			{
				padding = new RectOffset(4, 4, 4, 4),
				normal =
				{
					textColor = Color.red
				}
			};
			_styles[TerminalLogItemType.Assert] = assertStyle;

			var warningStyle = new GUIStyle
			{
				padding = new RectOffset(4, 4, 4, 4),
				normal =
				{
					textColor = Color.yellow
				}
			};
			_styles[TerminalLogItemType.Warning] = warningStyle;

			var logStyle = new GUIStyle
			{
				padding = new RectOffset(4, 4, 4, 4),
				normal =
				{
					textColor = Color.white
				}
			};
			_styles[TerminalLogItemType.Log] = logStyle;

			var exceptionStyle = new GUIStyle
			{
				padding = new RectOffset(4, 4, 4, 4),
				normal =
				{
					textColor = Color.red
				}
			};
			_styles[TerminalLogItemType.Exception] = exceptionStyle;

			var inputStyle = new GUIStyle
			{
				padding = new RectOffset(4, 4, 4, 4),
				normal =
				{
					textColor = Color.green
				}
			};
			_styles[TerminalLogItemType.Input] = inputStyle;

			var terminalStyle = new GUIStyle
			{
				padding = new RectOffset(4, 4, 4, 4),
				normal =
				{
					textColor = Color.magenta
				}
			};
			_styles[TerminalLogItemType.Shell] = terminalStyle;
		}

		private static void _OnUnityMessage(string condition, string stackTrace, LogType type)
		{
			TerminalLogItemType logType = (TerminalLogItemType) type;
			_AppendLog(logType, condition);
		}

		private static void _AppendLog(TerminalLogItemType logType, string message)
		{
			var item = new TerminalLogItem(logType, message);
			_items.Add(item);
			if (_items.Count > MaxLogCount)
			{
				_items.RemoveAt(0);
			}
		}

		private void _DrawInput()
		{
			GUILayout.BeginHorizontal();

			if (!string.IsNullOrEmpty(Caret))
			{
				GUILayout.Label(Caret, _InputStyle, GUILayout.Width(FontSize), GUILayout.Height(FontSize));
			}

			GUI.SetNextControlName("GMInput");
			_CommandText = GUILayout.TextField(_CommandText, _InputStyle, GUILayout.Height(FontSize));

			if (_FirstDraw)
			{
				GUI.FocusControl("GMInput");
				_FirstDraw = false;
			}

			GUILayout.EndHorizontal();
		}

		private void _DrawLogs()
		{
			foreach (var logItem in _items)
			{
				var style = _styles[logItem.LogItemType];
				GUILayout.Label(logItem.ToString(), style);
			}
		}

		private void _DrawWindow(int windowID)
		{
			GUILayout.BeginVertical();
			GUILayout.Space(FontSize);
			_ScrollPosition = GUILayout.BeginScrollView(_ScrollPosition, false, false, GUILayout.Width(Screen.width));
			GUILayout.FlexibleSpace();
			_DrawLogs();
			GUILayout.EndScrollView();
			_HandleInput();
			_DrawInput();
			GUILayout.EndVertical();
		}

		private void _RefreshInputContentByHistory()
		{
			_HistoryCmdIndex = System.Math.Min(_HistoryCmdIndex, _HistoryCmdList.Count - 1);
			_HistoryCmdIndex = System.Math.Max(_HistoryCmdIndex, 0);

			if (_HistoryCmdIndex < 0 || _HistoryCmdIndex >= _HistoryCmdList.Count)
			{
				return;
			}
			
			var historyCmd = _HistoryCmdList[_HistoryCmdIndex];
			_CommandText = historyCmd;
		}

		private void _UseNextHistoryCmd()
		{
			_HistoryCmdIndex += 1;
			_RefreshInputContentByHistory();
		}

		private void _UsePreviousHistoryCmd()
		{
			_HistoryCmdIndex -= 1;
			_RefreshInputContentByHistory();
		}

		private void _ResetHistoryCmdIndexToLast()
		{
			_HistoryCmdIndex = _HistoryCmdList.Count;
		}

		private void _AppendHistoryCmd(string cmd)
		{
			_HistoryCmdList.Add(cmd);
			if (_HistoryCmdList.Count > _HistoryCmdMaxCount)
			{
				_HistoryCmdList.RemoveAt(0);
			}
		}

		private void _RunCmd()
		{
			_AppendLog(TerminalLogItemType.Shell, $"executing [{_CommandText}]");
			if (!string.IsNullOrEmpty(_CommandText))
			{
				if (GMManager.Execute(_CommandText))
				{
					_AppendHistoryCmd(_CommandText);
					_ResetHistoryCmdIndexToLast();
				}
			}
			_ScrollPosition.y = float.MaxValue;
		}

		private void _HandleInput()
		{
			if (Event.current.Equals(Event.KeyboardEvent(HotKey)))
			{
				StartCoroutine(_ToggleState());
				return;
			}

			if (Event.current.Equals(Event.KeyboardEvent("down")))
			{
				_UseNextHistoryCmd();
				return;
			}

			if (Event.current.Equals(Event.KeyboardEvent("up")))
			{
				_UsePreviousHistoryCmd();
				return;
			}

			if (Event.current.Equals(Event.KeyboardEvent("return")))
			{
				_RunCmd();
				_CommandText = "";
			}
		}
		
		/// <summary>
		/// 最大日志条目数量
		/// </summary>
		private const int MaxLogCount = 256;

		/// <summary>
		/// 唤起命令行的热键
		/// </summary>
		[HideInInspector]
		public string HotKey = "`";

		/// <summary>
		/// 输入提示符
		/// </summary>
		[HideInInspector]
		public string Caret = ">";

		/// <summary>
		/// 命令行窗口标题
		/// </summary>
		[HideInInspector]
		public string Title = "Console";

		/// <summary>
		/// 命令行字体大小
		/// </summary>
		[HideInInspector]
		public int FontSize = 12;

		/// <summary>
		/// 命令行字体基本颜色
		/// </summary>
		private readonly Color ForegroundColor = Color.white;

		/// <summary>
		/// 定义窗口大小的矩形
		/// </summary>
		private Rect _windowRect;

		/// <summary>
		/// 定义窗口的基本样式
		/// </summary>
		private GUIStyle _windowStyle;

		/// <summary>
		/// 输入区域的样式
		/// </summary>
		private GUIStyle _InputStyle;

		/// <summary>
		/// 命令行当前状态
		/// 打开或者关闭
		/// </summary>
		private TerminalState _state;

		/// <summary>
		/// 所以日志条目
		/// </summary>
		private static readonly List<TerminalLogItem> _items = new();

		/// <summary>
		/// 消息监听中
		/// </summary>
		private static bool _Listening = false;

		/// <summary>
		/// 显示日志的scroll view当前的位置
		/// </summary>
		private Vector2 _ScrollPosition;

		/// <summary>
		/// 输入缓冲
		/// </summary>
		private string _CommandText = "";

		/// <summary>
		/// 条目类型对应的样式
		/// </summary>
		private Dictionary<TerminalLogItemType, GUIStyle> _styles;

		/// <summary>
		/// 用来处理窗口刚弹出的一些操作
		/// </summary>
		private bool _FirstDraw;

		/// <summary>
		/// 当前历史命令索引
		/// </summary>
		private int _HistoryCmdIndex;
		
		/// <summary>
		/// 历史命令最大数量
		/// </summary>
		private const int _HistoryCmdMaxCount = 10;

		/// <summary>
		/// 历史命令列表
		/// </summary>
		private readonly List<string> _HistoryCmdList = new();

	}
}

