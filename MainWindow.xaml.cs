using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace KeyboardTrainer
{
	public partial class MainWindow : Window
	{
		private List<string> _sentences = new List<string>() ///< List of the given sentences
		{
			"Max Joykner sneakily drove his car around every corner looking for his dog.",
			"The two boys collected twigs outside, for over an hour, in the freezing cold.",
			"Trixie and Veronica, our two cats, just love to play with their pink ball of yarn."
		};

		private bool _isCapsLock = false;
		private bool _isShift = false;
		private int _sentenceIndex = 0;
		private int _fails = 0;
		private string _typedText = "";
		
		private int _charPerMinute = 0;
		private int _typedChars = 0;
		private DateTime _startTime;
		private DateTime _lastKeyPressTime;
        private TimeSpan _elapsedTime;

        public MainWindow()
		{
			InitializeComponent();
		}

		private void Start_Click(object sender, RoutedEventArgs e)
		{
            _sentenceIndex = 0;
            _fails = 0;
            _typedText = "";
            _typedChars = 0;
            _elapsedTime = TimeSpan.Zero;
            _startTime = DateTime.Now;

            ShowSentence(0); ///< Show the first sentence
			UpdateFails();
			UpdateSpeed();
		}

		private void Stop_Click(object? sender, RoutedEventArgs? e)
		{
			PrintSentence.Text = "Game over!";
			InputBox.Text = "";
		}

		private void ShowSentence(int sentenceNumber)
		{
			PrintSentence.Text = _sentences[sentenceNumber];
		}

		private void UpdateFails()
		{
			Fails.Text = $"Fails: {_fails}";
		}

		private void UpdateSpeed()
		{
			Speed.Text = $"Speed: {CalculateSpeed()} chars/min";
		}

		private double CalculateSpeed()
		{
            TimeSpan elapsedTime = DateTime.Now - _startTime;

			if (elapsedTime.TotalSeconds == 0)
			{
				return _charPerMinute;
			}

            _charPerMinute = (int)((_typedChars / elapsedTime.TotalMinutes));

            return _charPerMinute;
		}

        private void KeyPressed(object sender, KeyEventArgs e)
        {
            if (Char.IsLetter((char)KeyInterop.VirtualKeyFromKey(e.Key)) || e.Key == Key.Space) ///< Check if the pressed key is a letter or a space
			{
                char keyChar = (char)KeyInterop.VirtualKeyFromKey(e.Key);

                if (ShiftButtonIsDown()) ///< Check if shift is pressed and holded
				{
                    keyChar = Char.ToUpper(keyChar);
                }
                else if (_isCapsLock) ///< Check if caps lock is on
				{
                    keyChar = Char.ToUpper(keyChar);
                }
                else ///< No caps and no shift
				{
                    keyChar = Char.ToLower(keyChar);
                }

                _typedText += keyChar;
                InputBox.Text = _typedText; ///< Write the typed text to the input box

                CheckSentenceComplete();
                UpdateFails();
				UpdateSpeed();
            }
            else if (e.Key >= Key.D0 && e.Key <= Key.D9) ///< Handle numbers 0 - 9
			{
                int number = e.Key - Key.D0;
                _typedText += number.ToString();
                InputBox.Text = _typedText;

                CheckSentenceComplete();
                UpdateFails();
				UpdateSpeed();
            }
            else if (e.Key == Key.OemComma) ///< Handle comma
			{
                _typedText += ",";
                InputBox.Text = _typedText;

                CheckSentenceComplete();
                UpdateFails();
				UpdateSpeed();
            }
            else if (e.Key == Key.OemPeriod) ///< Handle dot
			{
                _typedText += ".";
                InputBox.Text = _typedText;

                CheckSentenceComplete();
                UpdateFails();
				UpdateSpeed();
            }
            else if (e.Key == Key.Back) ///< Handle backspace
			{
                if (_typedText.Length > 0)
                {
                    _typedText = _typedText.Remove(_typedText.Length - 1);
                    InputBox.Text = _typedText;
                }
            }

			_typedChars++;
			_lastKeyPressTime = DateTime.Now;

			UpdateSpeed();
        }

        private void CapsLockButton_Click(object sender, RoutedEventArgs e) 
		{ 
			if (_isCapsLock)
			{
				_isCapsLock = false;
			}
			else
			{ 
				_isCapsLock = true; 
			}
			
			CheckSentenceComplete();
		}

		private void SpaceButton_Click(object sender, RoutedEventArgs e)
		{
			_typedText += " ";
			InputBox.Text = _typedText;
			CheckSentenceComplete();
		}

		private bool ShiftButtonIsDown()
		{
			if (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift))
			{
				return true;
			}

			return false;
		}

		private void BackspaceButton_Click(object sender, RoutedEventArgs e)
		{
			if (_typedText.Length > 0)
			{
				_typedText = _typedText.Remove(_typedText.Length - 1);
				InputBox.Text = _typedText;
			}
		}

		///< Check if the sentence is complete
		private void CheckSentenceComplete()
		{
			if (_typedText == _sentences[_sentenceIndex])
			{
				_sentenceIndex++; ///< Move to next sentence

				if (_sentenceIndex < _sentences.Count)
				{
					ShowSentence(_sentenceIndex);
					InputBox.Text = ""; ///< Reset own text
					_typedText = "";
				}
				else
				{
					Stop_Click(null, null); ///< Stop the game
					PrintSentence.Text = "Game over!";
				}
			}
			else if (_typedText.Length > 0) ///< Check for fail
			{
				try
				{
					if (_typedText[_typedText.Length - 1] != _sentences[_sentenceIndex][_typedText.Length - 1])
					{
						_fails++;
						UpdateFails();
					}
				}
				catch (IndexOutOfRangeException ex)
				{
					MessageBox.Show($"Error: {ex}");
				}
			}
		}

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag != null)
            {
                string? input = button.Tag.ToString();

                _typedText += input;
                InputBox.Text = _typedText;

                CheckSentenceComplete();
            }
        }
    }
}