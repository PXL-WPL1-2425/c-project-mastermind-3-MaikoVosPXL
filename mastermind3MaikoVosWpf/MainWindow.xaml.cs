using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace mastermind3MaikoVosWpf
{
    public partial class MainWindow : Window
    {
        DispatcherTimer timer;
        TimeSpan elapsedTime;
        TimeSpan totalTime = TimeSpan.FromSeconds(10);
        Random rnd = new Random();
        ComboBox[] guess = new ComboBox[4];
        Ellipse[] selectedEllipse = new Ellipse[4];
        string[] randomNumberColor = new string[4];
        string[] naamInput = new string[15];
        string[] highscores = new string[15];
        List<string> playerNames = new List<string>();
        List<int> availableHintRightPosition = new List<int>();
        List<int> availableHintRightColor = new List<int>();
        bool addAnotherPlayer = true;
        bool bypassClosingGame = false;
        string randomColorSolution;
        string username;
        string highscoreText;
        int playerIndex;
        int maxAttempts = 0;
        int points = 100;
        int rows = 0;
        int attempts = 0;

        public MainWindow()
        {
            InitializeComponent();

            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(100);
            timer.Tick += Timer_Tick;
        }
        /// <summary>
        /// Start het hoofdvenster en stelt het spel en de timer in bij het laden van het venster.
        /// </summary>
        /// <param name="sender"> Het laad-event wordt geactiveerd door het hoofdvenster.</param>
        /// <param name="e"> De gegevens van het event.</param>
        private void MainWindowLoader(object sender, RoutedEventArgs e)
        {
            StartCountdown();
            StartGame();
            playerIndex = 0;
        }
        /// <summary>
        /// Behandelt de tick van de timer, om de verstreken tijd en pogingen bij te werken.
        /// </summary>
        /// <param name="sender"> Het object dat de tick-event heeft geactiveerd.</param>
        /// <param name="e"> De gegevens van het event.</param>
        private void Timer_Tick(object sender, EventArgs e)
        {
            elapsedTime -= timer.Interval;
            timerText.Text = $"Timer: {elapsedTime.TotalSeconds.ToString("N2")} / 10";

            if (attempts < maxAttempts)
            {
                if (CheckingIfWonGame())
                {
                    StopCountdown();
                }
                else if (elapsedTime <= TimeSpan.Zero)
                {
                    StopCountdown();
                    CheckCodeButton_Click(null, null); 
                }
            }
            else if (attempts == maxAttempts)
            {
                StopCountdown();
            }

            totalAttempts.Content = $"Attempts: {attempts}/{maxAttempts}";
        }

        private void StartCountdown()
        {
            elapsedTime = totalTime;
            timer.Start();
                    
        }

        private void ResumeCountdown()
        {
            timer.Start();
        }
        private void PauseCountdown()
        {
            timer.Stop();
        }

        private void StopCountdown()
        {
            timer.Stop();
        }
        /// <summary>
        /// Schakelt debugfunctie in of uit op basis van specifieke toetscombinaties.
        /// </summary>
        /// <param name="sender">Het object dat het evenement heeft geactiveerd.</param>
        /// <param name="e">De gegevens van het toetsenbord en de ingedrukte toetsen.</param>
        private void ToggleDebug(object sender, KeyEventArgs e)
        {
            if (e.KeyboardDevice.Modifiers == ModifierKeys.Control && e.Key == Key.F12)
            {
                showRandomColors.Visibility = Visibility.Visible;
                showRandomColors.Text = randomColorSolution;
            }
            else
            {
                showRandomColors.Visibility = Visibility.Hidden;
            }

            if (e.KeyboardDevice.Modifiers == ModifierKeys.Control && e.Key == Key.F2)
            {
                timerText.Visibility = Visibility.Visible;
                timerText.Text = $"Timer: {elapsedTime.TotalSeconds.ToString("N3")} / 10";
            }
            else
            {
                timerText.Visibility = Visibility.Hidden;
            }
        }
        /// <summary>
        /// Start het spel door willekeurige kleuren te genereren, de oplossing vast te leggen en de spelstatus bij te werken.
        /// </summary>
        private void StartGame()
        {
            Array.Clear(highscores, 0, highscores.Length);
            randomNumberColorGenerator();
            StopCountdown();
            InputAttempts();
            InputName();
            StartCountdown();

            totalScore.Content = $"Score: {points}/100";
            totalAttempts.Content = $"Attempts: {attempts}/{maxAttempts}";
            
        }

        private void randomNumberColorGenerator()
        {
            randomNumberColor[0] = PickingRandomColor(rnd.Next(0, 6));
            randomNumberColor[1] = PickingRandomColor(rnd.Next(0, 6));
            randomNumberColor[2] = PickingRandomColor(rnd.Next(0, 6));
            randomNumberColor[3] = PickingRandomColor(rnd.Next(0, 6));

            randomColorSolution = $"{randomNumberColor[0]}, {randomNumberColor[1]}, {randomNumberColor[2]}, {randomNumberColor[3]}";
            showRandomColors.Text = randomColorSolution;
        }

        private void Highscores()
        {
            StopCountdown();
            highscoreText = "";
            for (int i = 0; i <= playerIndex; i++)
            {
                highscoreText += $"{highscores[i]}\n";
            }
            MessageBox.Show($"Highscore: \n{highscoreText}");
            ResumeCountdown();
        }

        private void Highscore_Click(object sender, RoutedEventArgs e)
        {
            Highscores();
        }

        private void InputAttempts()
        {
            string inputAttempt = Interaction.InputBox("Geef een het aantal pogingen in 3-20.", "Invoer", "0", 500);
            bool isValidAantalPogingen = int.TryParse(inputAttempt, out maxAttempts);

            while (!isValidAantalPogingen || maxAttempts < 3 || maxAttempts > 20 || string.IsNullOrEmpty(inputAttempt))
            {
                MessageBox.Show("Geef een getal!", "Foutieve invoer");
                inputAttempt = Interaction.InputBox("Geef een het aantal pogingen 3-20 in!", "Invoer", "0", 500);
                isValidAantalPogingen = int.TryParse(inputAttempt, out maxAttempts);
            }
        }

        private void InputName()
        {
            while (addAnotherPlayer)
            {
                username = Interaction.InputBox("Geef een naam in.", "Invoer", "", 500);
                bool naamIsNumber = int.TryParse(username, out int inputNaamIsNumber);

                while (username == "" || naamIsNumber || string.IsNullOrEmpty(username))
                {
                    MessageBox.Show("Geef een naam in!", "Foutieve invoer");
                    username = Interaction.InputBox("Geef een naam in.", "Invoer", "", 500);
                    naamIsNumber = int.TryParse(username, out inputNaamIsNumber);
                }

                playerNames.Add(username);

                MessageBoxResult result = MessageBox.Show("Do you want to add a new name?","Adding player",MessageBoxButton.YesNo, MessageBoxImage.Question);
                if ( result == MessageBoxResult.No)
                {
                    addAnotherPlayer = false;
                }
            }
            activePlayer.Content = $"The current player is {playerNames[playerIndex]}";
        }

        /// <summary>
        /// Aan de hand van de index word er een string aan de randomNumber gekoppelt.
        /// </summary>
        /// <param name="randomNumber"> Een random getal dat de kleur bepaalt.</param>
        /// <returns> Een string die de naam van de kleur beschrijft.</returns>
        private string PickingRandomColor(int randomNumber)
        {
            switch (randomNumber)
            {
                case 0:
                    return "Red";
                case 1:
                    return "Yellow";
                case 2:
                    return "Orange";
                case 3:
                    return "White";
                case 4:
                    return "Green";
                case 5:
                    return "Blue";
                default:
                    return "Black";
            }
        }
        /// <summary>
        /// Verandert de Ellipse background/fill aan de hand van de geselecteerde index van de combobox
        /// </summary>
        /// <param name="sender"> De Combobox waarvan de index wordt geselecteerd.</param>
        /// <param name="e"> De selectie van de Combobox.</param>
        private void ColorChange(object sender, EventArgs e)
        {
            ComboBox changedComboBox = sender as ComboBox;

            if (changedComboBox == colorOneComboBox)
            {
                colorFieldOne.Fill = Colorindex(changedComboBox.SelectedIndex);
            }
            else if (changedComboBox == colorTwoComboBox)
            {
                colorFieldTwo.Fill = Colorindex(changedComboBox.SelectedIndex);
            }
            else if (changedComboBox == colorThreeComboBox)
            {
                colorFieldThree.Fill = Colorindex(changedComboBox.SelectedIndex);
            }
            else if (changedComboBox == colorFourComboBox)
            {
                colorFieldFour.Fill = Colorindex(changedComboBox.SelectedIndex);
            }
        }
        /// <summary>
        /// Checkt aan de hand van de index van de Combobox welke Brushes deze returns.
        /// </summary>
        /// <param name="selectedindex"> De geselecteerde index.</param>
        /// <returns> Brushes aan de hand van de geselecteerde index.</returns>
        private Brush Colorindex(int selectedindex)
        {
            switch (selectedindex)
            {
                case 0:
                    return Brushes.Red;
                case 1:
                    return Brushes.Yellow;
                case 2:
                    return Brushes.Orange;
                case 3:
                    return Brushes.White;
                case 4:
                    return Brushes.Green;
                case 5:
                    return Brushes.Blue;
                default:
                    return null;
            }
        }
        /// <summary>
        /// Checkt de invoer van de gebruiker ten opzichte van de oplossing op een specifieke positie.
        /// </summary>
        /// <param name="colorChecker"> De Ellipse dat feedback geeft voor de kleurcontrole.</param>
        /// <param name="randomNumberColor"> Een array van strings met de oplossing.</param>
        /// <param name="position"> De index van de array met de oplossing.</param>
        /// <param name="input">De Combobox waarvan de text vergeleken wordt met de oplossing.</param>
        private void EllipseColorCheck(Ellipse colorChecker, string[] randomNumberColor, int position, ComboBox input)
        {
            string solution;
            switch (position)
            {
                case 0:
                    solution = randomNumberColor[0];
                    break;
                case 1:
                    solution = randomNumberColor[1];
                    break;
                case 2:
                    solution = randomNumberColor[2];
                    break;
                case 3:
                    solution = randomNumberColor[3];
                    break;
                default:
                    return;
            }

            if (input.Text == "" || !randomNumberColor.Contains(input.Text))
            {
                points -= 2;
                colorChecker.StrokeThickness = 5;
            }
            else if (randomNumberColor.Contains(input.Text) && input.Text != "" && input.Text != solution)
            {
                points -= 1;
                colorChecker.Stroke = Brushes.Wheat;
                colorChecker.StrokeThickness = 4;
            }
            else
            {
                colorChecker.Stroke = Brushes.DarkRed;
                colorChecker.StrokeThickness = 4;
            }
            totalScore.Content = $"Score: {points}/100";

            if (input.Text == "")
            {
                MessageBox.Show("Invalid color", "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        /// <summary>
        /// Create nieuwe rows en plaatst de Ellipse in deze rows.
        /// </summary>
        private void ShowGuess()
        {
            RowDefinition newRow = new RowDefinition();
            newRow.Height = GridLength.Auto;
            addRows.RowDefinitions.Add(newRow);

            guess[0] = colorOneComboBox;
            guess[1] = colorTwoComboBox;
            guess[2] = colorThreeComboBox;
            guess[3] = colorFourComboBox;

            selectedEllipse[0] = colorFieldOne;
            selectedEllipse[1] = colorFieldTwo;
            selectedEllipse[2] = colorFieldThree;
            selectedEllipse[3] = colorFieldFour;

            for (int i = 0; i < guess.Length; i++)
            {
                Ellipse makingNewEllipse = new Ellipse();
                makingNewEllipse.Fill = Colorindex(guess[i].SelectedIndex);
                makingNewEllipse.Stroke = selectedEllipse[i].Stroke;
                makingNewEllipse.StrokeThickness = selectedEllipse[i].StrokeThickness;
                makingNewEllipse.Height = 30;
                makingNewEllipse.Width = 30;
                makingNewEllipse.Margin = new Thickness(1.8);

                Grid.SetRow(makingNewEllipse, rows);
                Grid.SetColumn(makingNewEllipse, i);

                addRows.Children.Add(makingNewEllipse);
            }
            rows++;
        }
        /// <summary>
        /// Checkt de code bij iedere click van de knop.
        /// </summary>
        ///  /// <param name="sender">Is de Button knop</param>
        /// <param name="e">Is de Button click event</param>
        private void CheckCodeButton_Click(object sender, RoutedEventArgs e)
        {

            if (attempts != maxAttempts)
            {
                EllipseColorCheck(colorFieldOne, randomNumberColor, 0, colorOneComboBox);
                EllipseColorCheck(colorFieldTwo, randomNumberColor, 1, colorTwoComboBox);
                EllipseColorCheck(colorFieldThree, randomNumberColor, 2, colorThreeComboBox);
                EllipseColorCheck(colorFieldFour, randomNumberColor, 3, colorFourComboBox);

                ShowGuess();
                if (attempts >= maxAttempts)
                {
                    CheckingAttempts();
                    StopCountdown();
                }
                else
                {
                    attempts++;
                    totalAttempts.Content = $"Attempts: {attempts}/ {maxAttempts}";
                    CheckingAttempts();
                    StartCountdown();
                }
            }
        }
        /// <summary>
        /// Checkt of de attempts niet overschreden zijn.
        /// </summary>
        private void CheckingAttempts()
        {
            if (attempts >= maxAttempts && !CheckingIfWonGame())
            {
                highscores[playerIndex] = $" {playerNames[playerIndex]} - {attempts}/{maxAttempts} attempts - {points}/100";

                if (playerIndex +1 < playerNames.Count)
                {
                    MessageBoxResult result = MessageBox.Show($"You failed, the code was {randomColorSolution}.\nNow it's {playerNames[playerIndex + 1]}'s turn.", $"{playerNames[playerIndex]}", MessageBoxButton.OK, MessageBoxImage.Information);
                    playerIndex++;
                    activePlayer.Content = $"The current player is {playerNames[playerIndex]}";
                }
                else
                {
                    activePlayer.Content = "";
                    MessageBoxResult result = MessageBox.Show($"You failed, the code was {randomColorSolution}.\nAll players have completed their turns. Check the highscores!", $"{playerNames[playerIndex]}", MessageBoxButton.OK, MessageBoxImage.Information);
                    Highscores();
     
                }
                attempts = 0;
                points = 100;
                totalAttempts.Content = $"Attempts: {attempts}/ {maxAttempts}";
                totalScore.Content = $"Score: {points}/100";
                addRows.Children.Clear();
                ClearingOutMainEllipse();
                randomNumberColorGenerator();
                

            }
            else if (CheckingIfWonGame())
            {
                highscores[playerIndex] = $" {playerNames[playerIndex]} - {attempts}/{maxAttempts} attempts - {points}/100";
                

                if (playerIndex +1 < playerNames.Count)
                {
                    MessageBoxResult result = MessageBox.Show($"You won in {attempts} attempts!\nNow it's {playerNames[playerIndex + 1]}'s turn.", $"{playerNames[playerIndex]}", MessageBoxButton.OK, MessageBoxImage.Information);
                    playerIndex++;
                    activePlayer.Content = $"The current player is {playerNames[playerIndex]}";
                }
                else
                {
                    activePlayer.Content = "";
                    MessageBoxResult result = MessageBox.Show($"You won in {attempts} attempts!\nAll players have completed their turns. Check the highscores!", $"{playerNames[playerIndex]}", MessageBoxButton.OK, MessageBoxImage.Information);
                    Highscores();
                                        
                }
                attempts = 0;
                points = 100;
                totalAttempts.Content = $"Attempts: {attempts}/ {maxAttempts}";
                totalScore.Content = $"Score: {points}/100";
                addRows.Children.Clear();
                ClearingOutMainEllipse();
                randomNumberColorGenerator();
                
            }
        }
        /// <summary>
        /// CheckingIfWonGame checkt of de vier gekozen Ellipse een DarkRed brush heeft.
        /// </summary>
        /// <returns> Als alle Strokes == Brushes.DarkRed dan true, anders false. </returns>
        private bool CheckingIfWonGame()
        {
            selectedEllipse[0] = colorFieldOne;
            selectedEllipse[1] = colorFieldTwo;
            selectedEllipse[2] = colorFieldThree;
            selectedEllipse[3] = colorFieldFour;

            if (selectedEllipse[0].Stroke == Brushes.DarkRed &&
                selectedEllipse[1].Stroke == Brushes.DarkRed &&
                selectedEllipse[2].Stroke == Brushes.DarkRed &&
                selectedEllipse[3].Stroke == Brushes.DarkRed)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        /// <summary>
        /// ClearingOutMainEllipse reset uw hoofd Ellipse on uw Comboboxen terug naar default.
        /// </summary>
        private void ClearingOutMainEllipse()
        {
            colorOneComboBox.Text = "";
            colorTwoComboBox.Text = "";
            colorThreeComboBox.Text = "";
            colorFourComboBox.Text = "";

            colorFieldOne.StrokeThickness = 0;
            colorFieldTwo.StrokeThickness = 0;
            colorFieldThree.StrokeThickness = 0;
            colorFieldFour.StrokeThickness = 0;

            colorFieldOne.Stroke = default;
            colorFieldTwo.Stroke = default;
            colorFieldThree.Stroke = default;
            colorFieldFour.Stroke = default;
        }
        /// <summary>
        /// Bij het vroegtijdig afsluiten komt er een Messagebox met de optie om toch nog door te spelen of het venster toch te sluiten.
        /// De BypassClosingGame bool wordt alleen gebruikt bij de Failed en de Winner Messagebox om geen herhaling te krijgen voor het afsluiten.
        /// </summary>
        /// <param name="sender"> De Window die wordt afgesloten.</param>
        /// <param name="e"> Is het sluiting van de MainWindow.</param>
        private void ClosingGame(object sender, System.ComponentModel.CancelEventArgs e)
        {
            StopCountdown();
            if (bypassClosingGame)
            {
                e.Cancel = false;
                return;
            }

            MessageBoxResult result = MessageBox.Show("You sure you want to stop the game?", $"{attempts}/{maxAttempts} attempts", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result == MessageBoxResult.Yes)
            {
                e.Cancel = false;
            }
            else
            {
                e.Cancel = true;
                ResumeCountdown();
            }
        }

        private void Afsluiten_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void NieuwSpel_Click(object sender, RoutedEventArgs e)
        {
            addAnotherPlayer = true;
            playerNames.Clear();
            playerIndex = 0;
            attempts = 0;
            points = 100;
            totalAttempts.Content = $"Attempts: {attempts}/ {maxAttempts}";
            totalScore.Content = $"Score: {points}/100";
            addRows.Children.Clear();
            ClearingOutMainEllipse();
            StartGame();
            
        }

        private void Pogingen_Click(object sender, RoutedEventArgs e)
        {
            StopCountdown();
            InputAttempts();
            ResumeCountdown();
        }

        private void BuyHint_Click(object sender, RoutedEventArgs e)
        {
            
            PauseCountdown();
            MessageBoxResult result = MessageBox.Show("Yes = Correct Color (15 points)\nNo = Correct Color in the Correct Position (25 points)\n",
                                                        "Buy Hint",
                                                        MessageBoxButton.YesNoCancel,
                                                        MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                if (points >= 15)
                {
                    points -= 15;
                    TheRightColor();
                }
                else
                {
                    MessageBox.Show("Not enough points for this hint!", "Insufficient Points", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else if (result == MessageBoxResult.No)
            {
                if (points >= 25)
                {
                    points -= 25;
                    TheRightColorAndPosition();
                }
                else
                {
                    MessageBox.Show("Not enough points for this hint!", "Insufficient Points", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("No hint purchased.", "Hint Cancelled", MessageBoxButton.OK, MessageBoxImage.Information);
            }

            totalScore.Content = $"Score: {points}/100";
            ResumeCountdown();
        }

        private void TheRightColor()
        {
            selectedEllipse[0] = colorFieldOne;
            selectedEllipse[1] = colorFieldTwo;
            selectedEllipse[2] = colorFieldThree;
            selectedEllipse[3] = colorFieldFour;

            for (int i = 0; i < selectedEllipse.Length; i++)
            {
                if (selectedEllipse[i].Stroke != Brushes.DarkRed)
                {
                    availableHintRightColor.Add(i);
                }
            }

            if (availableHintRightColor.Count > 0)
            {
                Random random = new Random();
                int randomHintIndex = availableHintRightColor[random.Next(availableHintRightColor.Count)];

                MessageBox.Show($"Hint: Color {randomNumberColor[randomHintIndex]} is correct.", "Hint");
            }
            else
            {
                MessageBox.Show("You got all the colors right", "Hint");
            }

            availableHintRightColor.Clear();
        }

        private void TheRightColorAndPosition()
        {
            selectedEllipse[0] = colorFieldOne;
            selectedEllipse[1] = colorFieldTwo;
            selectedEllipse[2] = colorFieldThree;
            selectedEllipse[3] = colorFieldFour;

            for (int i = 0; i < selectedEllipse.Length; i++)
            {
                if (selectedEllipse[i].Stroke != Brushes.DarkRed)
                {
                    availableHintRightPosition.Add(i);
                }
            }

            if (availableHintRightPosition.Count > 0)
            {
                Random random = new Random();
                int randomHintIndex = availableHintRightPosition[random.Next(availableHintRightPosition.Count)];

                MessageBox.Show($"Hint: Color {randomNumberColor[randomHintIndex]} is correct and in position {randomHintIndex + 1}.", "Hint");
            }
            else
            {
                MessageBox.Show("You got all the colors right.", "Hint");
            }
            
            availableHintRightPosition.Clear();
        }
    }
}
